using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace TRBook {
    /// <summary>
    /// ContentOptionPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ContentOptionPanel : UserControl {
       
        public ContentOptionPanel() {
            InitializeComponent();
            seMaxLen.ValueChanged += seMaxLen_ValueChanged;
            seMinLen.ValueChanged += seMinLen_ValueChanged;
            seMaxLen.Value = Chapter.MaxChapterLength;
            seMinLen.Value = Chapter.MinChapterLength;
        }

        void seMinLen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e) {
            Chapter.MinChapterLength = e.NewValue;
        }

        void seMaxLen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e) {
            Chapter.MaxChapterLength = e.NewValue;
        }
    }
}
