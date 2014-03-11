using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using TRSpider;

namespace TRSpider {
    class LogStateBrushConverter : IValueConverter{

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var s = (DownloadingChapterEventArgs.States)value;
            var r = StateToBrush(s);
            if (parameter!=null&&parameter.Equals("F")) {
                if ((r.Color.R + r.Color.G + r.Color.B) / 3 > 128) r = Brushes.Black;
                else r = Brushes.White;
            }
            return r;            
        }

        private SolidColorBrush StateToBrush(DownloadingChapterEventArgs.States s) {
            switch (s) {
                case DownloadingChapterEventArgs.States.AllFail: return Brushes.Red;
                case DownloadingChapterEventArgs.States.Exception: return Brushes.DarkRed;
                case DownloadingChapterEventArgs.States.MatchFail: return Brushes.DarkBlue;
                case DownloadingChapterEventArgs.States.MatchSuccess: return Brushes.Cyan;
                case DownloadingChapterEventArgs.States.NonstandardFail: return Brushes.LightGray;
                case DownloadingChapterEventArgs.States.NonstandardSuccess: return Brushes.DarkGreen;
                case DownloadingChapterEventArgs.States.StandardFail: return Brushes.Gray;
                case DownloadingChapterEventArgs.States.StandardSuccess: return Brushes.Green;
                case DownloadingChapterEventArgs.States.ValidateFail: return Brushes.Purple;
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
