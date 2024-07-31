using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RssReader2.Models;
using RssReader2.Models.Dbs;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GroupAdditionPageViewModel : BindableBase, IDialogAware
    {
        private readonly WebSiteGroupService webSiteGroupService;
        private ObservableCollection<WebSiteGroup> webSiteGroups;

        public GroupAdditionPageViewModel(WebSiteGroupService webSiteGroupService)
        {
            this.webSiteGroupService = webSiteGroupService;
            WebSiteGroups = new ObservableCollection<WebSiteGroup>(webSiteGroupService.GetAllWebSiteGroups());
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public string GroupName { get; set; }

        public ObservableCollection<WebSiteGroup> WebSiteGroups
        {
            get => webSiteGroups;
            set => SetProperty(ref webSiteGroups, value);
        }

        public DelegateCommand AddWebSiteGroupCommand => new DelegateCommand(() =>
        {
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                return;
            }

            webSiteGroupService.AddWebSiteGroup(new WebSiteGroup { Name = GroupName, });
            WebSiteGroups = new ObservableCollection<WebSiteGroup>(webSiteGroupService.GetAllWebSiteGroups());
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
        }
    }
}