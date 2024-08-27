using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using RssReader2.ViewModels;

namespace RssReader2.Views.Behaviors
{
    public class ListViewKeyDownBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += OnKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not ListView lv || lv.Items.Count == 0)
            {
                return;
            }

            var dt = lv.DataContext;
            MainWindowViewModel vm;

            switch (e.Key)
            {
                case Key.J:
                    if (lv.SelectedIndex < lv.Items.Count - 1)
                    {
                        lv.SelectedIndex++;
                    }

                    break;
                case Key.K:
                    if (lv.SelectedIndex > 0)
                    {
                        lv.SelectedIndex--;
                    }

                    break;

                case Key.L:
                    vm = dt as MainWindowViewModel;
                    vm?.FeedListViewModel.NextPageCommand.Execute();
                    break;

                case Key.H:
                    vm = dt as MainWindowViewModel;
                    vm?.FeedListViewModel.PrevPageCommand.Execute();
                    break;

                case Key.U:
                    vm = dt as MainWindowViewModel;
                    vm?.FeedListViewModel.RevertToUnread();
                    break;

                case Key.M:
                    vm = dt as MainWindowViewModel;
                    vm?.FeedListViewModel.ToggleMark();
                    break;
            }

            lv.ScrollIntoView(lv.SelectedItem);
        }
    }
}