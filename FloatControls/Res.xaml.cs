using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FloatControls {
    public partial class Res : ResourceDictionary{
        private static Res instance = null;
        public static Res Instance { get { if (instance == null) instance = new Res(); return instance; } }
        public Res() {
            InitializeComponent();
        }
    }
}
