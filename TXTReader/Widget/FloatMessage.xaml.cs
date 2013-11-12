using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace TXTReader.Widget {
    /// <summary>
    /// FloatMessage.xaml 的交互逻辑
    /// </summary>
    public partial class FloatMessage : TextBlock {

        private bool visited = false;

        private FloatMessage alignLeft;
        private FloatMessage alignRight;
        private FloatMessage alignTop;
        private FloatMessage alignButtom;

        public FloatMessage AlignLeft { get { return alignLeft; } set; }
        public FloatMessage AlignRight { get { return alignRight; } set; }
        public FloatMessage AlignTop { get { return alignTop; } set; }
        public FloatMessage AlignBottom { get { return alignButtom; } set; }
        public bool AlignParentLeft { get; set; }
        public bool AlignParentRight { get; set; }
        public bool AlignParentTop { get; set; }
        public bool AlignParentBottom { get; set; }

        public int Degree {
            get {
                int ret = 0;
                if (alignLeft!=null) ++ret;
                if (alignRight!=null) ++ret;
                if (alignTop!=null) ++ret;
                if (alignButtom!=null) ++ret;
                return ret;
            }
        }

        public void Detach(FloatMessage message, FloatMessage from) {
            if (message == null) return;
            if (message.visited) return;
            if (message.alignLeft == from) message.alignLeft = null;
            Detach(message.alignLeft, from);

        }

        public FloatMessage() {
            InitializeComponent();
            AlignLeft = AlignRight = AlignTop = AlignBottom = null;
        }
    }
}
