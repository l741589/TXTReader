using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace TXTReader.FloatMessages {
    public class FloatLog : FloatMessage {
        private readonly Storyboard action = App.Current.Resources["anim_in_delay_fadeout"] as Storyboard;
   
        public FloatLog(FloatMessagePanel panel) :base(panel){
            ValueChanged += FloatLog_ValueChanged;
        }

        void FloatLog_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e) {
            BeginStoryboard(action, HandoffBehavior.Compose);
        }
    }
}
