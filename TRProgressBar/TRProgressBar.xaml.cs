using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TXTReader.Interfaces;
using TXTReader;
using Zlib.Utility;
using FloatControls;
using System.Windows.Controls;

namespace TRProgressBar {
    /// <summary>
    /// TRProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBar : System.Windows.Controls.ProgressBar, IValueConverter, IFloatControl {

        private Storyboard show;
        private Storyboard hide;
        private IBook Book { get { return G.Book; } }

        public double Percent { get { return (double)GetValue(PercentProperty); } set { SetValue(PercentProperty, value); } }
        public static readonly DependencyProperty PercentProperty = DependencyProperty.Register("Percent", typeof(double), typeof(ProgressBar));

        public ProgressBar() {
            InitializeComponent();
            Name = "进度条";
            UpdateBinding();
            show = Resources["show"] as Storyboard;
            hide = Resources["hide"] as Storyboard;
            ValueChanged += TRProgressBar_ValueChanged;
            G.BookChanged += (d, e) => UpdateBinding();
            Loaded += ProgressBar_Loaded;
            Tag = GetType().Name;
            this.Register();
        }

        void ProgressBar_Loaded(object sender, RoutedEventArgs e) {
            G.MainWindow.MouseMove += MainWindow_MouseMove;
            Hide();
        }

        void MainWindow_MouseMove(object sender, MouseEventArgs e) {
            Canvas c = G.MainCanvas;
            if ((e.GetPosition(c).Y < c.ActualHeight - 32) || e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed) {
                Hide();
            } else {
                e.Handled = true;
                Show();
            }
        }

        void TRProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Percent = e.NewValue / Maximum;
        }

        private void ProgressBar_MouseDown(object sender, MouseButtonEventArgs e) {
            if (Book.IsNull()) {
                Value = 0;
                return;
            }
            var p = e.GetPosition(this);
            double percent = p.X / ActualWidth;
            Value = 100 * percent;
        }

        public void UpdateBinding() {
            BindingOperations.ClearAllBindings(this);
            if (Book.NotNull()) SetBinding(ValueProperty, new Binding("Position") { Source = Book, Converter = this, Mode = BindingMode.TwoWay });
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (Book.IsNull()) return 0;
            double x = (double)(int)value;
            return x * 100 / Book.TotalText.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (Book.IsNull()) return 0;
            double x = (double)value;
            return Book.TotalText.Count * x / 100;
        }

        public void Show() {
            if (ActualHeight == 8) return;
            BeginStoryboard(show, HandoffBehavior.Compose);
        }

        public void Hide() {
            if (ActualHeight == 2) return;
            BeginStoryboard(hide, HandoffBehavior.Compose);
        }

        public FloatPosition Position {
            get { return FloatPosition.Bottom; }
        }
    }
}
