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

        public bool IsHolding { get; set; }
        private System.Windows.Point? lastPoint = null;

        public MainWindow() {
            InitializeComponent();           
            /*全屏
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Maximized;
            //*/            
        }

        public void Hold() {
            if (IsHolding) return;
            IsHolding = true;
            toolPanel.Hide();
            Cursor = Cursors.SizeAll;
            G.Timer.Pause();
        }

        public void ReleaseHold() {
            if (!IsHolding) return;
            IsHolding = false;
            Cursor = Cursors.Arrow;
            G.Timer.Resume();
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
            G.NotifyIcon = new TRNotifyIcon();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            switch (e.Key) {
                case Key.OemComma: --toolPanel.pn_option.se_speed.Value; break;
                case Key.OemPeriod: ++toolPanel.pn_option.se_speed.Value; break;
                case Key.Up: displayer.LineModify(+1); break;
                case Key.Down: displayer.LineModify(-1); break;
                case Key.PageUp: displayer.PageModify(+1); break;
                case Key.PageDown: displayer.PageModify(-1); break;
                case Key.LeftShift: Hold(); break;
                case Key.Escape: Toggle(); break;
            }            
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            switch (e.Key) {
                case Key.LeftShift: ReleaseHold(); break;
            }            
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            BookParser.Save();
            RuleParser.Save();
            displayer.CloseFile();
            G.Timer.Stop();
            G.IsRunning = false;
            G.NotifyIcon.Close();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            toolPanel.Hide();
            if (e.ChangedButton == MouseButton.Left) {
                if (WindowState == WindowState.Normal) lastPoint = e.GetPosition(this);
                G.Timer.Pause();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if (!IsHolding) {
                if (e.GetPosition(canvas).X > canvas.ActualWidth - 32) {
                    toolPanel.Show();
                }
            } else {
                if (lastPoint != null) {
                    var p = e.GetPosition(this);
                    var v = lastPoint.Value - p;
                    Left -= v.X;
                    Top -= v.Y;
                }
                
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            lastPoint = null;
            if (!IsHolding) G.Timer.Resume();
        }


        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
            Canvas.SetLeft(toolPanel, e.NewSize.Width);
            toolPanel.Hide();
        }

        public void Toggle() {
            if (IsVisible) {
                G.Timer.Pause();
                Visibility = Visibility.Hidden;                
            } else {
                Visibility = Visibility.Visible;
                Focus();
                G.Timer.Resume();
            }
        }
    }
}
