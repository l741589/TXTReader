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

namespace TXTReader.Display {
    /// <summary>
    /// Displayer2.xaml 的交互逻辑
    /// </summary>
    public partial class Displayer3 : UserControl, IDisplayer {
        public String FileName { get; set; }
        public String[] Text { get; set; }
        public int FirstLine { get; set; }
        public double Offset { get { return Canvas.GetTop(scroller); } set { Canvas.SetTop(scroller, value); } }

        public static readonly DependencyProperty SkinProperty = DependencyProperty.Register("Skin", typeof(Skin), typeof(Displayer), new PropertyMetadata(new Skin(), OnSkinChanged));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(Displayer));
        public static readonly RoutedEvent ShutdownEvent = EventManager.RegisterRoutedEvent("Shutdown", RoutingStrategy.Direct, typeof(ShutdownHandler), typeof(Displayer));

        public event ShutdownHandler Shutdown { add { AddHandler(ShutdownEvent, value); } remove { RemoveHandler(ShutdownEvent, value); } }
        private double Speed { get { return (double)GetValue(SpeedProperty); } set { SetValue(SpeedProperty, value); } }
        private Skin Skin { get { return (Skin)GetValue(SkinProperty); } set { SetValue(SkinProperty, value); } }

        private Point AnchorTo { get; set; }
        private TRText2 Anchor { get; set; }
        private ITRTimer timer;
        private Point? lastPoint = null;
        

        public Displayer3() {
            InitializeComponent();
            InitComponent();
        }

        public Displayer3(XmlDocument xml) {
            InitializeComponent();
            InitComponent(xml);
        }

        public static void OnSkinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            Skin val = (Skin)e.NewValue;
            if ((Thickness)d.GetValue(PaddingProperty) != val.Padding) d.SetValue(PaddingProperty, val.Padding);
            if ((Brush)d.GetValue(BackgroundProperty) != val.Background) d.SetValue(PaddingProperty, val.Background);
            if ((Brush)d.GetValue(ForegroundProperty) != val.Background) d.SetValue(ForegroundProperty, val.Foreground);
        }

        public void UpdateSkin() {
            Padding = Skin.Padding;
            Background = Skin.Background;
            Foreground = Skin.Foreground;
        }

        private void InitComponent(XmlDocument xml = null) {
            //SetBinding(SpeedProperty, new Binding("Interval") { Source = timer });
            SetBinding(SpeedProperty, new Binding("Speed") { Source = Options.Instance });
            SetBinding(SkinProperty, new Binding("Skin") { Source = Options.Instance });

            Skin.LineSpacing = 4;
            Skin.ParaSpacing = 8;
            Skin.Font = new Typeface("宋体");
            Skin.FontSize = 12;
            Skin.Foreground = Brushes.Yellow;
            Skin.BackColor = Colors.DarkBlue;
            Skin.BackGroundType = BackGroundType.SolidColor;
            Skin.Padding = new Thickness(16);
            Skin.EffetSize = 1;
            Skin.Effect = Colors.Black;
            Skin.EffectType = EffectType.Shadow;

            FirstLine = 0;
            Offset = 0;
            if (xml != null) parseSkin(xml);

            timer = new TRTimer();
            timer.Timer += timer_Timer;

            UpdateSkin();
        }

        private void parseSkin(XmlNode node) {
            switch (node.Name.ToLower()) {
                case "part": Skin.Padding = (Thickness)new ThicknessConverter().ConvertFrom(node.Attributes["padding"].Value); break;
                case "color":
                    if (node.ParentNode.Name.ToLower().Equals("background")) {
                        Skin.BackColor = (Color)ColorConverter.ConvertFromString(node.InnerText);
                        Skin.BackGroundType = BackGroundType.SolidColor;
                    } else if (node.ParentNode.Name.ToLower().Equals("font"))
                        Skin.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(node.InnerText));
                    break;
                case "img": {
                        Skin.BackImage = (ImageSource)new ImageSourceConverter().ConvertFromString(node.InnerText);
                        Skin.BackGroundType = BackGroundType.Image;
                    } break;
                case "name": FontFamily = new FontFamily(node.InnerText); break;
                case "size": Skin.FontSize = double.Parse(node.InnerText); break;
                case "linespacing": Skin.LineSpacing = double.Parse(node.InnerText); break;
                case "paraspacing": Skin.ParaSpacing = double.Parse(node.InnerText); break;
                default: break;
            }

            if (node.HasChildNodes) parseSkin(node.FirstChild);
            if (node.NextSibling != null) parseSkin(node.NextSibling);

            switch (node.Name.ToLower()) {
                case "font": Skin.Font = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch); break;
                default: break;
            }
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

        public bool IsScrolling {
            get { return timer.Status != TRTimerStatus.STOPED; }
            set {
                if (timer.Status != TRTimerStatus.STOPED && !value) {
                    timer.Stop();
                } else if (timer.Status == TRTimerStatus.STOPED && value) {
                    timer.Start();
                }
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
            Update();
        }

        public void ReopenFile() {
            CopyText(File.ReadAllLines(FileName, Encoding.Default));
            Update();
        }


        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
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
            lastPoint = null;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            base.OnMouseWheel(e);
            Offset += e.Delta * (Skin.FontSize + Skin.LineSpacing) / 120;
            Update();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
            base.OnMouseDoubleClick(e);
            if (e.ChangedButton == MouseButton.Left) {
                IsScrolling = !IsScrolling;
            }
        }

        public void Update() {
            HashSet<UIElement> gc = new HashSet<UIElement>();
            foreach (UIElement e in scroller.Children)
            {
                if (Canvas.GetTop(e) >= CanvasHeight) gc.Add(e);
                if (Canvas.GetBottom(e) <= 0) gc.Add(e);
            }
            while (Offset > 0) {
                int i = ((TRText2)scroller.Children[0]).Index;
                var tb = new TRText2(Text[i-1],i-1);
                Offset-=tb.GuessedHeight;
                scroller.Children.Insert(0, tb);
            }
            if (scroller.Children.Count == 0) {
                var tb = new TRText2(Text[FirstLine], FirstLine);
                scroller.Children.Add(tb);
            }
            var last = scroller.Children[scroller.Children.Count - 1] as TRText2;
            var lastbottom = Canvas.GetBottom(last);
            if (lastbottom==0 || double.IsNaN(lastbottom)) lastbottom=Offset;
            while (lastbottom<CanvasHeight){
                int i = last.Index;
                var tb = new TRText2(Text[i + 1], i + 1);
                lastbottom += tb.GuessedHeight;
                scroller.Children.Add(tb);
                last = tb;
            }
            foreach (var e in gc) scroller.Children.Remove(e);
        }


        public double CanvasHeight { get { return canvas.ActualHeight; } }
        public double CanvasWidth { get { return canvas.ActualWidth; } }
    }
}
