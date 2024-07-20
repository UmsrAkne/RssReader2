using System.Windows;
using Prism.Ioc;
using RssReader2.Models;
using RssReader2.Models.Dbs;
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
            containerRegistry.RegisterSingleton<DatabaseContext>();
            containerRegistry.RegisterSingleton<IRepository<Feed>, FeedRepository>();
            containerRegistry.RegisterSingleton<IRepository<WebSite>, WebSiteRepository>();
            containerRegistry.RegisterSingleton<IRepository<WebSiteGroup>, WebSiteGroupRepository>();

            containerRegistry.RegisterSingleton<FeedService>();
            containerRegistry.RegisterSingleton<WebSiteService>();
            containerRegistry.RegisterSingleton<WebSiteGroupService>();

            var d = Container.Resolve<DatabaseContext>();
            d.Database.EnsureCreated();

            #if DEBUG
                containerRegistry.Register<IFeedProvider, DummyFeedProvider>();
            #else
                containerRegistry.Register<IFeedProvider, FeedService>();
            #endif
        }
    }
}