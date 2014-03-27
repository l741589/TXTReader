using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Zlib.UI.Utility;
using Zlib.Utility;
using System.Diagnostics;

namespace TRDisplay {

    
    /// <summary>
    /// BackgroundBrushSelector.xaml 的交互逻辑
    /// </summary>

    public partial class BackgroundBrushSelector : UserControl {

        public static Dictionary<ScaleType, string> ImageScaleDict { get; set; }
        private ZBrushes brushes = null;

        static BackgroundBrushSelector() {
            ImageScaleDict = new Dictionary<ScaleType, string>() {
                { ScaleType.None, "无图片" }, 
                { ScaleType.Center, "居中" }, 
                { ScaleType.Stretch, "拉伸" }, 
                { ScaleType.Tile, "平铺" }
            };
        }

        public Rect Viewport { get { return (Rect)GetValue(ViewportProperty); } set { SetValue(ViewportProperty, value); } }
        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register("Viewport", typeof(Rect), typeof(BackgroundBrushSelector));
        public Color Color { get { return (Color)GetValue(ColorProperty); } set { SetValue(ColorProperty, value); } }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(BackgroundBrushSelector),new PropertyMetadata(Colors.White));
        public ImageSource Image { get { return (ImageSource)GetValue(ImageProperty); } set { SetValue(ImageProperty, value); } }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(BackgroundBrushSelector));
        public ScaleType Scale { get { return (ScaleType)GetValue(ScaleProperty); } set { SetValue(ScaleProperty, value); } }
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(ScaleType), typeof(BackgroundBrushSelector));
        public Brush Value { get { return (Brush)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Brush), typeof(BackgroundBrushSelector), new PropertyMetadata(Brushes.White, (d, e) => { (d as BackgroundBrushSelector).OnValueChanged(); }));
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(BackgroundBrushSelector));        

        public BackgroundBrushSelector() {
            brushes = G.Background;
            InitializeComponent();            
            root.DataContext = this;
            this.SetBinding(ColorProperty, "Color", brushes, null, BindingMode.TwoWay);
            this.SetBinding(ImageProperty, "Image", brushes, null, BindingMode.TwoWay);
            this.SetBinding(ScaleProperty, "Scale", brushes, null, BindingMode.TwoWay);
            this.SetBinding(ViewportProperty, "Viewport", brushes, null, BindingMode.TwoWay);
            brushes.ValueChanged += (d, e) => { Value = brushes.Value; };
        }

        private void OnValueChanged() {
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent, this));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
        }

        private void btnBackgroundImage_Click(object sender, RoutedEventArgs e) {
            Button button = sender as Button;

            //set new imagesource via OpenFileDiaLog.
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "All Supported|*.jpg;*.jpeg;*.bmp;*.dib;*.png;*.gif|JPEG file|*.jpg|BITMAP file|*.bmp;*.dib|PNG file|*.png";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == true) {
                string imageFilename = dlg.FileName;
                try {
                    ImageSource imgsrc = new BitmapImage(new Uri(imageFilename));
                    Image = imgsrc;
                }catch{
                    Image = null;
                }
            }
            //Update(e);
        }
    }
}
