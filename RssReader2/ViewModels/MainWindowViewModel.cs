using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RssReader2.Models;
using RssReader2.Models.Dbs;
using RssReader2.Views;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly IDialogService dialogService;

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
        }

        public FeedService FeedService { get; set; }

        public WebSiteService WebSiteService { get; set; }

        public TextWrapper TitleBarText { get; } = new ();

        public ObservableCollection<Feed> Feeds { get; set; }

        public TreeViewVm TreeViewVm { get; private set; } = new ();

        public DelegateCommand ShowWebSiteAdditionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(WebSiteAdditionPage), new DialogParameters(), (_) => { });
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

        private IFeedProvider FeedProvider { get; set; }
    }
}