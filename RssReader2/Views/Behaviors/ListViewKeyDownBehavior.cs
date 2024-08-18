using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

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
            }

            lv.ScrollIntoView(lv.SelectedItem);
        }
    }
}