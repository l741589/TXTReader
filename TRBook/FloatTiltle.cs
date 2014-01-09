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
            Book.Empty.Loaded += Empty_Loaded;
            Book.Empty.Closed += Empty_Closed;
            if (Book.I.IsNull()) return;
            SetBinding(ValueProperty, new Binding("CurrentTitle") { Source = Book.I });
            Name = "章节";
            FloatControlsPanel.FloatControls.Add(this);
        }

        void Empty_Closed(object sender, EventArgs e) {
            BindingOperations.ClearAllBindings(this);
            Value = null;
        }

        void Empty_Loaded(object sender, EventArgs e) {
            if (sender.IsNull()) return;
            BindingOperations.ClearAllBindings(this);
            SetBinding(ValueProperty, new Binding("CurrentTitle") { Source = sender });
        }
    }
}
