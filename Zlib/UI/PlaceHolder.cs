using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Zlib.UI {
    public class PlaceHolder :DependencyObject{


        public static String GetPlaceHolder(DependencyObject obj) {
            return (String)obj.GetValue(PlaceHolderProperty);
        }

        public static void SetPlaceHolder(DependencyObject obj, String value) {
            obj.SetValue(PlaceHolderProperty, value);
        }

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.RegisterAttached("PlaceHolder", typeof(String), typeof(PlaceHolder), new PropertyMetadata((d, e) => {
                if (GetBackground(d) == null) SetBackground(d, (d as Control).Background);
                if (d is TextBox) {
                    (d as TextBox).TextChanged += PlaceHolder_TextChanged;
                    PlaceHolder_TextChanged(d, null);
                }else if (d is PasswordBox){
                    (d as PasswordBox).PasswordChanged += PlaceHolder_PasswordChanged;
                    PlaceHolder_PasswordChanged(d, null);
                }
            }));

        public static Brush GetBackground(DependencyObject obj) {
            return (Brush)obj.GetValue(BackgroundProperty);
        }

        public static void SetBackground(DependencyObject obj, Brush value) {
            obj.SetValue(BackgroundProperty, value);
        }

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.RegisterAttached("Background", typeof(Brush), typeof(PlaceHolder), new PropertyMetadata(null));




        static void PlaceHolder_TextChanged(object sender, TextChangedEventArgs e) {
            TextBox tb = sender as TextBox;
            if (tb.Text == null || tb.Text == "") {
                VisualBrush b = new VisualBrush(new TextBlock() { Text = GetPlaceHolder(tb),FontStyle = FontStyles.Italic }) { TileMode = TileMode.None, Opacity = 0.3, Stretch = Stretch.None, AlignmentX = AlignmentX.Left };
                tb.Background = b;
            } else {
                tb.Background = GetBackground(tb);
            }
        }

        static void PlaceHolder_PasswordChanged(object sender, RoutedEventArgs e) {
            PasswordBox tb = sender as PasswordBox;
            if (tb.Password == null || tb.Password == "") {
                VisualBrush b = new VisualBrush(new TextBlock() { Text = " "+GetPlaceHolder(tb), FontStyle = FontStyles.Italic }) { TileMode = TileMode.None, Opacity = 0.3, Stretch = Stretch.None, AlignmentX = AlignmentX.Left };
                tb.Background = b;
            } else {
                tb.Background = GetBackground(tb);
            }
        }

        
    }
}
