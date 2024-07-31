using System.Windows;
using Prism.Ioc;
using RssReader2.Models;
using RssReader2.Models.Dbs;
using RssReader2.ViewModels;
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
            containerRegistry.RegisterDialog<WebSiteAdditionPage, WebSiteAdditionPageViewModel>();
            containerRegistry.RegisterDialog<GroupAdditionPage, GroupAdditionPageViewModel>();

            containerRegistry.RegisterSingleton<DatabaseContext>();
            containerRegistry.RegisterSingleton<IRepository<Feed>, FeedRepository>();
            containerRegistry.RegisterSingleton<IRepository<WebSite>, WebSiteRepository>();
            containerRegistry.RegisterSingleton<IRepository<WebSiteGroup>, WebSiteGroupRepository>();
            containerRegistry.RegisterSingleton<IRepository<NgWord>, NgWordRepository>();

            containerRegistry.RegisterSingleton<FeedService>();
            containerRegistry.RegisterSingleton<WebSiteService>();
            containerRegistry.RegisterSingleton<WebSiteGroupService>();
            containerRegistry.RegisterSingleton<NgWordService>();

            var d = Container.Resolve<DatabaseContext>();
            d.Database.EnsureCreated();

            #if DEBUG
            containerRegistry.Register<IFeedProvider, DummyFeedProvider>();
            containerRegistry.Register<IWebSiteProvider, DummyWebSiteProvider>();
            #else
            containerRegistry.Register<IFeedProvider, FeedService>();
            containerRegistry.Register<IWebSiteProvider, WebSiteService>();
            #endif
        }
    }
}