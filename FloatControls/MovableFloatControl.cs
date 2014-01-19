using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FloatControls {
    public class MovableFloatControl : UserControl, IFloatControl {
        private System.Windows.Point? lastPoint = null;
        public FloatControlsPanel Panel { get; private set; }

        public MovableFloatControl()
            : base() {
            Panel = FloatControlsPanel.Instance;
            MouseMove += UserControl_MouseMove;
            MouseLeftButtonDown += UserControl_MouseLeftButtonDown;
            MouseLeftButtonUp += UserControl_MouseLeftButtonUp;
            SetBinding(ToolTipProperty, new Binding("Name") { Source=this,Mode = BindingMode.OneWay });
            Tag = GetType().Name;
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

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (lastPoint != null) {
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

        protected FloatPosition position = FloatPosition.LeftTop;
        virtual public FloatPosition Position {
            get {
                if (Parent == null) return position;
                return position = (FloatPosition)G.FloatControlsPanel[Parent as Panel];
            }
            set {
                if (position != value) position = value;
                G.FloatControls.Remove(this);
                G.FloatControls.Add(this);
            }
        }
    }
}
