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

namespace TXTReader.Widget
{
    /// <summary>
    /// OptionPanel3.xaml 的交互逻辑
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
    /// Now most of the options are working with bindings and converters, 
    /// while the former version of OptionPanel is based on the events of UI elements.
    /// </summary>
    public partial class OptionPanel3 : UserControl
    {

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options", typeof(TXTReader.Data.Options), typeof(OptionPanel), new PropertyMetadata(null));

        public TXTReader.Data.Options Options
        {
            get
            {
                return (TXTReader.Data.Options)GetValue(OptionsProperty);
            }

            set
            {
                SetValue(OptionsProperty, value);
            }
        }

        public OptionPanel3()
        {
            InitializeComponent();
            Options = Data.Options.Instance;
            if (Options.Skin.Font != null)
            {
                cbxFont.SelectedFont = Options.Skin.Font.FontFamily;
            }
        }

        /// <summary>
        /// call this method to update the binding target(UI) according to the Options instance.
        /// use this instead of implementing interface INotifyPropertyChanged in Options class and Skin class.
        /// </summary>
        public void UpdateOptionsUI()
        {
            cbxBackgroundColor.GetBindingExpression(StandardColorPicker.SelectedColorProperty).UpdateTarget();
            cbxEffectColor.GetBindingExpression(StandardColorPicker.SelectedColorProperty).UpdateTarget();
            cbxFontColor.GetBindingExpression(StandardColorPicker.SelectedColorProperty).UpdateTarget();
            cbxFont.GetBindingExpression(FontPickerCombobox.SelectedFontProperty).UpdateTarget();
            cbxEffectType.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateTarget();
            cbxBackgroundType.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateTarget();
            seEffectSize.GetBindingExpression(SpinEdit.ValueProperty).UpdateTarget();
            seFontSize.GetBindingExpression(SpinEdit.ValueProperty).UpdateTarget();
            seSpeed.GetBindingExpression(SpinEdit.ValueProperty).UpdateTarget();
            seLineSpacing.GetBindingExpression(SpinEdit.ValueProperty).UpdateTarget();
            seParaSpacing.GetBindingExpression(SpinEdit.ValueProperty).UpdateTarget();
        }

        private void cbxBackgroundType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = sender as ComboBox;
            if (combobox != null && cbxBackgroundColor != null && btnBackgroundImage != null)
            {
                //toggle usercontrol according to selected type.
                switch ((Data.BackGroundType)combobox.SelectedValue)
                {
                    case Data.BackGroundType.SolidColor:
                        cbxBackgroundColor.Visibility = Visibility.Visible;
                        btnBackgroundImage.Visibility = Visibility.Collapsed;
                        break;
                    case Data.BackGroundType.Image:
                        cbxBackgroundColor.Visibility = Visibility.Collapsed;
                        btnBackgroundImage.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void btnBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            //set new imagesource via OpenFileDialog.
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPEG file|*.jpg";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == true)
            {
                string imageFilename = dlg.FileName;
                ImageSource imgsrc = new BitmapImage(new Uri(imageFilename));
                Options.Skin.BackImage = imgsrc;
            }
        }
    }
}
