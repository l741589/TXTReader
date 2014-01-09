using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Utility {
    public class NullObject {

        private static NullObject instance = new NullObject();
        private static HashSet<object> objects = new HashSet<object>();

        public static NullObject I { get { return instance; } }
        public static NullObject Instance { get { return instance; } }
        public static NullObject NULL { get { return instance; } }

        public static bool IsNull(object obj){
            if (obj == null) return true;
            if (objects.Contains(obj)) return true;
            return false;
        }

        public static bool NotNull(object obj) {
            return !IsNull(obj);
        }

        public override bool Equals(object obj) {
            return IsNull(obj);
        }

        public static void Add(Object obj){
            objects.Add(obj);
        }

        public static bool Remove(Object obj){
            if (objects.Count>0) return objects.Remove(obj);
            return false;
        }

        public override int GetHashCode() {
            return 0;
        }

        public static object Find(Type t) {
            foreach(var e in objects)
                if (t.IsInstanceOfType(e)) return e;
            return null;
        }
    }
}
