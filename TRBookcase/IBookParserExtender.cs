using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader.Interfaces;
using Zlib.Text.Xml;

namespace TRBookcase {
    public interface IBookParserExtender {
        XmlParser.Reader Read(XmlParser.Reader r, Dictionary<String, object> target);
        XmlParser.Writer Write(XmlParser.Writer w, Dictionary<string, object> source);
    }
}
