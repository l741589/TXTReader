using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace Zlib.Text {
    public class Json {
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
