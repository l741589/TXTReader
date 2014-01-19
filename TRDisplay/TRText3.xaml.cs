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
using System.Windows.Media.Effects;
using TXTReader.ToolPanel;

namespace TRDisplay {
    /// <summary>
    /// TRText3.xaml 的交互逻辑
    /// </summary>
    public partial class TRText3 : TextBlock {

        public FormattedText FormattedText { get; set; }
        public double GuessedHeight { get { return FormattedText.Height; } }
        public int Index { get; set; }
        public bool Updated { get; set; }
        private double singleLineHeight;
        public double SingleLineHeight { get { return singleLineHeight; } }

        static TRText3() {
            LineHeightProperty.OverrideMetadata(typeof(TRText3), new FrameworkPropertyMetadata(
               (d, e) => { (d as TRText3).FormattedText.LineHeight = (double)e.NewValue; }));
        }

        public TRText3() {
            InitializeComponent();
            init();
        }

        public TRText3(String text,int i){
            InitializeComponent();
            if (text == "") text += " ";
            Text = text;
            Index = i;
            init();
        }       

        public void init() {
            FlowDirection = FlowDirection.LeftToRight;
            FontFamily = Options.Instance.Skin.Font.FontFamily;
            FontStyle = Options.Instance.Skin.Font.Style;
            FontStretch = Options.Instance.Skin.Font.Stretch;
            FontWeight = Options.Instance.Skin.Font.Weight;
            FontSize = Options.Instance.Skin.FontSize;
            Effect = Options.Instance.Skin.Effect;
            Foreground = Options.Instance.Skin.Foreground;
            FormattedText = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection,
                 Options.Instance.Skin.Font, FontSize, Foreground);
            singleLineHeight = FormattedText.Height;
            TextTrimming = FormattedText.Trimming = TextTrimming.None;
            TextAlignment = FormattedText.TextAlignment = TextAlignment.Left;
            Updated = false;
        }

        private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e) {
            FormattedText.MaxTextWidth = e.NewSize.Width;
        }
    }
}
