﻿using System;
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
using Zlib.Utility;

namespace Zlib.UI {
    /// <summary>
    /// SpinEdit.xaml 的交互逻辑
    /// </summary>
    public partial class SpinEdit : UserControl {
        
        public event RoutedPropertyChangedEventHandler<int> ValueChanged { add { this.AddHandler(ValueChangedEvent, value); } remove { this.RemoveHandler(ValueChangedEvent, value); } }
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(SpinEdit));

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(SpinEdit), new PropertyMetadata(0x7fffffff));
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(SpinEdit), new PropertyMetadata(0x0));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(SpinEdit), new PropertyMetadata(0, OnValueChanged, CoerceValue));

        public int MaxValue { get { return (int)GetValue(MaxValueProperty); } set { SetValue(MaxValueProperty, value); } }
        public int MinValue { get { return (int)GetValue(MinValueProperty); } set { SetValue(MinValueProperty, value); } }
        public int Value { get { return (int)GetValue(ValueProperty); } set { SetValue(ValueProperty, CoerceValue(this,value)); tb.Text = Value.ToString(); } }

        public SpinEdit() {  InitializeComponent(); }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            RoutedPropertyChangedEventArgs<int> arg =
                new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, ValueChangedEvent);
            SpinEdit se = ((SpinEdit)d);
            if (se.tb.Text != se.Value.ToString()) se.tb.Text = se.Value.ToString();
            se.RaiseEvent(arg);
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

        private bool mousedown = false;
        private bool valuelock = false;
        private async void Plus() {
            ++Value;
            if (valuelock) return;
            valuelock = true;
            int time = 1024;
            while (mousedown) {                
                await time.Wait();
                if (!mousedown) break;
                ++Value;
                if (time > 16) time >>= 1;
            }
            valuelock = false;
        }

        private async void Minus() {
            --Value;
            if (valuelock) return;
            valuelock = true;            
            int time = 1024;
            while (mousedown) {                
                await time.Wait();
                if (!mousedown) break;
                --Value;
                if (time > 16) time >>= 1;
            }
            valuelock = false;
        }

        private void up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            mousedown = true;
            Plus();
        }

        private void down_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            mousedown = true;
            Minus();
        }

        private void up_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            mousedown = false;
        }

        private void down_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            mousedown = false;
        }

        private void tb_PreviewKeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Up: ++Value; break;
                case Key.Down: --Value; break;
            }
        }

        private void tb_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (tb.IsFocused) {
                Value += (int)((double)e.Delta / 120);
                e.Handled = true;
            }
        }


    }
}
