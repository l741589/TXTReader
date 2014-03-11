using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

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

        public static object Invoke(this Dispatcher d, Action action) {
            return d.Invoke(action);
        }

        public static T Invoke<T>(this Dispatcher d, Func<T> func) {
            return (T)d.Invoke(func);
        }
    }
}
