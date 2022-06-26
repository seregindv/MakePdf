using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace MakePdf.Controls
{
    public class DoubleClickCollapseGridSplitterBehavior : Behavior<GridSplitter>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDoubleClick += AssociatedObject_PreviewMouseDoubleClick;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseDoubleClick -= AssociatedObject_PreviewMouseDoubleClick;
            base.OnDetaching();
        }

        void AssociatedObject_PreviewMouseDoubleClick
            (object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var splitter = sender as GridSplitter;
            if (splitter == null)
                return;

            var parent = splitter.Parent as Grid;
            if (parent == null)
                return;

            var splitterWidth = (double)splitter.GetValue(FrameworkElement.WidthProperty);
            if (Double.IsNaN(splitterWidth))
            {
                var row = (int)splitter.GetValue(Grid.RowProperty);
                var splitterHeight = (double)splitter.GetValue(FrameworkElement.HeightProperty);
                var rowHeight = (GridLength)parent.RowDefinitions[row].GetValue(RowDefinition.HeightProperty);
                //System.Diagnostics.Debug.WriteLine("Row Height = {0}, rowheightvalue = {1}, actualrowhe={2}", rowHeight, rowHeight.Value);
                parent.RowDefinitions[row].SetValue
                    (RowDefinition.HeightProperty, rowHeight.Value >= splitterHeight + .25 ? new GridLength(splitterHeight) : new GridLength((parent.RowDefinitions[row - 1].ActualHeight + parent.RowDefinitions[row].ActualHeight) / 2.0));
            }
            else
            {
                var col = (int)splitter.GetValue(Grid.ColumnProperty);
                parent.ColumnDefinitions[col].SetValue
                    (ColumnDefinition.WidthProperty, new GridLength(splitterWidth));
            }
        }
    }
}
