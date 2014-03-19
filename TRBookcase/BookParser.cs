using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Windows.Media;
using System.Diagnostics;
using Zlib.Text;
using Zlib.Algorithm;
using Zlib.Utility;
using Zlib.Text.Xml;
using TXTReader.Interfaces;
using TRBookcase;
using Zlib.Net;
using System.Windows.Media.Imaging;

namespace TRBookcase {
    public class BookParser :XmlParser{

        public const String S_SOURCE = "source";
        public const String S_COVER = "cover";
        public const String S_AUTHOR = "author";
        public const String S_TIME = "time";
        public const String S_LENGTH = "length";
        public const String S_PREVIEW = "preview";
        public const String S_BOOK = "book";
        public const String S_BOOKMARK = "bookmark";
        public const String S_FILE = "file";
        public const String S_POSITION = "position";
        public const String S_OFFSET = "offset";
        public const String S_MARK = "mark";
        public const String S_AUTO = "auto";
        public const String S_TITLE = "title";
        public const String S_ID = "id";
        public const String S_EXTEND = "extend";

        public static List<IBookParserExtender> Extend = new List<IBookParserExtender>();

        public static String GetBookPath(IBook book) {
            if (Path.GetExtension(book.Source) == ".trb") throw new ApplicationException("invalid source type");
            return GetBookPath(book.Source);
        }

        public static String GetBookPath(String src) {
            if (Path.GetExtension(src)==".trb") return src;
            return G.PATH_BOOK + A.MD5(src) + G.EXT_BOOK;
        }

        public static void Load() {
            String[] files = Directory.GetFiles(G.PATH_BOOK);
            G.Books.Clear();
            foreach (var f in files)
                if (Path.GetExtension(f) == G.EXT_BOOK) {
                    var b = new BookCaseItem();
                    Load(f, b);
                    G.Books.Add(b);
                }
            Debug.WriteLine("Loaded Books");
        }

        public static IBook Load(BookCaseItem book) {
            return Load(GetBookPath(book), book);
        }

        public static IBook Load(String filename, BookCaseItem target = null) {
            if (!File.Exists(filename)) return target;
            BookCaseItem b = null;
            if (target == null) b = new BookCaseItem();
            else b = target;
            var r = new Reader(filename)
                .Read(S_SOURCE, (n) => { b.Source = n.InnerText; })
                .Read(S_AUTHOR, (n) => { b.Author = n.InnerText; })
                .Read(S_PREVIEW, (n) => { b.Preview = n.InnerText; })
                .Read(S_TITLE, (n) => { b.Title = n.InnerText; })
                .Read(S_ID, (n) => { b.Id = n.InnerText; })
                .Read(S_POSITION, n => b.Position = int.Parse(n.InnerText))
                .Read(S_OFFSET, n => b.Offset = double.Parse(n.InnerText))
                .Read(S_TIME, n => b.LastLoadTime = DateTime.Parse(n.InnerText))
                .Read(S_COVER, (n) => {
                    String uri = n.InnerText;
                    if (uri.IsNullOrWhiteSpace()) b.Cover = G.NO_COVER;
                    b.Cover = uri;
                }).Child(S_EXTEND);
            foreach (var e in Extend) r = e.Read(r, b.Data);
            r = r.Parent;
            target.IsLoaded = true;
            return b;
        }

        public static void Save() {
            //Debug.WriteLine("Save Books");
            //foreach (var b in Book.Books) Save(b);
        }

        public static void Save(IBook book) {
            if (book.IsNull()) return;
            if (book is BookCaseItem && !(book as BookCaseItem).IsLoaded) return;
            if (book.Source.IsNullOrWhiteSpace()) return;            
            var w = new Writer(S_BOOK)
                .Write(S_SOURCE, book.Source)
                .Write(S_AUTHOR, book.Author)
                .Write(S_COVER, book.Cover, G.NO_COVER, null)
                .Write(S_PREVIEW, book.Preview, "", "没有预览", null)
                .Write(S_TITLE, book.Title)
                .Write(S_ID, book.Id)
                .Write(S_POSITION, book.Position)
                .Write(S_OFFSET, book.Offset)
                .Write(S_TIME,book.LastLoadTime)
                .Start(S_EXTEND);
            foreach (var e in Extend) {
                if (book is BookCaseItem) {
                    w = e.Write(w, (book as BookCaseItem).Data);
                } else {
                    var bb = book.GetBindingTarget();
                    if (bb != null && bb.Count() > 0 && bb[0] is BookCaseItem) {
                        w = e.Write(w, (bb[0] as BookCaseItem).Data);
                    }
                }
            }
            w = w.End;
            w.WriteTo(GetBookPath(book));
        }

       
    }
}