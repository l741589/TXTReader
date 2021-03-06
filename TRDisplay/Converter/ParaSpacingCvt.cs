﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TRDisplay.Converter {
    class ParaSpacingCvt : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((Thickness)value).Top;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return new Thickness(0, (double)value, 0, 0); 
        }
    }
}
