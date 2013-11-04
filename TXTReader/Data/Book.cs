using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TXTReader.Utility;
using System.Security.Policy;
using System.Text.RegularExpressions;
using TXTReader.WebApi;

namespace TXTReader.Data {
    public class Book : Chapter, ContentAdapter, Positionable {
        public const String NO_PREVIEW = "暂无预览";
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(int), typeof(Book));
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(double), typeof(Book));
        private ImageSource cover = null;
        public bool IsLocal { get { return Source == null ? false : !Source.ToLower().StartsWith(G.HTTP_HEAD); } }
        public ImageSource Cover { get { if (cover == null) return G.NoCover; else return cover; } set { cover = value; } }
        public int Position { get { return (int)GetValue(PositionProperty); } set { SetValue(PositionProperty, value); } }
        public double Offset { get { return (double)GetValue(OffsetProperty); } set { SetValue(OffsetProperty, value); } }
        public String Author { get; set; }
        public String Source { get; set; }
        public DateTime LastLoadTime { get; set; }
        public double SortArgument { get; set; }
        private String preview = null;

        public Book() : base() { Position = 0; Offset = 0; }
        public Book(String src) : this() { Init(src); }

        public void Init(String src) {
            if (Path.GetExtension(src) == G.EXT_BOOK) {
                BookcaseParser.Load(src, this);
            } else {
                Source = src;
                SortArgument = 0;
                if (IsLocal) Title = Path.GetFileNameWithoutExtension(src);
                else {
                    var m = Regex.Match(src, @"(?<[\\/])(?<R>[\w\d_ ]+?)((?=$)|(?=\?)|(?=\.)");
                    var c = m.Groups["R"].Captures;
                    if (c.Count > 0) {
                        Title = c[c.Count - 1].Value;
                    }
                }
                LastLoadTime = default(DateTime);
            }
        }

        public String GetPreview(int? position=null) {
            if (position == null) position = Position;
            if (TotalText == null || TotalText.Count == 0) {
                if (preview != null) return preview;
                return NO_PREVIEW;
            }
            if (position < 0) return preview;
            if (position >= TotalText.Count) return preview;
            int i = position.Value;
            preview = TotalText[i++];
            while (preview.Length < 256 && i < G.Book.TotalText.Count)
                preview += "\n" + TotalText[i++];
            return preview;
        }

        public String Preview {
            get {
                return GetPreview();
            }
            set {
                if (value != null && value != "") preview = value;
            }
        }

        private Chapter Insert(List<String> subtitles, int level, Chapter node) {
            Chapter ret = null;
            if (level < subtitles.Count) {
                ret = Insert(subtitles, level + 1, (Chapter)node[subtitles[level]]);
            } else {
                ret = node;
            }
            return ret;
        }

        public Chapter Match(Trmex trmex, ICollection<String> texts) {
            Chapter node = this;
            foreach (var s in texts) {
                var r = trmex.Match(s);
                if (r != null) {
                    if (trmex.LCs == null) {
                        node = Insert(r.SubTitle, 0, this);
                    } else {
                        var n = node;
                        while (n.Children != null) n = n.Children.Last.Value as Chapter;
                        while (n.Level < r.Level - 1) n = n["未命名章节"];
                        while (n.Level > r.Level - 1 && n != null) n = n.Parent as Chapter;
                        node = n[r.Title];
                    }
                } else {
                    node.AppendText(s);
                }
            }
            return this;
        }

        public void Load(String file = null) {
            if (file == null) file = Source; else Source = file;
            if (IsLocal) {
                Clear();
                var ss = File.ReadAllLines(file, Encoding.Default);
                Title = Path.GetFileNameWithoutExtension(file);
                if (Text != null) Text.Clear();
                Match(G.Trmex, ss);
                BookcaseParser.Load(this);                
            } else {
                //TODO 添加Download逻辑
            }
            LastLoadTime = DateTime.Now;
        }

        public String ToolTip {
            get {
                String ret = "";
                if (Title != null) ret += Title + "\n";
                if (Author != null) ret += "作者：" + Author + "\n";
                if (Length != 0) ret += "长度：" + Length + "字\n";
                //if (Preview != null) ret += "内容：\n" + Preview + "\n";
                return ret.Trim();
            }
        }

        public void MoreInfo() {
            Douban.MoreInfo(this);
        }

        public override void Close() {
            GetPreview();
            BookcaseParser.Save(this);
            base.Close();
        }
    }
}
