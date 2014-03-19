using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TRBookcase;
using TXTReader.Interfaces;
using Zlib.Text.Xml;
using Zlib.Utility;

namespace TRBookmark {
    public class BookmarkParser : IBookParserExtender{

        public XmlParser.Reader Read(XmlParser.Reader r, Dictionary<string, object> target) {
            ObservableCollection<Bookmark> bmks = new ObservableCollection<Bookmark>();
            r = r.Child("TRBookmark");
            r.ForChildren("bookmark", n => {
                r = r.ReadEntity<Bookmark>(null, bmk => bmks.Add(bmk));
            });
            target.Add("bookmark", bmks);
            return r.Parent;
        }

        public XmlParser.Writer Write(XmlParser.Writer w, Dictionary<string, object> source) {
            ObservableCollection<Bookmark> bmks = null;
            if (!source.ContainsKey("bookmark")) return w;
            else bmks = source["bookmark"] as ObservableCollection<Bookmark>;
            w = w.Start("TRBookmark");
            foreach (var e in bmks) w = w.WriteEntity("bookmark", e);
            return w.End;
        }
    }
}
