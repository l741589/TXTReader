using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;
using Zlib.Text;
using Zlib.Text.Xml;
using Zlib.Utility;

namespace TRSpider {

    [XmlEntity]
    public class ChapterDescEx : ChapterDesc, INotifyPropertyChanged {
       
        public enum States { Ready, Pending, Fail, Success };
        public enum StandardStates { Ready, Pending, Fail, Success, FailAll, NonStandardSuccess }
        [XmlIgnore]
        public ChapterDescEx Standard { get; set; }
        public String Text { get; set; }
        private String mtext = null;
        public String ManualText { get { return mtext; } set { mtext = value; IsManual = !mtext.IsNullOrWhiteSpace(); } }
        public bool IsManual { get { return _IsManual; } set { _IsManual = value; OnPropertyChanged("IsManual"); } } private bool _IsManual;      
        private States state;
        public States State {
            get { return state; }
            set {
                state = value;
                if (Standard != null) {
                    switch (state) {
                        case States.Ready: if (Standard == this) Standard.StandardState = StandardStates.Ready; break;
                        case States.Pending: Standard.StandardState = StandardStates.Pending; break;
                        case States.Success: if (Standard == this) Standard.StandardState = StandardStates.Success;
                            else Standard.StandardState = StandardStates.NonStandardSuccess;break;
                        case States.Fail: Standard.StandardState = StandardStates.Fail; break;
                    }
                }
            }
        }
        public StandardStates StandardState { get { return _StandardStates; } set { _StandardStates = value; OnPropertyChanged("StandardState"); } } private StandardStates _StandardStates;

        public ChapterDescEx(ChapterDesc cd) {
            Title = cd.Title;
            TextUrl = cd.TextUrl;
            Id = cd.Id;
            Preview = cd.Preview;
            State = States.Ready;
        }

        public ChapterDescEx() {
            State = States.Ready;
        }

        public override object Clone() {
            return MemberwiseClone();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String prop) {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class SpiderClosure : ICloneable, INotifyPropertyChanged {

        public ISpider Spider { get; private set; }
        public BookDesc BookDesc { get; set; }
        public List<ChapterDescEx> Chapters { get; private set; }
        public bool IsFailed { get { return _IsFailed; } private set { _IsFailed = value; OnPropertyChanged("IsFailed"); UpdateBrush();} } private bool _IsFailed;      
        public bool IsStandard { get { return _IsStandard; } set { _IsStandard = value; OnPropertyChanged("IsStandard"); UpdateBrush();} } private bool _IsStandard;
        public bool IsInUse { get { return _IsInUse; } set { _IsInUse = value; OnPropertyChanged("IsInUse"); } } private bool _IsInUse;
        public Brush Border { get { return _Border; } set { _Border=value; OnPropertyChanged("Border"); } } private Brush _Border;

        private void UpdateBrush(){
            if (IsFailed) { Border = Brushes.Red; return; }
            if (IsStandard) { Border = Brushes.Lime; return; }
            Border=null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String prop) {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void Reset() {
            Chapters = null;
            IsFailed = BookDesc == null;
        }

        public SpiderClosure(ISpider spider) {
            this.Spider = spider;
            IsFailed = true;
            IsStandard = false;
            IsInUse = false;
        }

        public override string ToString() {
            return BookDesc.Title + " - " + BookDesc.Author + "  [" + Name + "]";
        }

        public object ToolTip {
            get {
                StringBuilder sb = new StringBuilder();
                if (!BookDesc.Title.IsNullOrWhiteSpace()) sb.Append("书名：").Append(BookDesc.Title).Append("\r\n");
                if (!BookDesc.Author.IsNullOrWhiteSpace()) sb.Append("作者：").Append(BookDesc.Author).Append("\r\n");
                if (!BookDesc.Description.IsNullOrWhiteSpace()) sb.Append("简介：").Append(BookDesc.Description).Append("\r\n");
                if (Spider != null) {
                    if (!Spider.Name.IsNullOrWhiteSpace()) sb.Append("来自：").Append(Spider.Name).Append("\r\n");
                    sb.Append("标准等级：").Append(Spider.StandardLevel).Append("\r\n");
                }
                return sb.ToString().Trim();
            }
        }

        public string Site {
            get { return Spider.Site; }
        }

        public string Name {
            get { return Spider.Name; }
        }

        public int StandardLevel {
            get { return Spider.StandardLevel; }
        }

        public BookDesc Search(string keyWord) {
            try {
                IsFailed = false;
                if (BookDesc != null) return BookDesc;                                
                return BookDesc = Spider.Search(keyWord);
            } catch {
                IsFailed = true;
                return null;
            }
        }

        public List<ChapterDescEx> GetContent(string contentUrl) {
            if (IsFailed) return null;
            try {
                IsFailed = false;
                if (Chapters != null) return Chapters;                
                Chapters = new List<ChapterDescEx>();
                var cs = Spider.GetContent(contentUrl);
                foreach (var e in cs) Chapters.Add(new ChapterDescEx(e));
                return Chapters;
            } catch {
                IsFailed = true;
                throw;
            }
        }

        public void AddChapter(ChapterDescEx chapter){
            if (Chapters==null) Chapters=new List<ChapterDescEx>();
            Chapters.Add(chapter);
        }

        public ChapterDescEx GetText(ChapterDescEx cd) {
            if (IsFailed) {
                if (cd!=null) cd.State = ChapterDescEx.States.Fail;
                return cd;
            }
            cd.State = ChapterDescEx.States.Pending;
            try {
                var s = Spider.GetText(cd.TextUrl);
                if (s.IsNullOrEmpty() || s.StartsWith(TRZSS.ERROR_HEADER)) {
                    cd.State = ChapterDescEx.States.Fail;
                    cd.Text = s.IsNullOrEmpty() ? TRZSS.ERROR_HEADER : s;
                    return cd;
                }
                cd.Text = s;
                cd.State = ChapterDescEx.States.Success;
                return cd;
            } catch(Exception e) {
                cd.State = ChapterDescEx.States.Fail;
                cd.Text = TRZSS.ERROR_HEADER + e.Message;
                return cd;
            }
        }

        //public ChapterDescEx GetChapter(string keyWord) {
        //    if (IsFailed) return null;
        //    try {
        //         var r=Spider.GetChapter(BookDesc.ContentUrl, keyWord);
        //        if (r==null) return null;
        //        return new ChapterDescEx(r);
        //    } catch {
        //        return null;
        //    }
        //}

        public ChapterDesc GetLatestChapter(BookDesc entry) {
            if (IsFailed) return null;
            return Spider.GetLatestChapter(entry);
        }

        public object Clone() {
            var o = MemberwiseClone() as SpiderClosure;
            o.BookDesc = BookDesc == null ? null : BookDesc.Clone() as BookDesc;
            if (Chapters != null) {
                o.Chapters = new List<ChapterDescEx>();
                foreach (var e in Chapters)
                    o.Chapters.Add(e.Clone() as ChapterDescEx);
            }
            return o;
        }
    }
}
