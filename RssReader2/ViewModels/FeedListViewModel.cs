using System;
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

        private IFeedProvider FeedProvider { get; set; }

        private NgWordService NgWordService { get; set; }

        public void ReloadFeeds(int pageNum)
        {
            if (WebSite == null)
            {
                return;
            }

            var enabledNgWords = NgWordService.GetAllNgWords().Where(w => !w.IsDeleted);
            Feeds = new ObservableCollection<Feed>(ShowUnreadOnly
                    ? FeedProvider.GetUnreadFeedsByWebSiteId(WebSite.Id, PageSize, pageNum, enabledNgWords)
                    : FeedProvider.GetFeedsByWebSiteId(WebSite.Id, PageSize, pageNum, enabledNgWords));

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
    }
}