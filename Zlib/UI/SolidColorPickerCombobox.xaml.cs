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

using System.Reflection;

namespace Zlib.UI
{
    /// <summary>
    /// SolidColorPickerCombobox.xaml 的交互逻辑
    /// A UserControl shown as a ComboBox which contains all colors in System.Windows.Media.Colors.
    /// Property:
    ///     [DP] (Color) SelectedColor
    /// Event:
    ///     SelectedColorChanged
    /// 
    /// </summary>
    public partial class SolidColorPickerCombobox : UserControl
    {
        //used to initialize the colorNames list.
        private static Type colorType = typeof(Colors);
        private static List<string> colorNames = (from MemberInfo color
                                                      in colorType.GetMembers()
                                                  where color.MemberType == MemberTypes.Property
                                                  select color.Name).ToList<string>();
        //List<string> colorNames;

        public SolidColorPickerCombobox()
        {
            InitializeComponent();
            cbxColor.ItemsSource = colorNames;
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(SolidColorPickerCombobox),
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

        private static void SelectedColorPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender != null && sender is SolidColorPickerCombobox)
            {
                SolidColorPickerCombobox picker = sender as SolidColorPickerCombobox;
                picker.OnSelectedColorChanged((Color)arg.OldValue, (Color)arg.NewValue);

            }
        }

        public static readonly RoutedEvent SelectedColorChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedColorChanged",
             RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(SolidColorPickerCombobox));

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
            //try to get the index of the Color in the colorNames list and set it as selected in combobox.
            int colorIndex = IndexOfColor(newValue);
            if (colorIndex != -1)
            {
                cbxColor.SelectedIndex = colorIndex;
            }

            RoutedPropertyChangedEventArgs<Color> arg =
                new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue, SelectedColorChangedEvent);
            this.RaiseEvent(arg);

        }

        //get the index of the color in colorNames list which equals the colorItem.
        private int IndexOfColor(Color colorItem)
        {
            return colorNames.FindIndex(x =>
                colorItem.Equals(ColorConverter.ConvertFromString(x)));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if (colorNames == null)
            //{
            //    var colorNamesResult = from MemberInfo color
            //                           in colorType.GetMembers()
            //                           where color.MemberType == MemberTypes.Property
            //                           select color.Name;
            //    colorNames = colorNamesResult.ToList<string>();
            //    cbxColor.ItemsSource = colorNames;
            //}
        }

        private void cbxColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //because the itemssource is a collection of strings.
            var tryColor = ColorConverter.ConvertFromString(cbxColor.SelectedItem as string);
            if (tryColor != null)
            {
                SelectedColor = (Color)tryColor;
            }
        }

    }
}
