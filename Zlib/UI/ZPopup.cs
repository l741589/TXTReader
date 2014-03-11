using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Zlib.UI {
    public class ZPopup : Popup {

        //private Control Target { get { return PlacementTarget as Control; } }
        public Control Target { get { return (Control)GetValue(TargetProperty); } set { SetValue(TargetProperty, value); } }
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Control), typeof(ZPopup), new PropertyMetadata(null, (d, e) => {
            (d as ZPopup).UpdateEventCall();
        }));

        static ZPopup() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZPopup), new FrameworkPropertyMetadata(typeof(ZPopup)));
        }

        public ZPopup()
            : base() {
            MouseLeave += _MouseLeave;
            MouseUp += _MouseUp;
        }

        void UpdateEventCall() {
            if (Target != null) {
                Target.MouseEnter += Target_MouseEnter;
                Target.MouseLeave += Target_MouseLeave;                
            }
        }

        private void Target_MouseEnter(object sender, MouseEventArgs e) {
            IsOpen = true;
        }

        private void Target_MouseLeave(object sender, MouseEventArgs e) {
            if (IsMouseOver) return;
            IsOpen = false;
        }

        private void _MouseLeave(object sender, MouseEventArgs e) {
            if (Target.IsMouseOver) return;
            IsOpen = false;
        }

        private void _MouseUp(object sender, MouseButtonEventArgs e) {
            IsOpen = false;
        }
    }
}
