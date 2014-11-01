using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MakePdf.Converters
{
    public class ArithmeticConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double paramValue, valueValue;
            if (!Double.TryParse(value.ToString(), out valueValue))
                return value;
            if (!Double.TryParse(parameter.ToString(), out paramValue))
                return value;
            if (Math.Abs(paramValue) < .001)
                return value;
            return valueValue / paramValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
