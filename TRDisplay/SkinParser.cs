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
using Zlib.Text.Xml;

namespace TRDisplay {
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

        public static void SetDefaultSkin() {
            Options.Instance.Skin.LineSpacing = 4;
            Options.Instance.Skin.ParaSpacing = 8;
            Options.Instance.Skin.Font = new Typeface("宋体");
            Options.Instance.Skin.FontSize = 12;
            Options.Instance.Skin.Foreground = Brushes.Black;
            Options.Instance.Skin.Padding = new Thickness(16);
            Options.Instance.Skin.Effect = null;
            Options.Instance.Skin.Background = Brushes.Black;
        }

/*//废弃

        public static Typeface ParseFont(XmlNode node) {
            if (!node.HasChildNodes) return Options.Instance.Skin.Font;
            FontStretch stretch = Options.Instance.Skin.Font != null ? Options.Instance.Skin.Font.Stretch : FontStretches.Normal;
            FontStyle style = Options.Instance.Skin.Font != null ? Options.Instance.Skin.Font.Style : FontStyles.Normal;
            FontWeight weight = Options.Instance.Skin.Font != null ? Options.Instance.Skin.Font.Weight : FontWeights.Normal;
            FontFamily family = null;
            for (var n = node.FirstChild; n != null; n = n.NextSibling) {
                switch (n.Name.ToLower()) {
                    case "name": family = new FontFamily(n.InnerText); break;
                    case "size": Options.Instance.Skin.FontSize = double.Parse(n.InnerText); break;
                    case "color": Options.Instance.Skin.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(n.InnerText)); break;
                    case "weight": weight = ParseFontWeight(n.InnerText); break;
                    case "style": style = ParseFontStyle(n.InnerText); break;
                    case "stretch": stretch = ParseFontStretch(n.InnerText); break;
                }
            }
            if (family == null) return Options.Instance.Skin.Font;
            return new Typeface(family, style, weight, stretch);
        }

        public static FontWeight ParseFontWeight(String text) {
            switch (text) {
                case "black": return FontWeights.Black;
                case "bold": return FontWeights.Bold;
                case "demibold": return FontWeights.DemiBold;
                case "extrablack": return FontWeights.ExtraBlack;
                case "extrabold": return FontWeights.ExtraBold;
                case "extralight": return FontWeights.ExtraLight;
                case "heavy": return FontWeights.Heavy;
                case "light": return FontWeights.Light;
                case "medium": return FontWeights.Medium;
                case "normal": return FontWeights.Normal;
                case "regular": return FontWeights.Regular;
                case "semibold": return FontWeights.SemiBold;
                case "thin": return FontWeights.Thin;
                case "ultrablack": return FontWeights.UltraBlack;
                case "ultrabold": return FontWeights.UltraBold;
                case "ultralight": return FontWeights.UltraLight;
                default: return default(FontWeight);
            }
        }

        public static String ParseFontWeight(FontWeight weight) {
            if (weight == FontWeights.Black) return "black";
            if (weight == FontWeights.Bold) return "bold";
            if (weight == FontWeights.DemiBold) return "demibold";
            if (weight == FontWeights.ExtraBlack) return "extrablack";
            if (weight == FontWeights.ExtraBold) return "extrabold";
            if (weight == FontWeights.ExtraLight) return "extralight";
            if (weight == FontWeights.Heavy) return "heavy";
            if (weight == FontWeights.Light) return "light";
            if (weight == FontWeights.Medium) return "medium";
            if (weight == FontWeights.Normal) return "normal";
            if (weight == FontWeights.Regular) return "regular";
            if (weight == FontWeights.SemiBold) return "semibold";
            if (weight == FontWeights.Thin) return "thin";
            if (weight == FontWeights.UltraBlack) return "ultrablack";
            if (weight == FontWeights.UltraBold) return "ultrabold";
            if (weight == FontWeights.UltraLight) return "ultralight";
            return weight.ToString().ToLower();
        }

        public static FontStretch ParseFontStretch(String text) {
            switch (text) {
                case "condensed": return FontStretches.Condensed;
                case "expanded": return FontStretches.Expanded;
                case "extraCondensed": return FontStretches.ExtraCondensed;
                case "extraExpanded": return FontStretches.ExtraExpanded;
                case "medium": return FontStretches.Medium;
                case "normal": return FontStretches.Normal;
                case "semiCondensed": return FontStretches.SemiCondensed;
                case "semiExpanded": return FontStretches.SemiExpanded;
                case "ultraCondensed": return FontStretches.UltraCondensed;
                case "ultraExpanded": return FontStretches.UltraExpanded;
                default: try { return FontStretch.FromOpenTypeStretch(int.Parse(text)); } catch { return default(FontStretch); }
            }
        }

        public static String ParseFontStretch(FontStretch stretch) {
            if (stretch == FontStretches.Condensed) return "condensed";
            if (stretch == FontStretches.Expanded) return "expanded";
            if (stretch == FontStretches.ExtraCondensed) return "extraCondensed";
            if (stretch == FontStretches.ExtraExpanded) return "extraExpanded";
            if (stretch == FontStretches.Medium) return "medium";
            if (stretch == FontStretches.Normal) return "normal";
            if (stretch == FontStretches.SemiCondensed) return "semiCondensed";
            if (stretch == FontStretches.SemiExpanded) return "semiExpanded";
            if (stretch == FontStretches.UltraCondensed) return "ultraCondensed";
            if (stretch == FontStretches.UltraExpanded) return "ultraExpanded";
            return stretch.ToString().ToLower();
        }

        public static FontStyle ParseFontStyle(String text) {
            switch (text) {
                case "normal": return FontStyles.Normal;
                case "oblique": return FontStyles.Oblique;
                case "italic": return FontStyles.Italic;
                default: return default(FontStyle);
            }
        }

        public static String ParseFontStyle(FontStyle style) {
            if (style==FontStyles.Normal    ) return "normal";
            if (style== FontStyles.Oblique  ) return "oblique";
            if (style==FontStyles.Italic    ) return "italic";
            return style.ToString().ToLower();
        }

        public static String ParseEffect(EffectType effect) {
            switch (effect) {
                case EffectType.None: return "none";
                case EffectType.Shadow: return "shadow";
                case EffectType.Stroke: return "stroke";
                default: return effect.ToString().ToLower();
            }
        }

        public static EffectType ParseEffect(String text) {
            switch (text) {
                case "none": return EffectType.None;
                case "shadow": return EffectType.Shadow;
                case "stroke": return EffectType.Stroke;
                default: return EffectType.None;
            }
        }

        public static void ParseEffect(XmlNode ndoe) {

        }

        public static Reader ParseSkin(Reader r) {
            r = r.Child(S_PART).Do(n => Options.Instance.Skin.Padding = (Thickness)new ThicknessConverter().ConvertFrom(n.Attributes[S_PADDING].Value))
                .Child(S_FONT).Do(n => Options.Instance.Skin.Font = ParseFont(n)).Parent
                .Child(S_FORMAT)
                    .Read(S_LINESPACING, n => Options.Instance.Skin.LineSpacing = int.Parse(n.InnerText))
                    .Read(S_PARASPACING, n => Options.Instance.Skin.ParaSpacing = int.Parse(n.InnerText))
                .Parent
            .Parent;
            return r;
        }

        public static void ParseSkin(XmlNode node) {
            if (node == null) return;
            ParseSkin(new Reader(node).Child(S_SKIN));
        }

        public static void Load(String filename = null) {
            XmlDocument xml = new XmlDocument();
            if (filename != null) {
                xml.Load(filename);
                ParseSkin(xml);
                Options.Instance.Skin.Path = filename;
            } else {
                Options.Instance.Skin.Path = null;
                if (File.Exists(G.NAME_SKIN)) {
                    Load(G.NAME_SKIN);
                } else {
                    Uri uri = new Uri("/TXTReader;component/res/defaultskin.xml", UriKind.Relative);                    
                    StreamResourceInfo info = Application.GetResourceStream(uri);
                    xml.Load(info.Stream);
                    SetDefaultSkin();
                    ParseSkin(xml);
                }
            }
        }

        public static Writer Save(Writer w) {
            var Skin = Options.Instance.Skin;
            if (Skin == null) return w;
            w = w.Start(S_PART).Attr(S_PADDING, Skin.Padding.ToString());

          
            w = w.Start("font");
            w = w.Write("name", Skin.Font.FontFamily.ToString())
                .Write("color", Skin.Foreground)
                .Write("size", Skin.FontSize)
                .Write("stretch", ParseFontStretch(Skin.Font.Stretch))
                .Write("weight", ParseFontWeight(Skin.Font.Weight))
                .Write("style", ParseFontStyle(Skin.Font.Style));
            w = w.End;

            w = w.Start("format");
            w = w.Write("linespacing", Skin.LineSpacing)
                .Write("paraspacing", Skin.ParaSpacing);
            w = w.End;

            return w.End;
        }

        public static void Save(String filename = null) {
            if (filename == null) filename = G.NAME_SKIN;
            var w = new Writer(S_SKIN);
            Save(w).WriteTo(filename);
        }*/

