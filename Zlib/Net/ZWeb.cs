using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Input;
using Zlib.Async;
using System.ComponentModel;
using System.Net.Mime;
using System.Threading;

namespace Zlib.Net {
    
    public class ZWeb : WebClient  {

        private CookieContainer cookieContainer = new CookieContainer();
        public String Url { get; set; }
        public String ContentType { get; set; }
        public String UserAgent { get; set; }
        public bool AllowAutoRedirect { get; set; }
        public bool IsKeepCookie { get; set; }
        public int Timeout { get; set; }
        public static List<string> UserAgents = new List<string>();

        static ZWeb() {
            InitUserAgent();
        }

        public ZWeb(){
            AllowAutoRedirect = true;
            IsKeepCookie = false;
            Timeout = int.MaxValue;
            ContentType = "application/x-www-form-urlencoded";
        }

        public static void Lock() {
            Monitor.Enter(Instance);
        }

        public static void ReleaseLock() {
            Monitor.Exit(Instance);
        }

        private static ZWeb instance = null;
        public static ZWeb Instance {
            get {
                if (instance==null) instance = new ZWeb();
                return instance;
            }
        }

        private void Create(){
            
        }

        protected override WebRequest GetWebRequest(Uri address) {
            var wr = base.GetWebRequest(address); ;
            if (wr is HttpWebRequest) {
                var hwr=(wr as HttpWebRequest);
                if (IsKeepCookie) hwr.CookieContainer = cookieContainer;
                if (Timeout!=int.MaxValue) hwr.Timeout = Timeout;
                if (ContentType != null) hwr.ContentType = ContentType;
                if (UserAgent != null) {
                    if (UserAgent.ToLower() == "auto") hwr.UserAgent = RandomUserAgent();
                    hwr.UserAgent = UserAgent;
                }                
                hwr.AllowAutoRedirect = AllowAutoRedirect;
            }
            return wr;
        }

        public CookieContainer Cookies {
            get {
                if (this.cookieContainer == null) this.cookieContainer = new CookieContainer();
                return this.cookieContainer; 
            }
            set { this.cookieContainer = value; }
        }

        public static ZWeb Get(String url) {
            ZWeb w = new ZWeb();
            w.Url = url;            
            return w;
        }

        public static ZWeb CreateWithBase(String url) {
            ZWeb w = Instance;
            w.Url = w.BaseAddress + url;
            return w;
        }

        public static ZWeb CreateNew() {
            ZWeb w = new ZWeb();
            return w;            
        }

        private static void InitUserAgent() {            
            UserAgents.Add("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727)");
            UserAgents.Add("Mozilla/4.0 (compatible; MSIE 8.0; AOL 9.5; AOLBuild 4337.43; Windows NT 6.0; Trident/4.0; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 3.5.30729; .NET CLR 3.0.30618)");
            UserAgents.Add("Mozilla/4.0 (compatible; MSIE 7.0; AOL 9.5; AOLBuild 4337.34; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.5.30729; .NET CLR 3.0.30618)");
            UserAgents.Add("Mozilla/5.0 (X11; U; Linux i686; pl-PL; rv:1.9.0.2) Gecko/20121223 Ubuntu/9.25 (jaunty) Firefox/3.8");
            UserAgents.Add("Mozilla/5.0 (Windows; U; Windows NT 5.1; ja; rv:1.9.2a1pre) Gecko/20090402 Firefox/3.6a1pre (.NET CLR 3.5.30729)");
            UserAgents.Add("Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.1b4) Gecko/20090423 Firefox/3.5b4 GTB5 (.NET CLR 3.5.30729)");
            UserAgents.Add("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Avant Browser; .NET CLR 2.0.50727; MAXTHON 2.0)");
            UserAgents.Add("Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; Media Center PC 6.0; InfoPath.2; MS-RTC LM 8)");
            UserAgents.Add("Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; WOW64; Trident/4.0; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; InfoPath.2; .NET CLR 3.5.21022; .NET CLR 3.5.30729; .NET CLR 3.0.30618)");
            UserAgents.Add("Mozilla/4.0 (compatible; MSIE 7.0b; Windows NT 6.0)");
            UserAgents.Add("Mozilla/4.0 (compatible; MSIE 7.0b; Windows NT 5.1; Media Center PC 3.0; .NET CLR 1.0.3705; .NET CLR 1.1.4322; .NET CLR 2.0.50727; InfoPath.1)");
            UserAgents.Add("Opera/9.70 (Linux i686 ; U; zh-cn) Presto/2.2.0");
            UserAgents.Add("Opera 9.7 (Windows NT 5.2; U; en)");
            UserAgents.Add("Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.8pre) Gecko/20070928 Firefox/2.0.0.7 Navigator/9.0RC1");
            UserAgents.Add("Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.8.1.7pre) Gecko/20070815 Firefox/2.0.0.6 Navigator/9.0b3");
            UserAgents.Add("Mozilla/5.0 (Windows; U; Windows NT 5.1; en) AppleWebKit/526.9 (KHTML, like Gecko) Version/4.0dp1 Safari/526.8");
            UserAgents.Add("Mozilla/5.0 (Windows; U; Windows NT 6.0; ru-RU) AppleWebKit/528.16 (KHTML, like Gecko) Version/4.0 Safari/528.16");
            UserAgents.Add("Opera/9.64 (X11; Linux x86_64; U; en) Presto/2.1.1");
        }
        private String RandomUserAgent(){
            Random r = new Random();
            return UserAgents[r.Next(0, UserAgents.Count)];
        }

 


