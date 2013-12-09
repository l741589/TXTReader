using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TXTReader.Utility;
using System.Diagnostics;
using System.Windows.Threading;

namespace TXTReader.Data
{
    enum MatchType { NoMatch, List, Tree, Both };
    enum MatchLang { Trmex, Regex };
    public class Chapter : DependencyObject, ContentItemAdapter {

        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register("Length", typeof(int), typeof(Chapter), new PropertyMetadata(0));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(String), typeof(Chapter), new PropertyMetadata((d, e) => { var s = (d as Chapter).TotalTitle; }));
        public static readonly DependencyPropertyKey ContentStatusProperty = DependencyProperty.RegisterReadOnly("ContentStatus", typeof(ContentStatus), typeof(Chapter), new PropertyMetadata(ContentStatus.None));
        public static readonly DependencyPropertyKey TotalTitleProperty = DependencyProperty.RegisterReadOnly("TotalTitle", typeof(String), typeof(Chapter), new PropertyMetadata(null));       

        public string Title { get { return (String)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }
        public List<String> Text { get;  set; }
        public ChapterCollection Children { get; private set; }
        public ContentItemAdapter Parent { get; private set; }
        private List<String> totalText = null;
        public int? Number { get; set; }

        public List<String> TotalText {
            get {
                if (totalText != null) return totalText;
                totalText = new List<string>();
                if (TotalTitle!=null&&Text!=null) totalText.Add(TotalTitle);
                if (Text != null) totalText.AddRange(Text);
                if (Children != null) 
                    foreach (var e in Children)
                        if (e.TotalText != null)
                            totalText.AddRange(e.TotalText);
                return totalText;
            }
            protected set {
                totalText = value;
            }
        }

        public int LineCount {
            get {
                if (Text == null) return 0;
                return Text.Count;
            }
        }

        public int TotalLineCount {
            get {
                if (TotalText == null) return 0;
                return TotalText.Count;
            }
        }

        public LinkedListNode<ContentItemAdapter> Node { get; set; }
        private int absolutePosition = -1;
        public int AbsolutePosition {
            get {
                if (absolutePosition == -1) {
                    if (Node == null||Node.Previous==null) {
                        if (Parent == null) absolutePosition = 0;
                        else absolutePosition = Parent.AbsolutePosition + Parent.LineCount;
                    } else {
                        var p = Node.Previous.Value;
                        absolutePosition = p.AbsolutePosition + p.TotalLineCount;
                    }
                }
                return absolutePosition;
            }
        }

        private ContentStatus GetContentStatus() {
            if (Children != null && Children.Count > 0) return ContentStatus.None;
            if (Length >= G.Options.MaxChapterLength) return ContentStatus.TooLong;
            if (Length < G.Options.MinChapterLength) return ContentStatus.TooShort;
            try {
                if (Number == null) return ContentStatus.None;
                if (Node == null || Node.Previous == null) {
                    if (Number == null || Number.Value == 0 || Number.Value == 1) return ContentStatus.None;
                    if (Parent != null && Parent.Node != null) {
                        LinkedListNode<ContentItemAdapter> anyPrev = Parent.Node.Previous;
                        do {
                            if (anyPrev == null) return ContentStatus.ConfusingIndex;
                            while (anyPrev.Value.Number != null) {
                                if (anyPrev.Value.Number + 1 == Number) return ContentStatus.None;
                                if (anyPrev.Value.Children == null || anyPrev.Value.Children.Count == 0) break;
                                anyPrev = anyPrev.Value.Children.Last;
                            }
                            if (anyPrev.Value.Number + 1 == Number) return ContentStatus.None;
                            anyPrev = anyPrev.Value.Parent != null && anyPrev.Value.Parent.Node != null ?
                                anyPrev.Value.Parent.Node.Previous : null;
                        } while (anyPrev != null && Parent != null);
                        if (anyPrev == null) return ContentStatus.ConfusingIndex;
                        if (anyPrev.Value.Number + 1 != Number) return ContentStatus.ConfusingIndex;
                    }
                } else {
                    if (Node.Previous.Value.Number == Number) return ContentStatus.LowLevelConfusingIndex;
                    if (Node.Previous.Value.Number + 1 != Number) return ContentStatus.ConfusingIndex;
                }
            } catch (Exception e) {
                Debug.WriteLine(e.StackTrace);
            }
            return ContentStatus.None;
        }

        public ContentStatus ContentStatus {
            get {
                var cs = GetContentStatus();
                SetValue(ContentStatusProperty, cs);
                return cs;
            }
        }


        public string TotalTitle {
            get {
                String ret = null;
                if (Level == 0) ret = null;
                else {
                    if (Parent != null) {
                        if (Level == 1) ret = Title;
                        else ret = Parent.TotalTitle + " " + Title;
                    }
                }
                SetValue(TotalTitleProperty, ret);
                return ret;
            }
        }
        public int Level { get { if (Parent == null) return 0; else return Parent.Level + 1; } }

        public int Length {
            get {
                int len=(int)GetValue(LengthProperty);
                if (len != 0) return len;
                len = 0;
                if(Text!=null)
                    foreach (var e in Text) len += e.Length;
                if (Children != null)
                    foreach (var e in Children) len += e.Length;
                SetValue(LengthProperty, len);
                return len;
            }
            set { SetValue(LengthProperty, value); }
        }

        public ContentItemAdapter FindChildByTitle(String title) {
            if (Children==null) return null;
            foreach (var e in Children)
                if (e.Title == title) return e;
            return null;
        }

        public Chapter this[String subtitle] {
            get {
                if (subtitle == null) return this;
                var r = FindChildByTitle(subtitle);
                if (r != null) return r as Chapter;
                Chapter c = new Chapter() { Title = subtitle };
                if (Children == null) Children = new ChapterCollection();
                Children.AddLast(c);
                c.Parent = this;
                return c;
            }
        }       

        public Chapter AppendText(String text){
            if (Text==null) Text=new List<String>();
            Text.Add(text);
            return this;
        }

        public virtual void Clear() {
            Title = null;
            Text = null;
            totalText = null;
            Length = 0;
            if (Children != null) {
                foreach (var e in Children) (e as Chapter).Clear();
                Children.Clear();
                Children = null;
            }
            if (G.Bookmark!=null) G.Bookmark.Clear();
            Parent = null;            
        }

        public virtual void Close() {
            Text = null;
            totalText = null;
            if (Children != null) {
                foreach (var e in Children) (e as Chapter).Close();
                Children.Clear();
                Children = null;
            }
            G.Bookmark.Clear();
            Parent = null;
        }

        public override string ToString() {
            return TotalTitle;
        }

        public virtual void Notify() {
            if (Children != null) {
                Children.Notify();
                foreach (ContentItemAdapter c in Children)
                    c.Notify();
            }
        }

        public void Update(){
            if (Children != null) {
                foreach (Chapter c in Children)
                    c.Update();
            }
            var cs = ContentStatus;
            SetValue(TotalTitleProperty, TotalTitle);
            Length = 0;
            var l = Length;
        }
    }
}
