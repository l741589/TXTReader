using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Books
{
    public class ChapterDesc
    {
        public ChapterDesc() { SubTitle = new List<string>(); Numbers = new List<int?>(); }
        public String Title { get; set; }
        public List<String> SubTitle { get; set; }
        public String this[int index] { get { return SubTitle[index]; } set { SubTitle[index] = value; } }
        public int Level { get; set; }
        public override string ToString() { return Title; }
        public List<int?> Numbers { get; set; }
    }
}
