using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Zlib.UI;

namespace TRWebBook {
    public partial class Res : ResourceDictionary{

        public Res() {
            InitializeComponent();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) {
#if DEBUG
            var s = InputBox.Show("请输入要搜索的书名", "TXTReader", "星辰变");
            if (s == null) return;
#else
            var s = InputBox.Show("请输入要搜索的书名");
            if (s == null) return;
#endif
            var bsw=new BookSelectWindow(s);
            bsw.ShowDialog();
        }        
    }
}
