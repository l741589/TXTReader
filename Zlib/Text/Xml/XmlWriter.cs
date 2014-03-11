using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Zlib.Text.Xml {
    public partial class XmlParser {

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

            public Writer() {
                xml = new XmlDocument();
                nsm = new XmlNamespaceManager(xml.NameTable);
                WriteHead(xml);
                bak = node = xml;
                nulldepth = 0;
            }

            public Writer Write<T>(String name, T value, params T[] ignoreWhenEqual) {
                return Start(name, value, ignoreWhenEqual).End;
            }

            public Writer Write<T>(String name, T value) {
                return Start(name, value).End;
            }

            public Writer Write(String name) {
                return Start(name).End;
            }

            private bool IsXmlEntity(MemberInfo mi, bool def = false) {
                if (!mi.IsDefined(typeof(XmlEntity), false)) return def;
                var aa = mi.GetCustomAttributes(typeof(XmlEntity), false);
                return (aa[0] as XmlEntity).IsParsable;
            }

            public Writer WriteEntity<T>(String name, T value, XmlParserWriterCallback writerCallback = null) {
                if (value != null) {
                    var t = value.GetType();
                    if (IsXmlEntity(t)) {
                        Start(name).Attr("class", t.FullName).Do(writerCallback);
                        XmlSerializer s = new XmlSerializer(t);
                        StringBuilder sb = new StringBuilder();
                        using (var xw = XmlWriter.Create(sb, FragmentSettings)) {
                            try {
                                s.Serialize(xw, value);
                            } catch (Exception e) {
                                Debug.WriteLine(e);
                            }
                        }
                        node.InnerXml = sb.ToString();
                        return End;
                    } else {
                        return Start(name, value).Do(writerCallback).End;
                    }
                } else {
                    return Start(name).Do(writerCallback).End;
                }
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

            public Writer DoEnd() {
                if (nulldepth == 0) node = node.ParentNode;
                else if ((--nulldepth) == 0) node = bak;
                if (node != null) bak = node;
                return this;
            }

            public Writer End { get { return DoEnd(); } }

            public Writer Attr<T>(String name, T value, params T[] ignoreWhenEqual) {
                AddAttribute(node, name, value, ignoreWhenEqual);
                return this;
            }

            public Writer Attr<T>(String name, T value) {
                AppendAttribute(node, name, value);
                return this;
            }

            public Writer Ver(Version version) {
                return Attr("version", version);
            }

            public void WriteTo(String FileName) {
                XmlParser.WriteTo(xml, FileName);
            }

            public Writer Do(XmlParserNodeCallback callback) {
                if (xml == null) return this;
                if (node != null) callback.Invoke(node);
                return this;
            }

            public Writer Do(XmlParserWriterCallback callback) {
                if (callback == null) return this;
                return callback(this);
            }

            public Writer Do(IXmlParsable parsable) {
                if (parsable == null) return this;
                return Do(parsable.Write);
            }

            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                XmlParser.WriteTo(xml, sb);
                return sb.ToString();
            }
        }
    }
}
