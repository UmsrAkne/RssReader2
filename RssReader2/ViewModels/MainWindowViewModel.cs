﻿using System.Collections.ObjectModel;
using Prism.Ioc;
using Prism.Mvvm;
using RssReader2.Models;
using RssReader2.Models.Dbs;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            Feeds = new ObservableCollection<Feed>(new DummyFeedProvider().GetAllFeeds());
            WebSiteTreeViewItems = new ObservableCollection<IWebSiteTreeViewItem>(new DummyWebSiteProvider().GetAllWebSites());
        }

        public MainWindowViewModel(IContainerProvider containerProvider)
        {
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

        private IFeedProvider FeedProvider { get; set; }
    }
}