using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Zlib.Converter;
using Zlib.Utility;
using Zlib.Text;
using System.Windows.Media.Imaging;

namespace Zlib.UI.Utility {

    public enum ScaleType { 
        Center = 0, 
        Left = 1,
        Right = 2,
        Top = 4, 
        Bottom = 8,
        Uniform = 16,
        Tile = 64,
        None = 255,
        StretchX = Left | Right, 
        StretchY = Top | Bottom,
        Stretch = StretchX | StretchY,
        UniformX = Uniform | StretchX,
        UniformY = Uniform | StretchY,
        UniformToFill = UniformX | UniformY,
    };

    public class ScaleTypeMask{
        public const ScaleType NULL = ScaleType.Center;
        public const ScaleType POSITION = ScaleType.UniformToFill;
        public const ScaleType HORIZONTAL = ScaleType.StretchX;
        public const ScaleType VERTICAL = ScaleType.StretchY;
        public static ScaleType UNIFORM(ScaleType sc) {
            var ret=sc;
            if ((ret & VERTICAL) != VERTICAL) ret &= ~VERTICAL;
            if ((ret & HORIZONTAL) != HORIZONTAL) ret &= ~HORIZONTAL;
            return ret;
        }
    }
    
    public class ZBrushes : DependencyObject, IXmlParsable{

        public Rect Viewport { get { return (Rect)GetValue(ViewportProperty); } set { SetValue(ViewportProperty, value); } }
        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register("Viewport", typeof(Rect), typeof(ZBrushes));

