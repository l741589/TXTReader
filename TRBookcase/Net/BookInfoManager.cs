using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Async;
using Zlib.Utility;
using TRSpider;
using Zlib.Net;
using System.IO;
using System.Windows.Media.Imaging;
using Zlib.Algorithm;
using TXTReader.Interfaces;

namespace TRBookcase.Net {
    static class BookInfoManager {
        private static ZTask infoTask = new ZTask();
        private static ZTask coverTask = new ZTask();

        public static bool IsNeedMoreInfo(IBook b) {
            if (b.IsNull()) return false;
            if (b.Title.IsNullOrWhiteSpace()) return false;
            if (b.Author.IsNullOrWhiteSpace()) return true;
            if (b.Cover == G.NO_COVER) return true;
            return false;
        }

        public static async void MoreInfo(IBook b) {
            
            if (!SpiderCollection.Instance.IsLoaded) {
                //EventHandler holder=null;
                //var et = new ZEventTask(zet => SpiderCollection.Instance.Loaded += holder = (d, e) => zet.SetResultAndContinue(null));
                //et.AfterGetResult = zet => SpiderCollection.Instance.Loaded -= holder;
                //await et;
                await ZEventTask.Wait(SpiderCollection.Instance, "Loaded", r => r.Handler = new EventHandler((d, e) => { r.Continue(); }));
            }
            String title = b.Title;
            foreach (var e in SpiderCollection.Instance) {
                if (!IsNeedMoreInfo(b)) break;
                BookDesc cd = (BookDesc)await infoTask.Run(() => {
                    try {
                        return e.Search(title);
                    } catch { }
                        return null;
                });
                if (cd == null) continue;
                if (!cd.Author.IsNullOrWhiteSpace()) {
                    b.Author = cd.Author;
                }
                if (!cd.CoverUrl.IsNullOrWhiteSpace()) {                    
                    b.Cover = cd.CoverUrl;
                }
            }
            //b.Update();
        }
    }
}
