using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System;
using System.Xml;

namespace TXTReader.Data
{

    public enum BackGroundType { SolidColor, Image }
    public enum EffectType { None, Shadow, Stroke }//无，阴影，描边

    public class Skin
    {
        //the problem of binding in optionpanel.
        public Skin()
        {
            //no need to initialize after OptionPanel3.
        }

        private BackGroundType backGroundType;
        private Brush background = null;

        public Thickness Padding { get; set; }

        public Brush Foreground { get; set; }//FontColor
        public Typeface Font { get; set; }
        public double FontSize { get; set; }
        public EffectType EffectType { get; set; }//字体特效
        public Brush Effect { get; set; }//特效所用的颜色

        ///////////////////////////////////////////////////////////////////
        //recommended to fix.
        public double EffetSize { get; set; }//特效的大小
        //spelling mistake...?
        //Though I turned out to use the name "EffetSize" in my implementation of OptionPanel.
        ///////////////////////////////////////////////////////////////////

        public double LineSpacing { get; set; }//行间距
        public double ParaSpacing { get; set; }//段间距

        public Color BackColor { get; set; }//背景颜色
        public ImageSource BackImage { get; set; }//背景图片
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
                //what if the BackColor is changed?
                //if (background != null) return background;
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
