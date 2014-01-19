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
    /// StandardColorPicker.xaml 的交互逻辑
    /// A UserControl shown as a customized button 
    /// which can call a System.Windows.Forms.ColorDialog .
    /// A WPF-wrap of the WinForm's ColorDialog.
    /// Property:
    ///     [DP] (System.Windows.Media.Color) SelectedColor
    /// Event:
    ///     SelectedColorChanged
    ///     
    /// </summary>
    public partial class StandardColorPicker : UserControl
    {
        //used to save custom colors in the corresponding ColorDialog.
        private int[] customColorsFromDialog;

        public StandardColorPicker()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(StandardColorPicker),
                new PropertyMetadata(new PropertyChangedCallback(SelectedColorPropertyChangedCallback)));

        public Color SelectedColor
        {
            get
            {
                return (Color)GetValue(SelectedColorProperty);
            }

            set
            {
                SetValue(SelectedColorProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedColorStringProperty =
           DependencyProperty.Register("SelectedColorString", typeof(String), typeof(StandardColorPicker));

        public String SelectedColorString {
            get {
                return (String)GetValue(SelectedColorStringProperty);
            }

            set {
                SetValue(SelectedColorStringProperty, value);
            }
        }

        private static void SelectedColorPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender != null && sender is StandardColorPicker)
            {
                StandardColorPicker picker = sender as StandardColorPicker;
                picker.OnSelectedColorChanged((Color)arg.OldValue, (Color)arg.NewValue);

            }
        }

        public static readonly RoutedEvent SelectedColorChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedColorChanged",
             RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(StandardColorPicker));

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add
            {
                this.AddHandler(SelectedColorChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(SelectedColorChangedEvent, value);
            }
        }

        protected virtual void OnSelectedColorChanged(Color oldValue, Color newValue)
        {
            if ((newValue.R + newValue.G + newValue.B) / 3 > 128) Foreground = Brushes.Black;
            else Foreground = Brushes.White;
            SelectedColorString= "#" + SelectedColor.ToString().Substring(3);
            //update displayed color if necessary
            RoutedPropertyChangedEventArgs<Color> arg =
                new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue, SelectedColorChangedEvent);
            this.RaiseEvent(arg);

        }

        private void btnChooseColor_Click(object sender, RoutedEventArgs e)
        {
            //set and show a ColorDialog.
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            dlg.Color = WpfColorToWinformColor(SelectedColor);
            dlg.FullOpen = true;
            if (customColorsFromDialog != null)
            {
                dlg.CustomColors = customColorsFromDialog;
            }
            //change SelectedColor according to the result of the dialog.
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedColor = WinformColorToWpfColor(dlg.Color);
            }
            //save custom colors.
            customColorsFromDialog = dlg.CustomColors;
        }

        //a method used to get a new WinForm Color from a WPF Color.
        private System.Drawing.Color WpfColorToWinformColor(System.Windows.Media.Color wpfColor)
        {
            return System.Drawing.ColorTranslator.FromHtml(wpfColor.ToString());
        }

        //a method used to get a new WPF Color from a WinForm Color.
        private System.Windows.Media.Color WinformColorToWpfColor(System.Drawing.Color winformColor)
        {
            System.Windows.Media.Color wpfColor = new System.Windows.Media.Color();
            wpfColor.A = winformColor.A;
            wpfColor.R = winformColor.R;
            wpfColor.G = winformColor.G;
            wpfColor.B = winformColor.B;
            return wpfColor;
        }
    }
}
