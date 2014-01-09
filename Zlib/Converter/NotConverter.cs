using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Zlib.Converter {
    public class NotConverter :IValueConverter{

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (parameter == null) return !(bool)value;
            else return (parameter as IValueConverter).Convert(!(bool)value, targetType, null, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (parameter == null) return !(bool)value;
            else return (parameter as IValueConverter).ConvertBack(!(bool)value, targetType, null, culture);
        }
    }
}
