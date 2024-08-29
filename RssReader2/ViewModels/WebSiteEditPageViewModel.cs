using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RssReader2.Models;
using RssReader2.Models.Dbs;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebSiteEditPageViewModel : BindableBase, IDialogAware
    {
        private string siteUrl;
        private string siteName;
        private List<WebSiteGroup> webSiteGroups;
        private WebSiteGroup currentWebSiteGroup;

        public WebSiteEditPageViewModel(WebSiteGroupService webSiteGroupService)
        {
            WebSiteGroupService = webSiteGroupService;
        }

        public event Action<IDialogResult> RequestClose;

        public WebSiteGroupService WebSiteGroupService { get; set; }

        public string Title => string.Empty;

        public string SiteName { get => siteName; set => SetProperty(ref siteName, value); }

        public string SiteUrl { get => siteUrl; set => SetProperty(ref siteUrl, value); }

        public WebSite WebSite { get; set; }

        public List<WebSiteGroup> WebSiteGroups
        {
            get => webSiteGroups;
            set => SetProperty(ref webSiteGroups, value);
        }

        public WebSiteGroup CurrentWebSiteGroup
        {
            get => currentWebSiteGroup;
            set => SetProperty(ref currentWebSiteGroup, value);
        }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            if (string.IsNullOrWhiteSpace(SiteName) || string.IsNullOrWhiteSpace(SiteUrl))
            {
                return;
            }

            WebSite.Title = SiteName;
            WebSite.Url = SiteUrl;
            if (CurrentWebSiteGroup != null)
            {
                WebSite.GroupId = CurrentWebSiteGroup.Id;
            }
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (!parameters.TryGetValue<WebSite>(nameof(WebSite), out var site))
            {
                return;
            }

            WebSite = site;
            siteName = WebSite.Name;
            siteUrl = WebSite.Url;

            WebSiteGroups = new List<WebSiteGroup>(WebSiteGroupService.GetAllWebSiteGroups());
            CurrentWebSiteGroup = WebSiteGroups.FirstOrDefault(g => g.Id == WebSite.GroupId);
        }
    }
}