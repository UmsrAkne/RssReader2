using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RssReader2.Models;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SettingPageViewModel : BindableBase, IDialogAware
    {
        private ApplicationSettings applicationSettings = new ();

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public ApplicationSettings ApplicationSettings
        {
            get => applicationSettings;
            private set => SetProperty(ref applicationSettings, value);
        }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            ApplicationSettings.SaveToJson(ApplicationSettings.SettingFileName);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            ApplicationSettings = ApplicationSettings.LoadJson(ApplicationSettings.SettingFileName);
        }
    }
}