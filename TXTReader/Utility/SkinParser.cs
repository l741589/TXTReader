using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TXTReader.Data;

namespace TXTReader.Utility {
    public class SkinParser {

        public static void SetDefaultSkin() {
            Options.Instance.Skin.LineSpacing = 4;
            Options.Instance.Skin.ParaSpacing = 8;
            Options.Instance.Skin.Font = new Typeface("宋体");
            Options.Instance.Skin.FontSize = 12;
            Options.Instance.Skin.Foreground = Brushes.Yellow;
            Options.Instance.Skin.BackColor = Colors.DarkBlue;
            Options.Instance.Skin.BackGroundType = BackGroundType.SolidColor;
            Options.Instance.Skin.Padding = new Thickness(16);
            Options.Instance.Skin.EffetSize = 1;
            Options.Instance.Skin.Effect = Brushes.Black;
            Options.Instance.Skin.EffectType = EffectType.Shadow;
        }

        public static Typeface ParseFont(XmlNode node) {
            if (!node.HasChildNodes) return Options.Instance.Skin.Font;
            FontStretch stretch = Options.Instance.Skin.Font.Stretch;
            FontStyle style = Options.Instance.Skin.Font.Style;
            FontWeight weight = Options.Instance.Skin.Font.Weight;
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

        public static FontStyle ParseFontStyle(String text) {
            switch (text) {
                case "normal": return FontStyles.Normal;
                case "oblique": return FontStyles.Oblique;
                case "italic": return FontStyles.Italic;
                default: return default(FontStyle);
            }
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
                case "font": Options.Instance.Skin.Font = ParseFont(node); break;
                default: if (node.HasChildNodes) ParseSkin(node.FirstChild); break;
            }
            if (node.NextSibling != null) ParseSkin(node.NextSibling);
        }
    }
}
