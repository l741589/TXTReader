using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Diagnostics;
using TXTReader.Widget;
using System.Windows;
using TXTReader.Data;

namespace TXTReader.Display {
    public class TextDisplayCacheElem {
        public int Index { get; set; }
        public FormattedText FormattedText { get; set; }
        public Brush Brush { get; set; }
        public Pen Pen { get; set; }
        public Geometry Geometry { get; set; }

        public Brush SecondaryBrush { get; set; }
        public Pen SecondaryPen { get; set; }
        public Geometry SecondaryGeometry { get; set; }

        public TextDisplayCacheElem(int index, FormattedText fomattedText) { Index = index; FormattedText = fomattedText; }
    }
    public class TextDisplayCache : LinkedList<TextDisplayCacheElem> {

        private Displayer owner;
        public TextDisplayCache(Displayer owner) { this.owner = owner; }

        public TextDisplayCacheElem Append(TextDisplayCacheElem e) {
            Debug.Assert(Count == 0 || Last.Value.Index + 1 == e.Index);
            AddLast(e);
            return e;
        }

        public TextDisplayCacheElem Prepend(TextDisplayCacheElem e) {
            Debug.Assert(Count == 0 || First.Value.Index - 1 == e.Index);
            AddFirst(e);
            return e;
        }

        public TextDisplayCacheElem Add(TextDisplayCacheElem e) {
            if (Count==0) return Append(e);
            if (e.Index < First.Value.Index) return Prepend(e);
            if (e.Index > Last.Value.Index) return Append(e);
            return e;
        }

        public void Limit(int first, int last) {
            if (first != -1) while (Count > 0 && First.Value.Index < first) RemoveFirst();
            if (last != -1) while (Count > 0 && Last.Value.Index > last) RemoveLast();
        }

        public TextDisplayCacheElem this[int index] {
            get {
                if (Count>0&& index >= First.Value.Index && index <= Last.Value.Index) {
                    foreach (var e in this) if (e.Index == index) return e;
                    return null;
                } else {
                    var ft = owner.createFormattedText(owner.Text[index]);
                    var geo = ft.BuildGeometry(new Point(0, 0));
                    switch (Options.Instance.Skin.EffectType) {
                        case EffectType.None:
                            return Add(new TextDisplayCacheElem(index, ft) {
                                Brush = Options.Instance.Skin.Foreground,
                                Geometry = geo
                            });
                        case EffectType.Stroke:
                            return Add(new TextDisplayCacheElem(index, ft) {
                                Brush = Options.Instance.Skin.Foreground,
                                Geometry = geo,
                                Pen = new Pen(new SolidColorBrush(Options.Instance.Skin.Effect), Options.Instance.Skin.EffectSize)
                            });
                        case EffectType.Shadow:
                            var geotemp = geo.Clone().Clone();
                            int l = (int)Options.Instance.Skin.EffectSize;
                            var geo2 = new PathGeometry();
                            for (int i = 0; i <= l; ++i) {
                                geotemp.Transform = new TranslateTransform(i, i);
                                geo2 = Geometry.Combine(geo2, geotemp, GeometryCombineMode.Union, null);
                            }
                            geo2 = Geometry.Combine(geo2, geo, GeometryCombineMode.Exclude, null);
                            return Add(new TextDisplayCacheElem(index, ft) {
                                Brush = Options.Instance.Skin.Foreground,
                                Geometry = geo,
                                SecondaryBrush = new SolidColorBrush(Options.Instance.Skin.Effect),
                                SecondaryGeometry = geo2
                            });
                        default: return null;
                    }
                }
            }
        }
    }
}
