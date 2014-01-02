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

namespace Zlib.Widget {
    /// <summary>
    /// SpinEdit.xaml 的交互逻辑
    /// </summary>
    public partial class SpinEdit : UserControl {
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<int> ValueChanged { add { this.AddHandler(ValueChangedEvent, value); } remove { this.RemoveHandler(ValueChangedEvent, value); } }
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(SpinEdit));

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(SpinEdit), new PropertyMetadata(0x7fffffff));
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(SpinEdit), new PropertyMetadata(0x0));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(SpinEdit), new PropertyMetadata(0, OnValueChanged, CoerceValue));

        public int MaxValue { get { return (int)GetValue(MaxValueProperty); } set { SetValue(MaxValueProperty, value); } }
        public int MinValue { get { return (int)GetValue(MinValueProperty); } set { SetValue(MinValueProperty, value); } }
        public int Value { get { return (int)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); tb.Text = Value.ToString(); } }

        public SpinEdit() {  InitializeComponent(); }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            RoutedPropertyChangedEventArgs<int> arg =
                new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, ValueChangedEvent);
            ((UIElement)d).RaiseEvent(arg);
        }

        private static object CoerceValue(DependencyObject d, object baseValue) {
            int max = (int)d.GetValue(MaxValueProperty);
            int min = (int)d.GetValue(MinValueProperty);
            if ((int)baseValue > max) return max;
            if ((int)baseValue < min) return min;
            return baseValue;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0) {
                int n = 0;
                if (!int.TryParse(textBox.Text, out n)) {
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
            int num = 0;
            if (!int.TryParse(tb.Text, out num)) num = 0;
            Value = num;
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e) { Value = Value; }

        private void up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { ++Value; }

        private void down_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { --Value; }

        private void tb_PreviewKeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Up: ++Value; break;
                case Key.Down: --Value; break;
            }
        }
    }
}