        public static void Save(String filename) {
            new Writer().Do(new SkinParser()).WriteTo(filename);
        }

        public static void Load(String filename) {
            new Reader(filename, false).Do(new SkinParser());
        }

        public new XmlParserReaderCallback Read {
            get {
                return r => r.Child(S_SKIN, new Version("1.0.0.0"))
                        .Do(G.FontParser).Do(n => Options.Instance.Skin.Font = G.FontParser.Font)
                        .Read(S_FONTSIZE, n => Options.Instance.Skin.FontSize = double.Parse(n.InnerText))
                        .Read(S_FONTCOLOR, n => Options.Instance.Skin.Foreground = n.InnerText!=null?(Brush)new BrushConverter().ConvertFrom(n.InnerText):null)
                        .Do(G.EffectParser).Do(n => Options.Instance.Skin.Effect = G.EffectParser.Effect)
                        .Do(G.Background)
                        .Read(S_PADDING, n => Options.Instance.Skin.Padding = (Thickness)new ThicknessConverter().ConvertFrom(n.InnerText))
                        .Read(S_LINESPACING, n => Options.Instance.Skin.LineSpacing = int.Parse(n.InnerText))
                        .Read(S_PARASPACING, n => Options.Instance.Skin.ParaSpacing = int.Parse(n.InnerText))
                    .Parent;
            }
        }

        public new XmlParserWriterCallback Write {
            get {
                return w => w.Start(S_SKIN).Ver(new Version("1.0.0.0"))
                    .Do(n => G.FontParser.Font = Options.Instance.Skin.Font).Do(G.FontParser)
                    .Write(S_FONTSIZE, Options.Instance.Skin.FontSize)
                    .Write(S_FONTCOLOR, Options.Instance.Skin.Foreground)
                    .Do(n => G.EffectParser.Effect = Options.Instance.Skin.Effect).Do(G.EffectParser)
                    .Do(G.Background)
                    .Write(S_PADDING, Options.Instance.Skin.Padding)
                    .Write(S_LINESPACING, Options.Instance.Skin.LineSpacing)
                    .Write(S_PARASPACING, Options.Instance.Skin.ParaSpacing)
                .End;
            }
        }
    }
}
