using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Mime;

namespace Zlib.Net {
    public delegate void MaintainHttpAction(Http request);
    public class Http :IHttpDelegate,IDisposable {
        private String path;
        private Dictionary<String, String> args;
        private MemoryStream ms;
        private HttpWebRequest request;
        private HttpWebResponse response;
        private Encoding resEncoding = Encoding.UTF8;
        private Encoding reqEncoding = Encoding.UTF8;
        private String contentType = "application/x-www-form-urlencoded";
        private static bool doRefuse = false;
        public static bool IsDoRefuse { get; set; }
        private static async void ResetDoRefuse() { await Task.Run(() => { Thread.Sleep(5000); }); doRefuse = false; }
        private static bool DoRefuse { get { return doRefuse && IsDoRefuse; } set { if (!doRefuse && value) ResetDoRefuse(); doRefuse = value; } }

        public String Url { get { return path; } }
        public HttpDelegateEvent AfterRequest { get; set; }
        public HttpDelegateEvent AfterResponse { get; set; }
        public HttpDelegateEvent BeforeRequest { get; set; }
        public HttpDelegateEvent BeforeResponse { get; set; }
        public HttpWebRequest Request { get { return request; } }
        public HttpWebResponse Response { get { return response; } }

        public static HttpDelegate SharedHttpDelegate { get; private set; }
        public IHttpDelegate PrivateHttpDelegate { get; set; }

        static Http() {
            IsDoRefuse = false;
            SharedHttpDelegate = new HttpDelegate();
        }

        
        public void OnBeforeRequest(Http http) {
            if (SharedHttpDelegate.BeforeRequest!=null) SharedHttpDelegate.BeforeRequest(http);
            if (BeforeRequest != null) BeforeRequest(http);
            if (PrivateHttpDelegate != null && PrivateHttpDelegate.BeforeRequest != null) PrivateHttpDelegate.BeforeRequest(http);
        }
        public void OnAfterRequest(Http http) {
            if (SharedHttpDelegate.AfterRequest != null) SharedHttpDelegate.AfterRequest(http);
            if (AfterRequest != null) AfterRequest(http);
            if (PrivateHttpDelegate != null && PrivateHttpDelegate.AfterRequest != null) PrivateHttpDelegate.AfterRequest(http);
        }
        public void OnBeforeResponse(Http http) {
            if (SharedHttpDelegate.BeforeResponse != null) SharedHttpDelegate.BeforeResponse(http);
            if (BeforeResponse != null) BeforeResponse(http);
            if (PrivateHttpDelegate != null && PrivateHttpDelegate.BeforeResponse != null) PrivateHttpDelegate.BeforeResponse(http);
        }
        public void OnAfterResponse(Http http) {
            if (SharedHttpDelegate.AfterResponse != null) SharedHttpDelegate.AfterResponse(http);
            if (AfterResponse != null) AfterResponse(http);
            if (PrivateHttpDelegate != null && PrivateHttpDelegate.AfterResponse != null) PrivateHttpDelegate.AfterResponse(http);
        }


        public static Http Create(String path) {
            Http r = new Http();
            r.path = path;
            HttpWebRequest request = null;
            if (path.StartsWith("http")) request = WebRequest.Create(path) as HttpWebRequest;
            r.request = request;
            return r;
        }

        public Http Add(String key, String value) {
            if (args == null) args = new Dictionary<string, string>();
            args.Add(key, value);
            return this;
        }

        public Http AddCookie(Cookie cookie) {
            if (request.CookieContainer == null) request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookie);
            return this;
        }

        public Http Add(byte[] data) {
            if (ms == null) ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            return this;
        }

        public Http Add(String str) {
            if (ms == null) ms = new MemoryStream();
            var data = reqEncoding.GetBytes(str);
            ms.Write(data, 0, data.Length);
            return this;
        }

        public Http Do(MaintainHttpAction action) {
            if (action != null) action(this);
            return this;
        }

        public Http SetContentType(String contentType) {
            this.contentType = contentType;
            return this;
        }

        public Http AddHeader(HttpRequestHeader key,String value) {
            request.Headers.Add(key, value);
            return this;
        }

        public Http AddHeader(HttpResponseHeader key, String value) {
            request.Headers.Add(key, value);
            return this;
        }

        public Http SetRequestEncoding(Encoding encoding) {
            reqEncoding = encoding;
            return this;
        }

        public Http SetResponseEncoding(Encoding encoding) {
            resEncoding = encoding;
            return this;
        }

        public Http SetHttpDelegate(IHttpDelegate httpDeleagate) {
            PrivateHttpDelegate = httpDeleagate;
            return this;
        }

        private String BuildArgs(Dictionary<String, String> args) {
            if (args == null) return "";
            String s = "";
            Boolean start = true;
            Dictionary<String, String>.KeyCollection keys = args.Keys;
            foreach (String key in keys) {
                if (start) start = false;
                else s += "&";
                s += key + "=" + args[key];
            }
            return s;
        }

        #region ASYNC

        public async Task<String> PostAsync() {
            if (DoRefuse) return null;
            try {
                request.Method = "POST";
                if (args != null) await RequestStringAsync(args);
                else await RequestBytesAsync(ms);
                return await ResponseStringAsync();
            } catch (Exception) {
                //MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                DoRefuse = true;
                return null;
            }
        }

