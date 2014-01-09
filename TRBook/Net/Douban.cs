using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Zlib.Async;
using Zlib.Net;
using Zlib.Algorithm;

namespace TRBook.Net {
    class Douban {
        public const String URL_SEARCH = "http://api.douban.com/book/subjects?q={0}&max-results=1";
        public static readonly Regex R_TOTALRESULTS = new Regex(@"<opensearch:totalResults>(?<R>[^<]*)</opensearch:totalResults>");
        public static readonly Regex R_COVER = new Regex(@"<link href=""(?<R>[^""]*)"" rel=""image""/>");
        public static readonly Regex R_AUTHOR = new Regex(@"<db:attribute name=""author"">(?<R>[^<]*)</db:attribute>");

        private static ZTask infoTask = new ZTask();
        public static async void MoreInfo(Book b){
            var author =b.Author;
            var title = b.Title;
            var cover = b.Cover;
            var src = b.Source;
            String uri = null;
            await infoTask.Run(() => {
                if (cover != null && cover != G.NO_COVER && author != null) return;
                String url = String.Format(URL_SEARCH, title);
                String xml = Http.Create(url).Get();
                 if (xml == null) return;
                try {
                    if (int.Parse(R_TOTALRESULTS.Match(xml).Groups["R"].ToString()) > 0) {
                        //G.MainWindow.Dispatcher.BeginInvoke(new Action(() => { G.Log = "拉取 "+title+" 信息成功"; }));
                        if (author == null) author = R_AUTHOR.Match(xml).Groups["R"].ToString();
                        if (cover == null || cover == G.NO_COVER) {
                            uri = R_COVER.Match(xml).Groups["R"].ToString();
                            if (uri.StartsWith(A.HTTP_HEAD)) {
                                String path = G.PATH_COVER + title + src.GetHashCode() + Path.GetExtension(uri);
                                if (!File.Exists(path)) {
                                    byte[] bs = Http.Create(uri).GetBytes();
                                    File.WriteAllBytes(path, bs);
                                    //G.MainWindow.Dispatcher.BeginInvoke(new Action(() => { G.Log = "拉取 " + title + " 封面成功"; }));
                                }
                                uri = path;
                            }
                        }
                    }
                } catch { }
            });
            b.Author = author;
            if (uri!=null) b.Cover = new ImageSourceConverter().ConvertFrom(uri) as ImageSource;
        }

        public static async Task MoreInfoAsync(Book b) {
            if (b.Cover != null && b.Cover != G.NO_COVER && b.Author != null) return;
            String url = String.Format(URL_SEARCH, b.Title);
            /*网络最优策略*/String xml = await Http.Create(url).GetAsync();
            //*单机最优策略*/String xml = await Task<String>.Run(() => { return Http.Create(url).Get(); });
            //*最不给力策略*/String xml = await Task<String>.Run(async () => { return await Http.Create(url).GetAsync(); });            
            if (xml == null) return;
            try {
                if (int.Parse(R_TOTALRESULTS.Match(xml).Groups["R"].ToString()) > 0) {
                    if (b.Author==null) b.Author = R_AUTHOR.Match(xml).Groups["R"].ToString();
                    if (b.Cover == null || b.Cover == G.NO_COVER) {
                        String uri = R_COVER.Match(xml).Groups["R"].ToString();
                        if (uri.StartsWith(A.HTTP_HEAD)) {
                            String path = G.PATH_COVER + b.Title + b.Source.GetHashCode() + Path.GetExtension(uri);
                            if (!File.Exists(path)) {
                                byte[] bs = await Http.Create(uri).GetBytesAsync();
                                File.WriteAllBytes(path, bs);
                            }
                            uri = path;
                        }
                        b.Cover = new ImageSourceConverter().ConvertFrom(uri) as ImageSource;
                    }
                }
            } catch { }
        }

        public static void MoreInfoSync(Book b) {
           
        }
    }
}
