using Prism.Mvvm;
using RssReader2.Models;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        public TextWrapper TitleBarText { get; } = new ();
    }
}