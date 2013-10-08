using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Diagnostics;
using TXTReader.Util;
using System.Windows.Threading;
using TXTReader.Data;

namespace TXTReader.Widget {
    public delegate void ShutdownHandler();

    public class Displayer : Control {
        public const int DEFAULT_SPEED = 5;
        public String FileName { get; set; }
        public String[] Text { get; set; }
        public int FirstLine { get; set; }
        public double Offset { get; set; }
        public bool IsToUpdated { get; set; }

        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(Displayer));
        public static readonly RoutedEvent ShutdownEvent = EventManager.RegisterRoutedEvent("Shutdown", RoutingStrategy.Direct, typeof(ShutdownHandler), typeof(Displayer));

        public event ShutdownHandler Shutdown { add { AddHandler(ShutdownEvent, value); } remove { RemoveHandler(ShutdownEvent, value); } }
        public double Speed { get { return (double)GetValue(SpeedProperty); } set { SetValue(SpeedProperty, value); } }

        private TRTimer timer;
        private Point? lastPoint = null;

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

        static Displayer() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Displayer), new FrameworkPropertyMetadata(typeof(Displayer)));
        }

        void timer_Timer(long tick) {
            Offset -= (double)tick / (Speed * 10000);
            Update();
        }

        public Displayer() : base() { InitComponent(); }

        public Displayer(XmlDocument xml) : base() { InitComponent(xml); }

        private void InitComponent(XmlDocument xml = null) {
            Skin Skin = Options.Instance.Skin;
            Skin.LineSpacing = 4;
            Skin.ParaSpacing = 8;
            Skin.Font = new Typeface("宋体");
            Skin.FontSize = 12;
            Skin.Foreground = Brushes.Yellow;
            Skin.BackColor = Colors.DarkBlue;
            Skin.BackGroundType = BackGroundType.SolidColor;
            Skin.Padding = new Thickness(16);

            FirstLine = 0;
            Offset = 0;            
            if (xml != null) parseSkin(xml);

            timer = new TRTimer();
            timer.Timer += timer_Timer;
            SetBinding(SpeedProperty, new Binding("Interval") { Source = timer });

            Options.Instance.Speed = DEFAULT_SPEED;
            IsToUpdated = true;
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

        private void parseSkin(XmlNode node) {
            Skin sk = Options.Instance.Skin;
            switch (node.Name.ToLower()) {
                case "part": sk.Padding = (Thickness)new ThicknessConverter().ConvertFrom(node.Attributes["padding"].Value); break;
                case "color":
                    if (node.ParentNode.Name.ToLower().Equals("background")) {
                        sk.BackColor = (Color)ColorConverter.ConvertFromString(node.InnerText);
                        sk.BackGroundType = BackGroundType.SolidColor;
                    } else if (node.ParentNode.Name.ToLower().Equals("font"))
                        sk.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(node.InnerText));
                    break;
                case "img": {
                        sk.BackImage = (ImageSource)new ImageSourceConverter().ConvertFromString(node.InnerText);
                        sk.BackGroundType = BackGroundType.Image;
                    } break;
                case "name": FontFamily = new FontFamily(node.InnerText); break;
                case "size": sk.FontSize = double.Parse(node.InnerText); break;
                case "linespacing": sk.LineSpacing = double.Parse(node.InnerText); break;
                case "paraspacing": sk.ParaSpacing = double.Parse(node.InnerText); break;
                default: break;
            }

            if (node.HasChildNodes) parseSkin(node.FirstChild);
            if (node.NextSibling != null) parseSkin(node.NextSibling);

            switch (node.Name.ToLower()) {
                case "font": sk.Font = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch); break;
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

        private FormattedText createFormattedText(String s) {
            Skin sk = Options.Instance.Skin;
            FormattedText ft = null;
            ft = new FormattedText(s, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, sk.Font, sk.FontSize, sk.Foreground);
            ft.MaxTextWidth = Math.Max(sk.FontSize, ActualWidth - sk.Padding.Left - sk.Padding.Right);
            ft.Trimming = TextTrimming.None;
            ft.TextAlignment = TextAlignment.Left;
            ft.LineHeight = sk.FontSize + sk.LineSpacing;
            return ft;
        }

        protected override void OnRender(DrawingContext drawingContext) {
            Skin sk = Options.Instance.Skin;
            if (ActualHeight > 0 && ActualWidth > 0) {
                drawingContext.DrawRectangle(sk.Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
                if (Text != null) {
                    Debug.WriteLine("<<<LN{0},OFF{1}", FirstLine, Offset);
                    double last_bottom = Offset;
                    for (int i = FirstLine; last_bottom < ActualHeight - sk.Padding.Bottom - sk.Padding.Top && i < Text.Length; ++i) {
                        var ft = createFormattedText(Text[i]);
                        drawingContext.DrawText(ft, new Point(sk.Padding.Left, sk.Padding.Top + last_bottom));
                        //var geo = ft.BuildGeometry(new Point(Padding.Left, Padding.Top + last_bottom));
                        //geo.Transform = new TranslateTransform(Padding.Left, Padding.Top + last_bottom);
                        //geo.Transform.Value.Translate();
                        //drawingContext.DrawGeometry(Foreground, new Pen(Brushes.Black, 1), geo);
                        last_bottom += ft.Height + sk.ParaSpacing;
                        if (last_bottom < 0) {
                            Offset = last_bottom;
                            ++FirstLine;
                        }
                    }
                    if (FirstLine <= 0) FirstLine = 0;
                    if (FirstLine == 0 && Offset > 0) Offset = 0;
                    while (Offset > 0 && FirstLine > 0) {
                        --FirstLine;
                        FormattedText ft = createFormattedText(Text[FirstLine]);
                        Offset -= ft.Height + sk.ParaSpacing;
                        drawingContext.DrawText(ft, new Point(sk.Padding.Left, sk.Padding.Top + Offset));
                    }
                    Geometry g1 = new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight));
                    Geometry g2 = new RectangleGeometry(new Rect(sk.Padding.Left, sk.Padding.Top, ActualWidth - sk.Padding.Right - sk.Padding.Left, ActualHeight - sk.Padding.Bottom - sk.Padding.Top));
                    PathGeometry g = Geometry.Combine(g1, g2, GeometryCombineMode.Exclude, null);
                    drawingContext.PushClip(g);
                    drawingContext.DrawRectangle(sk.Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
                    drawingContext.Pop();
                    Debug.WriteLine(">>>LN{0},OFF{1}", FirstLine, Offset);
                }
            }
            IsToUpdated = false;
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
            Skin sk = Options.Instance.Skin;
            base.OnMouseWheel(e);
            Offset += e.Delta * (sk.FontSize + sk.LineSpacing) / 120;
            Update();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
            base.OnMouseDoubleClick(e);
            if (e.ChangedButton == MouseButton.Left) {
                IsScrolling = !IsScrolling;
            }
        }

        public void Update() {
            if (IsToUpdated) return;
            IsToUpdated = true;
            InvalidateVisual();
        }
    }
}