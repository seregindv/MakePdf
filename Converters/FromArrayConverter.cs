using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MakePdf.Converters
{
    public class FromArrayConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var paramArray = parameter as object[];
            if (paramArray == null)
                return value;
            int index;
            try
            {
                index = System.Convert.ToInt32(value);
            }
            catch
            {
                return value;
            }
            if (paramArray.Length <= index)
                return value;
            return paramArray[index];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
