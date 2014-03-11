using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Zlib.Text.Xml {
    public partial class XmlParser {
        public class Reader {
            private XmlDocument xml;
            private XmlNode node;
            private XmlNode bak;
            private uint nulldepth;
            private String keyword;

            public Reader(String FileName, bool initToRoot = true) {
                try {
                    xml = new XmlDocument();
                    xml.LoadXml(File.ReadAllText(FileName));
                    if (initToRoot)
                        for (node = xml.FirstChild; node != null && node.NodeType != XmlNodeType.Element; node = node.NextSibling) ;
                    else
                        node = xml;
                    nulldepth = 0;
                } catch (Exception e) {
                    Debug.WriteLine(e.StackTrace);
                    xml = null;
                }
            }

            public Reader(XmlNode node) {
                if (node != null) {
                    if (node is XmlDocument) xml = node as XmlDocument;
                    else xml = node.OwnerDocument;
                    bak = this.node = node;
                    nulldepth = 0;
                    keyword = null;
                }
            }

            public Reader Do(XmlParserNodeCallback callback) {
                if (xml == null) return this;
                if (node != null) callback.Invoke(node);
                return this;
            }

            public Reader Do(XmlParserReaderCallback callback) {
                if (xml == null) return this;
                if (callback == null) return this;
                return callback(this);
            }

            public Reader Do(IXmlParsable parsable) {
                if (xml == null) return this;
                if (parsable == null) return this;
                return Do(parsable.Read);
            }

            public Reader Child(String name, Version version) {
                return Child(name, new Dictionary<String, object> { { "version", version } });
            }

            public Reader Child(String name = null, Dictionary<String, object> attrs = null) {
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
                            if (attrs == null) {
                                node = n;
                                found = true;
                            } else {
                                found = true;
                                foreach (var attr in attrs) {
                                    if (n.Attributes[attr.Key] == null) continue;
                                    if (n.Attributes[attr.Key].Value != attr.Value.ToString()) {
                                        found = false;
                                        break;
                                    }
                                }
                            }
                            if (found) {
                                node = n;
                                break;
                            }
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

            public Reader Next() {
                if (xml == null) return this;
                if (node == null) return this;
                for (XmlNode n = node.NextSibling; n != null; n = n.NextSibling)
                    if (n.Name == keyword || keyword == null) {
                        node = n;
                        return this;
                    }
                return this;
            }

            public Reader ForChildren(XmlParserNodeCallback callback) {
                if (callback == null) return this;
                if (xml == null) return this;
                for (XmlNode n = Child().node; n != null; n = n.NextSibling) {
                    callback.Invoke(node = n);
                }
                return Parent;
            }

            public Reader ForChildren(String name, XmlParserNodeCallback callback) {
                if (callback == null) return this;
                if (xml == null) return this;
                for (XmlNode n = Child(name).node; n != null; n = n.NextSibling) {
                    if (n.Name == name || name == null) 
                        callback.Invoke(node = n);
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

            public Reader Read(String name, XmlParserNodeCallback callback) {
                if (xml == null) return this;
                return Child(name).Do(callback).Parent;
            }

            public Reader ReadEntity<T>(String name, Action<T> callback) {
                if (callback == null) return this;
                if (name!=null) Child(name);
                var t = typeof(T);
                Do(new XmlParserNodeCallback(n => {
                    if (t == typeof(object) && (n.Attributes["class"] != null)) {
                        t = Type.GetType(n.Attributes["class"].Value);
                    }
                }));
                try {
                    var s = new XmlSerializer(t);
                    T ret = default(T);
                    using (var sr = new StringReader(node.InnerXml)) ret = (T)s.Deserialize(sr);
                    callback(ret);
                } catch(Exception) {                    
                #if DEBUG
                    throw;
                #endif
                }

                return name != null ? Parent : this;
            }
        }
    }
}