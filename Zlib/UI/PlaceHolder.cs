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



        public static String GetText(DependencyObject obj) {
            return (String)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, String value) {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(String), typeof(PlaceHolder), new PropertyMetadata((d, e) => {
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



        public static AlignmentX GetAlignmentX(DependencyObject obj) {
            return (AlignmentX)obj.GetValue(AlignmentXProperty);
        }

        public static void SetAlignmentX(DependencyObject obj, AlignmentX value) {
            obj.SetValue(AlignmentXProperty, value);
        }

        public static readonly DependencyProperty AlignmentXProperty =
            DependencyProperty.RegisterAttached("AlignmentX", typeof(AlignmentX), typeof(PlaceHolder), new PropertyMetadata(AlignmentX.Left));



        public static AlignmentY GetAlignmentY(DependencyObject obj) {
            return (AlignmentY)obj.GetValue(AlignmentYProperty);
        }

        public static void SetAlignmentY(DependencyObject obj, AlignmentY value) {
            obj.SetValue(AlignmentYProperty, value);
        }

        // Using a DependencyProperty as the backing store for AlignmentY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlignmentYProperty =
            DependencyProperty.RegisterAttached("AlignmentY", typeof(AlignmentY), typeof(PlaceHolder), new PropertyMetadata(AlignmentY.Center));


        static void PlaceHolder_TextChanged(object sender, TextChangedEventArgs e) {
            TextBox tb = sender as TextBox;
            if (tb.Text == null || tb.Text == "") {
                VisualBrush b = new VisualBrush(new TextBlock() { Text = GetText(tb), FontStyle = FontStyles.Italic }) { TileMode = TileMode.None, Opacity = 0.3, Stretch = Stretch.None, AlignmentX = GetAlignmentX(tb), AlignmentY = GetAlignmentY(tb) };
                tb.Background = b;
            } else {
                tb.Background = GetBackground(tb);
            }
        }

        static void PlaceHolder_PasswordChanged(object sender, RoutedEventArgs e) {
            PasswordBox tb = sender as PasswordBox;
            if (tb.Password == null || tb.Password == "") {
                VisualBrush b = new VisualBrush(new TextBlock() { Text = " " + GetText(tb), FontStyle = FontStyles.Italic }) { TileMode = TileMode.None, Opacity = 0.3, Stretch = Stretch.None, AlignmentX = GetAlignmentX(tb), AlignmentY = GetAlignmentY(tb) };
                tb.Background = b;
            } else {
                tb.Background = GetBackground(tb);
            }
        }

        
    }
}
