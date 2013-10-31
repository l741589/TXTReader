using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TXTReader.Utility;

namespace TXTReader.Data
{
    enum MatchType { NoMatch, List, Tree, Both };
    enum MatchLang { Trmex, Regex };
    public class Chapter : DependencyObject,ContentItemAdapter
    {
        public string Title { get; set; }
        public List<String> Text { get;  set; }
        public LinkedList<ContentItemAdapter> Children { get; private set; }
        public ContentItemAdapter Parent { get; private set; }
        private List<String> totalText = null;
        private int len = 0;

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
        }

        public ContentStatus ContentStatus {
            get { throw new NotImplementedException(); }
        }

        
        public string TotalTitle {
            get {
                if (Level == 0) return null;
                if (Parent != null) {
                    if (Level == 1) return Title;
                    else return Parent.TotalTitle + " " + Title;
                }
                return null;
            }
        }
        public int Level { get { if (Parent == null) return 0; else return Parent.Level + 1; } }

        public int Length {
            get {
                if (len != 0) return len;
                len = 0;
                if(Text!=null)
                    foreach (var e in Text) len += e.Length;
                if (Children != null)
                    foreach (var e in Children) len += e.Length;
                return len;
            }
            set { len = value; }
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
                if (Children == null) Children = new LinkedList<ContentItemAdapter>();
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
            len = 0;
            if (Children != null) {
                foreach (var e in Children) (e as Chapter).Clear();
                Children.Clear();
                Children = null;
            }
            G.Bookmark.Clear();
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
    }
}
