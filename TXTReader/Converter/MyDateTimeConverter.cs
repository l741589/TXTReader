using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TXTReader.Converter {
    class MyDateTimeConverter : IValueConverter{

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return ((DateTime)value).ToString("yyyy-MM-dd hh:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return DateTime.Parse(value.ToString());
        }
    }
}
