using System.Collections.ObjectModel;
using Prism.Mvvm;
using RssReader2.Models;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FeedListViewModel : BindableBase
    {
        public FeedListViewModel(IFeedProvider feedProvider)
        {
            FeedProvider = feedProvider;
        }

        public ObservableCollection<Feed> Feeds { get; set; }

        public int PageNumber { get; set; }

        public int TotalPageNumber { get; set; }

        private IFeedProvider FeedProvider { get; set; }
    }
}