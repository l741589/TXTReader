using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader.Data;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;

namespace TXTReader.Utility {
    class BookmarkParser : Parser{

        public const String S_BOOKMARK = "bookmark";
        public const String S_FILE = "file";
        public const String S_POSITION = "position";
        public const String S_OFFSET = "offset";
        public const String S_MARK = "mark";
        public const String S_AUTO = "auto";
        public const String S_TIME = "time";

        //public static String FileName { get { return G.PATH_BOOKMARK + G.Book.Title + G.Book.Source.GetHashCode() + G.EXT_BOOKMARK; } }

        /*private static void Save(XmlElement root,Positionable p,bool isAuto = false) {
            XmlDocument xml = root.OwnerDocument;
            var mark=xml.CreateElement(S_MARK);
            root.AppendChild(mark);

            if (isAuto) {
                var auto = xml.CreateAttribute(S_AUTO);
                mark.Attributes.Append(auto);
            }

            var pos = xml.CreateElement(S_POSITION);
            pos.InnerText = p.Position.ToString();
            mark.AppendChild(pos);

            var off = xml.CreateElement(S_OFFSET);
            off.InnerText = p.Offset.ToString();
            mark.AppendChild(off);

            var time = xml.CreateElement(S_TIME);
            if (p is Bookmark) time.InnerText = (p as Bookmark).Time.ToString();
            else time.InnerText = DateTime.Now.ToString();
            mark.AppendChild(time);
        }

        public static void Save(XmlElement parent = null) {
            if (G.Book == null || G.Book.Title == null || G.Book.Title == "") return;
            XmlDocument xml;
            XmlElement root;
            if (parent == null) {
                xml = new XmlDocument();
                root = xml.CreateElement(S_BOOKMARK);
                xml.AppendChild(root);
            } else {
                xml = parent.OwnerDocument;
                root = xml.CreateElement(S_BOOKMARK);
                parent.AppendChild(root);
            }

            if (G.Book.Source != null && G.Book.Source != "") {
                var file = xml.CreateAttribute(S_FILE);
                file.Value = G.Book.Source;
                root.Attributes.Append(file);
            }

            Save(root, G.Book, true);
            foreach (var e in G.Bookmark)
            {
                Save(root, e as Positionable);
            }
            Debug.WriteLine(G.PATH_BOOKMARK);
            XmlWriter wrt = XmlWriter.Create(FileName);
            xml.WriteTo(wrt);
            wrt.Close();
        }*/

       /* public static Bookmark LoadMark(XmlReader xr) {
            Bookmark bmk = new Bookmark();
            var s = xr.GetAttribute(S_AUTO);
            if (s != null) bmk.IsAuto = true; else bmk.IsAuto = false;
            while (xr.Read()) {
                if (xr.NodeType == XmlNodeType.EndElement && xr.Name == S_MARK) break;
                var loop = true;
                while (loop) {
                    if (xr.NodeType == XmlNodeType.Element) {
                        switch (xr.Name) {
                            case S_POSITION: bmk.Position = xr.ReadElementContentAsInt(); break;
                            case S_OFFSET: bmk.Offset = xr.ReadElementContentAsDouble(); break;
                            case S_TIME: bmk.Time = DateTime.Parse(xr.ReadElementContentAsString()); break;
                            default: loop = false; break;
                        }
                    } else loop = false;
                }
                if (xr.NodeType == XmlNodeType.EndElement && xr.Name == S_MARK) break;
            }
            return bmk;
        }

        public static void Load(XmlReader xr = null) {
            if (G.Book == null || G.Book.Title == null || G.Book.Title == "") return;
            try {
                if (xr == null) xr = XmlReader.Create(FileName);
                xr.MoveToElement();
                while (xr.Read()) {
                    if (xr.NodeType==XmlNodeType.Element&&xr.Name==S_BOOKMARK) {
                        if (xr.GetAttribute(S_FILE) != G.Book.Source) return;
                        while (xr.Read()) {
                            if (xr.NodeType == XmlNodeType.Element && xr.Name == S_MARK) {
                                var mark = LoadMark(xr);
                                if (mark.IsAuto) mark.AssignTo(G.Book);
                                else {
                                    G.Bookmark.Add(mark);
                                }
                            }
                        }
                    }                
                }
            } catch (Exception e) {
                Debug.WriteLine(e);
            }
            if (xr != null) xr.Close();
        }*/

        public static Reader Load(Reader r) {
            r.Child(S_BOOKMARK).ForChildren(S_MARK, (n) => {
                Bookmark bmk=new Bookmark();                
                if (n.Attributes[S_AUTO] != null) bmk.IsAuto = true;else bmk.IsAuto=false;
                new Reader(n)
                    .Read(S_OFFSET, (m) => { bmk.Offset = double.Parse(m.InnerText); })
                    .Read(S_POSITION, (m) => { bmk.Position = int.Parse(m.InnerText); })
                    .Read(S_TIME, (m) => { bmk.Time = DateTime.Parse(m.InnerText); });
                if (bmk.IsAuto) bmk.AssignTo(G.Book);
                else G.Bookmark.Add(bmk);
            });
            return r.Parent;
        }

        public static Writer Save(Writer w) {
            w=w.Start(S_BOOKMARK);
            w=w.Start(S_MARK).Attr(S_AUTO,"true")
                .Write(S_POSITION,G.Book.Position)
                .Write(S_OFFSET,G.Book.Offset)
                .Write(S_TIME,G.Book.LastLoadTime)
            .End;
            foreach (var e in G.Bookmark) {
                w = w.Start(S_MARK)
                    .Write(S_POSITION, e.Position)
                    .Write(S_OFFSET,e.Offset)
                    .Write(S_TIME, DateTime.Now)
                .End;
            }
            return w.End;
        }
    }
}
