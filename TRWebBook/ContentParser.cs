using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRContent;
using TRSpider;
using Zlib.Text.Xml;

namespace TRWebBook {
    internal partial class Book {
        class ContentParser : IXmlParsable {
            private Dictionary<int, ChapterDescEx> dic = null;
            private Book root;
            public ContentParser(Book root) {
                this.root = root;
            }


            public XmlParserReaderCallback Read {
                get {
                    return r => {
                        if (root == null) return r;
                        if (root.Downloader == null) return r;
                        dic = root.Downloader.GenerateChapterHolder();
                        r = r.Child("content");
                        if (root.Children != null) root.Children.Clear();
                        r = r.Child("item");
                        R(r, root);
                        dic = null;
                        root.hasContent = root.Children!=null&&root.Children.Count()>0;
                        r = r.Parent;
                        return r.Parent;
                    };                    
                }
            }

            private XmlParser.Reader R(XmlParser.Reader r, Chapter target) {
                r = r.Read("title", n => target.Title = n.InnerText);
                r = r.Read("number", n => target.Number = int.Parse(n.InnerText));
                r=r.Read("text",n => {
                    var a = n.Attributes["ref"];
                    if (a == null&&dic!=null) {
                        target.Text=(n.InnerText.Split(new String[] { "\r\n" }, StringSplitOptions.None)).ToList();
                    } else {
                        ChapterDescEx cd;
                        dic.TryGetValue(int.Parse(a.Value), out cd);
                        target.ChapterDesc = cd;
                    }
                });
                if (target.Children != null) target.Children.Clear();
                r.ForChildren("item", n => {
                    Chapter c=new Chapter();
                    R(r, c);
                    target.AppendSub(c);
                });
                return r;
            }

            public XmlParserWriterCallback Write {
                get {
                    return w => {
                        w = w.Start("content");
                        w = W(w, root);
                        return w.End;
                    };
                }
            }

            private XmlParser.Writer W(XmlParser.Writer w, IContentItemAdapter a) {
                w = w.Start("item");
                w = w.Write("title", a.Title);
                if (a.Number != null) w = w.Write("number", a.Number);
                if (a.SerializeId == 0) {
                    if (a.Text != null) {
                        var text = String.Join("\r\n", a.Text);
                        w = w.Write("text", text);
                    }
                } else {
                    w = w.Start("text").Attr("ref", a.SerializeId).End;
                }
                if (a.Children != null) {
                    foreach (var e in a.Children) W(w, e);
                }
                return w.End;
            }
        }
    }
}