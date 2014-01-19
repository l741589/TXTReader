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
using Zlib.Text;

namespace Zlib.UI {
    /// <summary>
    /// FontEditPanel.xaml 的交互逻辑
    /// </summary>
    public partial class FontEditPanel : UserControl {

        public Typeface Font { get { return (Typeface)GetValue(FontProperty); } set { SetValue(FontProperty, value); } }
        public static readonly DependencyProperty FontProperty = DependencyProperty.Register("Font", typeof(Typeface), typeof(FontEditPanel), new PropertyMetadata(
            (d, e) => {
                FontEditPanel o = d as FontEditPanel;
                o.UpdateFontValue(e.NewValue as Typeface);
                o.OnFontChanged();
            }
        ));

        private bool ui2evt = false;
        public void UpdateFontValue(Typeface v) {
            if (!ui2evt) {
                cb_name.SelectedFont = v.FontFamily;
                bn_italic.IsChecked = v.Style == FontStyles.Italic;
                bn_bold.IsChecked = v.Weight == FontWeights.Bold;
            }
        }

        public static readonly RoutedEvent FontChangedEvent = EventManager.RegisterRoutedEvent("FontChanged", RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(FontEditPanel));
        private void OnFontChanged() { RaiseEvent(new RoutedEventArgs(FontChangedEvent, this)); }

        public FontEditPanel() {
            InitializeComponent();
        }

        public void OnChanged(object sender, RoutedEventArgs e){
            if (ui2evt) return;
            ui2evt = true;
            Font = new Typeface(cb_name.SelectedFont,
                bn_italic.IsChecked == true ? FontStyles.Italic : FontStyles.Normal,
                bn_bold.IsChecked == true ? FontWeights.Bold : FontWeights.Normal,
                FontStretches.Normal
                );
            ui2evt = false;
        }

        private void cb_name_SelectedFontChanged(object sender, RoutedPropertyChangedEventArgs<FontFamily> e) {
            OnChanged(sender, e);
        }       
    }
}
