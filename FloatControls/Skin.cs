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

namespace FloatControls
{

    public enum BackGroundType { SolidColor, Image }
    public enum EffectType { None, Shadow, Stroke }//无，阴影，描边

    public class Skin : INotifyPropertyChanged
    {
        private static Skin instance = null;
        public static Skin Instance{
            get{
                if (instance == null) instance = new Skin();
                return instance;
            }
        }
        private Skin() {
            Font = new Typeface("宋体");
            FontSize = 10;
            Padding = new Thickness(0);
            Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            Foreground = Brushes.Black;
        }
        
        public Thickness Padding { get { return _Padding; } set { _Padding = value; OnPropertyChanged("Padding"); } } private Thickness _Padding;
        public Brush Foreground { get { return _Foreground; } set { _Foreground = value; OnPropertyChanged("Foreground"); } } private Brush _Foreground;
        public Typeface Font { get { return _Font; } set { _Font = value; OnPropertyChanged("Font"); } } private Typeface _Font;
        public double FontSize { get { return _FontSize; } set { _FontSize = value; OnPropertyChanged("FontSize"); } } private double _FontSize;
        public Brush Background { get { return _Background; } set { _Background = value; OnPropertyChanged("Background"); } } private Brush _Background;      

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String prop) {
            //if (prop == "BackGroundType" || prop == "BackColor" || prop == "BackImage") background = null;
            if (PropertyChanged!=null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
