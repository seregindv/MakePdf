using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MakePdf.Tmp
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MakePdf.Tmp"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MakePdf.Tmp;assembly=MakePdf.Tmp"
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
    ///     <MyNamespace:DatePickerEx/>
    ///
    /// </summary>
    [TemplatePart(Name = "PART_TextBox", Type = typeof(DatePickerTextBox))]
    public class DatePickerEx : DatePicker
    {
        static DatePickerEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePickerEx), new FrameworkPropertyMetadata(typeof(DatePicker)));
        }

        private static readonly DependencyPropertyKey IsDateOutOfRangePropertyKey = DependencyProperty.RegisterReadOnly(
            "IsDateOutOfRange", typeof(bool), typeof(DatePickerEx), new PropertyMetadata(false));
        public bool IsDateOutOfRange
        {
            get { return (bool)GetValue(IsDateOutOfRangePropertyKey.DependencyProperty); }
            private set { SetValue(IsDateOutOfRangePropertyKey, value); }
        }

        public static readonly DependencyProperty IsMonthProperty = DependencyProperty.Register(
            "IsMonth", typeof(bool), typeof(DatePickerEx), new PropertyMetadata(false));
        public bool IsMonth
        {
            get { return (bool)GetValue(IsMonthProperty); }
            set { SetValue(IsMonthProperty, value); }
        }

        private static readonly DependencyPropertyKey ErrorTextPropertyKey = DependencyProperty.RegisterReadOnly(
            "ErrorText", typeof(string), typeof(DatePickerEx), new PropertyMetadata(String.Empty));
        public string ErrorText
        {
            get { return (string)GetValue(ErrorTextPropertyKey.DependencyProperty); }
            private set { SetValue(ErrorTextPropertyKey, value); }
        }

        private DatePickerTextBox _datePickerTextBox;
        private DateTime? _savedDate;
        private DateTime? _savedStartDate, _savedEndDate;

        public override void OnApplyTemplate()
        {
            if (_datePickerTextBox != null)
            {
                _datePickerTextBox.TextChanged -= DatePickerTextBox_TextChanged;
                _datePickerTextBox.LostFocus -= DatePickerTextBox_LostFocus;
                _datePickerTextBox.GotFocus -= DatePickerTextBox_GotFocus;
            }
            base.OnApplyTemplate();
            _datePickerTextBox = GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            if (_datePickerTextBox != null)
            {
                _datePickerTextBox.TextChanged += DatePickerTextBox_TextChanged;
                _datePickerTextBox.LostFocus += DatePickerTextBox_LostFocus;
                _datePickerTextBox.GotFocus += DatePickerTextBox_GotFocus;
            }
        }

        private void DatePickerTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _savedDate = SelectedDate;
            _savedStartDate = DisplayDateStart;
            _savedEndDate = DisplayDateEnd;
            ErrorText = GetTooltipText();
        }

        private string GetTooltipText()
        {
            // replace to read from resource
            if (_savedStartDate != DateTime.MinValue && _savedStartDate != DateTime.MaxValue)
                return String.Format("Date must be between {0:d} and {1:d}", _savedStartDate, _savedEndDate);
            if (_savedStartDate == DateTime.MinValue && _savedEndDate != DateTime.MaxValue)
                return String.Format("Date must be less or equal {0:d}", _savedEndDate);
            if (_savedStartDate != DateTime.MinValue && _savedEndDate == DateTime.MaxValue)
                return String.Format("Date must be greater or equal {0:d}", _savedStartDate);
            return String.Empty;
        }

        private void DatePickerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as DatePickerTextBox;
            if (textBox == null)
                return;
            //Debug.WriteLine("DatePickerTextBox_LostFocus, text = " + textBox.Text);
            DateTime dateValue;
            if (DateTime.TryParse(textBox.Text, out dateValue) && IsOutOfRange(dateValue))
            {
                SelectedDate = GetCorrectedDate();
                IsDateOutOfRange = false;
                //Debug.WriteLine("DatePickerTextBox_LostFocus: selected date set to " + _savedDate);
            }
        }

        private DateTime? GetCorrectedDate()
        {
            if (IsMonth && SelectedDate > _savedEndDate && SelectedDate.Value.Month == _savedEndDate.Value.Month)
                return GetFirstDay(_savedEndDate.Value);
            return _savedDate;
        }

        private DateTime GetFirstDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        private bool IsOutOfRange(DateTime dateValue)
        {
            return dateValue < _savedStartDate || dateValue > _savedEndDate;
        }

        private void DatePickerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_savedStartDate == DateTime.MinValue && _savedEndDate == DateTime.MaxValue)
                return;
            var textBox = sender as DatePickerTextBox;
            if (textBox == null)
                return;
            DateTime dateValue;
            IsDateOutOfRange = DateTime.TryParse(textBox.Text, out dateValue)
                && IsOutOfRange(dateValue);
            //Debug.Write(textBox.Text + "-" + parsed);
            //if (parsed) Debug.WriteLine("-" + dateValue);
            //else Debug.WriteLine(string.Empty);
        }
    }
}
