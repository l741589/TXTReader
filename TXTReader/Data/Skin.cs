using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TXTReader.Data {

    enum BackGroundType { SolidColor, Image }
    enum TextEffect { None, Shadow, Stroke }//无，阴影，描边

    class Skin {
        private BackGroundType backGroundType;
        private Brush background = null;

        public Thickness Padding { get; set; }

        public Brush Foreground { get; set; }//FontColor
        public Typeface Font { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderLine { get; set; }
        public TextEffect TextEffect { get; set; }//字体特效
        public Color EffetColor { get; set; }//特效所用的颜色

        public int LineSpacing { get; set; }//行间距
        public int paraspacing { get; set; }//段间距

        public Color BackColor { get; set; }//背景颜色
        public ImageSource BackImage { get; set; }//背景图片
        public BackGroundType BackGroundType {
            get { return backGroundType; }
            set {
                if (backGroundType != value) {
                    backGroundType = value; 
                    background = null;
                }
            }
        }
        public Brush Background {
            get {
                if (background==null) return background;
                switch (BackGroundType)
                {
                case BackGroundType.SolidColor:return new SolidColorBrush(BackColor);
                case BackGroundType.Image: return new ImageBrush(BackImage);
                default: return null;
                }
            }
        }
    }
}
