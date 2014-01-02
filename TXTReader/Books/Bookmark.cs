using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Books {
    public class Bookmark : Positionable {
        public bool IsAuto { get; set; }
        public int Position { get; set; }
        public double Offset { get; set; }
        public DateTime Time { get; set; }
        public void AssignTo(Positionable target) {
            if (target == null) return;
            target.Position = this.Position;
            target.Offset = this.Offset;
        }

        public Bookmark() { IsAuto = false; }
        public Bookmark(Positionable src)
            : this() {
            this.Time = DateTime.Now;
            this.Position = src.Position;
            this.Offset = src.Offset;
        }
    }
}
