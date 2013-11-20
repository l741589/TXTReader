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

        public FloatMessagePanel() {
            InitializeComponent();
            Time = new FloatMessage(this);
            Position = new FloatMessage(this) { Format = "{0}L" };
            Fps = new FloatMessage(this) { Format = "{0}Fps" };
            Title = new FloatMessage(this);
            pn_lefttop.Children.Add(Time);
            pn_lefttop.Children.Add(Position);
            pn_lefttop.Children.Add(Fps);
            pn_lefttop.Children.Add(Title);
            timer();
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
            if (G.Book != null) {
                Position.SetBinding(FloatMessage.ValueProperty, new Binding("Position") { Source = G.Book });
                Title.Value = G.Book.Title;
            }
            Fps.SetBinding(FloatMessage.ValueProperty, new Binding("Fps") { Source = G.Displayer });
        }
    }
}
