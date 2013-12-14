using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using TXTReader.Data;
using TXTReader.Utility;

namespace TXTReader.Widget {
    /// <summary>
    /// FloatMessageManager.xaml 的交互逻辑
    /// </summary>
    public partial class FloatMessagePanel : UserControl {

        public FloatMessage Time;
        public FloatMessage Position;
        public FloatMessage Fps;
        public FloatMessage Title;
        public FloatMessage Speed;

        public FloatMessagePanel() {
            InitializeComponent();
            Time = new FloatMessage(this);
            Position = new FloatMessage(this) { Format = "{0:#0.00%}" };
            Fps = new FloatMessage(this) { Format = "{0}Fps" };
            Speed = new FloatMessage(this) { Format = "SpeedLevel:{0}" };
            Title = new FloatMessage(this);
            pn_lefttop.Children.Add(Time);
            pn_lefttop.Children.Add(Position);
            pn_lefttop.Children.Add(Fps);
            pn_lefttop.Children.Add(Title);
            pn_lefttop.Children.Add(Speed);
            timer();
            UpdateBinding();
        }
        
        async void timer() {
            while (G.IsRunning) {
                Time.tb.Text = DateTime.Now.ToString("hh:mm:ss");
                await Task.Run(() => { Thread.Sleep(100); });
            }
        }

        public void UpdateBinding() {
            BindingOperations.ClearAllBindings(Position.tb);
            BindingOperations.ClearAllBindings(Fps.tb);
            BindingOperations.ClearAllBindings(Speed.tb);
            BindingOperations.ClearAllBindings(Title.tb);
            if (G.Book != null) {
                Position.SetBinding(FloatMessage.ValueProperty, new Binding("Percent") { Source = G.MainWindow.progressBar });
                Title.SetBinding(FloatMessage.ValueProperty, new Binding("CurrentTitle") { Source = G.Book });
            }
            Fps.SetBinding(FloatMessage.ValueProperty, new Binding("Fps") { Source = G.Displayer });
            Speed.SetBinding(FloatMessage.ValueProperty, new Binding("Speed") { Source = G.Displayer });            
        }

        public void UpdateColor(){
            Brush bg = null;
            Brush fg = Brushes.Black;
            if (G.Options.Skin.BackGroundType == BackGroundType.Image) {
                bg = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
                fg = Brushes.Black;
            } else {
                bg = null;
                Color c = G.Options.Skin.BackColor;
                if ((c.R + c.G + c.B) > 128 * 3) fg = Brushes.Black;
                else fg = Brushes.White;                
            }
            Time.Background = Speed.Background = Fps.Background = Title.Background = Position.Background = bg;
            Time.Foreground = Speed.Foreground = Fps.Foreground = Title.Foreground = Position.Foreground = fg;
        }
    }
}
