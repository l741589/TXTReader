using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.IO;
using Zlib.Utility;
using System.Reflection;

namespace Zlib.Text.Xml {
    public delegate void XmlParserNodeCallback(XmlNode n);
    public delegate XmlParser.Writer XmlParserWriterCallback(XmlParser.Writer w);
    public delegate XmlParser.Reader XmlParserReaderCallback(XmlParser.Reader r);
    public partial class XmlParser {

        public static XmlNode AppendEmpty(XmlNode parent, String name) {
            XmlNode node =null;
            if (parent.OwnerDocument!=null)
                node= parent.OwnerDocument.CreateElement(name);
            else if (parent is XmlDocument) node = (parent as XmlDocument).CreateElement(name);
            parent.AppendChild(node);
            return node;
        }

        public static void Read(String path, IXmlParsable obj, bool initToRoot = true) {
            new Reader(path, initToRoot).Do(obj.Read);
        }

        public static void Read(XmlDocument doc, IXmlParsable obj) {
            new Reader(doc).Do(obj.Read);
        }

        public static void Write(String path, IXmlParsable obj) {
            new Writer().Do(obj.Write).WriteTo(path);
        }

        public static XmlElement Append<T>(XmlNode parent, String name, T value) 
        { return Append(parent, name, value, null); }
        public static XmlElement Append<T>(XmlNode parent, String name, T value, params T[] ignoreWhenEqual) {
            if (ignoreWhenEqual != null && ignoreWhenEqual.Contains(value)) return null;
            if (value == null) return null;
            var node = parent.OwnerDocument.CreateElement(name);            
            node.InnerText = value.ToString();
            parent.AppendChild(node);
            return node;
        }


        public static XmlNode AppendAttribute<T>(XmlNode parent, String name, T value) 
        { return AddAttribute(parent, name, value, null); }
        public static XmlNode AddAttribute<T>(XmlNode elem, String name, T value, params T[] ignoreWhenEqual) {
            if (ignoreWhenEqual != null && ignoreWhenEqual.Contains(value)) return null;
            if (value == null) return null;
            XmlAttribute attr=elem.OwnerDocument.CreateAttribute(name);
            attr.Value=value.ToString();
            elem.Attributes.Append(attr);
            return elem;
        }

        public static void WriteHead(XmlDocument xml) {
            XmlDeclaration d = xml.CreateXmlDeclaration("1.0","utf-8",null);            
            xml.AppendChild(d);
        }

        public static void WriteTo(XmlDocument xml, StringBuilder sb) {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.ConformanceLevel = ConformanceLevel.Auto;
            xws.IndentChars = "\t";
            xws.OmitXmlDeclaration = false;
            XmlWriter wrt = XmlWriter.Create(sb, Settings);
            xml.WriteTo(wrt);
            wrt.Close();
        }

        public static void WriteTo(XmlDocument xml, String FileName) {           
            XmlWriter wrt = XmlWriter.Create(FileName, Settings);
            xml.WriteTo(wrt);
            wrt.Close();
        }

        public static XmlWriterSettings Settings { get; private set; }
        public static XmlWriterSettings FragmentSettings { get; private set; }
        static XmlParser() {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.ConformanceLevel = ConformanceLevel.Auto;
            xws.IndentChars = "\t";
            xws.OmitXmlDeclaration = false;
            Settings = xws;

            xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.ConformanceLevel = ConformanceLevel.Auto;
            xws.IndentChars = "\t";
            xws.OmitXmlDeclaration = true;
            xws.NamespaceHandling = NamespaceHandling.OmitDuplicates;
            FragmentSettings = xws;
        }        
    }
}
