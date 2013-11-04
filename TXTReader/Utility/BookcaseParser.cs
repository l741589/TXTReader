using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TXTReader.Data;
using System.Xml;
using System.Windows.Media;
using System.Diagnostics;

namespace TXTReader.Utility {
    class BookcaseParser :Parser{

        public const String S_SOURCE = "source";
        public const String S_COVER = "cover";
        public const String S_AUTHOR = "author";
        public const String S_TIME = "time";
        public const String S_LENGTH = "length";
        public const String S_PREVIEW = "preview";
        public const String S_BOOK = "book";

        public static String GetBookPath(Book book) {
            return G.PATH_BOOK + book.Title + book.Source.GetHashCode() + G.EXT_BOOK;
        }

        public static void Load(){
            String[] files = Directory.GetFiles(G.PATH_BOOK);
            G.Books.Clear();
            foreach (var f in files)
                G.Books.Add(new Book(f));
            Debug.WriteLine("Load Books");
        }

        public static Book Load(Book book) {
            return Load(GetBookPath(book), book);
        }

        public static Book Load(String filename, Book target = null) {
            if (!File.Exists(filename)) return target;
            Book b = null;
            if (target == null) b = new Book();
            else b = target;
            new Reader(filename)
                .Read(S_SOURCE, (n) => { b.Init(n.InnerText); })
                .Read(S_AUTHOR, (n) => { b.Author = n.InnerText; })
                .Read(S_COVER, (n) => {
                    String uri = n.InnerText;
                    if (uri.StartsWith(G.HTTP_HEAD) || A.FileExists(uri))
                        b.Cover = (ImageSource)new ImageSourceConverter().ConvertFrom(uri);
                })
                .Read(S_TIME, (n) => { b.LastLoadTime = DateTime.Parse(n.InnerText); })
                .Read(S_LENGTH, (n) => { b.Length = int.Parse(n.InnerText); })
                .Read(S_PREVIEW, (n) => { b.Preview = n.InnerText; })
                .Do(BookmarkParser.Load);
            Debug.WriteLine("Load Book: " + b.Source + " :" + GetBookPath(b));
            //Debug.WriteLine("Load Book: " + b.Source);
            return b;
        }

        public static void Save() {
            Debug.WriteLine("Save Books");
            foreach (var b in G.Books) Save(b);
        }

        public static void Save(Book book) {            
            if (book.Source == null) return;
            Debug.WriteLine("Save Book: " + book.Source + " :" + GetBookPath(book));
            new Writer(S_BOOK)
                .Write(S_SOURCE, book.Source)
                .Write(S_AUTHOR, book.Author)
                .Write(S_COVER, book.Cover, G.NoCover, null)
                .Write(S_LENGTH, book.Length)
                .Write(S_TIME, book.LastLoadTime)
                .Write(S_PREVIEW, book.Preview, "", Book.NO_PREVIEW, null)
                .Do(BookmarkParser.Save)
            .WriteTo(GetBookPath(book));
        }
      
    }
}
