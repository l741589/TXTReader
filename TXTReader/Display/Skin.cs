using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace TXTReader.Display
{

    public enum BackGroundType { SolidColor, Image }
    public enum EffectType { None, Shadow, Stroke }//无，阴影，描边

    public class Skin
    {
        //the problem of binding in optionpanel.
        public Skin()
        {
        }

        public String Path { get; set; }
        private BackGroundType backGroundType;
        private Brush background = null;

        public Thickness Padding { get; set; }

        public Brush Foreground { get; set; }//FontColor
        public Typeface Font { get; set; }
        public double FontSize { get; set; }
        public EffectType EffectType { get; set; }//字体特效
        public Color Effect { get; set; }//特效所用的颜色
        public double EffectSize { get; set; }//特效的大小

        public double LineSpacing { get; set; }//行间距
        public double ParaSpacing { get; set; }//段间距

        private Color backColor;
        public Color BackColor { get { return backColor; } set { backColor = value; background = null; } }//背景颜色
        private ImageSource backImage;
        public ImageSource BackImage { get { return backImage; } set { backImage = value; background = null; } }//背景图片
        public BackGroundType BackGroundType
        {
            get { return backGroundType; }
            set
            {
                if (backGroundType != value)
                {
                    backGroundType = value;
                    background = null;
                }
            }
        }
        
        public Brush Background
        {
            get
            {
                if (background != null) return background;
                switch (BackGroundType)
                {
                    case BackGroundType.SolidColor: return background = new SolidColorBrush(BackColor);
                    case BackGroundType.Image: return background = new ImageBrush(BackImage);
                    default: return null;
                }
            }
        }
    }
}
