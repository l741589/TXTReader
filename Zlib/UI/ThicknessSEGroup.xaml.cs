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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zlib.UI
{
    /// <summary>
    /// ThicknessSEGroup.xaml 的交互逻辑
    /// a usercontrol that contains 4 SpinEdit's designed to adjust a Thickness object.
    /// </summary>
    public partial class ThicknessSEGroup : UserControl
    {
        public ThicknessSEGroup()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(Thickness), typeof(ThicknessSEGroup),
                new PropertyMetadata(new PropertyChangedCallback(ThicknessPropertyChangedCallback)));

        public Thickness Thickness
        {
            get
            {
                return (Thickness)GetValue(ThicknessProperty);
            }

            set
            {
                SetValue(ThicknessProperty, value);
            }
        }

        private static void ThicknessPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender != null && sender is ThicknessSEGroup)
            {
                ThicknessSEGroup picker = sender as ThicknessSEGroup;
                picker.OnThicknessChanged((Thickness)arg.OldValue, (Thickness)arg.NewValue);
            }
        }

        public static readonly RoutedEvent ThicknessChangedEvent =
            EventManager.RegisterRoutedEvent("ThicknessChanged",
             RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Thickness>), typeof(ThicknessSEGroup));

        public event RoutedPropertyChangedEventHandler<Thickness> ThicknessChanged
        {
            add
            {
                this.AddHandler(ThicknessChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(ThicknessChangedEvent, value);
            }
        }

       /* public void UpdateValues()
        {
            //Thickness thk = Options.Instance.Skin.Padding;
            seL.Value = (int)thk.Left;
            seT.Value = (int)thk.Top;
            seR.Value = (int)thk.Right;
            seB.Value = (int)thk.Bottom;
        }*/

        protected virtual void OnThicknessChanged(Thickness oldValue, Thickness newValue)
        {
            seL.Value = (int)newValue.Left;
            seT.Value = (int)newValue.Top;
            seR.Value = (int)newValue.Right;
            seB.Value = (int)newValue.Bottom;

            RoutedPropertyChangedEventArgs<Thickness> arg =
                new RoutedPropertyChangedEventArgs<Thickness>(oldValue, newValue, ThicknessChangedEvent);
            this.RaiseEvent(arg);

        }

        private void se_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            this.Thickness = new Thickness(seL.Value, seT.Value, seR.Value, seB.Value);
        }
    }
}
