using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TXTReader.Converter
{
    class ToolPanelWidthCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double x = double.Parse(value.ToString());
            double tar = x / 3;
            if (tar > 512) tar = 512;
            if (tar < 256) tar = 256;
            return tar;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
