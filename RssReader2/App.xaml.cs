using System.Windows;
using Prism.Ioc;
using RssReader2.Models;
using RssReader2.Views;

namespace RssReader2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            #if DEBUG
                containerRegistry.Register<IFeedProvider, DummyFeedProvider>();
            #else
                containerRegistry.Register<IFeedProvider, FeedService>();
            #endif
        }
    }
}