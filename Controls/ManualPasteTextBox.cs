using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MakePdf.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MakePdf"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MakePdf;assembly=MakePdf"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ManualPasteTextBox/>
    ///
    /// </summary>
    public class ManualPasteTextBox : TextBox
    {
        public static readonly DependencyProperty PasteCommandProperty =
            DependencyProperty.Register("PasteCommand", typeof(ICommand), typeof(ManualPasteTextBox), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty AllSelectedProperty =
            DependencyProperty.Register("AllSelected", typeof(bool), typeof(ManualPasteTextBox), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty BindableSelectionStartProperty =
            DependencyProperty.Register("BindableSelectionStart", typeof(int), typeof(ManualPasteTextBox), new FrameworkPropertyMetadata(default(int)));

        public static readonly DependencyProperty BindableSelectionLengthProperty =
            DependencyProperty.Register("BindableSelectionLength", typeof(int), typeof(ManualPasteTextBox), new FrameworkPropertyMetadata(default(int)));

        public int BindableSelectionStart
        {
            get { return (int)GetValue(BindableSelectionStartProperty); }
            set { SetValue(BindableSelectionStartProperty, value); }
        }

        public int BindableSelectionLength
        {
            get { return (int)GetValue(BindableSelectionLengthProperty); }
            set { SetValue(BindableSelectionLengthProperty, value); }
        }

        public bool AllSelected
        {
            get { return (bool)GetValue(AllSelectedProperty); }
            set { SetValue(AllSelectedProperty, value); }
        }

        public ICommand PasteCommand
        {
            get { return (ICommand)GetValue(PasteCommandProperty); }
            set { SetValue(PasteCommandProperty, value); }
        }

        static ManualPasteTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ManualPasteTextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
        }

        public ManualPasteTextBox()
        {
            DataObject.AddPastingHandler(this, (sender, args) =>
            {
                if (PasteCommand == null)
                    return;
                var parameter = new PasteParameter
                {
                    ClipboardData = args.DataObject,
                    Processed = false
                };
                PasteCommand.Execute(parameter);
                if (parameter.Processed)
                {
                    args.CancelCommand();
                    args.Handled = true;
                }
            });
        }

        protected override void OnSelectionChanged(RoutedEventArgs e)
        {
            var newValue = SelectionLength == Text.Length;
            if (newValue != AllSelected)
                AllSelected = newValue;
            BindableSelectionStart = SelectionStart;
            BindableSelectionLength = SelectionLength;
        }
    }

    public class PasteParameter
    {
        public IDataObject ClipboardData { set; get; }
        public bool Processed { set; get; }
    }
}
