using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Zlib.Widget {
    public class CompoundContextMenu : ContextMenu{

        public List<object[]> Menus { get; private set; }

        public CompoundContextMenu() :base() {
            Menus = new List<object[]>();
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

        public CompoundContextMenu Build() {
            Items.Clear();
            for (int i = 0; i < Menus.Count; ++i) {
                object[] menu = Menus[i];
                foreach (var item in menu) Items.Add(item);
                if (i < Menus.Count - 1) Items.Add(new Separator());
            }
            return this;
        }
    }
}
