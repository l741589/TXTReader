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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using TXTReader.Utility;

namespace TXTReader.Widget
{
    /// <summary>
    /// ToolPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ToolPanels : UserControl
    {
        private readonly Storyboard toolPanelShow;
        private readonly Storyboard toolPanelHide;
      
        public ToolPanels()
        {
            InitializeComponent();
            toolPanelShow = Resources["toolPanelShow"] as Storyboard;
            toolPanelHide = Resources["toolPanelHide"] as Storyboard;
        }

        private void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            OpenFileDialog f = new OpenFileDialog();
            f.ShowDialog();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            e.Handled = true;
            base.OnMouseDown(e);
        }

        public void Show() {
            ActionUtil.Run(this, toolPanelShow);
            G.Timer.Pause();
        }

        public void Hide() {
            ActionUtil.Run(this, toolPanelHide);
            G.Timer.Resume();
            G.Displayer.Focus();
        }
    }
}
