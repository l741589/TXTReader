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
using System.Windows.Media.Animation;
using System.Xml;
using TXTReader.Widget;
using System.Diagnostics;
using System.Windows.Threading;
using TXTReader.Util;

namespace TXTReader {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        private readonly Storyboard toolPanelShow;
        private bool toolPanelShowing = false;
        private Displayer displayer;

        public MainWindow() {
            InitializeComponent();
            toolPanelShow = Resources["toolPanelShow"] as Storyboard;
            toolPanelShow.Completed += (arg0, arg1) => { toolPanelShowing = false; };
            /*全屏
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Maximized;
            //*/
        }


        private void window_MouseMove(object sender, MouseEventArgs e) {
            if (displayer != null) Title = displayer.FirstLine + ":" + displayer.Offset;
            if (e.GetPosition(canvas).X > canvas.ActualWidth - 32) {
                if (!toolPanelShowing) {
                    toolPanelShowing = true;
                    toolPanel.BeginStoryboard(toolPanelShow);
                }
            }
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e) {

        }

        private void window_Loaded(object sender, RoutedEventArgs e) {
            XmlDocument dom = new XmlDocument();
            try {
                dom.Load("res/defaultskin.xml");
            } catch (Exception ex) {
                Debug.Print(ex.StackTrace);
            }
            displayer = new Displayer(dom);
            canvas.Children.Add(displayer);
            Canvas.SetLeft(displayer, 0);
            Canvas.SetTop(displayer, 0);
            Binding b = new Binding("ActualWidth");
            b.ElementName = "canvas";
            displayer.SetBinding(Displayer.WidthProperty, b);
            b = new Binding("ActualHeight");
            b.ElementName = "canvas";
            displayer.SetBinding(Displayer.HeightProperty, b);
            Canvas.SetZIndex(displayer, 0);
            Canvas.SetZIndex(toolPanel, 1);
            displayer.ContextMenu = Resources["mainContextMenu"] as ContextMenu;

            b = new Binding("Value");
            b.Source = toolPanel.pn_option.se_speed;
            displayer.SetBinding(Displayer.SpeedProperty,b);
        }

        private void mi_open_Click(object sender, RoutedEventArgs e) {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                displayer.OpenFile(dlg.FileName);
            }
        }

        private void mi_close_Click(object sender, RoutedEventArgs e) {
            displayer.CloseFile();
        }

        private void mi_reopen_Click(object sender, RoutedEventArgs e) {
            displayer.ReopenFile();
        }

        private void mi_exit_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void mi_scroll_Click(object sender, RoutedEventArgs e) {
            displayer.IsScrolling = (sender as MenuItem).IsChecked;
        }

        private void window_KeyDown(object sender, KeyEventArgs e) {
            switch(e.Key){
                case Key.OemComma: --toolPanel.pn_option.se_speed.Value; break;
                case Key.OemPeriod: ++toolPanel.pn_option.se_speed.Value; break;
            }
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (displayer!=null) displayer.IsScrolling = false;
        }
    }
}
