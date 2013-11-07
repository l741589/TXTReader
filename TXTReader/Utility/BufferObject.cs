using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Utility {
    delegate T GetFunc<T>();
    delegate void SetFunc<T>(BufferObject<T> _this, T value);

    struct BufferObject<T> {
        public T Value;
        public bool HasValue;
      
        public T Get(GetFunc<T> get = null){
            if (HasValue) {
                return Value;
            } else {
                if (get == null) return default(T);
                else return get();
            }            
        }

        public void Set(T value,SetFunc<T> set = null){
            if (set == null) set(this, value);
            else Value=value;
            if (Object.Equals(Value,default(T))) HasValue=false;
        }
    
    }
}
