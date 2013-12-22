﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TXTReader.Data;
using System.Windows;

namespace TXTReader.Converter {
    class BookStateVisibilityConverter :IValueConverter{

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value.ToString() == parameter.ToString()) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
