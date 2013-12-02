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
    /// OptionPanel2.xaml 的交互逻辑
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
    /// </summary>
    public partial class OptionPanel2 : UserControl
    {

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options", typeof(TXTReader.Data.Options), typeof(OptionPanel2), new PropertyMetadata(null));

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

        public OptionPanel2()
        {
            InitializeComponent();
            Options = Data.Options.Instance;
            if (Options.Skin.Font != null)
            {
                cbxFont.SelectedFont = Options.Skin.Font.FontFamily;
            }
            //UpdateOptionsUI();
        }

        public void UpdateOptionsUI()
        {

        }

        //private void LoadComboboxEnumTypes()
        //{

        //}

        //private void btnTest_Click(object sender, RoutedEventArgs e)
        //{
        //    cbxFont.SelectedFont = new FontFamily("Impact");
        //    cbxFontColor.SelectedColor = Colors.DarkBlue;
        //}

        private void cbxFont_SelectedFontChanged(object sender, RoutedPropertyChangedEventArgs<FontFamily> e)
        {
            FontPickerCombobox picker = sender as FontPickerCombobox;
            if (Options != null)
            {
                //
                Options.Skin.Font = new Typeface(picker.SelectedFont, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            }
        }

        private void cbxFontColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            SolidColorPickerCombobox picker = sender as SolidColorPickerCombobox;
            if (Options != null)
            {
                Options.Skin.Foreground = new SolidColorBrush(cbxFontColor.SelectedColor);
            }
        }

        private void cbxEffectColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            SolidColorPickerCombobox picker = sender as SolidColorPickerCombobox;
            if (picker != null && Options != null)
            {
                Options.Skin.Effect = new SolidColorBrush(cbxEffectColor.SelectedColor);
            }
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

        private void cbxBackgroundColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            SolidColorPickerCombobox picker = sender as SolidColorPickerCombobox;
            if (picker != null && Options != null)
            {
                Options.Skin.BackColor = cbxBackgroundColor.SelectedColor;
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

        private void cbxBackgroundColor_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            StandardColorPicker picker = sender as StandardColorPicker;
            if (picker != null && Options != null)
            {
                Options.Skin.BackColor = cbxBackgroundColor.SelectedColor;
            }
        }

        //private void cbxEffectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ComboBox combobox = sender as ComboBox;
        //    if (combobox != null && Options!=null)
        //    {
        //        Options.Skin.EffectType = (Data.EffectType)combobox.SelectedValue;
        //    }
        //}

    }
}
