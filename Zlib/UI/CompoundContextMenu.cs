using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Zlib.UI {
    public class CompoundContextMenu : ContextMenu{

        public List<object[]> Menus { get; private set; }
        public bool IsMutexCommand { get; set; }

        public CompoundContextMenu() :base() {
            Menus = new List<object[]>();
            IsMutexCommand = false;
        }

        public CompoundContextMenu(params MenuBase[] menus) : base(){
            Menus = new List<object[]>();
            Add(menus);
            Build();
        }

        public CompoundContextMenu Add(params MenuBase[] menus) {
            foreach (var menu in menus) {
                if (menu == null) continue;
                object[] items = new object[menu.Items.Count];
                menu.Items.CopyTo(items, 0);
                Menus.Add(items);
                menu.Items.Clear();
            }
            return this;
        }

        private bool HasCommand(object item) {
            if (!(item is ICommandSource)) return false;
            return HasCommand((item as ICommandSource).Command);
        }

        private bool HasCommand(ICommand cmd) {
            foreach (var i in Items) {
                var cs = i as ICommandSource;
                if (cs == null || cs.Command == null) continue;
                if (cs.Command == cmd) return true;
            }
            return false;
        }

        public CompoundContextMenu Build() {
            Items.Clear();
            for (int i = 0; i < Menus.Count; ++i) {
                object[] menu = Menus[i];
                foreach (var item in menu) {
                    if (IsMutexCommand && HasCommand(item)) continue;
                    Items.Add(item);
                }
                if (i < Menus.Count - 1) Items.Add(new Separator());
            }
            return this;
        }
    }
}
