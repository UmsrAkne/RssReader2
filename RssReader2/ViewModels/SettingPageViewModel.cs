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
        private int autoUpdateInterval = 90;
        private bool autoUpdateEnabled;

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public int AutoUpdateInterval
        {
            get => autoUpdateInterval;
            set => SetProperty(ref autoUpdateInterval, value);
        }

        public bool AutoUpdateEnabled
        {
            get => autoUpdateEnabled;
            set => SetProperty(ref autoUpdateEnabled, value);
        }

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            var s = new ApplicationSettings
            {
                AutoUpdateInterval = AutoUpdateInterval,
                AutoUpdateEnabled = AutoUpdateEnabled,
            };

            s.SaveToJson(ApplicationSettings.SettingFileName);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var s = ApplicationSettings.LoadJson(ApplicationSettings.SettingFileName);
            AutoUpdateInterval = s.AutoUpdateInterval;
            AutoUpdateEnabled = s.AutoUpdateEnabled;
        }
    }
}