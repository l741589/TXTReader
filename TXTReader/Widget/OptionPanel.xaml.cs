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
using TXTReader.Data;
using TXTReader.Utility;
using TXTReader.Converter;
using Microsoft.Win32;

namespace TXTReader.Widget
{
    /// <summary>
    /// OptionPanel.xaml 的交互逻辑
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
    public partial class OptionPanel : UserControl
    {
        public const int MAX_TOOLTIP_SIZE = 300;


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

        public OptionPanel()
        {
            InitializeComponent();
            Options = Data.Options.Instance;
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
            segPadding.GetBindingExpression(ThicknessSEGroup.ThicknessProperty).UpdateTarget();
            seEffectSize.GetBindingExpression(SpinEdit.ValueProperty).UpdateTarget();
            seFontSize.GetBindingExpression(SpinEdit.ValueProperty).UpdateTarget();
            seSpeed.GetBindingExpression(SpinEdit.ValueProperty).UpdateTarget();
            seLineSpacing.GetBindingExpression(SpinEdit.ValueProperty).UpdateTarget();
            seParaSpacing.GetBindingExpression(SpinEdit.ValueProperty).UpdateTarget();
            cbxFont.GetBindingExpression(FontPickerCombobox.SelectedFontProperty).UpdateTarget();
            btnBackgroundImage.ToolTip = new Image() { Source = Options.Skin.BackImage, MaxHeight = MAX_TOOLTIP_SIZE, MaxWidth = MAX_TOOLTIP_SIZE };
            ckbBold.IsChecked = G.Options.Skin.Font.Style == FontStyles.Italic;
            ckbItalic.IsChecked = G.Options.Skin.Font.Weight == FontWeights.Bold;
            if (cbxBackgroundType.SelectedItem != null) {
                switch ((Data.BackGroundType)((KeyValuePair<BackGroundType, String>)cbxBackgroundType.SelectedItem).Key) {
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

        private void cbxBackgroundType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = sender as ComboBox;
            if (combobox != null && cbxBackgroundColor != null && btnBackgroundImage != null)
            {
                //toggle usercontrol according to selected type.
                switch ((Data.BackGroundType)((KeyValuePair<BackGroundType,String>)e.AddedItems[0]).Key)
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
            } else {
                if (e.RemovedItems!=null&&e.RemovedItems.Count>0)
                combobox.SelectedItem = e.RemovedItems[0];
            }
        }

        private void btnBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            //set new imagesource via OpenFileDialog.
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "All Supported|*.jpg;*.jpeg;*.bmp;*.dib;*.png;*.gif|JPEG file|*.jpg|BITMAP file|*.bmp;*.dib|PNG file|*.png";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == true)
            {
                string imageFilename = dlg.FileName;
                ImageSource imgsrc = new BitmapImage(new Uri(imageFilename));
                Options.Skin.BackImage = imgsrc;
            }
            Update(e);
        }

        private void ckbBold_Checked(object sender, RoutedEventArgs e)
        {
            if (Options.Skin.Font != null)
            {
                Options.Skin.Font = new Typeface(Options.Skin.Font.FontFamily, Options.Skin.Font.Style, FontWeights.Bold, Options.Skin.Font.Stretch);
            }
        }

        private void cbxFont_SelectedFontChanged(object sender, RoutedPropertyChangedEventArgs<FontFamily> e)
        {
            if (ckbBold != null && ckbItalic != null)
            {
                ckbBold.IsChecked = false;
                ckbItalic.IsChecked = false;
            }
        }

        private void ckbItalic_Checked(object sender, RoutedEventArgs e)
        {
            if (Options.Skin.Font != null)
            {
                Options.Skin.Font = new Typeface(Options.Skin.Font.FontFamily, FontStyles.Italic, Options.Skin.Font.Weight, Options.Skin.Font.Stretch);
            }
        }

        private void ckbBold_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Options.Skin.Font != null)
            {
                Options.Skin.Font = new Typeface(Options.Skin.Font.FontFamily, Options.Skin.Font.Style, FontWeights.Normal, Options.Skin.Font.Stretch);
            }
        }

        private void ckbItalic_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Options.Skin.Font != null)
            {
                Options.Skin.Font = new Typeface(Options.Skin.Font.FontFamily, FontStyles.Normal, Options.Skin.Font.Weight, Options.Skin.Font.Stretch);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs ev) {
            
            UpdateOptionsUI();
            AddHandler(ThicknessSEGroup.ThicknessChangedEvent, new RoutedPropertyChangedEventHandler<Thickness>((d, e) => { Update(e); }));
            AddHandler(SpinEdit.ValueChangedEvent, new RoutedPropertyChangedEventHandler<int>((d, e) => { Update(e); }));
            AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler((d, e) => { Update(e); }));
            AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler((d, e) => { Update(e); }));
            AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler((d, e) => { Update(e); }));
            AddHandler(StandardColorPicker.SelectedColorChangedEvent, new RoutedPropertyChangedEventHandler<Color>((d, e) => { Update(e); }));
            G.MainWindow.floatMessagePanel.Time.SetBinding(FloatMessage.VisibilityProperty, new Binding("IsChecked") { Source = ckbTime,Converter=new VisiblilityConverter() });
            G.MainWindow.floatMessagePanel.Title.SetBinding(FloatMessage.VisibilityProperty, new Binding("IsChecked") { Source = ckbChapterTitle, Converter = new VisiblilityConverter() });
            G.MainWindow.floatMessagePanel.Fps.SetBinding(FloatMessage.VisibilityProperty, new Binding("IsChecked") { Source = ckbFps, Converter = new VisiblilityConverter() });
            G.MainWindow.floatMessagePanel.Position.SetBinding(FloatMessage.VisibilityProperty, new Binding("IsChecked") { Source = ckbProgress, Converter = new VisiblilityConverter() });
            G.MainWindow.floatMessagePanel.Speed.SetBinding(FloatMessage.VisibilityProperty, new Binding("IsChecked") { Source = ckbSpeed, Converter = new VisiblilityConverter() });
            G.MainWindow.floatMessagePanel.SetBinding(FloatMessagePanel.VisibilityProperty, new Binding("IsChecked") { Source = ckbFloatMessage, Converter = new VisiblilityConverter() });
        }

        private void Update(RoutedEventArgs e) {
            G.Displayer.UpdateSkin();
            G.MainWindow.floatMessagePanel.UpdateColor();
            btnBackgroundImage.ToolTip = new Image() { Source = Options.Skin.BackImage, MaxHeight = MAX_TOOLTIP_SIZE, MaxWidth = MAX_TOOLTIP_SIZE };
            e.Handled = true;
        }

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
                G.Displayer.UpdateSkin();
                UpdateOptionsUI();
            }
        }
    }
}
