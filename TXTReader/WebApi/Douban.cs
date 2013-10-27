using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TXTReader.Data;
using TXTReader.Utility;
using System.Windows.Media;

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
            if (b.Cover != null && b.Author != null) return;
            String url = String.Format(URL_SEARCH, b.Title);
            String xml = await Http.Create(url).Get();
            if (xml == null) return;
            try {
                if (int.Parse(R_TOTALRESULTS.Match(xml).Groups["R"].ToString()) > 0) {
                    b.Author = R_AUTHOR.Match(xml).Groups["R"].ToString();
                    b.Cover = new ImageSourceConverter().ConvertFrom(R_COVER.Match(xml).Groups["R"].ToString()) as ImageSource;
                }
            } catch { }
        }
    }
}
