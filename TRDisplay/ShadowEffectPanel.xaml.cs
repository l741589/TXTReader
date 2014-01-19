using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace TRDisplay {
    /// <summary>
    /// ShadowEffectPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ShadowEffectPanel : UserControl {

        public DropShadowEffect Value { get { return (DropShadowEffect)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(DropShadowEffect), typeof(ShadowEffectPanel), new PropertyMetadata(new DropShadowEffect(), _OnValueChanged));
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(ShadowEffectPanel));
        public event RoutedEventHandler ValueChanged { add { AddHandler(ValueChangedEvent, value); } remove { RemoveHandler(ValueChangedEvent, value); } }

        public double BlurRadius { get { return (double)GetValue(BlurRadiusProperty); } set { SetValue(BlurRadiusProperty, value); } }
        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register("BlurRadius", typeof(double), typeof(ShadowEffectPanel), new PropertyMetadata(5.0));
        public Color Color { get { return (Color)GetValue(ColorProperty); } set { SetValue(ColorProperty, value); } }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ShadowEffectPanel), new PropertyMetadata(Colors.Black));
        public double ShadowDepth { get { return (double)GetValue(ShadowDepthProperty); } set { SetValue(ShadowDepthProperty, value); } }
        public static readonly DependencyProperty ShadowDepthProperty = DependencyProperty.Register("ShadowDepth", typeof(double), typeof(ShadowEffectPanel), new PropertyMetadata(5.0));
        public double Direction { get { return (double)GetValue(DirectionProperty); } set { SetValue(DirectionProperty, value); } }
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(double), typeof(ShadowEffectPanel), new PropertyMetadata(315.0));
        public double EffectOpacity { get { return (double)GetValue(EffectOpacityProperty); } set { SetValue(EffectOpacityProperty, value); } }
        public static readonly DependencyProperty EffectOpacityProperty = DependencyProperty.Register("EffectOpacity", typeof(double), typeof(ShadowEffectPanel),new PropertyMetadata(1.0));
        

        private static void _OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { (d as ShadowEffectPanel).OnValueChanged(); }

        public ShadowEffectPanel() {
            InitializeComponent();
        }

        private bool valuechangelock = false;
        private void OnValueChanged() {
            if (valuechangelock) return;
            valuechangelock = true;
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent, this));
            BlurRadius = Value.BlurRadius;
            Direction = Value.Direction;
            ShadowDepth = Value.ShadowDepth;
            Color = Value.Color;
            EffectOpacity = Value.Opacity;
            valuechangelock = false;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
            if (e.Property == EffectOpacityProperty || e.Property == BlurRadiusProperty ||
                e.Property == ShadowDepthProperty || e.Property == DirectionProperty || e.Property == ColorProperty) {
                    if (!valuechangelock) {
                        var v = new DropShadowEffect();
                        v.BlurRadius = BlurRadius;
                        v.Direction = Direction;
                        v.ShadowDepth = ShadowDepth;
                        v.Color = Color; ;
                        v.Opacity = EffectOpacity;
                        Value = v;
                    }
            }
        }
    }
}
