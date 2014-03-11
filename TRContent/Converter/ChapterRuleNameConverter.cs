using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Zlib.Algorithm;

namespace TRContent.Converter {
    class ChapterRuleNameConverter : IValueConverter{
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) return null;
            if (!A.IsFullFilename(value.ToString())) return value.ToString();
            return A.DecodeFilename(Path.GetFileNameWithoutExtension(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
