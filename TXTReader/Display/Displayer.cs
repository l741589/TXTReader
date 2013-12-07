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
using System.Windows.Threading;
using TXTReader.Data;

namespace TXTReader.Display {
    public delegate void ShutdownHandler();

    public class Displayer : Control, IDisplayer {

        public String FileName { get; set; }
        public String[] Text { get; set; }
        public int FirstLine { get; set; }
        public double Offset { get; set; }
        public bool IsToUpdated { get; set; }

        public static readonly DependencyProperty SkinProperty = DependencyProperty.Register("Skin", typeof(Skin), typeof(Displayer), new PropertyMetadata(new Skin()));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(Displayer));
        public static readonly RoutedEvent ShutdownEvent = EventManager.RegisterRoutedEvent("Shutdown", RoutingStrategy.Direct, typeof(ShutdownHandler), typeof(Displayer));

        public event ShutdownHandler Shutdown { add { AddHandler(ShutdownEvent, value); } remove { RemoveHandler(ShutdownEvent, value); } }
        private double Speed { get { return (double)GetValue(SpeedProperty); } set { SetValue(SpeedProperty, value); } }
        private Skin Skin { get { return (Skin)GetValue(SkinProperty); } set { SetValue(SkinProperty, value); } }

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
            SetBinding(SpeedProperty, new Binding("Interval") { Source = timer });
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
            Skin.EffectSize = 1;
            Skin.Effect = Colors.Black;
            Skin.EffectType = EffectType.Shadow;

            FirstLine = 0;
            Offset = 0;
            if (xml != null) parseSkin(xml);

            timer = new TRTimer();
            timer.Timer += timer_Timer;

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

        public FormattedText createFormattedText(String s) {
            FormattedText ft = null;
            ft = new FormattedText(s, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Skin.Font, Skin.FontSize, Skin.Foreground);
            ft.MaxTextWidth = Math.Max(Skin.FontSize, ActualWidth - Skin.Padding.Left - Skin.Padding.Right);
            ft.Trimming = TextTrimming.None;
            ft.TextAlignment = TextAlignment.Left;
            ft.LineHeight = Skin.FontSize + Skin.LineSpacing;
            return ft;
        }

        private void TextOut(DrawingContext dc, FormattedText ft, double x, double y) {
           
            dc.DrawText(ft,new Point(x, y));

        }

        protected override void OnRender(DrawingContext drawingContext) {
            if (ActualHeight > 0 && ActualWidth > 0) {
                drawingContext.DrawRectangle(Skin.Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
                if (Text != null) {
                    //Debug.WriteLine("<<<LN{0},OFF{1}", FirstLine, Offset);

                    //绘制文本
                    double last_bottom = Offset;
                    for (int i = FirstLine; last_bottom < ActualHeight - Skin.Padding.Bottom - Skin.Padding.Top && i < Text.Length; ++i) {
                        var ft = createFormattedText(Text[i]);
                        TextOut(drawingContext, ft, Skin.Padding.Left, Skin.Padding.Top + last_bottom);
                        last_bottom += ft.Height + Skin.ParaSpacing;
                        if (last_bottom < 0) {
                            Offset = last_bottom;
                            ++FirstLine;
                        }
                    }
                    if (FirstLine <= 0) FirstLine = 0;
                    if (FirstLine == 0 && Offset > 0) Offset = 0;
                    while (Offset > 0 && FirstLine > 0) {
                        --FirstLine;
                        var ft = createFormattedText(Text[FirstLine]);
                        Offset -= ft.Height + Skin.ParaSpacing;
                        TextOut(drawingContext, ft, Skin.Padding.Left, Skin.Padding.Top + Offset);
                    }

                    //裁剪外框
                    Geometry g1 = new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight));
                    Geometry g2 = new RectangleGeometry(new Rect(Skin.Padding.Left, Skin.Padding.Top, ActualWidth - Skin.Padding.Right - Skin.Padding.Left, ActualHeight - Skin.Padding.Bottom - Skin.Padding.Top));
                    PathGeometry g = Geometry.Combine(g1, g2, GeometryCombineMode.Exclude, null);
                    drawingContext.PushClip(g);
                    drawingContext.DrawRectangle(Skin.Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
                    drawingContext.Pop();

                    //Debug.WriteLine(">>>LN{0},OFF{1}", FirstLine, Offset);
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

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
        }

        public void Update() {
            if (IsToUpdated) return;
            IsToUpdated = true;
            InvalidateVisual();
        }

        public double CanvasHeight { get { return ActualHeight - Skin.Padding.Top - Skin.Padding.Bottom; } }
        public double CanvasWidth { get { return ActualWidth - Skin.Padding.Left - Skin.Padding.Right; } }

    }
}