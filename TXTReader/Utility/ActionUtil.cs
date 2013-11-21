using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace TXTReader.Utility {
    static class ActionUtil {
        //private static Dictionary<Storyboard, HashSet<FrameworkElement>> ownerRelation = new Dictionary<Storyboard, HashSet<FrameworkElement>>();
        //
        //public static void Run(FrameworkElement e, Storyboard storyboard, bool mutex = true) {
        //    HashSet<FrameworkElement> elem;
        //    if (ownerRelation.TryGetValue(storyboard, out elem)) {
        //        if (mutex&&elem.Contains(e)) return;
        //    } else {
        //        elem = new HashSet<FrameworkElement>();
        //        ownerRelation.Add(storyboard, elem);
        //    }
        //    storyboard.Completed += (s, ev) => { ownerRelation.Remove(storyboard); };
        //    storyboard.Begin(e);
        //    elem.Add(e);
        //    
        //}

        private static HashSet<Storyboard> RunningStoryBoard = new HashSet<Storyboard>();

        public static void Run(FrameworkElement e,Storyboard storyboard, bool mutex = true) {
            if (RunningStoryBoard.Contains(storyboard) && mutex) return;
            RunningStoryBoard.Add(storyboard);
            storyboard.Completed += (s, ev) => { RunningStoryBoard.Remove(storyboard); };
            storyboard.Begin(e);            
        }

        public static void Clear() {
            RunningStoryBoard.Clear();
        }
    }
}
