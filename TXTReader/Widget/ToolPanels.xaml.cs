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
using Microsoft.Win32;

namespace TXTReader.Widget
{
    /// <summary>
    /// ToolPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ToolPanels : UserControl
    {
        public ToolPanels()
        {
            InitializeComponent();
        }

        private void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            OpenFileDialog f = new OpenFileDialog();
            f.ShowDialog();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            e.Handled = true;
            base.OnMouseDown(e);
        }
    }
}
