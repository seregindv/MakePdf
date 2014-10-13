using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MakePdf.Dialog
{
    public partial class ModalDialog
    {
        public ModalDialog()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
        }

        private bool _result = false;
        private UIElement _parent;

        #region Properties

        public static readonly DependencyProperty IsShowingProperty = DependencyProperty.Register(
            "IsShowing", typeof(bool), typeof(ModalDialog), new PropertyMetadata(default(bool), ShowingChangedCallback));

        public bool IsShowing
        {
            get { return (bool)GetValue(IsShowingProperty); }
            set { SetValue(IsShowingProperty, value); }
        }

        #endregion

        private void ShowHandlerDialog()
        {
            //SetParentIsEnable(false);
            Visibility = Visibility.Visible;
        }

        private void HideHandlerDialog()
        {
            Visibility = Visibility.Hidden;
            //SetParentIsEnable(true);
        }

        private void SetParentIsEnable(bool enabled)
        {
            if (_parent == null)
                _parent = GetParent<UIElement>(this);
            if (_parent != null)
                _parent.IsEnabled = enabled;
        }

        private static T GetParent<T>(DependencyObject o) where T : class
        {
            var parent = LogicalTreeHelper.GetParent(o);
            if (parent == null)
                return null;
            var element = parent as T;
            if (element != null)
                return element;
            return GetParent<T>(parent);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _result = true;
            IsShowing = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _result = false;
            IsShowing = false;
        }

        private static void ShowingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool || e.OldValue is bool))
                return;
            var newValue = (bool)e.NewValue;
            if (newValue == (bool)e.OldValue)
                return;
            var dialog = d as ModalDialog;
            if (dialog == null)
                return;
            if (newValue)
                dialog.ShowHandlerDialog();
            else
                dialog.HideHandlerDialog();
        }
    }
}
