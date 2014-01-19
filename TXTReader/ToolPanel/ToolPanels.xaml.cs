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
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace TXTReader.ToolPanel
{
    /// <summary>
    /// ToolPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ToolPanels : UserControl
    {
        private readonly Storyboard toolPanelShow;
        private readonly Storyboard toolPanelHide;

        public bool IsHide { get { return Visibility != Visibility.Visible; } }
      
        public ToolPanels()
        {
            InitializeComponent();
            toolPanelShow = Resources["toolPanelShow"] as Storyboard;
            toolPanelHide = Resources["toolPanelHide"] as Storyboard;
            (tab.Items[0] as Control).Focus();
            Loaded += ToolPanels_Loaded;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            base.OnPreviewKeyDown(e);
            if (IsHide) {
                e.Handled = true;
                Debug.WriteLine("KD Blocked");
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e) {
            base.OnPreviewKeyUp(e);
            if (IsHide) {
                e.Handled = false;
                Debug.WriteLine("KU Blocked");
            }
        }

        void ToolPanels_Loaded(object sender, RoutedEventArgs e) {

        }

        public void Show() {
            Visibility = Visibility.Visible;
            this.BeginStoryboard(toolPanelShow, HandoffBehavior.Compose);
            //G.Timer.Pause();
        }

        public void Hide() {
            toolPanelHide.Completed += (d, e) => { Visibility = Visibility.Collapsed; };
            this.BeginStoryboard(toolPanelHide, HandoffBehavior.Compose);
            //if (!G.MainWindow.IsHolding) G.Timer.Resume();
            if (tab.TabIndex == 0) tab.Focus();
            (G.Displayer as Control).Focus();
        }
    }
}
