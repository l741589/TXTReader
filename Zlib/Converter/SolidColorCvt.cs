using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Zlib.Converter
{
    //SolidColorConverter
    //used for conversion between System.Windows.Media.Color and System.Windows.Media.SolidColorBrush.
    //
    public class SolidColorCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return (value as SolidColorBrush).Color;
            }
            else
            {
                return Colors.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(value.ToString()));
            }
            else
            {
                return null;
            }
        }
    }
}
