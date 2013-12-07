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

namespace TXTReader.Widget
{
    /// <summary>
    /// ContentTreePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ContentTreePanel : UserControl
    {
        public ContentTreePanel()
        {
            InitializeComponent();
        }

        private void trvContent_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Data.ContentAdapter selectedContent = sender as Data.ContentAdapter;
            if (selectedContent != null)
            {

            }
        }

        //There are some problems of setting binding to a static class in XAML.
        //(mainly because a binding breaks() when the binding source is set to null.)
        //So, [CALL THIS METHOD] when Utility.G.Book is changed.
        public void UpdateContentUI()
        {
            txbTitle.SetBinding(TextBlock.TextProperty, new Binding("Title") { Source = Utility.G.Book });
            txbLength.SetBinding(TextBlock.TextProperty, new Binding("Length") { Source = Utility.G.Book });
            trvContent.SetBinding(TreeView.ItemsSourceProperty, new Binding("Children") { Source = Utility.G.Book });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateContentUI();
        }
    }
}
