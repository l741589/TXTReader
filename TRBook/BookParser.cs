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

namespace TRBook {
    class BookParser :XmlParser{

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

        public static String GetBookPath(Book book) {
            return G.PATH_BOOK + Path.GetFileNameWithoutExtension(book.Source) + A.MD5(Encoding.UTF8.GetBytes(book.Source)) + G.EXT_BOOK;
        }

        public static void Load(){
            String[] files = Directory.GetFiles(G.PATH_BOOK);
            Book.Books.Clear();
            foreach (var f in files)
                if (Path.GetExtension(f)==G.EXT_BOOK)
                    Book.Books.Add(new Book(f));
            Debug.WriteLine("Loaded Books");
        }

        public static Book Load(Book book) {
            return Load(GetBookPath(book), book);
        }

        public static Book Load(String filename, Book target = null) {
            if (!File.Exists(filename)) return target;
            Book b = null;
            if (target == null) b = new Book();
            else b = target;
            var r = new Reader(filename)
                .Read(S_SOURCE, (n) => { b.Init(n.InnerText); })
                .Read(S_AUTHOR, (n) => { b.Author = n.InnerText; })
                .Read(S_COVER, (n) => {
                    String uri = n.InnerText;
                    if (uri.StartsWith(A.HTTP_HEAD) || A.FileExists(uri))
                        b.Cover = (ImageSource)new ImageSourceConverter().ConvertFrom(uri);
                })
                .Read(S_TIME, (n) => { b.LastLoadTime = DateTime.Parse(n.InnerText); })
                .Read(S_LENGTH, (n) => { b.Length = int.Parse(n.InnerText); })
                .Read(S_PREVIEW, (n) => { b.Preview = n.InnerText; })
                .Read(S_TITLE, (n) => { b.Title = n.InnerText; })
                .Read(S_ID, (n) => { b.Id = n.InnerText; })
                .Child(S_BOOKMARK).Do((n) => { b.Bookmark.Clear(); }).ForChildren(S_MARK, (n) => {
                    Bookmark bmk = new Bookmark();
                    if (n.Attributes[S_AUTO] != null) bmk.IsAuto = true; else bmk.IsAuto = false;
                    new Reader(n)
                        .Read(S_OFFSET, (m) => { bmk.Offset = double.Parse(m.InnerText); })
                        .Read(S_POSITION, (m) => { bmk.Position = int.Parse(m.InnerText); })
                        .Read(S_TIME, (m) => { bmk.Time = DateTime.Parse(m.InnerText); });
                    if (bmk.IsAuto) bmk.AssignTo(b);
                    else b.Bookmark.Add(bmk);
                }).Parent;
            Debug.WriteLine("Load Book: " + b.Source + " : " + GetBookPath(b));
            return b;
        }

        public static void Save() {
            //Debug.WriteLine("Save Books");
            //foreach (var b in Book.Books) Save(b);
        }

        public static void Save(Book book) {
            if (book == Book.Empty) return;
            if (book.Source == null) return;
            Debug.WriteLine("Save Book: " + book.Source + " : " + GetBookPath(book));
            var w = new Writer(S_BOOK)
                .Write(S_SOURCE, book.Source)
                .Write(S_AUTHOR, book.Author)
                .Write(S_COVER, book.Cover, G.NO_COVER, null)
                .Write(S_LENGTH, book.Length)
                .Write(S_TIME, book.LastLoadTime)
                .Write(S_PREVIEW, book.Preview, "", Book.NO_PREVIEW, null)
                .Write(S_TITLE, book.Title)
                .Write(S_ID, book.Id);
            w = w.Start(S_BOOKMARK);
            if (Book.I != Book.Empty) {
                w = w.Start(S_MARK).Attr(S_AUTO, "true")
                    .Write(S_POSITION, Book.I.Position)
                    .Write(S_OFFSET, Book.I.Offset)
                    .Write(S_TIME, Book.I.LastLoadTime)
                .End;
            }
            foreach (var e in book.Bookmark) {
                w = w.Start(S_MARK)
                    .Write(S_POSITION, e.Position)
                    .Write(S_OFFSET, e.Offset)
                    .Write(S_TIME, DateTime.Now)
                .End;
            }
            w=w.End;
            w.WriteTo(GetBookPath(book));
        }
      
    }
}
