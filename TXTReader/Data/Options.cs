using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TXTReader.Data {
    class Options : DependencyObject{
        public const int DEFAULT_SPEED = 5;

        static private Options instance;
        static public Options Instance { get { if (instance == null) instance = new Options(); return instance; } }

        public static readonly DependencyProperty SkinProperty = DependencyProperty.Register("Skin", typeof(Skin), typeof(Options),new PropertyMetadata(new Skin()));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(int), typeof(Options), new PropertyMetadata(DEFAULT_SPEED));
        public static readonly DependencyProperty FloatMessageProperty = DependencyProperty.Register("FloatMessage", typeof(FloatMessage), typeof(Options), new PropertyMetadata(new FloatMessage()));
        public static readonly DependencyProperty IsFloatMessageOpenProperty = DependencyProperty.Register("IsFloatMessageOpen", typeof(bool), typeof(Options), new PropertyMetadata(true));
        public static readonly DependencyProperty MaxChapterLengthProperty = DependencyProperty.Register("MaxChapterLength", typeof(int), typeof(Options), new PropertyMetadata(15000));
        public static readonly DependencyProperty MinChapterLengthProperty = DependencyProperty.Register("MinChapterLength", typeof(int), typeof(Options), new PropertyMetadata(1000));

        public Skin Skin { get { return (Skin)GetValue(SkinProperty); } set { SetValue(SkinProperty, value); } }
        public int Speed { get { return (int)GetValue(SpeedProperty); } set { SetValue(SpeedProperty, value); } }
        public FloatMessage FloatMessage { get { return (FloatMessage)GetValue(FloatMessageProperty); } set { SetValue(FloatMessageProperty, value); } }
        public bool IsFloatMessageOpen { get { return (bool)GetValue(IsFloatMessageOpenProperty); } set { SetValue(IsFloatMessageOpenProperty, value); } }
        public int MaxChapterLength { get { return (int)GetValue(MaxChapterLengthProperty); } set { SetValue(MaxChapterLengthProperty, value); } }
        public int MinChapterLength { get { return (int)GetValue(MinChapterLengthProperty); } set { SetValue(MinChapterLengthProperty, value); } }
    }
}