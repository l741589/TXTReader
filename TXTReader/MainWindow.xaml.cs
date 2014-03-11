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
using System.Diagnostics;
using System.Windows.Threading;
using TXTReader.Properties;
using System.Threading;
using System.IO;
using TXTReader.Commands;
using System.Windows.Resources;
using Zlib.Async;
using TXTReader.Plugins;
using Zlib.UI;
using Zlib.UI.Utility;
using TXTReader.Interfaces;
using Zlib;
using Zlib.Win32;

namespace TXTReader {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {

        public const int HCF_NORMAL  = 1;
        public const int HCF_MOVE = 2;

        public const int HC_NORNAL = HCF_NORMAL;
        public const int HC_MOVE = HCF_NORMAL | HCF_MOVE;
        public const int HC_ALL = 0x7fffffff;
        public IDisplayer Displayer{get{return G.Displayer;}}

        public int HoldCode { get; set; }
        public bool IsHolding { get { return HoldCode > 0; } set { if (value && !IsHolding) Hold(HC_NORNAL); } }
        private System.Windows.Point? lastPoint = null;        

        public MainWindow() {
            InitializeComponent();
            IsBordered = OptionsParser.IsBordered;
            IsFullScreen = OptionsParser.IsFullScreen;
            G.MainWindow = this;
            PluginManager.Instance.OnWindowCreate(this);
            if (Displayer!=null) Displayer.ContextMenu = G.ContextMenu.Add(Resources["mainContextMenu"] as ContextMenu).Build();
            Focus();
        }

        private bool lastIsBordered = true;
        public bool IsBordered { get { return (bool)GetValue(IsBorderedProperty); } set { SetValue(IsBorderedProperty, value); } }
        public static readonly DependencyProperty IsBorderedProperty = DependencyProperty.Register("IsBordered", typeof(bool), typeof(MainWindow), new PropertyMetadata(true, (d, e) => {
            MainWindow w = d as MainWindow;            
            if (e.NewValue.Equals(true)) {
                w.WindowStyle = WindowStyle.SingleBorderWindow;
                w.ResizeMode = ResizeMode.CanResize;
            }else if (e.NewValue.Equals(false)) {
                w.WindowStyle = WindowStyle.None;
                w.ResizeMode = ResizeMode.NoResize;
            }
        }));

        public bool IsFullScreen { get { return (bool)GetValue(IsFullScreenProperty); } set { SetValue(IsFullScreenProperty, value); } }
        public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register("IsFullScreen", typeof(bool), typeof(MainWindow), new PropertyMetadata(false, (d, e) => {
            MainWindow w = d as MainWindow;
            if (e.NewValue.Equals(true)) {
                w.lastIsBordered = w.IsBordered;
                w.IsBordered = false;
                w.WindowState = WindowState.Maximized;
            } else if (e.NewValue.Equals(false)) {
                w.IsBordered = w.lastIsBordered;
                w.WindowState = WindowState.Normal;
            }
        }));

        public bool IsHold(int flag) {
            return (HoldCode & flag) != 0;
        }

        public void Hold(int code) {
            if (code == 0) ReleaseHold(HC_ALL);
            if (IsHolding) return;
            HoldCode |= code;
            if (IsHold(HCF_NORMAL)) {
                toolPanel.Hide();
                G.Timer.Pause();
            }
            if (IsHold(HCF_MOVE)) {
                Cursor = Cursors.SizeAll;
            }
        }

