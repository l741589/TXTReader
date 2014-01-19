using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Zlib.Utility {
    public static class DependencyObjectExtension {
        public static void SetBinding(this DependencyObject d, DependencyProperty p, BindingBase b) {
            BindingOperations.SetBinding(d, p, b);
        }

        public static void SetBinding(this DependencyObject d, DependencyProperty p, String path, object source = null, IValueConverter converter = null, BindingMode mode = BindingMode.Default) {
            BindingOperations.SetBinding(d, p, new Binding(path) {
                Source = source,
                Mode = mode,
                Converter = converter
            });
        }
    }
}
