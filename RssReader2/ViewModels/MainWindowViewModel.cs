using System.Collections.ObjectModel;
using Prism.Mvvm;
using RssReader2.Models;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            Feeds = new ObservableCollection<Feed>(new DummyFeedProvider().GetAllFeeds());
        }

        public MainWindowViewModel(IFeedProvider feedProvider)
        {
            FeedProvider = feedProvider;
            Feeds = new ObservableCollection<Feed>(FeedProvider.GetAllFeeds());
        }

        public TextWrapper TitleBarText { get; } = new ();

        public ObservableCollection<Feed> Feeds { get; set; }

        private IFeedProvider FeedProvider { get; set; }
    }
}