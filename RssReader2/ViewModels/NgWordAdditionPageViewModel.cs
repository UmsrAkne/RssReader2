using System;
using System.Collections.ObjectModel;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RssReader2.Models;
using RssReader2.Models.Dbs;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NgWordAdditionPageViewModel : BindableBase, IDialogAware
    {
        private readonly NgWordService ngWordService;
        private string ngWordText;
        private Visibility listVisibility = Visibility.Hidden;

        public NgWordAdditionPageViewModel(NgWordService ngWordService)
        {
            this.ngWordService = ngWordService;
        }

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public string NgWordText { get => ngWordText; set => SetProperty(ref ngWordText, value); }

        public ObservableCollection<NgWord> NgWords { get; set; }

        public Visibility ListVisibility
        {
            get => listVisibility;
            set => SetProperty(ref listVisibility, value);
        }

        public DelegateCommand AddNgWordCommand => new DelegateCommand(() =>
        {
            if (string.IsNullOrWhiteSpace(NgWordText))
            {
                return;
            }

            ngWordService.AddNgWord(new NgWord { Word = NgWordText, });
            NgWords = new ObservableCollection<NgWord>(ngWordService.GetAllNgWords());
            RaisePropertyChanged(nameof(NgWords));
        });

        public DelegateCommand ToggleListVisibilityCommand => new DelegateCommand(() =>
        {
            ListVisibility = ListVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        });

        public DelegateCommand CloseCommand => new (() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            NgWords = new ObservableCollection<NgWord>(ngWordService.GetAllNgWords());
            RaisePropertyChanged(nameof(NgWords));
        }
    }
}