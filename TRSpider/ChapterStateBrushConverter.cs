using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace TRSpider {
    class ChapterStateBrushConverter : IValueConverter{
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var s = (ChapterDescEx.StandardStates)value;
            switch (s) {
                case ChapterDescEx.StandardStates.FailAll: return Brushes.Red;
                case ChapterDescEx.StandardStates.Fail: return Brushes.LightGray;
                case ChapterDescEx.StandardStates.Pending: return Brushes.Yellow;
                case ChapterDescEx.StandardStates.Success: return Brushes.Green;
                case ChapterDescEx.StandardStates.Ready: return Brushes.Black;
                case ChapterDescEx.StandardStates.NonStandardSuccess: return Brushes.DarkGreen;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
