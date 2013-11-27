using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TXTReader.Commands {
    public static class MyCommands {

        private static InputGestureCollection g(params InputGesture[] ig) {
            var igc = new InputGestureCollection();
            foreach (var i in ig) igc.Add(i);
            return igc;
        }

        private static readonly RoutedUICommand sizeable = new RoutedUICommand("可缩放", "Sizeable", typeof(MyCommands));
        private static readonly RoutedUICommand titleBar = new RoutedUICommand("标题栏", "TitleBar", typeof(MyCommands));
        private static readonly RoutedUICommand fullScreen = new RoutedUICommand("全屏", "FullScreen", typeof(MyCommands));
        private static readonly RoutedUICommand reopen = new RoutedUICommand("重开", "Reopen", typeof(MyCommands));
        private static readonly RoutedUICommand exit = new RoutedUICommand("退出", "Exit", typeof(MyCommands), g(new KeyGesture(Key.X, ModifierKeys.Alt)));
        private static readonly RoutedUICommand bossKey = new RoutedUICommand("隐藏到托盘", "BossKey", typeof(MyCommands), g(new KeyGesture(Key.T, ModifierKeys.Control), new KeyGesture(Key.Escape)));

        public static RoutedUICommand Sizeable { get { return sizeable; } }
        public static RoutedUICommand TitleBar { get { return titleBar; } }
        public static RoutedUICommand FullScreen { get { return fullScreen; } }
        public static RoutedUICommand Reopen { get { return reopen; } }
        public static RoutedUICommand Exit { get { return exit; } }
        public static RoutedUICommand BossKey { get { return bossKey; } }
    }
}
