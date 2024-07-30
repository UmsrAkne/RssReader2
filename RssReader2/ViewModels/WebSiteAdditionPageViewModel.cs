using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RssReader2.Models;
using RssReader2.Models.Dbs;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebSiteAdditionPageViewModel : BindableBase, IDialogAware
    {
        private readonly WebSiteService webSiteService;
        private string siteName;
        private string siteUrl;

        public WebSiteAdditionPageViewModel(WebSiteService webSiteService)
        {
            this.webSiteService = webSiteService;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public string SiteName { get => siteName; set => SetProperty(ref siteName, value); }

        public string SiteUrl { get => siteUrl; set => SetProperty(ref siteUrl, value); }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        /// <summary>
        /// ビューに入力されているテキストからWebSiteを生成し、データベースに追加します。
        /// ただし、入力されているサイト名、またはURLが空白の場合は処理をしません。
        /// </summary>
        public DelegateCommand AddWebSiteCommand => new DelegateCommand(() =>
        {
            if (string.IsNullOrWhiteSpace(SiteName) || string.IsNullOrWhiteSpace(siteUrl))
            {
                return;
            }

            webSiteService.AddWebSite(
                new WebSite
                {
                    Title = SiteName,
                    Url = SiteUrl,
                    GroupId = 0,
                });

            SiteName = string.Empty;
            SiteUrl = string.Empty;
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}