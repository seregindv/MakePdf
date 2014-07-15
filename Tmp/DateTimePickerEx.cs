using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MakePdf.Tmp
{
    public class DateTimePickerEx : DatePicker
    {
        private DatePickerTextBox _datePickerTextBox;
        private DateTime? _savedDate;
        private Brush _datePickerTextBoxForeground;
        private DateTime? _savedStartDate, _savedEndDate;
        private ToolTip _tooltip;

        private ToolTip GetTooltip(string text)
        {
            if (_tooltip == null)
            {
                _tooltip = new ToolTip
                {
                    IsEnabled = true,
                    Placement = PlacementMode.Top,
                    PlacementTarget = this,
                    StaysOpen = true
                };
            }
            _tooltip.Content = text;
            return _tooltip;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_datePickerTextBox != null)
            {
                _datePickerTextBox.TextChanged -= DatePickerTextBox_TextChanged;
                _datePickerTextBox.LostFocus -= DatePickerTextBox_LostFocus;
                _datePickerTextBox.GotFocus -= DatePickerTextBox_GotFocus;
            }
            _datePickerTextBox = GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            if (_datePickerTextBox != null)
            {
                _datePickerTextBox.TextChanged += DatePickerTextBox_TextChanged;
                _datePickerTextBox.LostFocus += DatePickerTextBox_LostFocus;
                _datePickerTextBox.GotFocus += DatePickerTextBox_GotFocus;
                _datePickerTextBoxForeground = _datePickerTextBox.Foreground;
            }
        }

        private void DatePickerTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _savedDate = SelectedDate;
            _savedStartDate = DisplayDateStart;
            _savedEndDate = DisplayDateEnd;
            if (ToolTip == null)
            {
                ToolTipService.SetToolTip(_datePickerTextBox, GetTooltip(GetTooltipText()));
            }
        }

        private string GetTooltipText()
        {
            if (_savedStartDate != DateTime.MinValue && _savedStartDate != DateTime.MaxValue)
                return String.Format("Date must be between {0:d} and {1:d}", _savedStartDate, _savedEndDate);
            if (_savedStartDate == DateTime.MinValue && _savedStartDate != DateTime.MaxValue)
                return String.Format("Date must be greater or equal {0:d}", _savedStartDate);
            if (_savedStartDate != DateTime.MinValue && _savedStartDate == DateTime.MaxValue)
                return String.Format("Date must be greater or equal {0:d}", _savedEndDate);
            return String.Empty;
        }

        private void DatePickerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as DatePickerTextBox;
            if (textBox == null)
                return;
            Debug.WriteLine("DatePickerTextBox_LostFocus, text = " + textBox.Text);
            DateTime dateValue;
            if (DateTime.TryParse(textBox.Text, out dateValue) && IsOutOfRange(dateValue))
            {
                SelectedDate = _savedDate;
                textBox.Foreground = _datePickerTextBoxForeground;
                Debug.WriteLine("DatePickerTextBox_LostFocus: selected date set to " + _savedDate);
            }
            _tooltip.IsOpen = false;
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
            var parsed = DateTime.TryParse(textBox.Text, out dateValue);
            bool outOfRange = parsed && IsOutOfRange(dateValue);
            _datePickerTextBox.Foreground = outOfRange
                ? Brushes.Red
                : _datePickerTextBoxForeground;
            _tooltip.IsOpen = outOfRange;
            Debug.Write(textBox.Text + "-" + parsed);
            if (parsed) Debug.WriteLine("-" + dateValue);
            else Debug.WriteLine(string.Empty);
        }
    }
}
