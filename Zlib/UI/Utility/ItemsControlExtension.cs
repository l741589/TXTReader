using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace Zlib.UI.Utility {
    public static class ItemsControlExtension {

        public static ScrollViewer GetScrollViewer(this ItemsControl c) {
            var border = VisualTreeHelper.GetChild(c, 0) as Decorator;
            if (border != null) {
                return border.Child as ScrollViewer;

            }
            return null;
        }

        public static void ScrollToBottom(this ItemsControl c) {
            var s = GetScrollViewer(c);
            if (s == null) return;
            s.ScrollToEnd();
        }

        public static void ScrollToTop(this ItemsControl c) {
            var s = GetScrollViewer(c);
            if (s == null) return;
            s.ScrollToHome();
        }
        public static bool IsScrollAtTop(this ItemsControl c, int offset = 0) {
            var s = GetScrollViewer(c);
            if (s == null) return false;
            return s.VerticalOffset == offset;
        }

        public static bool IsScrollAtBottom(this ItemsControl c, bool includeTop) {
            return IsScrollAtBottom(c, 0, includeTop);
        }

        public static bool IsScrollAtBottom(this ItemsControl c, int offset = 0, bool includeTop = false) {
            var s = GetScrollViewer(c);
            if (s == null) return false;
            bool isAtButtom = false;
            double dVer = s.VerticalOffset;
            double dViewport = s.ViewportHeight;
            double dExtent = s.ExtentHeight;
            if (dVer != 0 || includeTop) {
                if (dVer + dViewport >= dExtent - offset) {
                    isAtButtom = true;
                } else {
                    isAtButtom = false;
                }
            } else {
                isAtButtom = false;
            }
            if (s.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled
                || s.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden) {
                isAtButtom = true;
            }
            return isAtButtom;
        }
    }
}
