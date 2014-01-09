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


namespace FloatControls {
    /// <summary>
    /// FloatMessage.xaml 的交互逻辑
    /// </summary>
    public class FloatMessage : MovableFloatControl {

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(FloatMessage), new PropertyMetadata(OnValueChanged));
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(String), typeof(FloatMessage), new PropertyMetadata(OnValueChanged));
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(FloatMessage));

        public object Value { get { return GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public String Format { get { return (String)GetValue(FormatProperty); } set { SetValue(FormatProperty, value); } }
        public event RoutedPropertyChangedEventHandler<object> ValueChanged { add { AddHandler(ValueChangedEvent, value); } remove { RemoveHandler(ValueChangedEvent, value); } }
        public TextBlock tb = null;
        public Grid grid = null;        

        public FloatMessage() {
            init();
            Format = "{0}";
            Focusable = false;
        }

        private void init() {
            tb = new TextBlock();
            grid = new Grid();
            grid.Children.Add(tb);
            this.AddChild(grid);
        }

        static public void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            FloatMessage o = d as FloatMessage;
            o.tb.Text = String.Format(o.Format, o.Value);
            o.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(e.OldValue, e.NewValue, ValueChangedEvent));
        }

        
    }
}
