using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zlib.UI {
 public class MultiSelectionChangedEventArgs : RoutedEventArgs {
            public MultiSelectionChangedEventArgs(RoutedEvent e, object s) : base(e, s) { }
            public object ChangedItem { get; set; }
            public bool IsChecked { get; set; }
            public object[] SelectedItems { get; set; }
        }
        public delegate void MultiSelectionChangedEventHandler(object sender, MultiSelectionChangedEventArgs e);
    /// <summary>
    /// MultiSelectPanel.xaml 的交互逻辑
    /// </summary>
    public partial class MultiSelectPanel : ItemsControl {
       
        public event MultiSelectionChangedEventHandler MultiSelectionChanged {
            add { AddHandler(MultiSelectionChangedEvent, value); }
            remove { RemoveHandler(MultiSelectionChangedEvent, value); }
        }
        public static readonly RoutedEvent MultiSelectionChangedEvent = EventManager.RegisterRoutedEvent("MultiSelectionChanged", RoutingStrategy.Bubble, typeof(MultiSelectionChangedEventHandler), typeof(MultiSelectPanel));        

        public bool DefaultIsChecked { get { return (bool)GetValue(DefaultIsCheckedProperty); } set { SetValue(DefaultIsCheckedProperty, value); } }
        public Dictionary<object, bool> dict = new Dictionary<object, bool>();
        public static readonly DependencyProperty DefaultIsCheckedProperty = DependencyProperty.Register("DefaultIsChecked", typeof(bool), typeof(MultiSelectPanel), new PropertyMetadata(false));

        public String NamePath { get { return (String)GetValue(NamePathProperty); } set { SetValue(NamePathProperty, value); } }
        public static readonly DependencyProperty NamePathProperty = DependencyProperty.Register("NamePath", typeof(String), typeof(MultiSelectPanel), new PropertyMetadata(null));

        public MultiSelectPanel() {
            InitializeComponent();            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            var cb=sender as CheckBox;
            dict[cb.DataContext] = true;
            OnSelectionChanged(cb.DataContext, cb.IsChecked==true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            dict[cb.DataContext] = false;
            OnSelectionChanged(cb.DataContext, cb.IsChecked == true);
        }

        private void OnSelectionChanged(object item, bool isChecked) {
            var arg = new MultiSelectionChangedEventArgs(MultiSelectionChangedEvent, this) { ChangedItem = item, IsChecked = isChecked, SelectedItems = GetSelectedItems() };
            RaiseEvent(arg);
        }

        public bool IsChecked(object o) {
            if (o == null) return false;
            if (!Items.Contains(o)) return false;
            if (dict.ContainsKey(o)) return dict[o];
            else return dict[o] = DefaultIsChecked;
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e) {
            var tb = (sender as TextBlock);
            if (!dict.ContainsKey(tb.DataContext)) dict[tb] = DefaultIsChecked;
            tb.SetBinding(TextBlock.TextProperty, NamePath);
        }

        private void CheckBox_Loaded(object sender, RoutedEventArgs e) {
            var cb = sender as CheckBox;
            if (dict.ContainsKey(cb.DataContext)) cb.IsChecked=dict[cb.DataContext];
            else cb.IsChecked = DefaultIsChecked;
        }

        public object[] GetSelectedItems() {
            Stack<object> ss = new Stack<object>();
            foreach (var e in Items) {
                if (IsChecked(e)) ss.Push(e);
            }
            return ss.ToArray();
        }
    }
}
