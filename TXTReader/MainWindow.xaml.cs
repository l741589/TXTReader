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
using TXTReader.Commands;
using System.Windows.Resources;

namespace TXTReader {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {

        public const int HCF_NORMAL  = 1;
        public const int HCF_MOVE = 2;
        public const int HCF_FIND = 4;

        public const int HC_NORNAL = HCF_NORMAL;
        public const int HC_MOVE = HCF_NORMAL | HCF_MOVE;
        public const int HC_FIND = HCF_NORMAL | HCF_FIND;
        public const int HC_ALL = 0x7fffffff;

        public int HoldCode { get; set; }
        public bool IsHolding { get { return HoldCode > 0; } set { if (value && !IsHolding) Hold(HC_NORNAL); } }
        private System.Windows.Point? lastPoint = null;        

        public MainWindow() {
            InitializeComponent();

            /*全屏
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Maximized;
            //*/
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
            if (IsHold(HCF_FIND)) {
                sb_search.Show();
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
            if (!IsHold(HCF_FIND)) {
                sb_search.Hide();
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e) {
            XmlDocument dom = new XmlDocument();
            try {
                Uri uri = new Uri("/TXTReader;component/res/defaultskin.xml", UriKind.Relative);
                StreamResourceInfo info = Application.GetResourceStream(uri);
                dom.Load(info.Stream);
                SkinParser.SetDefaultSkin();
                SkinParser.ParseSkin(dom);
            } catch (Exception ex) {
                Debug.Print(ex.StackTrace);
            }
            displayer.UpdateSkin();
            //displayer.SetBinding(Displayer4.SpeedProperty, new Binding("Value") { Source = toolPanel.pn_option.se_speed });
            BookParser.Load();
            G.NotifyIcon = new TRNotifyIcon();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            switch (e.Key) {
                case Key.OemComma: --toolPanel.pn_option.seSpeed.Value; break;
                case Key.OemPeriod: ++toolPanel.pn_option.seSpeed.Value; break;
                case Key.Up: displayer.LineModify(+1); break;
                case Key.Down: displayer.LineModify(-1); break;
                case Key.PageUp: displayer.PageModify(+1); break;
                case Key.PageDown: displayer.PageModify(-1); break;
                case Key.LeftShift: Hold(HC_MOVE); break;                
            }            
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            switch (e.Key) {
                case Key.LeftShift: ReleaseHold(HC_MOVE); break;
            }
            ContextMenu c;

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
            ReleaseHold(HC_ALL);
            if (e.ChangedButton == MouseButton.Left) {
                if (WindowState == WindowState.Normal) lastPoint = e.GetPosition(this);
                G.Timer.Pause();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            var p=e.GetPosition(root);
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
            if (p.Y > canvas.ActualHeight - 32) progressBar.Show();
            else progressBar.Hide();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            lastPoint = null;
            if (!IsHolding) G.Timer.Resume();
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
                Focus();
                G.Timer.Resume();
            }
            await Task.Run(() => { Thread.Sleep(100); });
            toggled = false;
        }

        private void find_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = G.Book != null; }
        private void find_Executed(object sender, ExecutedRoutedEventArgs e) { Hold(HC_FIND); }

        private void open_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute= true; }
        private void open_Executed(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                G.Displayer.OpenFile(dlg.FileName);
            }
        }

        private void close_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = G.Book != null; }
        private void close_Executed(object sender, ExecutedRoutedEventArgs e) { G.Displayer.CloseFile(); }

        private void bossKey_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void bossKey_Executed(object sender, ExecutedRoutedEventArgs e) { Toggle(); }

        private void exit_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void exit_Executed(object sender, ExecutedRoutedEventArgs e) { Close(); }

        private void reopen_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = G.Book != null; }
        private void reopen_Executed(object sender, ExecutedRoutedEventArgs e) { G.Displayer.ReopenFile(); }

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