        public Color Color { get { return (Color)GetValue(ColorProperty); } set { SetValue(ColorProperty, value); } }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ZBrushes),new PropertyMetadata(Colors.White));
        public ImageSource Image { get { return (ImageSource)GetValue(ImageProperty); } set { SetValue(ImageProperty, value); } }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ZBrushes));
        public TransformGroup Transform { get { return (TransformGroup)GetValue(TransformProperty); } set { SetValue(TransformProperty, value); } }
        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(TransformGroup), typeof(ZBrushes));
        public ScaleType Scale { get { return (ScaleType)GetValue(ScaleProperty); } set { SetValue(ScaleProperty, value); } }
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(ScaleType), typeof(ZBrushes));
        public Rect ImageViewport { get { return (Rect)GetValue(ImageViewportProperty.DependencyProperty); } private set { SetValue(ImageViewportProperty, value); } }
        public static readonly DependencyPropertyKey ImageViewportProperty = DependencyProperty.RegisterReadOnly("ImageViewport", typeof(Rect), typeof(ZBrushes), new PropertyMetadata(new Rect(0,0,1,1)));
        public Brush Value { get { return (Brush)GetValue(ValueProperty.DependencyProperty); } private set { SetValue(ValueProperty, value); } }
        public static readonly DependencyPropertyKey ValueProperty = DependencyProperty.RegisterReadOnly("Value", typeof(Brush), typeof(ZBrushes), new PropertyMetadata(null, 
            (d, e) => {
            var o = d as ZBrushes;
            if (o.ValueChanged != null) o.ValueChanged(d, EventArgs.Empty);
        }));
        public event EventHandler ValueChanged;
        public TileMode TileMode { get { return (TileMode)GetValue(TileModeProperty.DependencyProperty); } set { SetValue(TileModeProperty, value); } }
        public static readonly DependencyPropertyKey TileModeProperty = DependencyProperty.RegisterReadOnly("TileMode", typeof(TileMode), typeof(ZBrushes), new PropertyMetadata(TileMode.None));


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
            if (e.Property == ScaleProperty || e.Property == ImageProperty || e.Property == ViewportProperty) {
                Value = GenerateValue();
            }
        }

        Brush GenerateValue() {
            switch (Scale) {
                case ScaleType.None: return SolidColor;
                case ScaleType.Uniform: return Uniform;
                case ScaleType.UniformToFill: return UniformToFill;
                case ScaleType.Center: return Center;
                case ScaleType.Stretch: return Stretch;
                default: break;
            }
            Rect r = new Rect();
            if (Image == null) return Stretch;
            if (Viewport == null) return Stretch;
            switch (ScaleTypeMask.UNIFORM(Scale)) {
                case ScaleType.UniformX: r.Width = Viewport.Width; r.Height = Viewport.Width * Image.Height / Image.Width; break;
                case ScaleType.UniformY: r.Width = Viewport.Height * Image.Width / Image.Height; r.Height = Viewport.Height; break;
                case ScaleType.Uniform:
                    if (Viewport.Width < Viewport.Height) goto case ScaleType.UniformX;
                    else goto case ScaleType.UniformY;
                case ScaleType.UniformToFill:
                    if (Viewport.Width > Viewport.Height) goto case ScaleType.UniformX;
                    else goto case ScaleType.UniformY;
                case ScaleType.Stretch: r = Viewport; break;
            }

            if ((Scale & ScaleType.Stretch) < ScaleType.Stretch) {
                if (r.Width == 0) r.Width = Image.Width;
                if (r.Height == 0) r.Height = Image.Height; 
                switch (Scale & ScaleTypeMask.HORIZONTAL) {
                    case ScaleType.StretchX: r.Width = Viewport.Width; break;
                    case ScaleType.Left: r.Offset(0, 0); goto default;
                    case ScaleType.Right: r.Offset(Viewport.Width - r.Width, 0); goto default;
                    case ScaleType.Center: r.Offset((Viewport.Width - r.Width) / 2, 0); goto default;
                    default: break;
                }

                switch (Scale & ScaleTypeMask.VERTICAL) {
                    case ScaleType.StretchY: r.Height = Viewport.Height; break;
                    case ScaleType.Top: r.Offset(0, 0); goto default;
                    case ScaleType.Bottom: r.Offset(0, Viewport.Height - r.Height); goto default;
                    case ScaleType.Center: r.Offset(0, (Viewport.Height - r.Height) / 2); goto default;
                    default: break;
                }
            }
            ImageViewport = r;
            if ((Scale & ScaleType.Tile) == ScaleTypeMask.NULL) TileMode = TileMode.None;
            else TileMode = TileMode.Tile;
            return DefaultValue;
        }


        public Brush None { get { return null; } }

        private Brush tile = null;
        public Brush Tile {
            get {
                if (tile != null) return tile;
                var b = new ImageBrush();
                b.SetBinding(ImageBrush.ImageSourceProperty, "Image", this);
                b.Stretch = System.Windows.Media.Stretch.None;
                b.TileMode = TileMode.Tile;
                b.ViewportUnits = BrushMappingMode.Absolute;
                b.SetBinding(ImageBrush.ViewportProperty, "ImageSource", b, new ImageSourceRectConverter());
                b.SetBinding(Brush.TransformProperty, "Transform", this);
                return tile = b;
            }
        }

        private Brush solidColor = null;
        public Brush SolidColor {
            get {
                if (solidColor != null) return solidColor;
                var b = new SolidColorBrush();
                b.SetBinding(SolidColorBrush.ColorProperty, "Color", this);
                return solidColor = b;
            }
        }

        private Brush center=null;
        public Brush Center {
            get {
                if (center != null) return center;
                var grp = new DrawingGroup();                

                var bg = new GeometryDrawing();
                bg.Brush = new SolidColorBrush();
                bg.Brush.SetBinding(SolidColorBrush.ColorProperty, "Color", this);
                bg.Geometry = new RectangleGeometry();
                bg.Geometry.SetBinding(RectangleGeometry.RectProperty, "Viewport", this);
                grp.Children.Add(bg);

                var img = new GeometryDrawing();
                var imgb = new ImageBrush();
                imgb.Stretch = System.Windows.Media.Stretch.None;
                imgb.TileMode = TileMode.None;
                imgb.SetBinding(ImageBrush.ImageSourceProperty, "Image", this);
                img.Brush = imgb;                
                img.Geometry = new RectangleGeometry();
                img.Geometry.SetBinding(RectangleGeometry.RectProperty, "Viewport", this);
                grp.Children.Add(img);

                var b = new DrawingBrush();                
                b.Drawing = grp;
                return center = b;
            }
        }

        private Brush uniform = null;
        public Brush Uniform {
            get {
                if (uniform != null) return uniform;
                var grp = new DrawingGroup();

                var bg = new GeometryDrawing();
                bg.Brush = new SolidColorBrush();
                bg.Brush.SetBinding(SolidColorBrush.ColorProperty, "Color", this);
                bg.Geometry = new RectangleGeometry();
                bg.Geometry.SetBinding(RectangleGeometry.RectProperty, "Viewport", this);
                grp.Children.Add(bg);

                var img = new GeometryDrawing();
                var imgb = new ImageBrush();
                imgb.Stretch = System.Windows.Media.Stretch.Uniform;
                imgb.TileMode = TileMode.None;
                imgb.SetBinding(ImageBrush.ImageSourceProperty, "Image", this);
                img.Brush = imgb;
                img.Geometry = new RectangleGeometry();
                img.Geometry.SetBinding(RectangleGeometry.RectProperty, "Viewport", this);
                grp.Children.Add(img);

                var b = new DrawingBrush();
                b.Drawing = grp;
                return uniform = b;
            }
        }

        private Brush uniformToFill = null;
        public Brush UniformToFill {
            get {
                if (uniformToFill != null) return uniformToFill;
                var b = new ImageBrush();
                b.SetBinding(ImageBrush.ImageSourceProperty, "Image", this);
                b.Stretch = System.Windows.Media.Stretch.UniformToFill;
                return uniformToFill = b;
            }
        }

        private Brush stretch = null;
        public Brush Stretch {
            get {
                if (stretch != null) return stretch;
                var b = new ImageBrush();
                b.SetBinding(ImageBrush.ImageSourceProperty, "Image", this);
                return stretch = b;
            }
        }

        private Brush defaultValue = null;
        public Brush DefaultValue {
            get {
                if (defaultValue != null) return defaultValue;
                var grp = new DrawingGroup();

                var bg = new GeometryDrawing();
                bg.Brush = new SolidColorBrush();
                bg.Brush.SetBinding(SolidColorBrush.ColorProperty, "Color", this);
                bg.Geometry = new RectangleGeometry();
                bg.Geometry.SetBinding(RectangleGeometry.RectProperty, "Viewport", this);
                grp.Children.Add(bg);

                var img = new GeometryDrawing();
                var imgb = new ImageBrush();
                imgb.Stretch = System.Windows.Media.Stretch.Fill;
                imgb.SetBinding(ImageBrush.TileModeProperty, "TileMode", this);
                imgb.SetBinding(ImageBrush.ImageSourceProperty, "Image", this);
                imgb.ViewportUnits = BrushMappingMode.Absolute;
                imgb.SetBinding(ImageBrush.ViewportProperty, "ImageViewport", this);
                img.Brush = imgb;
                img.Geometry = new RectangleGeometry();
                img.Geometry.SetBinding(RectangleGeometry.RectProperty, "Viewport", this);
                grp.Children.Add(img);

                var b = new DrawingBrush();
                b.Drawing = grp;
                return defaultValue = b;
            }
        }

        public ZBrushes() {
            Value = DefaultValue;
        }


        private const String S_ZBRUSH = "ZBrush";
        private const String S_SCALE = "scale";
        private const String S_COLOR = "color";

        private const String S_IMAGE = "image";
        public XmlParserReaderCallback Read {
            get {
                return r => r.Child(S_ZBRUSH, new Version("1.0.0.0"))
                    .Read(S_SCALE, n => Scale = (ScaleType)Enum.Parse(typeof(ScaleType), n.InnerText))
                    .Read(S_COLOR, n => Color = (Color)ColorConverter.ConvertFromString(n.InnerText))
                    .Read(S_IMAGE, n => Image = new BitmapImage(new Uri(n.InnerText)))
                    .Parent;
            }
        }

        public XmlParserWriterCallback Write {
            get {
                return w => w.Start(S_ZBRUSH).Ver(new Version("1.0.0.0"))
                    .Write(S_SCALE, Scale)
                    .Write(S_COLOR, Color)
                    .Write(S_IMAGE, Image)
                    .End;
            }
        }
    }
}
