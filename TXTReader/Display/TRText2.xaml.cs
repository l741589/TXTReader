using System;
using System.Collections.Generic;
using System.Globalization;
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
using TXTReader.Converter;

namespace TXTReader.Display {
    /// <summary>
    /// TRText2.xaml 的交互逻辑
    /// </summary>
    public partial class TRText2 : UserControl {

        public static readonly DependencyProperty LineSpacingProperty = DependencyProperty.Register("LineSpacing", typeof(double), typeof(TRText2), 
            new PropertyMetadata((d, e) => { ((TRText2)d).text.LineHeight = (double)e.NewValue; }));
        public static readonly DependencyProperty ParaSpacingProperty = DependencyProperty.Register("ParaSpacing", typeof(double), typeof(TRText2));

        public double LineSpacing { get { return (double)GetValue(LineSpacingProperty); } set { SetValue(LineSpacingProperty, value); } }//行间距
        public double ParaSpacing { get { return (double)GetValue(ParaSpacingProperty); } set { SetValue(ParaSpacingProperty, value); } }//段间距
        public TextBlock TextBlock { get { return text; } }
        public FormattedText FormattedText { get; set; }
        public double GuessedHeight { get { return FormattedText.Height + ParaSpacing; } }
        public int Index { get; set; }

        public TRText2() {
            InitializeComponent();
            init();
        }

        public TRText2(String text,int index) {
            InitializeComponent();
            Index = index;
            TextBlock.Text = text;
            init();
        }

        public void init(){
            SetBinding(LineSpacingProperty, new Binding("LineHeight") { Source = text });
            SetBinding(ParaSpacingProperty, new Binding("Margin") { Source = text, Converter = new ParaSpacingCvt() });
            FormattedText = new FormattedText(text.Text, CultureInfo.CurrentCulture, text.FlowDirection,
                new Typeface(text.FontFamily, text.FontStyle, text.FontWeight, text.FontStretch)
                , text.FontSize, text.Foreground);
            FormattedText.LineHeight = LineSpacing;
        }
    }
}
