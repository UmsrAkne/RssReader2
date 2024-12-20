﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
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
        private readonly DispatcherTimer timer;
        private readonly ApplicationSettings applicationSettings;
        private bool uiEnabled = true;
        private bool autoUpdate;

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

            TreeViewVm = new TreeViewVm(null, null, null)
            {
                WebSiteTreeViewItems = new ObservableCollection<IWebSiteTreeViewItem>(new DummyWebSiteProvider().GetAllWebSites()),
            };

            TreeViewVm.WebSiteTreeViewItems.Insert(0, new WebSite { IsSelected = false, Title = "WebSite", HasUnreadItem = true, });
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

            applicationSettings = ApplicationSettings.LoadJson(ApplicationSettings.SettingFileName);
            FeedListViewModel = containerProvider.Resolve<FeedListViewModel>();
            FeedListViewModel.PageSize = applicationSettings.PageSize;

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(applicationSettings.AutoUpdateInterval),
                IsEnabled = applicationSettings.AutoUpdateEnabled,
            };

            AutoUpdate = applicationSettings.AutoUpdateEnabled;

            timer.Tick += (_, _) =>
            {
                _ = GetAllSiteRssFeedsCommandAsync.ExecuteAsync();
            };
        }

        public TextWrapper TitleBarText { get; } = new ();

        public TreeViewVm TreeViewVm { get; private init; }

        public bool UiEnabled { get => uiEnabled; set => SetProperty(ref uiEnabled, value); }

        public Logger Logger { get; set; } = new ();

        public bool AutoUpdate
        {
            get => autoUpdate;
            set
            {
                switch (value)
                {
                    case false:
                        timer.Stop();
                        break;
                    case true:
                        timer.Start();
                        break;
                }

                if (SetProperty(ref autoUpdate, value))
                {
                    applicationSettings.AutoUpdateEnabled = autoUpdate;
                    applicationSettings.SaveToJson(ApplicationSettings.SettingFileName);
                }
            }
        }

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

        public DelegateCommand ShowSettingPageCommand => new DelegateCommand(() =>
        {
            dialogService.ShowDialog(nameof(SettingPage), new DialogParameters(), (_) => { });
            var settings = ApplicationSettings.LoadJson(ApplicationSettings.SettingFileName);
            timer.Interval = TimeSpan.FromMinutes(settings.AutoUpdateInterval);
            AutoUpdate = settings.AutoUpdateEnabled;
            FeedListViewModel.PageSize = settings.PageSize;
        });

        /// <summary>
        /// 選択中のウェブサイトのフィード取得を非同期で実行します。<br/>
        /// ウェブサイトを選択していない状態の場合は処理を打ち切ります。<br/>
        /// また、更新を実施したことがログに出力されます。<br/>
        /// </summary>
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

        /// <summary>
        /// 全てのウェブサイトのフィード取得を非同期で実行します。<br/>
        /// また、更新を実施したことがログに出力されます。<br/>
        /// </summary>
        public AsyncDelegateCommand GetAllSiteRssFeedsCommandAsync => new AsyncDelegateCommand(async () =>
        {
            var all = WebSiteService.GetAllWebSites();

            Logger.Log("RSSフィードの更新を開始します。");
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

            TreeViewVm.ReloadTreeViewItems();
            Logger.Log("全てのサイトの更新を行いました。");
            UiEnabled = true;
        });

        /// <summary>
        /// 起動時に実行する全更新のコマンドです。applicationSettings.UpdateOnStartup が true の場合にだけ動作します。
        /// </summary>
        public AsyncDelegateCommand StartupUpdateAsyncCommand => new AsyncDelegateCommand(async () =>
        {
            if (applicationSettings.UpdateOnStartup)
            {
                await GetAllSiteRssFeedsCommandAsync.ExecuteAsync();
            }
        });

        private NgWordService NgWordService { get; set; }

        private FeedService FeedService { get; set; }

        private WebSiteService WebSiteService { get; set; }

        /// <summary>
        /// ウェブサイトリストで選択中のアイテムを確認し、適切な値であれば、FeedListViewModel にセットします。
        /// </summary>
        public void ChangeWebSite()
        {
            var currentSite = TreeViewVm.FindSelectedItem(TreeViewVm.WebSiteTreeViewItems);

            if (currentSite is WebSite site)
            {
                FeedListViewModel.WebSite = site;
            }
        }
    }
}