        public void ReleaseHold(int code) {
            if (!IsHolding) return;
            HoldCode &= ~code;
            if (HoldCode > 0) HoldCode |= HCF_NORMAL;
            if (!IsHold(HCF_NORMAL)) {
                G.Timer.Resume();
            }
            if (!IsHold(HCF_MOVE)) {
                Cursor = Cursors.Arrow;
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e) {            
            G.NotifyIcon = new TRNotifyIcon();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            if (!OnDisplayer(e)) return;
            base.OnKeyDown(e);
            if (e.Handled) return;
            switch (e.Key) {
                //case Key.OemComma: --toolPanel.pn_option.seSpeed.Value; break;
                //case Key.OemPeriod: ++toolPanel.pn_option.seSpeed.Value; break;
                case Key.Up: Displayer.LineModify(+1); break;
                case Key.Down: Displayer.LineModify(-1); break;
                case Key.PageUp: Displayer.PageModify(+1); break;
                case Key.PageDown: Displayer.PageModify(-1); break;
                case Key.Enter:
                case Key.Space: G.Displayer.IsScrolling = !G.Displayer.IsScrolling; break;
                case Key.LeftShift: Hold(HC_MOVE); break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            if (!OnDisplayer(e)) return;
            base.OnKeyUp(e);
            if (e.Handled) return;
            if (toolPanel.IsAncestorOf(e.OriginalSource as DependencyObject)) return;
            switch (e.Key) {
                case Key.LeftShift: ReleaseHold(HC_MOVE); break;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            base.OnClosing(e);
            if (G.Timer!=null) G.Timer.Stop();
            G.IsRunning = false;
            G.NotifyIcon.Close();
            ZTask.StopAll();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            if (!OnDisplayer(e)) return;
            base.OnMouseDown(e);
            if (e.Handled) return;

            toolPanel.Hide();
            ReleaseHold(HC_ALL);
            if (e.ChangedButton == MouseButton.Left) {
                if (WindowState == WindowState.Normal) lastPoint = e.GetPosition(this);
                if (G.Timer!=null) G.Timer.Pause();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if (!OnDisplayer(e)) return;
            base.OnMouseMove(e);
            if (e.Handled) return;

            var p=e.GetPosition(canvas);
            if (!IsHolding) {
                if (p.X > canvas.ActualWidth - 32) {
                    toolPanel.Show();
                }
            } else {
                if (lastPoint != null) {
                    var v = lastPoint.Value - p;
                    Left -= v.X;
                    Top -= v.Y;
                }                
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            if (!OnDisplayer(e)) return;
            //if (toolPanel.IsAncestorOf(e.OriginalSource as DependencyObject)) return;
            base.OnMouseUp(e);
            if (e.Handled) return;
            
            lastPoint = null;
            if (!IsHolding&&G.Timer!=null) G.Timer.Resume();
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
            Canvas.SetLeft(toolPanel, e.NewSize.Width);
            toolPanel.Hide();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
        }

        static bool toggled = false;
        public async void Toggle() {
            if (toggled) return;
            toggled = true;
            if (IsVisible) {
                G.Timer.Pause();
                Visibility = Visibility.Hidden;                
            } else {
                Visibility = Visibility.Visible;
                Win32Util.SetForegroundWindow(this);
                Focus();
                G.Timer.Resume();
            }
            await TaskEx.Run(() => { Thread.Sleep(100); });
            toggled = false;
        }

        private bool OnDisplayer(RoutedEventArgs e) {
            if (Displayer == null) return true;
            return (Displayer as Control).IsImmediateFamilyOf(e.OriginalSource as UIElement);
        }

        private void bossKey_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void bossKey_Executed(object sender, ExecutedRoutedEventArgs e) { Toggle(); }

        private void exit_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void exit_Executed(object sender, ExecutedRoutedEventArgs e) { Close(); }

        private void mi_scroll_Loaded(object sender, RoutedEventArgs e) {
            (sender as MenuItem).SetBinding(MenuItem.IsCheckedProperty, new Binding("IsScrolling") { Source = G.Displayer });
        }

        private void mi_border_Loaded(object sender, RoutedEventArgs e) {
            (sender as MenuItem).SetBinding(MenuItem.IsCheckedProperty, new Binding("IsBordered") { Source = this });
        }

        private void mi_fullscreen_Loaded(object sender, RoutedEventArgs e) {
            (sender as MenuItem).SetBinding(MenuItem.IsCheckedProperty, new Binding("IsFullScreen") { Source = this });
        }
    }
}
