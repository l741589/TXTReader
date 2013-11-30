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
using TXTReader.Display;
using System.Diagnostics;
using System.Windows.Threading;
using TXTReader.Properties;
using TXTReader.Utility;
using TXTReader.Data;
using System.Threading;
using System.IO;

namespace TXTReader {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {   

        public MainWindow() {
            InitializeComponent();           
            /*全屏
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Maximized;
            //*/            
        }


        protected override void OnMouseMove(MouseEventArgs e) {
            if (e.GetPosition(canvas).X > canvas.ActualWidth - 32 ){
                toolPanel.Show();
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e) {
                XmlDocument dom = new XmlDocument();
                try {
                    dom.Load("res/defaultskin.xml");
                    SkinParser.SetDefaultSkin();
                    SkinParser.ParseSkin(dom);
                } catch (Exception ex) {
                    Debug.Print(ex.StackTrace);
                }
                displayer.UpdateSkin();
                displayer.SetBinding(Displayer4.SpeedProperty, new Binding("Value") { Source = toolPanel.pn_option.se_speed });
                BookParser.Load();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            switch (e.Key) {
                case Key.OemComma: --toolPanel.pn_option.se_speed.Value; break;
                case Key.OemPeriod: ++toolPanel.pn_option.se_speed.Value; break;
                case Key.Up: displayer.LineModify(+1); break;
                case Key.Down: displayer.LineModify(-1); break;
                case Key.PageUp: displayer.PageModify(+1); break;
                case Key.PageDown: displayer.PageModify(-1); break;
            }            
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            BookParser.Save();
            RuleParser.Save();
            displayer.CloseFile();
            G.Timer.Stop();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            toolPanel.Hide();
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
            Canvas.SetLeft(toolPanel, e.NewSize.Width);
            toolPanel.Hide();
        }        
    }
}