        public async Task<String> GetAsync() {
            if (DoRefuse) return null;
            try {
                request.Method = "GET";
                if (args != null) await RequestStringAsync(args);
                else await RequestBytesAsync(ms);
                return await ResponseStringAsync();
            } catch (Exception) {
                //MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                DoRefuse = true;
                return null;
            }
        }

        public async Task<byte[]> PostBytesAsync() {
            if (DoRefuse) return null;
            try {
                request.Method = "POST";
                if (args != null) await RequestStringAsync(args);
                else await RequestBytesAsync(ms);
                return await ResponseBytesAsync();
            } catch (Exception) {
                //MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                DoRefuse = true;
                return null;
            }
        }

        public async Task<byte[]> GetBytesAsync() {
            if (DoRefuse) return null;
            try {
                request.Method = "GET";
                if (args != null) await RequestStringAsync(args);
                else await RequestBytesAsync(ms);
                return await ResponseBytesAsync();
            } catch (Exception) {
                //MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                DoRefuse = true;
                return null;
            }
        }

        public async Task RequestStringAsync(Dictionary<String, String> args = null) {
            OnBeforeRequest(this);
            string postData = BuildArgs(args);
            byte[] byteArray = reqEncoding.GetBytes(postData);
            request.ContentType = contentType;
            request.ContentLength = byteArray.Length;
            Stream dataStream = await request.GetRequestStreamAsync();
            await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
            dataStream.Close();
            OnAfterRequest(this);
        }

        public async Task RequestBytesAsync(MemoryStream data) {
            OnBeforeRequest(this);
            if (data != null) {
                byte[] byteArray = data.ToArray();
                request.ContentType = contentType;
                request.ContentLength = byteArray.Length;               
                Stream dataStream = await request.GetRequestStreamAsync();
                await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
                dataStream.Close();                
            }
            OnAfterRequest(this);
        }

        public async Task<String> ResponseStringAsync() {
            response = (await request.GetResponseAsync()) as HttpWebResponse;
            OnBeforeResponse(this);            
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream, resEncoding);
            string responseFromServer = await reader.ReadToEndAsync();
            reader.Close();
            dataStream.Close();            
            OnAfterResponse(this);
            response.Close();
            return responseFromServer;            
        }

        public async Task<byte[]> ResponseBytesAsync() {
            response = (await request.GetResponseAsync()) as HttpWebResponse;
            OnBeforeResponse(this);
            Stream dataStream = response.GetResponseStream();
            MemoryStream reader = new MemoryStream();
            int num;
            byte[] buffer = new byte[0x400];
            do {
                num = await dataStream.ReadAsync(buffer, 0, buffer.Length);
                if (num > 0)
                    await reader.WriteAsync(buffer, 0, num);
            } while (num > 0);
            reader.Close();
            dataStream.Close();            
            OnAfterResponse(this);
            response.Close();
            return reader.ToArray();
        }
        #endregion

        #region SYNC
        public String Post() {
            if (DoRefuse) return null;
            try {
                request.Method = "POST";              
                if (args != null) RequestString(args);
                else RequestBytes(ms);
                return ResponseString();
            } catch (Exception) {
                //MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                DoRefuse = true;
                return null;
            }
        }

        public String Get() {
            if (DoRefuse) return null;
            try {
                request.Method = "GET";
                if (args != null) RequestString(args);
                else RequestBytes(ms);
                return ResponseString();
            } catch (Exception) {
                //MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                DoRefuse = true;
                return null;
            }
        }

        public byte[] PostBytes() {
            if (DoRefuse) return null;
            try {
                request.Method = "POST";
                if (args != null) RequestString(args);
                else RequestBytes(ms);
                return ResponseBytes();
            } catch (Exception) {
                //MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                DoRefuse = true;
                return null;
            }
        }

        public byte[] GetBytes() {
            if (DoRefuse) return null;
            try {
                request.Method = "GET";
                if (args != null) RequestString(args);
                else RequestBytes(ms);
                return ResponseBytes();
            } catch (Exception) {
                //MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                DoRefuse = true;
                return null;
            }
        }

        public void RequestString(Dictionary<String, String> args = null) {
            OnBeforeRequest(this);
            string postData = BuildArgs(args);
            byte[] byteArray = reqEncoding.GetBytes(postData);
            request.ContentType = contentType;
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            OnAfterRequest(this);
        }

        public void RequestBytes(MemoryStream data) {
            OnBeforeRequest(this);
            if (data != null) {
                byte[] byteArray = data.ToArray();
                request.ContentType = contentType;
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();                
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();                
            }
            OnAfterRequest(this);
        }

        public String ResponseString() {
            response = request.GetResponse() as HttpWebResponse;
            OnBeforeResponse(this);
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream, resEncoding);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            //dataStream.Close();
            OnAfterResponse(this);
            response.Close();
            return responseFromServer;            
        }

        public byte[] ResponseBytes() {
            response = request.GetResponse() as HttpWebResponse;
            OnBeforeResponse(this);
            Stream dataStream = response.GetResponseStream();
            MemoryStream reader = new MemoryStream();
            int num;
            byte[] buffer = new byte[0x400];
            do {
                num = dataStream.Read(buffer, 0, buffer.Length);
                if (num > 0)
                    reader.Write(buffer, 0, num);
            } while (num > 0);
            reader.Close();
            dataStream.Close();            
            OnAfterResponse(this);
            response.Close();
            return reader.ToArray();
        }
        #endregion      
    
        public void Dispose() {
            if (response != null) response.Dispose();
        }
    }
}
