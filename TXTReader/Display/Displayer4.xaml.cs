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
using System.Collections;
using System.Threading;

namespace TXTReader.Display {
    /// <summary>
    /// Displayer2.xaml 的交互逻辑
    /// </summary>
    public partial class Displayer4 : UserControl, IDisplayer {

        private int fps;
        private String[] text;

        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(Displayer4));
        public static readonly DependencyProperty FpsProperty = DependencyProperty.Register("Fps", typeof(int), typeof(Displayer4));
        public static readonly DependencyProperty IsScrollingProperty = DependencyProperty.Register("IsScrolling", typeof(bool), typeof(Displayer4), new PropertyMetadata(false, OnIsScrollingChanged));        
        public static readonly RoutedEvent ShutdownEvent = EventManager.RegisterRoutedEvent("Shutdown", RoutingStrategy.Direct, typeof(ShutdownHandler), typeof(Displayer4));

        public event ShutdownHandler Shutdown { add { AddHandler(ShutdownEvent, value); } remove { RemoveHandler(ShutdownEvent, value); } }
        public double Speed { get { return (double)GetValue(SpeedProperty); } set { SetValue(SpeedProperty, value); } }
        public bool IsScrolling { get { return (bool)GetValue(IsScrollingProperty); } set { SetValue(IsScrollingProperty, value); } }
        public int Fps { get { return (int)GetValue(FpsProperty); } set { SetValue(FpsProperty, value); } }
        public int FirstLine { get { return G.Book != null ? G.Book.Position : 0; } set { if (G.Book != null) G.Book.Position = value; } }
        public double Offset { get { return G.Book != null ? G.Book.Offset : 0; } set { if (G.Book != null) G.Book.Offset = value; } }        
        public double CanvasHeight { get { return canvas.ActualHeight; } }
        public double CanvasWidth { get { return canvas.ActualWidth; } }

        public String[] Text { get { if (text == null && G.Book != null) text = G.Book.TotalText.ToArray(); return text; } set { text = value; } }

        private Point? lastPoint = null;
        private Binding widthBinding;
        private Dictionary<int, TRText3> map = new Dictionary<int, TRText3>();
        private double lineHeight = 32;
        private HashSet<int> set = new HashSet<int>();

        public Displayer4() {
            InitializeComponent();
            InitComponent();
            if (G.Book != null) G.Book.LoadFinished += () => { Text = null; };
        }

        private void userControl_Loaded(object sender, RoutedEventArgs e) {
            if (G.Book != null) {
                //A.CopyText(out text, G.Book.TotalText);
                Update();
            }
        }       

       /* public static void OnSkinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            Skin val = (Skin)e.NewValue;
            if ((Thickness)d.GetValue(PaddingProperty) != val.Padding) d.SetValue(PaddingProperty, val.Padding);
            if ((Brush)d.GetValue(BackgroundProperty) != val.Background) d.SetValue(PaddingProperty, val.Background);
            if ((Brush)d.GetValue(ForegroundProperty) != val.Background) d.SetValue(ForegroundProperty, val.Foreground);            
        }*/

        public void UpdateSkin() {
            Padding = Options.Instance.Skin.Padding;
            Background = Options.Instance.Skin.Background;
            Clear();
            Update();
        }

        private void InitComponent() {
            SetBinding(SpeedProperty, new Binding("Speed") { Source = Options.Instance, Mode = BindingMode.TwoWay });            
            widthBinding = new Binding("ActualWidth") { Source = canvas };
            UpdateSkin();
            G.Timer.Timer += timer_Timer;
            canvas.SizeChanged += (d, e) => { Clear(); Update(); };
            fpsTimer();
        }

        private static void OnIsScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if ((bool)e.OldValue) {
                G.Timer.Stop();
            }
            if ((bool)e.NewValue) {
                G.Timer.Start();
            }
        }

        public bool IsPausing {
            get { return G.Timer.Status == TRTimerStatus.PAUSED; }
            set {
                if (G.Timer.Status != TRTimerStatus.PAUSED && value) G.Timer.Pause();
                else if (G.Timer.Status == TRTimerStatus.PAUSED && !value) G.Timer.Resume();
            }
        }

        void timer_Timer(long tick) {
            if (Speed == 0) return;
            Offset -= (double)tick / (Speed * 10000);
            Update();
        }

        public void OpenFile(String filename) {
            OpenBook(new Book(filename));
        }

        public void OpenBook(Book book) {
            if (G.Book != book) {
                Clear();
                G.Book = book;
                Text = null;
                Update();
            }
        }

        public void CloseFile() {
            Text = null;
            G.Book = null;
            Clear();          
            Update();
        }

        public void Clear() {
            canvas.Children.Clear();
            map.Clear();
        }

        public void ReopenFile() {
            Book tmp = G.Book;
            G.Book = null;
            Clear();
            Text = null;
            OpenFile(tmp.Source);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            if (G.MainWindow.IsHolding) return;
            base.OnMouseDown(e);
            G.Timer.Pause();
            lastPoint = e.GetPosition(this);
        }
        
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (G.MainWindow.IsHolding) return;            
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
            if (G.MainWindow.IsHolding) return;
            base.OnMouseUp(e);
            if (G.MainWindow.IsHolding) G.Timer.Resume();
            lastPoint = null;
        }
        
        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            if (G.MainWindow.IsHolding) return;
            base.OnMouseWheel(e);
            LineModify((double)e.Delta / 120);
        }
        
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
            if (G.MainWindow.IsHolding) return;
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
            ++fps;
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
            //Debug.WriteLine(canvas.Children.Count);
            if (reupdate) Update();
        }

     /*   private void mi_open_Click(object sender, RoutedEventArgs e) {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                OpenFile(dlg.FileName);
            }
        }
*/

        protected override void OnDrop(DragEventArgs e) {
            if (e.Data.GetFormats().Contains(DataFormats.FileDrop)){
                Array c = e.Data.GetData(DataFormats.FileDrop) as Array;
                if (c == null || c.Length <= 0) return;
                CloseFile();
                OpenFile(c.GetValue(0).ToString());
            }
            base.OnDrop(e);
        }

        
        async void fpsTimer() {
            while (G.IsRunning) {
                await Task.Run(() => { Thread.Sleep(1000); });
                Fps = fps;
                fps = 0;
            }
        }

//        private void mi_close_Click(object sender, RoutedEventArgs e) { CloseFile(); }
   //     private void mi_reopen_Click(object sender, RoutedEventArgs e) { ReopenFile(); }
  //      private void mi_exit_Click(object sender, RoutedEventArgs e) { App.Current.MainWindow.Close(); }
        public void LineModify(double n = 1) { Offset += lineHeight * n; Update(); }
        public void PageModify(double n = 1) { Offset += (CanvasHeight - lineHeight) * n; Update(); }  
    }
}
