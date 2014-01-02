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
using TXTReader.Utility;

namespace TXTReader.Books
{
    /// <summary>
    /// ContentTreePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ContentTreePanel : UserControl
    {
        public ContentTreePanel()
        {
            InitializeComponent();
            Loaded += (d, e) => { UpdateContentUI(); };
            if (G.Book != null) G.Book.LoadFinished += (o,e) => { UpdateContentUI(); };
        }

        private void trvContent_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ContentItemAdapter selectedContent = e.NewValue as ContentItemAdapter;
            if (selectedContent != null)
            {
                G.Displayer.FirstLine = selectedContent.AbsolutePosition;
                G.Displayer.Offset = 0;
                G.Displayer.Update();
            }
        }

        //There are some problems of setting binding to a static class in XAML.
        //(mainly because a binding breaks() when the binding source is set to null.)
        //So, [CALL THIS METHOD] when Utility.G.Book is changed.
        public void UpdateContentUI() {
            if (G.Book != null) {
                txbTitle.Text = G.Book.Title;
                txbLength.Text = G.Book.Length.ToString();
                trvContent.ItemsSource = null;
                trvContent.Items.Clear();
                trvContent.ItemsSource = G.Book.Children;
            } else {
                trvContent.ItemsSource = null;
                trvContent.Items.Clear();
                txbTitle.Text = "-";
                txbLength.Text = "-";

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateContentUI();
        }
    }
}
