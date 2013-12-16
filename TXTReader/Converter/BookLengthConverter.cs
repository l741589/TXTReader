using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TXTReader.Converter {
    class BookLengthConverter :IValueConverter{
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            int x = int.Parse(value.ToString());
            if (x==0) return "";
            return x + "字";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            int x = int.Parse(value.ToString());
            if (x == 0) return "";
            return x + "字";
        }
    }
}
