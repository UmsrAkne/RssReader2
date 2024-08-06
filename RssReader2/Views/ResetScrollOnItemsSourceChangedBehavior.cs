using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;

namespace RssReader2.Views
{
    public class ResetScrollOnItemsSourceChangedBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TargetUpdated += AssociatedObject_TargetUpdated;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TargetUpdated -= AssociatedObject_TargetUpdated;
        }

        private void AssociatedObject_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (AssociatedObject.Items.Count <= 0)
            {
                return;
            }

            if (AssociatedObject.Items[0] != null)
            {
                AssociatedObject.ScrollIntoView(AssociatedObject.Items[0]);
            }
        }
    }
}