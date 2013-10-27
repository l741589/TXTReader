using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TXTReader.Data;
using System.Xml;
using System.Windows.Media;

namespace TXTReader.Utility {
    class BookcaseParser {

        public const String S_SOURCE = "source";
        public const String S_COVER = "cover";
        public const String S_AUTHOR = "author";
        public const String S_BOOK = "book";

        public static String GetBookPath(Book book) {
            return G.PATH_BOOK + book.Title + book.Source.GetHashCode() + G.EXT_BOOK;
        }

        public static void Load(){
            String[] files = Directory.GetFiles(G.PATH_BOOK);
            G.Books.Clear();
            foreach (var f in files) G.Books.Add(Load(f));
        }

        public static Book Load(String filename) {
            Book b = new Book();
            XmlReader xr = XmlReader.Create(filename);
            while (xr.Read()) {
                if (xr.NodeType == XmlNodeType.Element) {
                    switch (xr.Name) {
                        case S_SOURCE:b.Init(xr.ReadElementContentAsString()); break;
                        case S_AUTHOR:b.Author = xr.ReadElementContentAsString(); break;
                        case S_COVER: b.Cover = (ImageSource)new ImageSourceConverter().ConvertFrom(xr.ReadElementContentAsString()); break;
                    }
                }
            }
            xr.Close();
            return b;
        }

        public static void Save() {
            foreach (var b in G.Books) Save(b);
        }

        public static void Save(Book book) {
            if (book.Source == null) return;
            XmlDocument xml = new XmlDocument();
            var root=xml.CreateElement(S_BOOK);

            var src = xml.CreateElement(S_SOURCE);
            src.InnerText = book.Source;
            root.AppendChild(src);

            if (book.Author != null) {
                var author = xml.CreateElement(S_AUTHOR);
                author.InnerText = book.Author;
                root.AppendChild(author);
            }

            if (book.Cover != null && book.Cover != G.NoCover) {
                var cover = xml.CreateElement(S_COVER);
                cover.InnerText = book.Cover.ToString();
                root.AppendChild(cover);
            }

            XmlWriter wr = XmlWriter.Create(GetBookPath(book));
            root.WriteTo(wr);
            wr.Close();            
        }
    }
}
