using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TXTReader.Display;
using TXTReader.FloatMessages;

namespace TXTReader.ToolPanel {
    public class Options : DependencyObject {
        public const int DEFAULT_SPEED = 5;

        static private Options instance;
        static public Options Instance { get { if (instance == null) instance = new Options(); return instance; } }

        public static readonly DependencyProperty SkinProperty = DependencyProperty.Register("Skin", typeof(Skin), typeof(Options), new PropertyMetadata(new Skin()));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(int), typeof(Options), new PropertyMetadata(DEFAULT_SPEED));
        public static readonly DependencyProperty FloatMessageProperty = DependencyProperty.Register("FloatMessage", typeof(FloatMessageOption), typeof(Options), new PropertyMetadata(new FloatMessageOption()));
        public static readonly DependencyProperty IsFloatMessageOpenProperty = DependencyProperty.Register("IsFloatMessageOpen", typeof(bool), typeof(Options), new PropertyMetadata(true));
        public static readonly DependencyProperty MaxChapterLengthProperty = DependencyProperty.Register("MaxChapterLength", typeof(int), typeof(Options), new PropertyMetadata(15000));
        public static readonly DependencyProperty MinChapterLengthProperty = DependencyProperty.Register("MinChapterLength", typeof(int), typeof(Options), new PropertyMetadata(1000));
        public static readonly DependencyProperty IsBorderedProperty = DependencyProperty.Register("IsBordered", typeof(bool), typeof(Options), new PropertyMetadata(true));
        public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register("IsFullScreen", typeof(bool), typeof(Options), new PropertyMetadata(false));
        public static readonly DependencyProperty IsFilterSpaceProperty = DependencyProperty.Register("IsFilterSpace", typeof(bool), typeof(Options), new PropertyMetadata(true));

        public Skin Skin { get { return (Skin)GetValue(SkinProperty); } set { SetValue(SkinProperty, value); } }
        public int Speed { get { return (int)GetValue(SpeedProperty); } set { SetValue(SpeedProperty, value); } }
        public FloatMessageOption FloatMessage { get { return (FloatMessageOption)GetValue(FloatMessageProperty); } set { SetValue(FloatMessageProperty, value); } }
        public bool IsFloatMessageOpen { get { return (bool)GetValue(IsFloatMessageOpenProperty); } set { SetValue(IsFloatMessageOpenProperty, value); } }
        public int MaxChapterLength { get { return (int)GetValue(MaxChapterLengthProperty); } set { SetValue(MaxChapterLengthProperty, value); } }
        public int MinChapterLength { get { return (int)GetValue(MinChapterLengthProperty); } set { SetValue(MinChapterLengthProperty, value); } }
        public bool IsBordered { get { return (bool)GetValue(IsBorderedProperty); } set { SetValue(IsBorderedProperty, value); } }        
        public bool IsFullScreen { get { return (bool)GetValue(IsFullScreenProperty); } set { SetValue(IsFullScreenProperty, value); } }
        public bool IsFilterSpace { get { return (bool)GetValue(IsFilterSpaceProperty); } set { SetValue(IsFilterSpaceProperty, value); } }
    }
}
