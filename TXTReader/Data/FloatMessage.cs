using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Data {
    public class FloatMessage {
        public bool Time { get; set; }
        public bool Fps { get; set; }
        public bool Speed { get; set; }
        public bool ChapterTitle { get; set; }
        public bool Progress { get; set; }
    }
}
