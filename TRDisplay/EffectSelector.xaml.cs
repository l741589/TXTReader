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

namespace TRDisplay {
    /// <summary>
    /// EffectSelector.xaml 的交互逻辑
    /// </summary>
    public partial class EffectSelector : UserControl {
        public Effect Value { get { return (Effect)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Effect), typeof(EffectSelector), new PropertyMetadata(
            (d, e) => {
                EffectSelector o = d as EffectSelector;
                if (e.NewValue is DropShadowEffect) {
                    o.shadow.Value = e.NewValue as DropShadowEffect;
                    o.root.SelectedIndex = 1;
                } else if (e.NewValue == null) {
                    o.root.SelectedIndex = 0;
                }
            }
        ));

        public EffectSelector() {
            InitializeComponent();
            shadow.ValueChanged += shadow_ValueChanged;
        }

        void shadow_ValueChanged(object sender, RoutedEventArgs e) {
            if (root.SelectedIndex == 1) {
                Value = shadow.Value;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            switch (root.SelectedIndex) {
                case 0: Value = null; break;
                case 1: Value = shadow.Value; break;
            }
        }
    }

    public class Effectable : DependencyObject {        
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e) { }
    }

}
