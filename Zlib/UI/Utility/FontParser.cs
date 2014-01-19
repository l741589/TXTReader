using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Zlib.Text;

namespace Zlib.UI.Utility {
    public class FontParser : IXmlParsable{

        public const String S_FONT = "font";
        public const String S_NAME = "name";
        public const String S_BOLD = "bold";
        public const String S_ITALIC = "italic";

        public Typeface Font = new Typeface("宋体");

        public XmlParserReaderCallback Read {
            get {
                FontFamily ff = new FontFamily("宋体");
                FontWeight fw = FontWeights.Normal;
                FontStyle fs = FontStyles.Normal;
                return r => r.Child(S_FONT,new Version("1.0.0.0"))
                        .Read(S_NAME, n => ff = new FontFamily(n.InnerText))
                        .Read(S_BOLD, n => fw = bool.Parse(n.InnerText) ? FontWeights.Bold : FontWeights.Normal)
                        .Read(S_ITALIC, n => fs = bool.Parse(n.InnerText) ? FontStyles.Italic : FontStyles.Normal)
                        .Do(n => { Font = new Typeface(ff, fs, fw, FontStretches.Normal); })
                    .Parent;
            }
        }

        public XmlParserWriterCallback Write {
            get {
                return w => w.Start(S_FONT).Ver(new Version("1.0.0.0"))
                    .Write(S_NAME, Font.FontFamily.Source)
                    .Write(S_BOLD, Font.Weight == FontWeights.Bold)
                    .Write(S_ITALIC, Font.Style == FontStyles.Italic)
                .End;
            }
        }
    }
}
