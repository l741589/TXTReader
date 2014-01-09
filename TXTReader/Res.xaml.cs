using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader {
    public partial class Res {
        private static Res instance = null;
        public static Res Instance { get { if (instance == null) instance = new Res(); return instance; } }

        public Res() {
            InitializeComponent();
        }
    }
}
