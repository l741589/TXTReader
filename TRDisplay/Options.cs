using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TRDisplay {
    public class Options : DependencyObject {
        public Options() {
            instance = this;
        }

        public const int DEFAULT_SPEED = 5;

        static private Options instance;
        static public Options Instance { get { if (instance == null) instance = new Options(); return instance; } }

        public static readonly DependencyProperty SkinProperty = DependencyProperty.Register("Skin", typeof(Skin), typeof(Options), new PropertyMetadata(new Skin()));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(int), typeof(Options), new PropertyMetadata(DEFAULT_SPEED));        
        public static readonly DependencyProperty IsFilterSpaceProperty = DependencyProperty.Register("IsFilterSpace", typeof(bool), typeof(Options), new PropertyMetadata(true));

        public Skin Skin { get { return (Skin)GetValue(SkinProperty); } set { SetValue(SkinProperty, value); } }
        public int Speed { get { return (int)GetValue(SpeedProperty); } set { SetValue(SpeedProperty, value); } }
        public bool IsFilterSpace { get { return (bool)GetValue(IsFilterSpaceProperty); } set { SetValue(IsFilterSpaceProperty, value); } }
    }
}
