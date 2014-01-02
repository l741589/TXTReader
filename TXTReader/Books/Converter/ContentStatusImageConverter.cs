using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TXTReader.Books.Converter {
    class ContentStatusImageConverter:IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var cs = (ContentStatus)value;
            switch (cs) {
                case ContentStatus.TooLong: return App.Current.Resources["cs_toolong"];
                case ContentStatus.TooShort: return App.Current.Resources["cs_tooshort"];
                case ContentStatus.ConfusingIndex: return App.Current.Resources["cs_index"];
                case ContentStatus.LowLevelConfusingIndex: return App.Current.Resources["cs_index_green"];
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return null;
        }
    }
}
