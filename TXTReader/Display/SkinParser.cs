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
using TXTReader.ToolPanel;
using Zlib.Text;

namespace TXTReader.Display {
    class SkinParser : XmlParser{

        public const String URI_SCHEME_SKIN = "http://txtreader.org/skin.xsd";

        public static void SetDefaultSkin() {
            Options.Instance.Skin.LineSpacing = 4;
            Options.Instance.Skin.ParaSpacing = 8;
            Options.Instance.Skin.Font = new Typeface("宋体");
            Options.Instance.Skin.FontSize = 12;
            Options.Instance.Skin.Foreground = Brushes.Yellow;
            Options.Instance.Skin.BackColor = Colors.DarkBlue;
            Options.Instance.Skin.BackGroundType = BackGroundType.SolidColor;
            Options.Instance.Skin.Padding = new Thickness(16);
            Options.Instance.Skin.EffectSize = 5;
            Options.Instance.Skin.Effect = Colors.Black;
            Options.Instance.Skin.EffectType = EffectType.Shadow;
        }

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

        public static void ParseSkin(XmlNode node) {
            if (node == null) return;
            switch (node.Name.ToLower()) {
                case "part": Options.Instance.Skin.Padding = (Thickness)new ThicknessConverter().ConvertFrom(node.Attributes["padding"].Value); break;
                case "color": {
                        Options.Instance.Skin.BackColor = (Color)ColorConverter.ConvertFromString(node.InnerText);
                        Options.Instance.Skin.BackGroundType = BackGroundType.SolidColor;
                    } break;
                case "img": {
                        Options.Instance.Skin.BackImage = (ImageSource)new ImageSourceConverter().ConvertFromString(node.InnerText);
                        Options.Instance.Skin.BackGroundType = BackGroundType.Image;
                    } break;
                case "linespacing": Options.Instance.Skin.LineSpacing = double.Parse(node.InnerText); break;
                case "paraspacing": Options.Instance.Skin.ParaSpacing = double.Parse(node.InnerText); break;
                
                default: break;
            }

            switch (node.Name.ToLower()) {
                case "effect": var r = new Reader(node).Child("none").Do((n) => { Options.Instance.Skin.EffectType = EffectType.None; }).Parent
                     .Child("shadow").Do((n) => { Options.Instance.Skin.EffectType = EffectType.Shadow; })
                         .Read("color", (n) => { Options.Instance.Skin.Effect = (Color)ColorConverter.ConvertFromString(n.InnerText); })
                         .Read("size", (n) => { Options.Instance.Skin.EffectSize = double.Parse(n.InnerText); })
                     .Parent
                     .Child("stroke").Do((n) => { Options.Instance.Skin.EffectType = EffectType.Stroke; }).Parent
                     .Do((n) => { });
                    break;
                case "font": Options.Instance.Skin.Font = ParseFont(node); break;
                default: if (node.HasChildNodes) ParseSkin(node.FirstChild); break;
            }
            if (node.NextSibling != null) ParseSkin(node.NextSibling);
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

        public static void Save(String filename = null) {
            if (filename == null) filename = G.NAME_SKIN;
            var Skin=G.Options.Skin;
            if (Skin==null) return;
            var w = new Writer("skin").AddNamespace(String.Empty, URI_SCHEME_SKIN);
            w=w.Start("part").Attr("padding",Skin.Padding.ToString());

            w = w.Start("background");
            switch (Skin.BackGroundType) {
                case BackGroundType.Image:
                    String uri=Skin.BackImage.ToString();
                    if (uri.StartsWith("file:///")) uri=uri.Substring("file:///".Length);
                    w=w.Write("img",uri);
                    break;
                case BackGroundType.SolidColor:
                    w = w.Write("color", Skin.BackColor.ToString());
                    break;
            }
            w = w.End;

            w = w.Start("font");
            w = w.Write("name", Skin.Font.FontFamily.ToString())
                .Write("color", Skin.Foreground)
                .Write("size", Skin.FontSize, new double[0])
                .Write("stretch", ParseFontStretch(Skin.Font.Stretch))
                .Write("weight", ParseFontWeight(Skin.Font.Weight))
                .Write("style", ParseFontStyle(Skin.Font.Style));
            w = w.End;

            w = w.Start("format");
            w = w.Write("linespacing", Skin.LineSpacing, new double[0])
                .Write("paraspacing", Skin.ParaSpacing, new double[0]);
            w = w.End;

            w = w.Start("effect").Start(ParseEffect(Skin.EffectType));
            switch (Skin.EffectType) {
                case EffectType.None: break;
                case EffectType.Shadow: w = w.Write("color", Skin.Effect.ToString()).Write("size", Skin.EffectSize); break;
                case EffectType.Stroke: break;
            }
            w = w.End.End;

            w=w.End;
            w.WriteTo(filename);
        }
    }
}
