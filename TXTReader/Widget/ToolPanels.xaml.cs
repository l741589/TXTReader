﻿using System;
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
using System.Diagnostics;

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
            (tab.Items[0] as Control).Focus();
            Loaded += ToolPanels_Loaded;
        }

        void ToolPanels_Loaded(object sender, RoutedEventArgs e) {

        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {            
            base.OnMouseDown(e);
            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {            
            base.OnMouseUp(e);
            e.Handled = true;
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            if (e.Key == Key.LeftShift) e.Handled = true;
            base.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            if (e.Key == Key.LeftShift) e.Handled = true;
            base.OnKeyDown(e);
        }

        public void Show() {
            ActionUtil.Run(this, toolPanelShow);
            G.Timer.Pause();
        }

        public void Hide() {
            ActionUtil.Run(this, toolPanelHide);
            if (!G.MainWindow.IsHolding) G.Timer.Resume();
            G.Displayer.Focus();
        }
    }
}
