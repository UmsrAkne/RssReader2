using System.Windows;
using System.Windows.Controls;

namespace RssReader2.Views
{
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();
        }
    }
}