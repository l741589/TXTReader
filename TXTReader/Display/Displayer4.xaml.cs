using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using TXTReader.Data;
using System.Diagnostics;
using TXTReader.Utility;
using TXTReader.Res;

namespace TXTReader.Display {
    /// <summary>
    /// Displayer2.xaml 的交互逻辑
    /// </summary>
    public partial class Displayer4 : UserControl, IDisplayer {
        public String FileName { get; set; }
        public String[] Text { get; set; }
        public int FirstLine { get; set; }
        public double Offset { get; set; }

        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(Displayer4));
        public static readonly DependencyProperty IsScrollingProperty = DependencyProperty.Register("IsScrolling", typeof(bool), typeof(Displayer4), new PropertyMetadata(false, OnIsScrollingChanged));
        public static readonly RoutedEvent ShutdownEvent = EventManager.RegisterRoutedEvent("Shutdown", RoutingStrategy.Direct, typeof(ShutdownHandler), typeof(Displayer4));

        public event ShutdownHandler Shutdown { add { AddHandler(ShutdownEvent, value); } remove { RemoveHandler(ShutdownEvent, value); } }
        public double Speed { get { return (double)GetValue(SpeedProperty); } set { SetValue(SpeedProperty, value); } }
        public bool IsScrolling { get { return (bool)GetValue(IsScrollingProperty); } set { SetValue(IsScrollingProperty, value); } }
        public double CanvasHeight { get { return canvas.ActualHeight; } }
        public double CanvasWidth { get { return canvas.ActualWidth; } }

        private ITRTimer timer = new TRTimer2();
        private Point? lastPoint = null;
        private Binding widthBinding;
        private Dictionary<int, TRText3> map = new Dictionary<int, TRText3>();
        private double lineHeight = 32;
        private HashSet<int> set = new HashSet<int>();

        public Displayer4() {
            InitializeComponent();
            InitComponent();
        }

