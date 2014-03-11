using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FloatControls;
using System.Windows.Data;
using Zlib.Utility;
namespace TRBook {
    class FloatTiltle : FloatMessage {

        public FloatTiltle() {
            G.BookChanged += G_BookChanged;
            //Book.Empty.Closed += Empty_Closed;
            if (G.Book.IsNull()) return;
            SetBinding(ValueProperty, new Binding("CurrentTitle") { Source = G.Book });
            Name = "章节";
            this.Register();
        }

        void G_BookChanged(object sender, TXTReader.BookChangedEventArgs e) {
            Value = null;
            if (sender.IsNull()) return;
            BindingOperations.ClearBinding(this, ValueProperty);
            SetBinding(ValueProperty, new Binding("CurrentTitle") { Source = sender });
        }
    }
}
