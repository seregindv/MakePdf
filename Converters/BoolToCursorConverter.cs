using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;

namespace MakePdf.Converters
{
    public class BoolToCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBusy;
            try
            {
                isBusy = (bool)value;
            }
            catch
            {
                isBusy = false;
            }

            var busyCursor = parameter as Cursor;
            return isBusy ? (busyCursor ?? Cursors.AppStarting) : Cursors.Arrow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
