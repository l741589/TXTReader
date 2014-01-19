using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Zlib.UI.Utility {
    public static class UIElementExtentions {
        public static bool IsImmediateFamilyOf(this UIElement target,UIElement e) {
            if (target==null) return false;
            if (e == null) return false;
            if (e.IsAncestorOf(target)) return true;
            if (target.IsAncestorOf(e)) return true;
            return false;
        }

        public static bool IsDescendantOf(this UIElement target, UIElement e) {
            if (target==null) return false;
            if (e == null) return false;
            return e.IsAncestorOf(target);
        }
    }
}
