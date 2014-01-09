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
using Zlib.Utility;

namespace TRBook
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
            Book.Empty.Loaded += (d, e) => {
                UpdateContentUI();
                if (d.NotNull())
                    (d as Book).LoadFinished += (dd, ee) => UpdateContentUI();
            };
            if (Book.I.NotNull()) {
                UpdateContentUI();
                Book.I.LoadFinished += (dd, ee) => UpdateContentUI();
            }
        }

        private void trvContent_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ContentItemAdapter selectedContent = e.NewValue as ContentItemAdapter;
            if (selectedContent != null) {
                if (Book.I.NotNull()) {
                    Book.I.Position = selectedContent.AbsolutePosition;
                    Book.I.Offset = 0;
                }
            }
        }

        //There are some problems of setting binding to a static class in XAML.
        //(mainly because a binding breaks() when the binding source is set to null.)
        //So, [CALL THIS METHOD] when Utility.Book.I is changed.
        public void UpdateContentUI() {
            if (Book.I != Book.Empty) {
                txbTitle.Text = Book.I.Title;
                txbLength.Text = Book.I.Length.ToString();
                trvContent.ItemsSource = null;
                trvContent.Items.Clear();
                trvContent.ItemsSource = Book.I.Children;
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
