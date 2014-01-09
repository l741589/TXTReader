using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Utility {
    public static class StringExtension {
        public static bool IsNullOrEmpty(this String s){
            return String.IsNullOrEmpty(s);
        }

        public static bool IsNullOrWhiteSpace(this String s) {
            return String.IsNullOrWhiteSpace(s);
        }
    }
}
