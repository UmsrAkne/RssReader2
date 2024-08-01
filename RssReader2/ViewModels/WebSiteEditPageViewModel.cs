using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebSiteEditPageViewModel : BindableBase, IDialogAware
    {
        private string siteUrl;
        private string siteName;

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public string SiteName { get => siteName; set => SetProperty(ref siteName, value); }

        public string SiteUrl { get => siteUrl; set => SetProperty(ref siteUrl, value); }

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
        }
    }
}