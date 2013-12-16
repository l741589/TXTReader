using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;

namespace TXTReader.Converter
{
    //TypefaceToFontFamilyCvt
    //used for conversion between System.Windows.Media.Typeface and System.Windows.Media.FontFamily.
    //
    class TypefaceToFontFamilyCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return ((Typeface)value).FontFamily;
            else
                return new FontFamily("Arial");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return new Typeface((FontFamily)value, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            else
                return null;
        }
    }
}
