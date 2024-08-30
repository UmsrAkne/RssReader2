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
        private WebSiteService webSiteService;

        public WebSitesManagementPageViewModel(WebSiteService webSiteService)
        {
            this.webSiteService = webSiteService;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => "WebSite の管理ページ";

        public ObservableCollection<WebSite> WebSites { get; set; }

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