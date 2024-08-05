using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using RssReader2.Models;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FeedListViewModel : BindableBase
    {
        private bool hasNextPage;
        private bool hasPrevPage;
        private int totalPageNumber;
        private int pageSize = 50;
        private int pageNumber;
        private ObservableCollection<Feed> feeds;
        private WebSite webSite;

        public FeedListViewModel(IFeedProvider feedProvider)
        {
            FeedProvider = feedProvider;
        }

        public ObservableCollection<Feed> Feeds { get => feeds; set => SetProperty(ref feeds, value); }

        public int PageNumber { get => pageNumber; set => SetProperty(ref pageNumber, value); }

        public int PageSize { get => pageSize; set => SetProperty(ref pageSize, value); }

        public int TotalPageNumber { get => totalPageNumber; set => SetProperty(ref totalPageNumber, value); }

        public bool HasNextPage { get => hasNextPage; set => SetProperty(ref hasNextPage, value); }

        public bool HasPrevPage { get => hasPrevPage; set => SetProperty(ref hasPrevPage, value); }

        public WebSite WebSite
        {
            get => webSite;
            set
            {
                if (SetProperty(ref webSite, value))
                {
                    ReloadFeeds();
                }
            }
        }

        public DelegateCommand ShowNextPageCommand => new DelegateCommand(() =>
        {
            Feeds = new ObservableCollection<Feed>(
                FeedProvider.GetFeedsByWebSiteId(WebSite.Id, PageSize, ++PageNumber));
        });

        public DelegateCommand ShowPrevPageCommand => new DelegateCommand(() =>
        {
            Feeds = new ObservableCollection<Feed>(
                FeedProvider.GetFeedsByWebSiteId(WebSite.Id, PageSize, --PageNumber));
        });

        private IFeedProvider FeedProvider { get; set; }

        public void ReloadFeeds()
        {
            Feeds = new ObservableCollection<Feed>(
                FeedProvider.GetFeedsByWebSiteId(WebSite.Id, PageSize, PageNumber));

            TotalPageNumber = (int)Math.Floor((double)FeedProvider.GetFeedCountByWebSiteId(WebSite.Id) / PageSize);
            PageNumber = 0;
            RaisePropertyChanged(nameof(HasNextPage));
            RaisePropertyChanged(nameof(HasPrevPage));
        }
    }
}