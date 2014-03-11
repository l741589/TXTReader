using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zlib.Text.ZSpiderScript;
using Zlib.Utility;

namespace TRSpider {
    class TRZSS : ZSS {

        private static TRZSS instance;
        public static TRZSS Instance { get { if (instance == null) instance = new TRZSS(); return instance; } }

        private TRZSS()
            : base() {
            init();
        }

        private String CheckUrl(String baseUrl, String url) {
            if (baseUrl.IsNullOrWhiteSpace()) return url;
            if (url.IsNullOrWhiteSpace()) return url;
            if (url[0] == '/') {
                if (baseUrl.Last() == '/') baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
                return baseUrl + url;
            } else return url;
        }

        private void init() {
            HostVarHandler = (d, e) => {
                var spider = (e.Spider.Tag as CustomSpider);
                if (e.IsSet) {
                    switch (e.Name) {
                        case "chapter": spider.CurrentChapter.Title = e.Value == null ? null : e.Value.Trim(); break;
                        case "texturl": spider.CurrentChapter.TextUrl = e.Value; break;
                        case "preview": spider.CurrentChapter.Preview = e.Value; break;
                        case "chapterid": spider.CurrentChapter.Id = e.Value; break;
                        case "title": spider.Book.Title = e.Value == null ? null : e.Value.Trim(); break;
                        case "author": spider.Book.Author = e.Value == null ? null : e.Value.Trim(); break;
                        case "content": spider.Book.ContentUrl = e.Value; break;
                        case "cover": spider.Book.CoverUrl = e.Value; break;
                        case "entry": spider.Book.EntryUrl = e.Value; break;
                        case "id": spider.Book.Id = e.Value; break;
                        case "description": spider.Book.Description = e.Value; break;
                        case "input": if (DebugWindow.Instance.IsVisible) DebugWindow.Instance.tb_input.Text = e.Value; break;
                    }
                    if (spider != null) {
                        if (spider.CurrentChapter != null) 
                            spider.CurrentChapter.TextUrl = CheckUrl(spider.Site, spider.CurrentChapter.TextUrl);
                        if (spider.Book != null) {
                            spider.Book.ContentUrl = CheckUrl(spider.Site, spider.Book.ContentUrl);
                            spider.Book.EntryUrl = CheckUrl(spider.Site, spider.Book.EntryUrl);
                            spider.Book.CoverUrl = CheckUrl(spider.Site, spider.Book.CoverUrl);
                        }
                    }
                    return null;
                } else {
                    switch (e.Name) {
                        case "chapter": return spider.CurrentChapter.Title.Trim();
                        case "texturl": return spider.CurrentChapter.TextUrl;
                        case "chapterid": return spider.CurrentChapter.Id.Trim();
                        case "preview": return spider.CurrentChapter.Preview.Trim();
                        case "title": return spider.Book.Title.Trim();
                        case "author": return spider.Book.Author.Trim();
                        case "content": return spider.Book.ContentUrl;
                        case "cover": return spider.Book.CoverUrl;
                        case "entry": return spider.Book.EntryUrl;
                        case "id": return spider.Book.Id.Trim();
                        case "description": return spider.Book.Description.Trim();
                        case "input": if (DebugWindow.Instance.IsVisible) return DebugWindow.Instance.tb_input.Text; else return null;
                        default: return null;
                    }
                }
            };

            NodeCreator = (d, e) => {
                switch (e.CommandDesc.Command) {
                    case "putchapter": return new PutChapterCommand(e.Parent, e.CommandDesc);
                    default: return null;
                }
            };

            CommandEquality.Add("put", "putchapter");

            Logger = (d, e) => DebugWindow.Log(e.Text);
        }

        public CustomSpider Load(String[] lines) {
            var sp = LoadStrings(lines);
            return new CustomSpider(sp);
        }

        public CustomSpider Load(String file) {
            var sp = LoadFile(file);
            return new CustomSpider(sp);
        }

        class PutChapterCommand : ZSSCommand {
            public override string Command {
                get { return "putchapter"; }
            }

            public PutChapterCommand(ParentCommand parent, CommandDesc cd) :
                base(parent, cd) { }

            public override string DoExecute(string input) {
                var cs = (Root.Tag as CustomSpider);
                cs.Chapters.Add(cs.CurrentChapter);
                cs.CurrentChapter = new ChapterDesc();
                return null;
            }
        }
    }
}
