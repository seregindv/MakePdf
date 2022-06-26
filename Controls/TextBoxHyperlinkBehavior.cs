using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Media;

namespace MakePdf.Controls
{
    public class TextBoxHyperlinkBehavior : Behavior<TextBox>
    {
        private bool _decorated = false;

        private static DependencyObject FindTopAncestor(DependencyObject obj)
        {
            while (true)
            {
                var prevObj = obj;
                obj = VisualTreeHelper.GetParent(obj);
                if (obj == null)
                    return prevObj;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            var window = FindTopAncestor(AssociatedObject);
            if (window != null)
            {
                Keyboard.AddPreviewKeyDownHandler(window, TextBox_KeyDown);
                Keyboard.AddPreviewKeyUpHandler(window, TextBox_KeyUp);
            }
            AssociatedObject.MouseEnter += TextBox_MouseEnter;
            AssociatedObject.MouseLeave += TextBox_MouseLeave;
            AssociatedObject.PreviewKeyDown += TextBox_KeyDown;
            AssociatedObject.PreviewKeyUp += TextBox_KeyUp;
            AssociatedObject.PreviewMouseLeftButtonDown += TextBox_PreviewMouseLeftButtonDown;
        }

        void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_decorated)
                return;
            System.Diagnostics.Process.Start(AssociatedObject.Text);
            e.Handled = true;
        }

        void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            Decorate();
        }

        void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            Decorate();
        }

        protected override void OnDetaching()
        {
            var window = FindTopAncestor(AssociatedObject);
            if (window != null)
            {
                Keyboard.RemovePreviewKeyDownHandler(window, TextBox_KeyDown);
                Keyboard.RemovePreviewKeyUpHandler(window, TextBox_KeyUp);
            }
            AssociatedObject.MouseEnter -= TextBox_MouseEnter;
            AssociatedObject.MouseLeave -= TextBox_MouseLeave;
            AssociatedObject.PreviewKeyDown -= TextBox_KeyDown;
            AssociatedObject.PreviewKeyUp -= TextBox_KeyUp;
            AssociatedObject.PreviewMouseLeftButtonDown -= TextBox_PreviewMouseLeftButtonDown;
            base.OnDetaching();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                Decorate();
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                Decorate();
            }
        }

        private void Decorate()
        {
            if (AssociatedObject.IsMouseOver && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                if (!_decorated)
                {
                    AssociatedObject.TextDecorations.Add(new TextDecoration { Location = TextDecorationLocation.Underline });
                    AssociatedObject.Cursor = Cursors.Hand;
                    //System.Diagnostics.Debug.WriteLine("Decorated");
                    _decorated = true;
                }
            }
            else
            {
                if (_decorated)
                {
                    var d =
                        AssociatedObject.TextDecorations.FirstOrDefault(
                            decor => decor.Location == TextDecorationLocation.Underline);
                    if (d != null)
                        AssociatedObject.TextDecorations.Remove(d);
                    AssociatedObject.Cursor = Cursors.Arrow;
                    //System.Diagnostics.Debug.WriteLine("Undecorated [{0}]", d);
                    _decorated = false;
                }
            }
        }
    }
}
