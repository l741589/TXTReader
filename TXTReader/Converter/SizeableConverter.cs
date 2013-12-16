using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace TXTReader.Converter {
    class SizeableConverter : IValueConverter{

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if ((bool?)value==true) return ResizeMode.CanResize;
            else return ResizeMode.NoResize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
