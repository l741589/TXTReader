using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using Zlib.Converter;
using Zlib.Utility;

namespace FloatControls {
    /// <summary>
    /// FloatControlOptionPanel.xaml 的交互逻辑
    /// </summary>
    public partial class FloatControlOptionPanel : UserControl {

        class ItemProxy : DependencyObject, IItemProxy {
            public object Target { get; private set; }
            public ItemProxy(IFloatControl target) {
                Target = target;
                BindingOperations.SetBinding(this, VisibilityProperty, new Binding("Visibility") { Mode=BindingMode.TwoWay,Source=target});
                BindingOperations.SetBinding(this, NameProperty, new Binding("Name") { Source=target});
            }

            public static readonly DependencyProperty VisibilityProperty =
                DependencyProperty.Register("Visibility", typeof(Visibility), typeof(ItemProxy));

            public static readonly DependencyProperty NameProperty =
                DependencyProperty.Register("Name", typeof(String), typeof(ItemProxy));

        }

        public FloatControlOptionPanel() {
            InitializeComponent();
            body.ItemsSource = new ItemsProxy(G.FloatControls, t => new ItemProxy(t as IFloatControl));
            header.SetBinding(CheckBox.IsCheckedProperty, new Binding("Visibility") { Source = G.FloatControlsPanel, Converter = new VisiblilityConverter(), Mode = BindingMode.TwoWay });
            header.IsChecked = G.FloatControls.Show;
            G.FloatControlOptionPanel = this;
        }
    }
}
