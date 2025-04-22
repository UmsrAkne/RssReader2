using System.Threading;
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
        // ReSharper disable once NotAccessedField.Local
        // 多重起動防止のためにアプリ起動中ずっと保持しておく必要がある。
        // GCされるとMutexが解放されてしまうため、明示的に保持している。
        private static Mutex mutex;

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<WebSiteAdditionPage, WebSiteAdditionPageViewModel>();
            containerRegistry.RegisterDialog<WebSitesManagementPage, WebSitesManagementPageViewModel>();
            containerRegistry.RegisterDialog<GroupAdditionPage, GroupAdditionPageViewModel>();
            containerRegistry.RegisterDialog<WebSiteEditPage, WebSiteEditPageViewModel>();
            containerRegistry.RegisterDialog<NgWordAdditionPage, NgWordAdditionPageViewModel>();
            containerRegistry.RegisterDialog<SettingPage, SettingPageViewModel>();

            containerRegistry.RegisterSingleton<DatabaseContext>();
            containerRegistry.RegisterSingleton<IRepository<Feed>, FeedRepository>();
            containerRegistry.RegisterSingleton<IRepository<WebSite>, WebSiteRepository>();
            containerRegistry.RegisterSingleton<IRepository<WebSiteGroup>, WebSiteGroupRepository>();
            containerRegistry.RegisterSingleton<IRepository<NgWord>, NgWordRepository>();

            containerRegistry.RegisterSingleton<FeedService>();
            containerRegistry.RegisterSingleton<WebSiteService>();
            containerRegistry.RegisterSingleton<WebSiteGroupService>();
            containerRegistry.RegisterSingleton<NgWordService>();
            containerRegistry.RegisterSingleton<FeedListViewModel>();
            containerRegistry.RegisterSingleton<TreeViewVm>();

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

        protected override void OnStartup(StartupEventArgs e)
        {
            #if DEBUG
            const string mutexName = "RssReader2_Mutex_Debug";
            #else
            const string mutexName = "RssReader2_Mutex";
            #endif

            mutex = new Mutex(true, mutexName, out var createdNew);

            if (!createdNew)
            {
                MessageBox.Show("このアプリは既に起動しています。", "多重起動防止", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }
}