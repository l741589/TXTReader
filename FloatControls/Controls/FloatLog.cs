using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace FloatControls.Controls {
    public class FloatLog : FloatMessage {
        private readonly Storyboard action = Res.Instance["anim_in_delay_fadeout"] as Storyboard;

        public FloatLog()
        {
            ValueChanged += FloatLog_ValueChanged;
            Name = "日志";
        }

        void FloatLog_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e) {
            BeginStoryboard(action, HandoffBehavior.Compose);
        }
    }
}
