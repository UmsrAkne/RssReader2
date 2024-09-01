using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using RssReader2.Models;
using RssReader2.ViewModels;

namespace RssReader2.Views
{
    public class TreeViewSelectedItemBehavior : Behavior<TreeView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += OnSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectedItemChanged -= OnSelectedItemChanged;
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // DataContextを取得してMainViewModelのプロパティをコールする
            if (AssociatedObject.DataContext is MainWindowViewModel viewModel)
            {
                viewModel.UpdateFeedsCommand.Execute();
                viewModel.TreeViewVm.SelectedItem = (IWebSiteTreeViewItem)((TreeView)sender).SelectedItem;
            }
        }
    }
}