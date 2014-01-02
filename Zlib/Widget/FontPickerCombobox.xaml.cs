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
using System.Windows.Markup;

namespace Zlib.Widget
{
    /// <summary>
    /// FontPickerCombobox.xaml 的交互逻辑
    /// a UserControl displayed as a ComboBox which contains all system fonts, 
    /// Property:
    ///     [DP] (System.Windows.Media.FontFamily) SelectedFont
    /// Event:
    ///     SelectedFontChanged
    ///     
    /// </summary>
    public partial class FontPickerCombobox : UserControl
    {
        //the list that holds all system fonts, initialized when FontPickerCombobox is loaded.
        private List<FontFamily> systemFontFamilyList;

        public FontPickerCombobox()
        {
            InitializeComponent();
            ////DependencyProperty default value initialized here to avoid referencing problems about PropertyMetadata.
            if (SelectedFont == null)
                SelectedFont = new FontFamily("宋体");
        }

        public static readonly DependencyProperty SelectedFontProperty =
            DependencyProperty.Register("SelectedFont", typeof(FontFamily), typeof(FontPickerCombobox),
                new PropertyMetadata(null, new PropertyChangedCallback(SelectedFontPropertyChangedCallback)));

        public FontFamily SelectedFont
        {
            get
            {
                return (FontFamily)GetValue(SelectedFontProperty);
            }

            set
            {
                SetValue(SelectedFontProperty, value);
            }
        }

        private static void SelectedFontPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender != null && sender is FontPickerCombobox)
            {
                FontPickerCombobox picker = sender as FontPickerCombobox;
                picker.OnSelectedFontChanged((FontFamily)(arg.OldValue), (FontFamily)(arg.NewValue));
            }
        }

        public static readonly RoutedEvent SelectedFontChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedFontChanged",
             RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontFamily>), typeof(FontPickerCombobox));

        public event RoutedPropertyChangedEventHandler<FontFamily> SelectedFontChanged
        {
            add
            {
                this.AddHandler(SelectedFontChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(SelectedFontChangedEvent, value);
            }
        }

        protected virtual void OnSelectedFontChanged(FontFamily oldValue, FontFamily newValue)
        {
            cbxFont.SelectedItem = newValue as FontFamily;

            RoutedPropertyChangedEventArgs<FontFamily> arg =
                new RoutedPropertyChangedEventArgs<FontFamily>(oldValue, newValue, SelectedFontChangedEvent);
            this.RaiseEvent(arg);
        }

        //set the ItemsSource of the combobox to the initialized fontlist.
        private void LoadFontItemsSource()
        {
            InitializeSystemFontFamilyList();
            cbxFont.ItemsSource = systemFontFamilyList;
        }

        //the reason why SystemFontList is not binded just in XAML is that
        //some fonts is needed to be shown in different languages.
        //this method saves system fontfamilies in specified languages into the list.
        //for example, standard Simplified Chinese system font SimSun 
        //will be shown as "宋体", not "SimSun", generally.
        //
        private void InitializeSystemFontFamilyList()
        {
            foreach (FontFamily ff in Fonts.SystemFontFamilies)
            {
                LanguageSpecificStringDictionary fontDics = ff.FamilyNames;

                //find fonts in specified languages.
                if (fontDics.ContainsKey(XmlLanguage.GetLanguage("zh-cn")))
                {
                    string fontName = null;
                    if (fontDics.TryGetValue(XmlLanguage.GetLanguage("zh-cn"), out fontName))
                    {
                        systemFontFamilyList.Add(new FontFamily(fontName));
                    }
                }
                else
                {
                    //other fonts goes in US English is just okay for now.
                    string fontName = null;
                    if (fontDics.TryGetValue(XmlLanguage.GetLanguage("en-us"), out fontName))
                    {
                        systemFontFamilyList.Add(new FontFamily(fontName));
                    }
                }
            }
            systemFontFamilyList.Sort(new Comparison<FontFamily>(
                (a, b) => a.ToString().CompareTo(b.ToString())));
        }

        private void cbxFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFont = cbxFont.SelectedItem as FontFamily;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (systemFontFamilyList == null)
            {
                systemFontFamilyList = new List<System.Windows.Media.FontFamily>();
                LoadFontItemsSource();
            }
        }
    }
}
