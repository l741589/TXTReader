using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRContent;
using Zlib.Utility;

namespace TRBook {
    class Bookmark : IPositionable {
        public bool IsAuto { get; set; }
        public int Position { get; set; }
        public double Offset { get; set; }
        public DateTime Time { get; set; }
        public IContentItemAdapter Chapter { get; private set; }
        

        public Bookmark() { IsAuto = false; }
        public Bookmark(IPositionable src)
            : this() {
            this.Time = DateTime.Now;
            this.Position = src.Position;
            this.Offset = src.Offset;
            this.Chapter = src.Chapter;
        }

        
    }
}
