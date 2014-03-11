using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.Xml {
    public interface IXmlParsable {
        XmlParserReaderCallback Read { get; }
        XmlParserWriterCallback Write { get; }
    }
}