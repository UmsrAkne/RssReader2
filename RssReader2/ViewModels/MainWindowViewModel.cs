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

        [Obsolete("プレビュー用。ダミーを入力するためのコンストラクタです。明示的に呼び出さないでください。")]
        public MainWindowViewModel()
        {
            FeedListViewModel = new FeedListViewModel(new DummyFeedProvider(), null);

            RaisePropertyChanged(nameof(FeedListViewModel));
            FeedListViewModel.Feeds.Add(new Feed() { Title = "testTitle_testTitle_testTitle_testTitle_testTitle_testTitle_testTitle_testTitle_testTitle_testTitle_testTitle_", });
            FeedListViewModel.Feeds.Add(new Feed() { Title = "testTitle", IsMarked = true, });
            FeedListViewModel.Feeds.Add(new Feed() { Title = "testTitle", });
            FeedListViewModel.Feeds.Add(new Feed() { Title = "testTitle", });
            FeedListViewModel.Feeds.AddRange(new DummyFeedProvider().GetAllFeeds());

            FeedListViewModel.Feeds[0].Description =
                "DescriptionDescriptionDescriptionDescriptionDescription"
                + "DescriptionDescriptionDescriptionDescriptionDescription"
                + "DescriptionDescriptionDescriptionDescriptionDescription"
                + "DescriptionDescriptionDescriptionDescriptionDescription"
                + "DescriptionDescriptionDescriptionDescriptionDescription"
                + "DescriptionDescriptionDescriptionDescriptionDescription";

            FeedListViewModel.SelectedItem = FeedListViewModel.Feeds[0];

            TreeViewVm = new TreeViewVm(null, null)
            {
                WebSiteTreeViewItems = new ObservableCollection<IWebSiteTreeViewItem>(new DummyWebSiteProvider().GetAllWebSites()),
            };

            TreeViewVm.WebSiteTreeViewItems.Insert(0, new WebSiteGroup() { Name = "WebSite Group1", });
            TreeViewVm.WebSiteTreeViewItems.Insert(0, new WebSiteGroup() { Name = "WebSite Group2", });

            RaisePropertyChanged(nameof(TreeViewVm));
        }

        public MainWindowViewModel(IContainerProvider containerProvider, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            FeedService = containerProvider.Resolve<FeedService>();
            WebSiteService = containerProvider.Resolve<WebSiteService>();
            NgWordService = containerProvider.Resolve<NgWordService>();

            TreeViewVm = containerProvider.Resolve<TreeViewVm>();
            TreeViewVm.ReloadTreeViewItems();

            FeedListViewModel = containerProvider.Resolve<FeedListViewModel>();
        }

        public TextWrapper TitleBarText { get; } = new ();

        public TreeViewVm TreeViewVm { get; private init; }

        public bool UiEnabled { get => uiEnabled; set => SetProperty(ref uiEnabled, value); }

        public FeedListViewModel FeedListViewModel { get; init; }

        public DelegateCommand ShowWebSiteAdditionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(WebSiteAdditionPage), new DialogParameters(), (_) => { });
            TreeViewVm.ReloadTreeViewItems();
        });

        public DelegateCommand ShowWebSitesManagementPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(WebSitesManagementPage), new DialogParameters(), (_) => { });
            TreeViewVm.ReloadTreeViewItems();
        });

        public DelegateCommand ShowWebSiteEditPageCommand => new DelegateCommand(() =>
        {
            var currentItem = TreeViewVm.FindSelectedItem(TreeViewVm.WebSiteTreeViewItems);
            if (currentItem is not WebSite site)
            {
                return;
            }

            var old = new WebSite
            {
                Title = site.Title,
                Url = site.Url,
                GroupId = site.GroupId,
            };

            var param = new DialogParameters { { nameof(WebSite), site }, };
            dialogService.ShowDialog(nameof(WebSiteEditPage), param, (_) =>
            {
                if (currentItem is not WebSite item)
                {
                    return;
                }

                if (old.Title == item.Title && old.Url == item.Url && old.GroupId == item.GroupId)
                {
                    return;
                }

                WebSiteService.UpdateWebSite(item);
            });

            TreeViewVm.ReloadTreeViewItems();
        });

        public DelegateCommand ShowGroupAdditionPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(GroupAdditionPage), new DialogParameters(), (_) => { });
            TreeViewVm.ReloadTreeViewItems();
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

        private NgWordService NgWordService { get; set; }

        private FeedService FeedService { get; set; }

        private WebSiteService WebSiteService { get; set; }
    }
}