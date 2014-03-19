using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TXTReader {
    public static class MyCommands {

        private static InputGestureCollection g(params InputGesture[] ig) {
            var igc = new InputGestureCollection();
            foreach (var i in ig) igc.Add(i);
            return igc;
        }

        private static readonly RoutedUICommand reopen = G.CommandManager.Get("Reopen", "重开", "Reopen", typeof(MyCommands));
        
        public static RoutedUICommand Reopen { get { return reopen; } }
    }
}
