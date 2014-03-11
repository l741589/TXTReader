using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Resources;
using System.Xml;
using TXTReader;
using TXTReader.ToolPanel;
using Zlib.Algorithm;
using Zlib.Text;
using System.Windows.Controls;
using Zlib.UI.Utility;
using Zlib.Text.Xml;

namespace FloatControls {
    class SkinParser : XmlParser, IXmlParsable{

        public const String URI_SCHEME_SKIN = "http://txtreader.org/skin.xsd";

        public const String S_SKIN = "skin";
        public const String S_PART = "part";
        public const String S_BACKGROUND = "background";
        public const String S_IMAGE = "img";
        public const String S_FONT = "font";
        public const String S_SIZE = "size";
        public const String S_COLOR = "color";
        public const String S_FONTSIZE = "fontsize";
        public const String S_FONTCOLOR = "fontcolor";
        public const String S_NAME = "name";
        public const String S_STRETCH = "stretch";
        public const String S_WEIGHT = "weight";
        public const String S_STYLE = "style";
        public const String S_FORMAT = "format";
        public const String S_LINESPACING = "linespacing";
        public const String S_PARASPACING = "paraspacing";
        public const String S_EFFECT = "effect";
        public const String S_SHADOW = "shadow";
        public const String S_TYPE = "type";
        public const String S_PADDING = "padding";
        public const String S_NONE = "none";
        public const String S_STROKE = "stroke";

        private FontParser FontParser = new FontParser();

        public new XmlParserReaderCallback Read {
            get {
                return r => r.Child(S_SKIN, new Version("1.0.0.0"))
                        .Do(FontParser).Do(n => Skin.Instance.Font = FontParser.Font)
                        .Read(S_FONTSIZE, n => Skin.Instance.FontSize = double.Parse(n.InnerText))
                        .Read(S_FONTCOLOR, n => Skin.Instance.Foreground = n.InnerText!=null?(Brush)new BrushConverter().ConvertFrom(n.InnerText):null)
                        .Read(S_BACKGROUND, n => Skin.Instance.Background = n.InnerText!=null?(Brush)new BrushConverter().ConvertFrom(n.InnerText):null)
                        .Read(S_PADDING, n => Skin.Instance.Padding = (Thickness)new ThicknessConverter().ConvertFrom(n.InnerText))
                    .Parent;
            }
        }

        public new XmlParserWriterCallback Write {
            get {
                return w => w.Start(S_SKIN).Ver(new Version("1.0.0.0"))
                    .Do(n => FontParser.Font = Skin.Instance.Font).Do(FontParser)
                    .Write(S_FONTSIZE, Skin.Instance.FontSize)
                    .Write(S_FONTCOLOR, Skin.Instance.Foreground)
                    .Write(S_BACKGROUND,Skin.Instance.Background)
                    .Write(S_PADDING, Skin.Instance.Padding)
                .End;
            }
        }
    }
}
