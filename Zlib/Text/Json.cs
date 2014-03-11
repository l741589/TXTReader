using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Xml;
using System.Collections;

namespace Zlib.Text {
    public static class Json {
        public static String Encode(object obj) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            
            return jss.Serialize(obj);
        }

        public static String Encode(params Object[] objects) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            String[] ss = new String[objects.Length];
            for (int i = 0; i < objects.Length; ++i) {
                ss[i] = jss.Serialize(objects[i]);
            }
            return jss.Serialize(ss);
        }

        public static T Decode<T>(String input) {
            try {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Deserialize<T>(input);
            } catch {
                Debug.WriteLine("Undecodable Json String '{0}'", (object)input);
                return default(T); 
            }
        }

        public static Object Decode(String input, Type type) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize(input, type);
        }

        public static Object Decode(String input, Type type, int limit) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RecursionLimit = limit;
            return jss.Deserialize(input, type);
        }

        public static Object Decode(String input, int limit) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RecursionLimit = limit;
            return jss.Deserialize<Object>(input);
        }

        public static Object Decode(String input) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<Object>(input);
        }

        public static void ToXml(XmlNode parent, String name, object o, int position = 0) {
            if (o is Dictionary<String, object>) ToXml(parent, name, o as Dictionary<String, object>, position);
            else if (o is IEnumerable && !(o is String)) ToXml(parent, name, o as IEnumerable, position);
            else {
                XmlDocument doc = parent is XmlDocument ? parent as XmlDocument : parent.OwnerDocument;
                XmlElement elem = doc.CreateElement(o is String ? "string" : o is int ? "int" : o is bool ? "bool" : o.GetType().Name);
                parent.AppendChild(elem);
                if (name != null) elem.SetAttribute("name", name);
                elem.SetAttribute("value", o.ToString());
                if (position > 0) elem.SetAttribute("position", position.ToString());
            }
        }

        public static void ToXml(XmlNode parent, String name, IEnumerable o, int position = 0) {
            XmlDocument doc = parent is XmlDocument ? parent as XmlDocument : parent.OwnerDocument;
            XmlElement elem = doc.CreateElement("array");
            parent.AppendChild(elem);
            if (name!=null) elem.SetAttribute("name", name);
            if (position > 0) elem.SetAttribute("position", position.ToString());
            int i = 0;
            foreach (var e in o) {
                ++i;
                ToXml(elem, null, e, i);
            }
        }

        public static void ToXml(XmlNode parent, String name, Dictionary<String, Object> o, int position = 0) {
            XmlDocument doc = parent is XmlDocument ? parent as XmlDocument : parent.OwnerDocument;
            XmlElement elem = doc.CreateElement("class");
            parent.AppendChild(elem);
            if (name != null) elem.SetAttribute("name", name);
            if (position > 0) elem.SetAttribute("position", position.ToString());            
            foreach (var e in o) {
                ToXml(elem, e.Key, e.Value);
            }
        }

        public static String ToXml(String input) {
            XmlDocument d = new XmlDocument();
            ToXml(d, null,Json.Decode(input));
            StringBuilder sb = new StringBuilder();

            //XmlWriterSettings xws = new XmlWriterSettings();
            //xws.Indent = true;
            //xws.ConformanceLevel = ConformanceLevel.Auto;
            //xws.IndentChars = "\t";
            //xws.OmitXmlDeclaration = false;

            XmlWriter w = XmlWriter.Create(sb);
            d.WriteTo(w);
            w.Close();
            return sb.ToString();
        }

        public static void FromXml(object parent, XmlElement o) {
            if (o.ChildNodes == null) return;
            if (parent is Queue<object>) FromXml(parent as Queue<object>, o);
            else FromXml(parent as Dictionary<String,object>, o);
        }

        public static void FromXml(Queue<object> parent, XmlElement o) {
            if (o.ChildNodes == null) return;
            foreach (var e in o.ChildNodes) {
                if (e is XmlElement){
                    var elem = e as XmlElement;
                    var x = FromXml(elem);
                    parent.Enqueue(x);
                    FromXml(x, elem);
                }
            }
        }

        public static void FromXml(Dictionary<String, Object> parent, XmlElement o) {
            if (o.ChildNodes == null) return;
            foreach (var e in o.ChildNodes) {
                if (e is XmlElement) {
                    var elem = e as XmlElement;
                    var x = FromXml(elem);
                    parent.Add(elem.Attributes["name"].Value, x);
                    FromXml(x, elem);
                }
            }
        }

        public static object FromXml(XmlElement o) {
            switch (o.Name) {
                case "class": return new Dictionary<String, object>();
                case "array": return new Queue<object>();
                case "string": return o.Attributes["value"].Value;
                case "int": return int.Parse(o.Attributes["value"].Value);
                case "bool": return bool.Parse(o.Attributes["value"].Value);
                default: return o.Attributes["value"];
            }
        }

        public static String FromXml(String xml) {            
            XmlDocument d = new XmlDocument();
            d.LoadXml(xml);
            object root = null;
            foreach (var e in d.ChildNodes) {
                if (e is XmlElement) {
                    var elem = e as XmlElement;
                    root = FromXml(elem);
                    FromXml(root, elem);
                }
            }
            
            return Json.Encode(root);
        }
       

        public static Object[] Decode(String input, params Type[] types) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            String[] ss = jss.Deserialize<String[]>(input);
            Object[] ret = new Object[ss.Length];
            for (int i = 0; i < ss.Length; ++i) {
                ret[i] = jss.Deserialize(ss[i], types[i]);
            }
            return ret;
        }
    }
}
