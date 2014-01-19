using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Zlib.Text;

namespace Zlib.UI.Utility {
    public class EffectParser : IXmlParsable {

        public Effect Effect {get;set;}
        public const String S_NAME = "effect";
        public const String S_NONE = "none";
        private DropShadowEffectParser dropShadowEffectParser;

        public EffectParser() {
            dropShadowEffectParser = new DropShadowEffectParser(this);
        }

        public XmlParserReaderCallback Read {
            get {
                return r => r.Child(S_NAME)
                        .Read(S_NONE, n => Effect = null)
                        .Do(dropShadowEffectParser)
                    .Parent;
            }
        }

        public XmlParserWriterCallback Write {
            get {
                return w => {
                    w = w.Start(S_NAME);
                    if (Effect == null) w = w.Start(S_NONE).End;
                    if (Effect is DropShadowEffect) w = w.Do(dropShadowEffectParser);
                    return w.End;
                };
            }
        }

        public class DropShadowEffectParser : IXmlParsable {

            private EffectParser Parent;

            public DropShadowEffect Effect { get { return Parent.Effect as DropShadowEffect; } set { Parent.Effect = value; } }

            public const String S_NAME = "shadow";
            public const String S_BLURRADIUS = "radius";
            public const String S_COLOR = "color";
            public const String S_SHADOWDEPTH = "depth";
            public const String S_DIRECTION = "direction";
            public const String S_OPACITY = "opacity";

            public DropShadowEffectParser(EffectParser parent){
                Parent = parent;
            }

            public XmlParserReaderCallback Read {
                get {

                    return r => r.Child(S_NAME, new Version("1.0.0.0"))
                            .Do(n => Effect = new DropShadowEffect())
                            .Read(S_BLURRADIUS, n => Effect.BlurRadius = double.Parse(n.InnerText))
                            .Read(S_DIRECTION, n => Effect.Direction = double.Parse(n.InnerText))
                            .Read(S_OPACITY, n => Effect.Opacity = double.Parse(n.InnerText))
                            .Read(S_SHADOWDEPTH, n => Effect.ShadowDepth = double.Parse(n.InnerText))
                            .Read(S_COLOR, n => Effect.Color = (Color)ColorConverter.ConvertFromString(n.InnerText))
                        .Parent;
                }
            }

            public XmlParserWriterCallback Write {
                get {
                    return w => w.Start(S_NAME).Ver(new Version("1.0.0.0"))
                        .Write(S_BLURRADIUS, Effect.BlurRadius)
                        .Write(S_COLOR, Effect.Color)
                        .Write(S_SHADOWDEPTH, Effect.ShadowDepth)
                        .Write(S_DIRECTION, Effect.Direction)
                        .Write(S_OPACITY, Effect.Opacity)
                        .End;
                }
            }
        }
    }
}
