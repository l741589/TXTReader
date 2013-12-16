using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TXTReader.Data;
using TXTReader.Utility;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace TXTReader.WebApi {
    class Douban {
        public const String URL_SEARCH = "http://api.douban.com/book/subjects?q={0}&max-results=1";
        public static readonly Regex R_TOTALRESULTS = new Regex(@"<opensearch:totalResults>(?<R>[^<]*)</opensearch:totalResults>");
        public static readonly Regex R_COVER = new Regex(@"<link href=""(?<R>[^""]*)"" rel=""image""/>");
        public static readonly Regex R_AUTHOR = new Regex(@"<db:attribute name=""author"">(?<R>[^<]*)</db:attribute>");

        public static async void MoreInfo(Book b){
            await MoreInfoAsync(b);
        }

        public static async Task MoreInfoAsync(Book b) {
            if (b.Cover != null&&b.Cover!=G.NoCover && b.Author != null) return;
            String url = String.Format(URL_SEARCH, b.Title);
            /*网络最优策略*/String xml = await Http.Create(url).GetAsync();
            //*单机最优策略*/String xml = await Task<String>.Run(() => { return Http.Create(url).Get(); });
            //*最不给力策略*/String xml = await Task<String>.Run(async () => { return await Http.Create(url).GetAsync(); });            
            if (xml == null) return;
            try {
                if (int.Parse(R_TOTALRESULTS.Match(xml).Groups["R"].ToString()) > 0) {
                    if (b.Author==null) b.Author = R_AUTHOR.Match(xml).Groups["R"].ToString();
                    if (b.Cover == null || b.Cover == G.NoCover) {
                        String uri = R_COVER.Match(xml).Groups["R"].ToString();
                        if (uri.StartsWith(G.HTTP_HEAD)) {
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
    }
}
