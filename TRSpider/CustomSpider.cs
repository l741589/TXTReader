using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Net;
using Zlib.Text.ZSpiderScript;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TRSpider {
    public class CustomSpider : ISpider {

        public BookDesc Book { get; private set; }
        public ObservableCollection<ChapterDesc> Chapters { get; private set; }
        public ChapterDesc CurrentChapter { get; set; }
        

        private IZSpider zssSpider;


        public CustomSpider(IZSpider zs) {
            zssSpider = zs;
        }

        public string Site { get { return zssSpider.Site; } }

        public string Name { get { return zssSpider.Name; } }

        public int StandardLevel { get { return zssSpider.StandardLevel; } }

        public BookDesc Search(string keyWord) {
            try {
                Book = new BookDesc();
                zssSpider.Execute("search", keyWord, this);
                Chapters = null;
                CurrentChapter = null;
                return Book;
            } catch (ZSSRuntimeException) {
                throw;
            } catch (Exception e) {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        public ObservableCollection<ChapterDesc> GetContent(string contentUrl) {
            try {
                if (Chapters == null) Chapters = new ObservableCollection<ChapterDesc>();
                else return Chapters;
                Chapters.Clear();
                CurrentChapter = new ChapterDesc();
                zssSpider.Execute("content", contentUrl, this);
                return Chapters;
            } catch (ZSSRuntimeException) {
                Chapters = null;
                throw;
            } catch (Exception e) {
                Debug.WriteLine(e.StackTrace);
                Chapters = null;
                return null;
            }
        }

        public string GetText(string textUrl) {
            try {
                return zssSpider.Execute("text", textUrl, this);
            } catch (ZSSRuntimeException) {
                throw;
            } catch (Exception e) {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        //public ChapterDesc GetChapter(string contentUrl, string keyWord) {
        //    try {
        //        if (Chapters == null) GetContent(contentUrl);
        //        if (Chapters == null) return null;
        //        ChapterDesc cd = null;
        //        int min = 0x7fffffff;
        //        foreach (var e in Chapters) {
        //            var lv = Zlib.Algorithm.StringCompare.LevenshteinDistance(e.Title, keyWord);
        //            if (lv < min) {
        //                min = lv;
        //                cd = e;
        //            }
        //        }
        //        do {
        //            if (min > cd.Title.Length / 2){
        //                var len=Zlib.Algorithm.StringCompare.LongestCommonSubsequenceLength(cd.Title.ToArray(),keyWord.ToArray());
        //                if (len < Math.Min(cd.Title.Length, keyWord.Length) * 0.8) break;
        //            }
        //            return cd;
        //        } while(false);
        //        
        //        DebugWindow.Log(String.Format("匹配失败[{0}]\t[{1}]", keyWord, cd.Title));
        //        return null;
        //    } catch (ZSSRuntimeException) {
        //        throw;
        //    } catch (Exception e) {
        //        Debug.WriteLine(e.StackTrace);
        //        return null;
        //    }
        //}

        public ChapterDesc GetLatestChapter(BookDesc entry) {
            try {
                CurrentChapter = new ChapterDesc();
                zssSpider.Execute("latest", entry.ContentUrl + "\0" + entry.EntryUrl, this);
                return CurrentChapter;
            } catch (ZSSRuntimeException) {
                throw;
            } catch (Exception e) {
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }
    }
}
