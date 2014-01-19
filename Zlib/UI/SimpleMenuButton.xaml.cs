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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zlib.UI {
    /// <summary>
    /// SimpleMenuButton.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleMenuButton : Button {
        public static readonly DependencyProperty SimpleMenuProperty = DependencyProperty.Register("SimpleMenu", typeof(ItemsControl), typeof(SimpleMenuButton));
        public ItemsControl SimpleMenu { get { return (ItemsControl)GetValue(SimpleMenuProperty); } set { SetValue(SimpleMenuProperty, value); } }
        
        public SimpleMenuButton() {
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            if (e.ChangedButton == MouseButton.Left) {
                if (SimpleMenu!=null) Show(this.PointToScreen(e.GetPosition(this)));
            }
        }

        private Window w = null;

        public void Show(Point position) {
            if (w != null) w.Close();
            w = new Window();
            //w.Owner = App.Current.MainWindow;
            w.Icon = null;
            w.ResizeMode = ResizeMode.NoResize;
            w.SizeToContent = SizeToContent.WidthAndHeight;
            w.Content = SimpleMenu;
            w.Left = position.X; ;
            w.Top = position.Y;
            w.Topmost = true;
            w.WindowStyle = WindowStyle.None;
            w.MouseUp += w_MouseUp;
            w.LostFocus += w_LostFocus;
            w.Show();
        }

        void w_LostFocus(object sender, RoutedEventArgs e) {
            if (w != null) {
                w.Close();
                w = null;
            }
        }

        void w_MouseUp(object sender, MouseButtonEventArgs e) {
            if (w != null) {
                w.Close();
                w = null;
            }
        }
    }
}
