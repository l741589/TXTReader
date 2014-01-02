using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Drawing;
using TXTReader.Utility;

namespace TXTReader.FloatMessages {
    /// <summary>
    /// FloatMessage.xaml 的交互逻辑
    /// </summary>
    public partial class FloatMessage : UserControl {

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(FloatMessage), new PropertyMetadata(OnValueChanged));
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(String), typeof(FloatMessage), new PropertyMetadata(OnValueChanged));
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(FloatMessage));

        public object Value { get { return GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public String Format { get { return (String)GetValue(FormatProperty); } set { SetValue(FormatProperty, value); } }
        public event RoutedPropertyChangedEventHandler<object> ValueChanged { add { AddHandler(ValueChangedEvent, value); } remove { RemoveHandler(ValueChangedEvent, value); } }
        private System.Windows.Point? lastPoint = null;

        public FloatMessagePanel Panel { get; private set; }

        public FloatMessage() {
            InitializeComponent();
            Format = "{0}";
            Focusable = false;
        }

        public FloatMessage(FloatMessagePanel panel)
            : this() {
            Panel = panel;
        }

        static public void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            FloatMessage o = d as FloatMessage;
            o.tb.Text = String.Format(o.Format, o.Value);
            o.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(e.OldValue, e.NewValue, ValueChangedEvent));
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e) {
            if (lastPoint == null) return;
            var p = e.GetPosition(Panel.pn_moving);
            Canvas.SetLeft(this, p.X - ActualWidth / 2);
            Canvas.SetTop(this, p.Y - ActualHeight / 2);
            lastPoint = p;
            e.Handled = true;
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (G.MainWindow.IsHold(MainWindow.HCF_MOVE)) {
                if (Parent != Panel.pn_moving) {
                    (Parent as Panel).Children.Remove(this);
                    var p = e.GetPosition(Panel.pn_moving);
                    Panel.pn_moving.Children.Add(this);
                    Canvas.SetLeft(this, p.X - ActualWidth / 2);
                    Canvas.SetTop(this, p.Y - ActualHeight / 2);
                    lastPoint = p;
                    e.Handled = true;
                }
                CaptureMouse();
            }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (lastPoint!=null) {
                if (Parent == Panel.pn_moving) {
                    Panel.pn_moving.Children.Remove(this);
                    var p = e.GetPosition(Panel.pn_moving);
                    if (p.X < Panel.ActualWidth / 2)
                        if (p.Y < Panel.ActualHeight / 2) Panel.pn_lefttop.Children.Add(this);
                        else Panel.pn_leftbottom.Children.Add(this);
                    else
                        if (p.Y < Panel.ActualHeight / 2) Panel.pn_righttop.Children.Add(this);
                        else Panel.pn_rightbottom.Children.Add(this);
                    e.Handled = true;
                }
                lastPoint = null;                
            }
            ReleaseMouseCapture();
        }
    }
}
