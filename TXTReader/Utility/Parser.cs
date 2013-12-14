using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.IO;

namespace TXTReader.Utility {
    delegate void ParserNodeCallback(XmlNode n);
    delegate Parser.Writer ParserWriterCallback(Parser.Writer w);
    delegate Parser.Reader ParserReaderCallback(Parser.Reader r);
    class Parser {

        public static XmlNode AppendEmpty(XmlNode parent, String name) {
            var node = parent.OwnerDocument.CreateElement(name);
            parent.AppendChild(node);
            return node;
        }
        public static XmlElement Append<T>(XmlNode parent, String name, T value) 
        { return Append(parent, name, value,default(T)); }
        public static XmlElement Append<T>(XmlNode parent, String name, T value, params T[] ignoreWhenEqual) {
            if (ignoreWhenEqual != null && ignoreWhenEqual.Contains(value)) return null;
            var node = parent.OwnerDocument.CreateElement(name);
            
            node.InnerText = value.ToString();
            parent.AppendChild(node);
            return node;
        }


        public static XmlNode AppendAttribute<T>(XmlNode parent, String name, T value) 
        { return AddAttribute(parent, name, value, default(T)); }
        public static XmlNode AddAttribute<T>(XmlNode elem, String name, T value, params T[] ignoreWhenEqual) {
            if (ignoreWhenEqual != null && ignoreWhenEqual.Contains(value)) return null;
            XmlAttribute attr=elem.OwnerDocument.CreateAttribute(name);
            attr.Value=value.ToString();
            elem.Attributes.Append(attr);
            return elem;
        }

        public static void WriteHead(XmlDocument xml) {
            XmlDeclaration d = xml.CreateXmlDeclaration("1.0","utf-8",null);            
            xml.AppendChild(d);
        }

        public static void WriteTo(XmlDocument xml, String FileName) {
            XmlWriterSettings xws=new XmlWriterSettings();
            xws.Indent=true;
            xws.ConformanceLevel = ConformanceLevel.Auto;
            xws.IndentChars = "\t";
            xws.OmitXmlDeclaration = false;
            XmlWriter wrt = XmlWriter.Create(FileName, xws);
            xml.WriteTo(wrt);
            wrt.Close();
        }

        public class Writer {
            private XmlDocument xml;
            private XmlNode bak;
            private XmlNode node;
            private uint nulldepth = 0;
            private XmlNamespaceManager nsm;
            public Writer(String rootname) {
                xml = new XmlDocument();
                nsm = new XmlNamespaceManager(xml.NameTable);                
                WriteHead(xml);
                bak = node = xml.CreateElement(rootname);
                xml.AppendChild(node);
                nulldepth = 0;
            }

            public Writer AddNamespace(String prefix,String uri) {
                nsm.AddNamespace(prefix, uri);
                return this;
            }

            public Writer Write<T>(String name, T value, params T[] ignoreWhenEqual) {
                return Start(name, value, ignoreWhenEqual).End;
            }

            public Writer Write<T>(String name, T value) {
                return Start(name, value).End;
            }

            public Writer Write<T>(String name) {
                return Start(name).End;
            }

            public Writer Start<T>(String name, T value, params T[] ignoreWhenEqual) {
                node = Append<T>(node, name, value, ignoreWhenEqual);
                if (node != null) bak = node;
                else ++nulldepth;
                return this;
            }

            public Writer Start<T>(String name, T value) {
                node = Append<T>(node, name, value);
                if (node != null) bak = node;
                else ++nulldepth;
                return this;
            }

            public Writer Start(String name) {
                node = AppendEmpty(node, name);
                if (node != null) bak = node;
                else ++nulldepth;
                return this;
            }

            public Writer End {
                get {
                    if (nulldepth == 0) node = node.ParentNode;
                    else if ((--nulldepth) == 0) node = bak;
                    if (node != null) bak = node;
                    return this;
                }
            }

            public Writer Attr<T>(String name, T value, T ignoreWhenEqual) {
                AddAttribute(node, name, value, ignoreWhenEqual);
                return this;
            }

            public Writer Attr<T>(String name,T value) {
                AppendAttribute(node, name, value);
                return this;
            }
           

            public void WriteTo(String FileName) {
                Parser.WriteTo(xml, FileName);
            }

            public Writer Do(ParserWriterCallback callback) {
                return callback(this);
            }
        }

        public class Reader {
            private XmlDocument xml;
            private XmlNode node;
            private XmlNode bak;
            private uint nulldepth;
            private String keyword;

            public Reader(String FileName) {
                try {
                    xml = new XmlDocument();
                    xml.LoadXml(File.ReadAllText(FileName));
                    for (node = xml.FirstChild; node != null && node.NodeType != XmlNodeType.Element; node = node.NextSibling) ;
                    nulldepth = 0;
                } catch (Exception e) {
                    Debug.WriteLine(e.StackTrace);
                    xml = null;
                }
            }

            public Reader(XmlNode node) {
                if (node != null) {
                    xml = node.OwnerDocument;
                    bak = this.node = node;
                    nulldepth = 0;
                    keyword = null;
                }
            }

            public Reader Do(ParserNodeCallback callback) {
                if (xml == null) return this;
                if (node != null) callback.Invoke(node);
                return this;
            }

            public Reader Do(ParserReaderCallback callback) {
                if (xml == null) return this;
                return callback(this);
            }

            public Reader Child(String name = null) {
                if (xml == null) return this;
                if (node == null) {
                    ++nulldepth;
                    return this;
                }
                keyword = name;
                bool found = false;
                if (node.ChildNodes != null) {
                    foreach (XmlNode n in node.ChildNodes) {
                        if (n.Name == name || keyword == null) {
                            node = n;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found) {
                    node = null;
                    ++nulldepth;
                }
                if (node != null) bak = node;
                return this;
            }

            public Reader Next {
                get {
                    if (xml == null) return this;
                    if (node == null) return this;
                    for (XmlNode n = node.NextSibling; n != null; n = n.NextSibling)
                        if (n.Name == keyword || keyword == null) {
                            node = n;
                            return this;
                        }
                    return this;
                }               
            }

            public Reader ForChildren(String name, ParserNodeCallback callback) {
                if (xml == null) return this;
                for (XmlNode n = Child(name).node; n != null;n=n.NextSibling) {
                    callback.Invoke(n);
                }
                return Parent;
            }



            public Reader Parent {
                get {
                    if (xml == null) return this;
                    if (nulldepth > 0) {
                        --nulldepth;
                        if (nulldepth == 0) node = bak;
                    } else {
                        node = node.ParentNode;
                    }
                    if (node != null) bak = node;
                    return this;
                }
            }

            public Reader Read(String name, ParserNodeCallback callback) {
                if (xml == null) return this;
                return Child(name).Do(callback).Parent;
            }
        }
    }
}
