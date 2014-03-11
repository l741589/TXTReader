using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Zlib.Utility;

namespace TRSpider {
    /// <summary>
    /// ChapterDisplayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChapterDisplayWindow : Window {
        private String input;
        public ChapterDisplayWindow(String text) {
            InitializeComponent();
            if (text==null||text.StartsWith(TRZSS.ERROR_HEADER)) tb.Text = "";
            else tb.Text = text;
            input = tb.Text;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (tb.Text.IsNullOrWhiteSpace() || tb.Text == input) return;
            switch (MessageBox.Show(this, "章节内容已更改，是否保存？", "保存", MessageBoxButton.YesNoCancel)) {
                case MessageBoxResult.Yes: DialogResult = true; break;
                case MessageBoxResult.No: DialogResult = false; break;
                case MessageBoxResult.Cancel: DialogResult = null; e.Cancel = true; break;
            }
        }

    }
}
