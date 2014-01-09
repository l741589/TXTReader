using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Diagnostics;

namespace TRBook.Converter {
    class BookmarkConverter : IValueConverter{

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            Debug.Assert(Book.I != Book.Empty, "书为空时不能有书签");
            switch (parameter as String) {                
                case "progress":{
                    double x=(int)value;
                    int y=Book.I.TotalText.Count;
                    return String.Format("{0:00.00}%({1}/{2})",x*100/y,x,y);
                }
                case "progressbar":{
                    double x=(int)value;
                    int y=Book.I.TotalText.Count;
                    return x*100 / y;
                }
                case "preview": default:{
                    return Book.I.GetPreview((int)value);
                    /*int i = (int)value;
                    String ret = Book.I.TotalText[i++];
                    while (ret.Length < 256 && i < Book.I.TotalText.Count)
                        ret += "\n" + Book.I.TotalText[i++];
                    return ret;*/
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return Book.I.TotalText.IndexOf(value.ToString());
        }
    }
}
