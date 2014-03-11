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
using Microsoft.Win32;
using TXTReader;
using Zlib.UI;

namespace TRDisplay {
    /// <summary>
    /// DisplayOptionPanel.xaml 的交互逻辑
    /// 
    /// A UserControl shown as a panel with all properties defined in TXTReader.Data.Options.
    /// Provides a way to adjust the options in a panel.
    /// Property:
    ///     Options (DependencyProperty)
    /// Notice:
    /// Most UI elements(except those related with font and color) 
    /// is binded to TXTReader.Data.Options.Instance in XAML via ObjectDataProvider.
    /// Although these elements' properties are binded in mode "TwoWay",
    /// the Data.Options class doesn't implements the interface INotifyPropertyChanged.
    /// So this kind of binding does not update the data in these UI elements 
    /// when the Options instance itself is modified by other things after the OptionPanel is initialized.
    /// 
    /// Last Modification:
    /// Added public method UpdateOptionsUI() which updates the target values of bindings.
    /// </summary>
    public partial class DisplayOptionPanel : UserControl {
        public DisplayOptionPanel() {           
            InitializeComponent();
            Options = Options.Instance;
            Loaded += DisplayOptionPanel_Loaded;
            (G.Displayer as Control).SizeChanged += (d, e) => {
                sel_bg.Viewport = new Rect(e.NewSize);
            };
        }

        void DisplayOptionPanel_Loaded(object sender, RoutedEventArgs e) {
        }

        public const int MAX_TOOLTIP_SIZE = 300;
        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register("Options", typeof(Options), typeof(DisplayOptionPanel), new PropertyMetadata(null));
        public Options Options { get { return (Options)GetValue(OptionsProperty); } set { SetValue(OptionsProperty, value); } }
      
        private void bn_saveskin_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog dlg=new SaveFileDialog(){
                Filter="TXTReader皮肤|*.trs",
                AddExtension=true,
                DefaultExt="*.trs"
            };
            if (dlg.ShowDialog()==true){
                SkinParser.Save(dlg.FileName);
            }
        }

        private void bn_loadskin_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog() {
                Filter = "TXTReader皮肤|*.trs",
                AddExtension = true,
                DefaultExt = "*.trs"
            };
            if (dlg.ShowDialog() == true) {
                SkinParser.Load(dlg.FileName);
                //Update(e);
            }
        }
    }
}
