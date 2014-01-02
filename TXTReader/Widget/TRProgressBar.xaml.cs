using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TXTReader.Display;
using TXTReader.Utility;

namespace TXTReader.Widget {
    /// <summary>
    /// TRProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class TRProgressBar : ProgressBar, IValueConverter {

        private Storyboard show;
        private Storyboard hide;

        public double Percent { get { return (double)GetValue(PercentProperty); } set { SetValue(PercentProperty, value); } }
        public static readonly DependencyProperty PercentProperty = DependencyProperty.Register("Percent", typeof(double), typeof(TRProgressBar));

        public TRProgressBar() {
            InitializeComponent();
            UpdateBinding();
            show = Resources["show"] as Storyboard;
            hide = Resources["hide"] as Storyboard;
            ValueChanged += TRProgressBar_ValueChanged;
        }

        void TRProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Percent = e.NewValue / Maximum;
        }

        private void ProgressBar_MouseDown(object sender, MouseButtonEventArgs e) {
            if (G.Book==null) {
                Value = 0;
                return;
            }
            var p = e.GetPosition(this);
            double percent = p.X / ActualWidth;
            Value = 100*percent;
            G.Displayer.Update();
        }

        public void UpdateBinding(){
            BindingOperations.ClearAllBindings(this);
            if (G.Book != null) SetBinding(ValueProperty, new Binding("Position") { Source = G.Book, Converter = this, Mode = BindingMode.TwoWay });
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (G.Book == null) return 0;
            double x = (double)(int)value;
            return x * 100 / G.Book.TotalText.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (G.Book == null) return 0;
            double x = (double)value;
            return G.Book.TotalText.Count * x / 100;            
        }

        public void Show() {
            if (ActualHeight == 8) return;
            BeginStoryboard(show, HandoffBehavior.Compose);
        }

        public void Hide() {
            if (ActualHeight == 2) return;
            BeginStoryboard(hide, HandoffBehavior.Compose);
        }

        
    }
}
