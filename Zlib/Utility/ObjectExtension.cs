using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Utility {
    public static class ObjectExtension {
        public static bool IsNull(this object obj) {
            return NullObject.IsNull(obj);
        }

        public static bool NotNull(this object obj) {
            return NullObject.NotNull(obj);
        }

        public static bool ToBool<T>(this T obj) {
            if (IsNull(obj)) return false;
            if (Object.Equals(obj,default(T))) return false;
            return true;
        }
        
        /// <summary>
        /// 获取已经在NullObject类中注册的空对象实例，
        /// 效率较低，谨慎使用。
        /// </summary>
        public static T GetEmpty<T>(this T obj) {
            return (T)NullObject.Find(typeof(T));
        }
    }
}
