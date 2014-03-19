using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FloatControls;
using System.Windows.Data;
using Zlib.Utility;
namespace TRBookcase {
    class FloatTiltle : FloatMessage {

        public FloatTiltle() {
            G.BookChanged += G_BookChanged;       
            Name = "章节";
            this.Register();
        }

        void G_BookChanged(object sender, TXTReader.BookChangedEventArgs e) {
            Value = null;
            if (sender.IsNull()) return;
            BindingOperations.ClearBinding(this, ValueProperty);
            if (e.NewBook != null) {
                SetBinding(ValueProperty, new Binding("CurrentTitle") { Source = e.NewBook });
                e.NewBook.Closed += NewBook_Closed;
            }
        }

        void NewBook_Closed(object sender, TXTReader.Interfaces.PluginEventArgs e) {
            BindingOperations.ClearBinding(this, ValueProperty);
        }
    }
}
