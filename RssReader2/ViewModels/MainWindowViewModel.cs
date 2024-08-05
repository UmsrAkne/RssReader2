using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RssReader2.Models;
using RssReader2.Models.Dbs;
using RssReader2.ViewModels.Commands;
using RssReader2.Views;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private ObservableCollection<Feed> feeds;
        private bool uiEnabled = true;

        public MainWindowViewModel()
        {
            Feeds = new ObservableCollection<Feed>(new DummyFeedProvider().GetAllFeeds());
            TreeViewVm.WebSiteTreeViewItems = new ObservableCollection<IWebSiteTreeViewItem>(new DummyWebSiteProvider().GetAllWebSites());
        }

        public MainWindowViewModel(IContainerProvider containerProvider, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            FeedProvider = containerProvider.Resolve<IFeedProvider>();
            FeedService = containerProvider.Resolve<FeedService>();
            WebSiteService = containerProvider.Resolve<WebSiteService>();
            Feeds = new ObservableCollection<Feed>(FeedProvider.GetAllFeeds());

            var webSiteProvider = containerProvider.Resolve<IWebSiteProvider>();
            TreeViewVm.WebSiteTreeViewItems = new ObservableCollection<IWebSiteTreeViewItem>(webSiteProvider.GetAllWebSites());
            FeedListViewModel = containerProvider.Resolve<FeedListViewModel>();
        }

        public FeedService FeedService { get; set; }

        public WebSiteService WebSiteService { get; set; }

        public TextWrapper TitleBarText { get; } = new ();

        public ObservableCollection<Feed> Feeds { get => feeds; private set => SetProperty(ref feeds, value); }

        public TreeViewVm TreeViewVm { get; private set; } = new ();

        public bool UiEnabled { get => uiEnabled; set => SetProperty(ref uiEnabled, value); }

        public FeedListViewModel FeedListViewModel { get; set; }

        public DelegateCommand ShowWebSiteAdditionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(WebSiteAdditionPage), new DialogParameters(), (_) => { });
            TreeViewVm.WebSiteTreeViewItems =
                new ObservableCollection<IWebSiteTreeViewItem>(WebSiteService.GetAllWebSites());
        });

        public DelegateCommand ShowWebSiteEditPageCommand => new DelegateCommand(() =>
        {
            var currentItem = TreeViewVm.FindSelectedItem(TreeViewVm.WebSiteTreeViewItems);
            if (currentItem is not WebSite)
            {
                return;
            }

            var param = new DialogParameters { { nameof(WebSite), currentItem }, };
            dialogService.ShowDialog(nameof(WebSiteEditPage), param, (_) =>
            {
                if (currentItem is WebSite item)
                {
                    WebSiteService.UpdateWebSite(item);
                }
            });
        });

        public DelegateCommand ShowGroupAdditionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(GroupAdditionPage), new DialogParameters(), (_) => { });
        });

        public DelegateCommand UpdateFeedsCommand => new DelegateCommand(() =>
        {
            var currentSite = TreeViewVm.FindSelectedItem(TreeViewVm.WebSiteTreeViewItems);

            if (currentSite is WebSite site)
            {
                Feeds = new ObservableCollection<Feed>(FeedProvider.GetFeedsByWebSiteId(site.Id));
            }
        });

        public AsyncDelegateCommand GetRssFeedsCommandAsync => new AsyncDelegateCommand(async () =>
        {
            var currentSite = TreeViewVm.FindSelectedItem(TreeViewVm.WebSiteTreeViewItems);
            if (currentSite is not WebSite site)
            {
                // 選択中のアイテムが WebSite ではない場合は、UIを止めない。
                return;
            }

            UiEnabled = false;

            var l = await FeedReader.GetRssFeedsAsync(site.Url);
            var list = l.ToList();
            foreach (var f in list)
            {
                f.ParentSiteId = site.Id;
            }

            FeedService.AddFeeds(list, new List<NgWord>());
            Feeds = new ObservableCollection<Feed>(FeedProvider.GetFeedsByWebSiteId(site.Id));
            UiEnabled = true;
        });

        public AsyncDelegateCommand GetAllSiteRssFeedsCommandAsync => new AsyncDelegateCommand(async () =>
        {
            var all = WebSiteService.GetAllWebSites();

            UiEnabled = false;

            var list = new List<Feed>();
            foreach (var site in all)
            {
                var l = await FeedReader.GetRssFeedsAsync(site.Url);
                var ll = l.ToList();
                foreach (var f in ll)
                {
                    f.ParentSiteId = site.Id;
                }

                list.AddRange(ll);
            }

            FeedService.AddFeeds(list, new List<NgWord>());
            if (TreeViewVm.FindSelectedItem(TreeViewVm.WebSiteTreeViewItems) is WebSite currentSite)
            {
                Feeds = new ObservableCollection<Feed>(FeedProvider.GetFeedsByWebSiteId(currentSite.Id));
            }

            UiEnabled = true;
        });

        private IFeedProvider FeedProvider { get; set; }
    }
}