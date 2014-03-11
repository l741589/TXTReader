using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Text;
using Zlib.Text.Xml;

namespace TRSpider {

    [XmlEntity]
    public class BookDesc : ICloneable{
        private static int serializeIdGenerator;
        public int SerializeId { get; set; }

        public String Title { get; set; }
        public String Id{ get; set; }
        public String EntryUrl { get; set; }
        public String CoverUrl { get; set; }
        public String Author { get; set; }
        public String ContentUrl { get; set; }
        public String Description { get; set; }

        public object Clone() { return MemberwiseClone(); }

        public BookDesc() { SerializeId = ++serializeIdGenerator; }
    }

    [XmlEntity]
    public class ChapterDesc : ICloneable {
        private static int serializeIdGenerator;
        public int SerializeId { get; set; }

        public String Title { get; set; }
        public String TextUrl { get; set; }
        public String Id { get; set; }
        public String Preview { get; set; }

        public virtual object Clone() { return MemberwiseClone(); }

        public ChapterDesc() { SerializeId = ++serializeIdGenerator; }
    }
    
    public interface ISpider {
        String Site { get; }
        String Name { get; }
        int StandardLevel { get; }

        BookDesc Search(String keyWord);
        ObservableCollection<ChapterDesc> GetContent(String contentUrl);        
        String GetText(String textUrl);

        //ChapterDesc GetChapter(String contentUrl, String keyWord);
        ChapterDesc GetLatestChapter(BookDesc entry);
    }
}
