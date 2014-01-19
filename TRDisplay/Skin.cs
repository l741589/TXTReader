using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.ComponentModel;
using System.Windows.Media.Effects;

namespace TRDisplay
{

    public enum BackGroundType { SolidColor, Image }
    public enum EffectType { None, Shadow, Stroke }//无，阴影，描边

    public class Skin : INotifyPropertyChanged
    {
        public String Path { get { return _Path; } set { _Path = value; OnPropertyChanged("Path"); } } private String _Path;
        
        public Thickness Padding { get { return _Padding; } set { _Padding = value; OnPropertyChanged("Padding"); } } private Thickness _Padding;
        public Brush Foreground { get { return _Foreground; } set { _Foreground = value; OnPropertyChanged("Foreground"); } } private Brush _Foreground;
        public Typeface Font { get { return _Font; } set { _Font = value; OnPropertyChanged("Font"); } } private Typeface _Font;
        public double FontSize { get { return _FontSize; } set { _FontSize = value; OnPropertyChanged("FontSize"); } } private double _FontSize;
        public Effect Effect { get { return _Effect; } set { _Effect = value; OnPropertyChanged("Effect"); } } private Effect _Effect;      
        //public Color Effect { get { return _Effect; } set { _Effect = value; OnPropertyChanged("Effect"); } } private Color _Effect;      
        //public double EffectSize { get { return _EffectSize; } set { _EffectSize = value; OnPropertyChanged("EffectSize"); } } private double _EffectSize;
        //public EffectType EffectType { get { return _EffectType; } set { _EffectType = value; OnPropertyChanged("EffectType"); } } private EffectType _EffectType;      

        public double LineSpacing { get { return _LineSpacing; } set { _LineSpacing = value; OnPropertyChanged("LineSpacing"); } } private double _LineSpacing;
        public double ParaSpacing { get { return _ParaSpacing; } set { _ParaSpacing = value; OnPropertyChanged("ParaSpacing"); } } private double _ParaSpacing;

        public Brush Background { get { return _Background; } set { _Background = value; OnPropertyChanged("Background"); } } private Brush _Background;      

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String prop) {
            //if (prop == "BackGroundType" || prop == "BackColor" || prop == "BackImage") background = null;
            if (PropertyChanged!=null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
