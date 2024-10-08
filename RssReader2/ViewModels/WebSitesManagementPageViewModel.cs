using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RssReader2.Models;
using RssReader2.Models.Dbs;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebSitesManagementPageViewModel : BindableBase, IDialogAware
    {
        private readonly FeedService feedService;
        private WebSiteService webSiteService;
        private bool hasItemsToDelete;

        public WebSitesManagementPageViewModel(WebSiteService webSiteService, FeedService feedService)
        {
            this.webSiteService = webSiteService;
            this.feedService = feedService;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "WebSite の管理ページ";

        public ObservableCollection<WebSite> WebSites { get; set; }

        /// <summary>
        /// 削除する予定のアイテムが存在するかを取得します。
        /// </summary>
        public bool HasItemsToDelete
        {
            get => hasItemsToDelete;
            private set => SetProperty(ref hasItemsToDelete, value);
        }

        public DelegateCommand ToggleDeleteFlagCommand => new DelegateCommand(() =>
        {
            HasItemsToDelete = WebSites.Any(w => w.IsSelected);
        });

        public DelegateCommand DeleteWebSiteCommand => new DelegateCommand(() =>
        {
            var sites = WebSites.Where(w => w.IsSelected).ToList();

            foreach (var w in sites)
            {
                webSiteService.DeleteWebSite(w.Id);
                feedService.DeleteFeeds(feedService.GetFeedsByWebSiteId(w.Id));
                WebSites.Remove(w);
            }
        });
        
        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            WebSites = new ObservableCollection<WebSite>(
                webSiteService.GetAllWebSites().OrderBy(w => w.Name));
        }
    }
}