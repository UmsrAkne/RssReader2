using System;
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
        private bool uiEnabled = true;

        public MainWindowViewModel()
        {
            FeedListViewModel = new FeedListViewModel(new DummyFeedProvider(), null)
            {
                WebSite = new WebSite() { Id = 1, },
            };

            TreeViewVm.WebSiteTreeViewItems = new ObservableCollection<IWebSiteTreeViewItem>(new DummyWebSiteProvider().GetAllWebSites());
        }

        public MainWindowViewModel(IContainerProvider containerProvider, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            FeedProvider = containerProvider.Resolve<IFeedProvider>();
            FeedService = containerProvider.Resolve<FeedService>();
            WebSiteService = containerProvider.Resolve<WebSiteService>();
            NgWordService = containerProvider.Resolve<NgWordService>();

            var webSiteProvider = containerProvider.Resolve<IWebSiteProvider>();
            TreeViewVm.WebSiteTreeViewItems = new ObservableCollection<IWebSiteTreeViewItem>(webSiteProvider.GetAllWebSites());
            FeedListViewModel = containerProvider.Resolve<FeedListViewModel>();
            FeedListViewModel.Feeds = new ObservableCollection<Feed>(FeedProvider.GetAllFeeds());
        }

        public NgWordService NgWordService { get; set; }

        public FeedService FeedService { get; set; }

        public WebSiteService WebSiteService { get; set; }

        public TextWrapper TitleBarText { get; } = new ();

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

        public DelegateCommand ShowNgWordAdditionPageCommand => new DelegateCommand(() =>
        {
            var all = NgWordService.GetAllNgWords().ToList();
            var latestChangeDate = default(DateTime);
            if (all.Count != 0)
            {
                latestChangeDate = all.Max(w => w.LastUpdated);
            }

            dialogService.ShowDialog(nameof(NgWordAdditionPage), new DialogParameters(), (_) => { });
            var allAfter = NgWordService.GetAllNgWords().ToList();
            if (allAfter.Count == 0)
            {
                return;
            }

            {
                if (latestChangeDate < allAfter.Max(w => w.LastUpdated))
                {
                    FeedListViewModel.ReloadFeeds(1);
                }
            }
        });

        public DelegateCommand UpdateFeedsCommand => new DelegateCommand(() =>
        {
            var currentSite = TreeViewVm.FindSelectedItem(TreeViewVm.WebSiteTreeViewItems);

            if (currentSite is WebSite site)
            {
                FeedListViewModel.WebSite = site;
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
            FeedListViewModel.ReloadFeeds(1);
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
            if (TreeViewVm.FindSelectedItem(TreeViewVm.WebSiteTreeViewItems) is WebSite)
            {
                FeedListViewModel.ReloadFeeds(1);
            }

            UiEnabled = true;
        });

        private IFeedProvider FeedProvider { get; set; }
    }
}