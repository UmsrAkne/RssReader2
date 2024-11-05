using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using RssReader2.Models;
using RssReader2.Models.Dbs;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FeedListViewModel : BindableBase
    {
        private bool hasNextPage;
        private bool hasPrevPage;
        private int totalPageNumber;
        private int pageSize = 50;
        private int pageNumber = 1;
        private ObservableCollection<Feed> feeds = new ();
        private WebSite webSite;
        private Feed selectedItem;
        private bool showUnreadOnly;

        public FeedListViewModel(IFeedProvider feedProvider, NgWordService ngWordService)
        {
            FeedProvider = feedProvider;
            NgWordService = ngWordService;
        }

        public ObservableCollection<Feed> Feeds { get => feeds; private set => SetProperty(ref feeds, value); }

        public int PageNumber { get => pageNumber; private set => SetProperty(ref pageNumber, value); }

        public int PageSize { get => pageSize; set => SetProperty(ref pageSize, value); }

        public int TotalPageNumber { get => totalPageNumber; private set => SetProperty(ref totalPageNumber, value); }

        public bool HasNextPage { get => hasNextPage; set => SetProperty(ref hasNextPage, value); }

        public bool HasPrevPage { get => hasPrevPage; set => SetProperty(ref hasPrevPage, value); }

        public bool ShowUnreadOnly { get => showUnreadOnly; set => SetProperty(ref showUnreadOnly, value); }

        public Feed SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public WebSite WebSite
        {
            get => webSite;
            set
            {
                if (SetProperty(ref webSite, value))
                {
                    ReloadFeeds(1);
                }
            }
        }

        public DelegateCommand NextPageCommand => new DelegateCommand(() =>
        {
            ReloadFeeds(++PageNumber);
        });

        public DelegateCommand PrevPageCommand => new DelegateCommand(() =>
        {
            ReloadFeeds(--PageNumber);
        });

        public DelegateCommand ReloadUnReadFeedsCommand => new DelegateCommand(() =>
        {
            ReloadFeeds(0);
        });

        public DelegateCommand SkipToOldestUnreadCommand => new DelegateCommand(SkipToOldestUnread);

        public DelegateCommand<Feed> UpdateIsReadPropertyCommand => new DelegateCommand<Feed>((param) =>
        {
            if (param is { IsRead: false, })
            {
                param.IsRead = true;
                FeedProvider.UpdateFeed(param);
            }
        });

        public DelegateCommand OpenUrlCommand => new DelegateCommand(() =>
        {
            if (SelectedItem == null || SelectedItem.ContainsNgWord)
            {
                return;
            }

            var pi = new ProcessStartInfo()
            {
                FileName = SelectedItem.Url,
                UseShellExecute = true,
            };

            Process.Start(pi);
        });

        public DelegateCommand MarkNgWordFeedsAsReadCommand => new DelegateCommand(() =>
        {
            if (WebSite == null)
            {
                return;
            }

            FeedProvider.MarkFeedsAsReadByWebSiteId(WebSite.Id, includeNgWord: true);
            ReloadFeeds(1);
        });

        /// <summary>
        /// 現在選択中のウェブサイトのフィードを全て既読に変更します。
        /// </summary>
        public DelegateCommand MarkAllFeedsAsReadCommand => new DelegateCommand(() =>
        {
            if (WebSite == null)
            {
                return;
            }

            FeedProvider.MarkFeedsAsReadByWebSiteId(WebSite.Id, includeNgWord: false);
            ReloadFeeds(1);
        });

        private IFeedProvider FeedProvider { get; set; }

        private NgWordService NgWordService { get; set; }

        public void RevertToUnread()
        {
            if (SelectedItem is not { IsRead: true, })
            {
                return;
            }

            SelectedItem.IsRead = false;
            FeedProvider.UpdateFeed(SelectedItem);
        }

        public void ToggleMark()
        {
            if (SelectedItem is not { IsRead: true, })
            {
                return;
            }

            SelectedItem.IsMarked = !SelectedItem.IsMarked;
            FeedProvider.UpdateFeed(SelectedItem);
        }

        public void ReloadFeeds(int pageNum)
        {
            if (WebSite == null)
            {
                return;
            }

            var enabledNgWords = NgWordService.GetAllNgWords().Where(w => !w.IsDeleted);

            var fs = ShowUnreadOnly
                ? FeedProvider.GetUnreadFeedsByWebSiteId(WebSite.Id, PageSize, pageNum, enabledNgWords)
                : FeedProvider.GetFeedsByWebSiteId(WebSite.Id, PageSize, pageNum, enabledNgWords);

            Feeds = new ObservableCollection<Feed>(fs.OrderBy(f => f.ContainsNgWord));

            pageNum = Math.Max(pageNum, 1);

            for (var i = 0; i < Feeds.Count; i++)
            {
                feeds[i].LineNumber = ShowUnreadOnly
                    ? i + 1
                    : ((pageNum - 1) * pageSize) + i + 1;
            }

            if (ShowUnreadOnly)
            {
                return;
            }

            TotalPageNumber = (int)Math.Ceiling((double)FeedProvider.GetFeedCountByWebSiteId(WebSite.Id) / PageSize);
            PageNumber = pageNum;
            HasNextPage = TotalPageNumber >= 2 && PageNumber < TotalPageNumber;
            HasPrevPage = PageNumber > 1;
            RaisePropertyChanged(nameof(HasNextPage));
            RaisePropertyChanged(nameof(HasPrevPage));
        }

        /// <summary>
        /// 未読のフィードが存在する、一番古いページまでジャンプします。
        /// </summary>
        /// <remarks>
        /// <para>このメソッドの動作には穴があります。</para>
        /// <para>例えば、複数のページがあり、 3 ページ目が全て既読だった場合、 4 ページ目に未読が存在しても 2 ページ目にジャンプします。
        /// これは、 3 ページ目の確認の時点で検索を打ち切るためです。パフォーマンスを優先した結果、このような仕様となっています。</para>
        /// <para>今後、実際に機能を使い込んでみて、不便そうなら修正します。</para>
        /// </remarks>
        private void SkipToOldestUnread()
        {
             if (WebSite == null)
             {
                 return;
             }

             var p = 0;
             bool hasUnreadFeeds;
             do
             {
                 p++;

                 // このループの目的は、古い未読フィードが存在するページの検索であるため、NGワードリストは空のリストを使い、処理を省く。
                 hasUnreadFeeds =
                     FeedProvider.GetFeedsByWebSiteId(WebSite.Id, PageSize, p, new List<NgWord>())
                     .Any(f => !f.IsRead);
             }
             while (hasUnreadFeeds);

             ReloadFeeds(p - 1);
        }
    }
}