        public static void OnSkinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            Skin val = (Skin)e.NewValue;
            if ((Thickness)d.GetValue(PaddingProperty) != val.Padding) d.SetValue(PaddingProperty, val.Padding);
            if ((Brush)d.GetValue(BackgroundProperty) != val.Background) d.SetValue(PaddingProperty, val.Background);
            if ((Brush)d.GetValue(ForegroundProperty) != val.Background) d.SetValue(ForegroundProperty, val.Foreground);
        }

        public void UpdateSkin() {
            Padding = Options.Instance.Skin.Padding;
            Background = Options.Instance.Skin.Background;
        }

        private void InitComponent() {
            SetBinding(SpeedProperty, new Binding("Speed") { Source = Options.Instance, Mode = BindingMode.TwoWay });
            SetBinding(IsScrollingProperty, new Binding("IsChecked") { Source = mi_scroll, Mode = BindingMode.TwoWay });
            widthBinding = new Binding("ActualWidth") { Source = canvas };
            FirstLine = 0;
            Offset = 0;
            UpdateSkin();
            timer.Timer += timer_Timer;
            canvas.SizeChanged += (d, e) => { Clear(); Update(); };
        }

        void CopyText(String[] src, int piecelen = 4096) {
            if (piecelen == 0) {
                Text = src;
                return;
            }
            List<String> ss = new List<String>();
            foreach (String s in src) {
                int i = 0;
                for (; i < s.Length - piecelen; i += piecelen) {
                    ss.Add(s.Substring(i, piecelen));
                }
                ss.Add(s.Substring(i));
            }
            Text = ss.ToArray();
        }


        private static void OnIsScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var o = d as Displayer4;
            if ((bool)e.OldValue) {
                o.timer.Stop();
            }
            if ((bool)e.NewValue) {
                o.timer.Start();
            }
        }

        public bool IsPausing {
            get { return timer.Status == TRTimerStatus.PAUSED; }
            set {
                if (timer.Status != TRTimerStatus.PAUSED && value) timer.Pause();
                else if (timer.Status == TRTimerStatus.PAUSED && !value) timer.Resume();
            }
        }

        void timer_Timer(long tick) {
            if (Speed == 0) return;
            Offset -= (double)tick / (Speed * 10000);
            Update();
        }

        public void OpenFile(String filename) {
            FileName = filename;
            CopyText(File.ReadAllLines(filename, Encoding.Default));
            Update();
        }

        public void CloseFile() {
            Text = null;
            FileName = null;
            FirstLine = 0;
            Offset = 0;
            Clear();
            Update();
        }

        public void Clear() {
            canvas.Children.Clear();
            map.Clear();
        }

        public void ReopenFile() {
            CopyText(File.ReadAllLines(FileName, Encoding.Default));
            Update();
        }


        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            timer.Pause();
            lastPoint = e.GetPosition(this);            
        }
        
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (e.LeftButton.Equals(MouseButtonState.Pressed)) {
                Point curPoint = e.GetPosition(this);
                if (lastPoint != null) {
                    Offset += curPoint.Y - lastPoint.Value.Y;
                    if (FirstLine <= 0) FirstLine = 0;
                    if (FirstLine == 0 && Offset > 0) Offset = 0;
                }
                lastPoint = curPoint;
                Update();
            }
        }
        
        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            timer.Resume();
            lastPoint = null;
        }
        
        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            base.OnMouseWheel(e);
            LineModify((double)e.Delta / 120);
        }
        
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
            base.OnMouseDoubleClick(e);
            if (e.ChangedButton == MouseButton.Left) {
                IsScrolling = !IsScrolling;
            }
        }

        private TRText3 ObtainText(int index) {
            TRText3 tb;
            if (!map.TryGetValue(index, out tb)) {
                if (index >= Text.Count()) return null;
                tb = new TRText3(Text[index], index);
                tb.SetBinding(WidthProperty, widthBinding);
                lineHeight = tb.LineHeight = Options.Instance.Skin.LineSpacing + tb.SingleLineHeight;
                tb.FormattedText.MaxTextWidth = canvas.ActualWidth;
                canvas.Children.Add(tb);
                map[index] = tb;
            }
            if (tb != null) tb.Updated = true;
            return tb;
        }

        public void Update() {
            bool reupdate = false;
            if (Text == null) {
                canvas.Children.Clear();
                return;
            }
            double lastbottom = Offset;
            foreach (var e in map) e.Value.Updated = false;
            for (int i = FirstLine; lastbottom < CanvasHeight; ++i) {
                TRText3 tb = ObtainText(i);
                if (tb == null) break;
                double newbottom = lastbottom + tb.GuessedHeight + Options.Instance.Skin.ParaSpacing;
                if (lastbottom < 0) {
                    FirstLine = i;
                    Offset = lastbottom;
                }
                Canvas.SetTop(tb, lastbottom);
                Canvas.SetLeft(tb, 0);
                lastbottom = newbottom;
            }
            while (Offset > 0 && FirstLine > 0) {
                --FirstLine;
                TRText3 tb = ObtainText(FirstLine);
                if (tb == null) break;
                Offset -= tb.GuessedHeight + Options.Instance.Skin.ParaSpacing;
                Canvas.SetTop(tb, Offset);
                Canvas.SetLeft(tb, 0);
            }
            if (FirstLine == 0 && Offset > 0) { Offset = 0; reupdate = true; }
            if (FirstLine != 0 && lastbottom < CanvasHeight) { Offset += CanvasHeight - lastbottom; reupdate = true; }
            set.Clear();
            foreach (var e in map) if (e.Value.Updated == false) set.Add(e.Key);
            foreach (var e in set) { canvas.Children.Remove(map[e]); map.Remove(e); }
            Debug.WriteLine(canvas.Children.Count);
            if (reupdate) Update();
        }

        private void mi_open_Click(object sender, RoutedEventArgs e) {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                OpenFile(dlg.FileName);
            }
        }

        private void mi_close_Click(object sender, RoutedEventArgs e) { CloseFile(); }
        private void mi_reopen_Click(object sender, RoutedEventArgs e) { ReopenFile(); }
        private void mi_exit_Click(object sender, RoutedEventArgs e) { App.Current.MainWindow.Close(); }
        public void LineModify(double n = 1) { Offset += lineHeight * n; Update(); }
        public void PageModify(double n = 1) { Offset += (CanvasHeight - lineHeight) * n; Update(); }
    }
}
