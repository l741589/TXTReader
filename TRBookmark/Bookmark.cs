using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TRContent;
using Zlib.Text.Xml;
using Zlib.Utility;

namespace TRBookmark {
    [XmlEntity]
    public class Bookmark : IPositionable {
        [XmlIgnore]
        public bool IsAuto { get; set; }
        public int Position { get; set; }
        public double Offset { get; set; }
        public DateTime Time { get; set; }
        [XmlIgnore]
        public IContentItemAdapter Chapter { get; set; }
        public String Preview { get; set; }
        public int Total { get; set; }
        public int ChapterSid { get; set; }


        public Bookmark() { IsAuto = false; Time = DateTime.Now; ChapterSid = 0; }
        public Bookmark(IPositionable src)
            : this() {
            this.Time = DateTime.Now;
            this.Position = src.Position;
            this.Offset = src.Offset;
            this.Chapter = src.Chapter;
            if (src.Chapter != null) ChapterSid = src.Chapter.SerializeId;
            else ChapterSid = 0;
        }

        public static Dictionary<int, IContentItemAdapter> ContentHolder;

        private void GenerateContent(IContentItemAdapter node) {
            if (node.SerializeId != 0) ContentHolder[node.SerializeId] = node;
            if (node.Children != null) {
                foreach (var e in node.Children)
                {
                    GenerateContent(e);
                }
            }
        }

        public void AssignTo(IPositionable p) {
            if (p == null) return;
            if (p is IContentItemAdapter) {
                var q = p as IContentItemAdapter;
                if (Chapter == null && ChapterSid != 0) {
                    if (ContentHolder == null) {
                        ContentHolder = new Dictionary<int, IContentItemAdapter>();
                        GenerateContent(q);
                        ContentHolder[0] = q;
                    }
                    if (ContentHolder.ContainsKey(ChapterSid)) {
                        Chapter = ContentHolder[ChapterSid];
                    } else {
                        Chapter = q;
                    }
                }
                PositionableExtension.AssignTo(this, p);
            } else {
                PositionableExtension.AssignTo(this, p);
            }
        }
    }
}
