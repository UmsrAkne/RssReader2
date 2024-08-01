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
        private IWebSiteTreeViewItem selectedTreeViewItem;

        public MainWindowViewModel()
        {
            Feeds = new ObservableCollection<Feed>(new DummyFeedProvider().GetAllFeeds());
            WebSiteTreeViewItems = new ObservableCollection<IWebSiteTreeViewItem>(new DummyWebSiteProvider().GetAllWebSites());
        }

        public MainWindowViewModel(IContainerProvider containerProvider, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            FeedProvider = containerProvider.Resolve<IFeedProvider>();
            FeedService = containerProvider.Resolve<FeedService>();
            Feeds = new ObservableCollection<Feed>(FeedProvider.GetAllFeeds());

            var webSiteProvider = containerProvider.Resolve<IWebSiteProvider>();
            WebSiteTreeViewItems = new ObservableCollection<IWebSiteTreeViewItem>(webSiteProvider.GetAllWebSites());
        }

        public FeedService FeedService { get; set; }

        public TextWrapper TitleBarText { get; } = new ();

        public ObservableCollection<Feed> Feeds { get; set; }

        public ObservableCollection<IWebSiteTreeViewItem> WebSiteTreeViewItems { get; set; } = new ();

        public IWebSiteTreeViewItem SelectedTreeViewItem
        {
            get => selectedTreeViewItem;
            set => SetProperty(ref selectedTreeViewItem, value);
        }

        public DelegateCommand ShowWebSiteAdditionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(WebSiteAdditionPage), new DialogParameters(), (_) => { });
        });

        public DelegateCommand ShowWebSiteEditPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(WebSiteEditPage), new DialogParameters(), (_) => { });
        });

        public DelegateCommand ShowGroupAdditionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(GroupAdditionPage), new DialogParameters(), (_) => { });
        });

        private IFeedProvider FeedProvider { get; set; }
    }
}