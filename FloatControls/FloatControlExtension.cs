using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatControls {
    public static class FloatControlExtension {
        public static void Register(this IFloatControl item){
            FloatControlCollection.Instance.Add(item);
        }

        public static void UnRegister(this IFloatControl item) {
            FloatControlCollection.Instance.Remove(item);
        }
    }
}
