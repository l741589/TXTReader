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
using TRContent;
using Zlib.Utility;
using TXTReader.Interfaces;
using TXTReader;

namespace TRContent {
    public delegate void ContentSelectedItemChangedEventHandler(object sender, ContentSelectedItemChangedEventArgs e);
    public class ContentSelectedItemChangedEventArgs : RoutedEventArgs {
        public ContentSelectedItemChangedEventArgs(object source) : base(ContentTreePanel.SelectedItemChangedEvent, source) { }
        public IContentItemAdapter Item { get; set; }
    }

    /// <summary>
    /// ContentTreePanel.xaml 的交互逻辑
    /// </summary>
    public partial class ContentTreePanel : UserControl
    {

        private static ContentTreePanel instance = null;
        public static ContentTreePanel Instance {
            get {
                if (instance == null) instance = new ContentTreePanel();
                return instance;
            }
        }

        public static readonly RoutedEvent SelectedItemChangedEvent = EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Bubble, typeof(ContentSelectedItemChangedEventHandler), typeof(ContentTreePanel));
        public event ContentSelectedItemChangedEventHandler SelectedItemChanged { add { AddHandler(SelectedItemChangedEvent, value); } remove { RemoveHandler(SelectedItemChangedEvent, value); } }

        //需要倒转依赖
        public ContentTreePanel()
        {
            InitializeComponent();
            Loaded += (d, e) => { UpdateContentUI(G.Book as IContentAdapter); };
            G.BookChanged += (d, e) => {
                UpdateContentUI(e.NewBook as IContentAdapter);
                if (e.NewBook != null) {
                    e.NewBook.LoadFinished += delegate { UpdateContentUI(e.NewBook as IContentAdapter); };
                }
            };
            
            //G.EmptyBook.Loaded += (d, e) => {
            //    UpdateContentUI();
            //    if (d.NotNull())
            //        (d as IBook).LoadFinished += (dd, ee) => UpdateContentUI();
            //};
            //if (G.Book.NotNull()) {
            //    UpdateContentUI();
            //    G.Book.LoadFinished += (dd, ee) => UpdateContentUI();
            //}
        }

        private void OnSelectedItemChanged(IContentItemAdapter item){
            RaiseEvent(new ContentSelectedItemChangedEventArgs(this){Item=item});
        }

        private void trvContent_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IContentItemAdapter selectedContent = e.NewValue as IContentItemAdapter;
            if (selectedContent != null) {
                OnSelectedItemChanged(selectedContent);
                //if (G.Book.NotNull()) {
                //    G.Book.Position = selectedContent.AbsolutePosition;
                //    G.Book.Offset = 0;
                //}
            }
        }

        //There are some problems of setting binding to a static class in XAML.
        //(mainly because a binding breaks() when the binding source is set to null.)
        //So, [CALL THIS METHOD] when Utility.Book.I is changed.
        public void UpdateContentUI(IContentAdapter apdater) {
            if (apdater.NotNull()) {
                txbTitle.Text = apdater.Title;
                txbLength.Text = apdater.Length.ToString();
                trvContent.ItemsSource = null;
                trvContent.Items.Clear();
                trvContent.ItemsSource = apdater.Children;
            } else {
                trvContent.ItemsSource = null;
                trvContent.Items.Clear();
                txbTitle.Text = "-";
                txbLength.Text = "-";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateContentUI(G.Book as IContentAdapter);
        }
    }
}
