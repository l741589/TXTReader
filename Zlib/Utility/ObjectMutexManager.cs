using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zlib.Utility {
    public class ObjectMutexManager<T> {
        private Dictionary<String, T> dic = new Dictionary<String, T>();

        public T Get(String name) {
            if (dic.ContainsKey(name)) return dic[name];
            else {
                var t = (T)typeof(T).GetConstructor(null).Invoke(null);
                dic[name] = t;
                return t;
            }
        }

        public T Get(String name, params object[] args) {
            if (dic.ContainsKey(name)) return dic[name];
            else {
                var c=typeof(T).GetConstructor((from arg in args select arg.GetType()).ToArray());
                T t=(T)c.Invoke(args);
                dic[name] = t;
                return t;
            }
       }

        public T Get(String name, Type[] types, object[] args) {
            if (dic.ContainsKey(name)) return dic[name];
            else {
                var c = typeof(T).GetConstructor(types);                
                T t = (T)c.Invoke(args);
                dic[name] = t;
                return t;
            }
        }
    }
}
