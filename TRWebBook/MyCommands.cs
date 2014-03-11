using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TXTReader;

namespace TRWebBook {
    public static class MyCommands {

        private static InputGestureCollection g(params InputGesture[] ig) {
            var igc = new InputGestureCollection();
            foreach (var i in ig) igc.Add(i);
            return igc;
        }

        //private static readonly RoutedUICommand search = new RoutedUICommand("在线搜索小说", "Search", typeof(MyCommands), g(new KeyGesture(Key.S, ModifierKeys.Control)));
        private static readonly RoutedUICommand search = G.CommandManager.Get("Search", "在线搜索小说", "Search", typeof(MyCommands), g(new KeyGesture(Key.S, ModifierKeys.Control)));

        public static RoutedUICommand Search { get { return search; } }

        private static readonly RoutedUICommand reopen = G.CommandManager.Get("Reopen", "重开", "Reopen", typeof(MyCommands));

        public static RoutedUICommand Reopen { get { return reopen; } }
    }
}