        public UrlBuilder<ZWeb> BuildUrl(){
            return new UrlBuilder<ZWeb>(Url,this);
        }

        public ZEventTask DownloadFileTaskAsync(String url, String filename, DownloadProgressChangedEventHandler progressChanged) {
            //AsyncCompletedEventHandler holder = null;
            if (progressChanged != null) DownloadProgressChanged += progressChanged;
            //var et = new ZEventTask(zet => DownloadFileCompleted += holder = (d, e) => zet.SetResultAndContinue(null));
            //
            //et.AfterGetResult = zet => {
            //    DownloadFileCompleted -= holder;
            //    if (progressChanged != null) DownloadProgressChanged -= progressChanged;
            //};
            //DownloadFileCompleted
            //var et=ZEventTask.Wait<AsyncCompletedEventArgs>(this, "DownloadFileCompleted");
            ZEventTask et = null;
            DownloadFileAsync(new Uri(url), filename);            
            return et;
        }

        public ZEventTask<byte[]> UploadFileTaskAsync(String url, String filename, UploadProgressChangedEventHandler progressChanged) {
            //UploadFileCompletedEventHandler holder = null;
            if (progressChanged != null) UploadProgressChanged += progressChanged;
            //var et = new ZEventTask<byte[]>(zet => UploadFileCompleted += holder = (d, e) => zet.SetResultAndContinue(e.Result));
            //
            //et.AfterGetResult = zet => {
            //    UploadFileCompleted -= holder;
            //    if (progressChanged != null) UploadProgressChanged -= progressChanged;
            //};
            //var et = ZEventTask.Wait<UploadFileCompletedEventArgs, byte[]>(this, "UploadFileCompleted", (d, e) => e.EventArgs.Result);
            ZEventTask<byte[]> et = null;
            UploadFileAsync(new Uri(BaseAddress + url), filename);
            return et;
        }

        public ZEventTask<byte[]> UploadEntityTaskAsync(String url, MimeEntity entity, UploadProgressChangedEventHandler progressChanged) {
            //UploadDataCompletedEventHandler holder = null;
            if (progressChanged != null) UploadProgressChanged += progressChanged;
            //var et = new ZEventTask<byte[]>(zet => UploadDataCompleted += holder = (d, e) => zet.SetResultAndContinue(e.Result));
            //
            //et.AfterGetResult = zet => {
            //    UploadDataCompleted -= holder;
            //    if (progressChanged != null) UploadProgressChanged -= progressChanged;
            //};
            //var et = ZEventTask.Wait<UploadDataCompletedEventArgs, byte[]>(this, "UploadDataCompleted", (d, e) => e.EventArgs.Result);
            ZEventTask<byte[]> et = null;
            if (!(entity is MultipartEntity))
                entity = new MultipartEntity(entity);
            var data = entity.Data;
            Headers[HttpRequestHeader.ContentType] = entity.ContentType.ToString();
            UploadDataAsync(new Uri(BaseAddress + url), data);
            return et;
        }

        public ZEventTask<byte[]> UploadFileTaskAsync(String url, String name, String filename, UploadProgressChangedEventHandler progressChanged) {
            var fe = new FileEntity(filename, name);
            return UploadEntityTaskAsync(url, fe, progressChanged);
        }

        
    }
}
