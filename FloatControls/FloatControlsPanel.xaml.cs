using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FloatControls {
    /// <summary>
    /// FloatMessageManager.xaml 的交互逻辑
    /// </summary>
    public partial class FloatControlsPanel : UserControl {

        private static FloatControlsPanel instance = null;
        internal static FloatControlsPanel Instance { get { if (instance == null) instance = new FloatControlsPanel(); return instance; } }

        public Panel this[FloatPosition pos] {
            get {
                switch (pos) {
                    case FloatPosition.Left: return pn_left;
                    case FloatPosition.Right: return pn_right;
                    case FloatPosition.Top: return pn_top;
                    case FloatPosition.Bottom: return pn_bottom;
                    case FloatPosition.LeftTop: return pn_lefttop;
                    case FloatPosition.LeftBottom: return pn_leftbottom;
                    case FloatPosition.RightTop: return pn_righttop;
                    case FloatPosition.RightBottom: return pn_rightbottom;
                    case FloatPosition.Canvas: return pn_moving;
                    case FloatPosition.Root: return pn_root;
                    default: return pn_lefttop;
                }
            }
        }

        public FloatPosition? this[Panel pos] {
            get {
                if (pos == pn_left) return FloatPosition.Left;
                if (pos == pn_right) return FloatPosition.Right;
                if (pos == pn_top) return FloatPosition.Top;
                if (pos == pn_bottom) return FloatPosition.Bottom;
                if (pos == pn_lefttop) return FloatPosition.LeftTop;
                if (pos == pn_leftbottom) return FloatPosition.LeftBottom;
                if (pos == pn_righttop) return FloatPosition.RightTop;
                if (pos == pn_rightbottom) return FloatPosition.RightBottom;
                if (pos == pn_moving) return FloatPosition.Canvas;
                if (pos == pn_root) return FloatPosition.Root;
                return null;
            }
        }

        public FloatControlsPanel() {
            InitializeComponent();
            Add(G.FloatControls);
            G.FloatControls.CollectionChanged += FloatControls_CollectionChanged;
        }

        void FloatControls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add: Add(e.NewItems); break;
                case NotifyCollectionChangedAction.Remove: Remove(e.OldItems); break;
                case NotifyCollectionChangedAction.Replace: Remove(e.OldItems); Add(e.NewItems); break;
                case NotifyCollectionChangedAction.Reset: Clear(); Add(G.FloatControls); break;
                case NotifyCollectionChangedAction.Move: break;
            }
        }

        public void Add(IFloatControl control) {
            this[control.Position].Children.Add(control as UIElement);
        }

        public void Add(params IFloatControl[] controls) {
            foreach (var control in controls)
                this[control.Position].Children.Add(control as UIElement);
        }

        public void Add(IList controls) {
            foreach (IFloatControl control in controls)
                this[control.Position].Children.Add(control as UIElement);
        }

        public void Remove(IList controls) {
            foreach (IFloatControl control in controls)
                this[control.Position].Children.Remove(control as UIElement);
        }

        public void Clear() {
            foreach (FloatPosition e in Enum.GetValues(typeof(FloatPosition))) {
                this[e].Children.Clear();
            }
        }
    }
}
