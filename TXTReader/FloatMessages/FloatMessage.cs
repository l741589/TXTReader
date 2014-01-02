using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.FloatMessages {
    public class FloatMessageOption {
        public bool Time { get; set; }
        public bool Fps { get; set; }
        public bool Speed { get; set; }
        public bool ChapterTitle { get; set; }
        public bool Progress { get; set; }
        public bool Log { get; set; }
    }
}
