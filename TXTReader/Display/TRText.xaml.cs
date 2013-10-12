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
using System.Diagnostics;
using TXTReader.Data;

namespace TXTReader.Display {
    /// <summary>
    /// TRText.xaml 的交互逻辑
    /// </summary>
    public partial class TRText : TextBlock {
        
        public TRText() {
            InitializeComponent();
        }

        public TRText(IDisplayer displayer, int index) {
            InitializeComponent();
            Displayer = displayer;
            Index = index;
            Text = Displayer.Text[index];            
        }

        public TRText Previous { get; set; }
        public TRText Next { get; set; }
        public IDisplayer Displayer { get; set; }
        public int Index { get; set; }

        public TRText Relocate() {
            if (Index == Displayer.FirstLine) {
                Canvas.SetTop(this, Displayer.Offset);
            }
            if (Index >= Displayer.FirstLine) {
                LocateByPrevious();
                if (Next == null && Canvas.GetTop(this)+ ActualHeight + Options.Instance.Skin.ParaSpacing < Displayer.CanvasHeight) Append();
                if (Next != null) Next.Relocate();
            }
            if (Index <= Displayer.FirstLine) {
                LocateByNext();
                if (Previous == null && Displayer.Offset - Options.Instance.Skin.ParaSpacing > 0) Prepend();
                if (Previous != null) {
                    var toReturn = Previous.Relocate();
                    Displayer.Offset -= Options.Instance.Skin.ParaSpacing + Previous.ActualHeight;
                    return toReturn;
                }
            }
            return this;
        }

        public TRText LocateByPrevious() {
            if (Previous==null) return this;
            double top = Canvas.GetBottom(Previous) + Options.Instance.Skin.ParaSpacing;
            if (top > Displayer.CanvasHeight) return Remove();
            Canvas.SetTop(this, top);            
            return this;
        }

        public TRText LocateByNext() {
            if (Next == null) return this;
            double bottom = Canvas.GetTop(Next) + Options.Instance.Skin.ParaSpacing;
            if (bottom <0) return Remove();
            Canvas.SetTop(this, bottom);
            return this;
        }

        public TRText Append(){
            if (Index >= Displayer.Text.Length - 1) return null;
            TRText t = new TRText(Displayer, Index + 1);
            t.Next = Next;
            Next = t;
            t.Previous = this;
            ((Canvas)Parent).Children.Add(t);
            if (Index < Displayer.FirstLine) Displayer.FirstLine = Index;
            return LocateByPrevious();
        }

        public TRText Prepend() {
            if (Index <= 0) return null;
            TRText t = new TRText(Displayer, Index - 1);
            t.Previous = Previous;
            Previous = t;
            t.Next = this;
            ((Canvas)Parent).Children.Add(t);
            if (Index < Displayer.FirstLine) Displayer.FirstLine = Index;
            return LocateByNext();
        }

        public TRText Remove() {
            if (Next != null) Next.Previous = Previous;
            if (Previous != null) Previous.Next = Next;
            ((Canvas)Parent).Children.Remove(this);
            return this;
        }
    }
}
