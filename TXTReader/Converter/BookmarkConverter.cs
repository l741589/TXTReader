using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TXTReader.Utility;

namespace TXTReader.Converter {
    class BookmarkConverter : IValueConverter{

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            switch (parameter as String) {                
                case "progress":{
                    double x=(int)value;
                    int y=G.Book.TotalText.Count;
                    return String.Format("{0:00.00}%({1}/{2})",x*100/y,x,y);
                }
                case "progressbar":{
                    double x=(int)value;
                    int y=G.Book.TotalText.Count;
                    return x*100 / y;
                }
                case "preview": default:{
                    return G.Book.GetPreview((int)value);
                    /*int i = (int)value;
                    String ret = G.Book.TotalText[i++];
                    while (ret.Length < 256 && i < G.Book.TotalText.Count)
                        ret += "\n" + G.Book.TotalText[i++];
                    return ret;*/
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return G.Book.TotalText.IndexOf(value.ToString());
        }
    }
}
