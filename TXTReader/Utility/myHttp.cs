using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Diagnostics;
using System.Net.Mime;
using System.Web;
using System.Threading.Tasks;

namespace TXTReader.Utility {
    class MyHttp : IHttpDelegate {
        public const int successCode = 1000;
        public const int missArgumentsCode = 1001;
        public const int databaseErrorCode = 1002;
        public const int invalidUsernameCode = 1003;
        public const int usernameExistCode = 1004;
        public const int notExitsUsernameCode = 1005;
        public const int wrongPasswordCode = 1006;
        public const int notLoginCode = 1007;
        public const int uploadErrorCode = 1008;
        public const int notExistBookCode = 1009;
        public const int unknownError = 9999;

        public static readonly ResponseEntity UnknownErrorEntity = new ResponseEntity { status = unknownError, msg = "Unknown Error" };

        private const long MAX_FILE_LENGTH = 16*1024*1000; //上传文件的大小上限
        private CookieContainer currentCookie; //用来保存用户登录成功后的cookie，登出后清空，再次登录会更新
        private string url; //服务器的开始url
        private const String S_USERAGENT = "Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0";
        //private const String S_USERAGENT = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
        private ResponseEntity R(ResponseEntity r) { return r != null ? r : UnknownErrorEntity; }

        public MyHttp(string url) {
            this.url = url;
            currentCookie = new CookieContainer();
        }

        public HttpDelegateEvent BeforeRequest {
            get {
                return (h) => {
                    h.Request.UserAgent = S_USERAGENT;
                    h.Request.CookieContainer = new CookieContainer();
                    //h.Request.Accept = "Accept-Language:zh-cn;Accept-Charset:utf-8;";
                    h.Request.Headers.Add("Accept-Charset", "utf-8"); 
                    h.Request.CookieContainer = currentCookie;
                    h.Request.ServicePoint.Expect100Continue = false;
                    //h.Request.KeepAlive = true;
                };
            }
        }

        public HttpDelegateEvent AfterRequest { get { return null; } }

        public HttpDelegateEvent AfterResponse {
            get {
                return (h) => {
                    if (h.Response.Cookies != null && h.Response.Cookies.Count != 0) {
                        foreach (Cookie e in h.Response.Cookies)
                            currentCookie.Add(e);
                    }
                };
            }
        }
       
        public HttpDelegateEvent BeforeResponse { get { return null; } }

        public async Task<ResponseEntity> SignUp(string username, string password) {
            string sendStr = "username=" + username + "&password=" + password;
            string signupUrl = url + "/signup";
            string tempStr = await Http.Create(signupUrl)
                .Add("username", username)
                .Add("password", password)
                .Add("password_comfirmation", password)
                .PostAsync();
            return R(Json.Decode<ResponseEntity>(tempStr));
        }

        public async Task<ResponseEntity> Login(string username, string password) {
            string sendStr = "username=" + username + "&password=" + password;
            string loginUrl = url + "/login";
            string tempStr = await Http.Create(loginUrl).SetHttpDelegate(this).Add("username", username).Add("password", password).PostAsync();
            return R(Json.Decode<ResponseEntity>(tempStr));
        }

        public async Task<ResponseEntity> Logout() {
            string loginUrl = url + "/logout";
            String tempStr = await Http.Create(loginUrl).SetHttpDelegate(this).SetContentType(null).Add("").PostAsync();
            currentCookie = new CookieContainer();
            return R(Json.Decode<ResponseEntity>(tempStr));
        }

        public async Task<ResponseEntity> Search(string bookname, string searchType) {
            var ret = R(Json.Decode<ResponseEntity>(
                    await Http.Create(url + "/search?own=0&book_name=" + HttpUtility.UrlEncode(bookname)).SetHttpDelegate(this).GetAsync()
                ));
            if (ret.status == 1010) return new ResponseEntity { status = successCode, msg = "success", data = new String[0][] };
            return ret;
        }

        public ResponseEntity Download(string bookid) {
            String title = null;
            byte[] res = Http.Create(url + "/download?id=" + bookid).SetHttpDelegate(this)
                .Do((h) => {
                    h.AfterResponse = (hh) => {
                        if (hh.Response.ContentType == "\"text/plain\"") {
                            //var header = Encoding.GetEncoding(String.IsNullOrEmpty(hh.Response.CharacterSet) ? "iso-8859-1" : hh.Response.CharacterSet).GetString(hh.Response.Headers.ToByteArray());
                            var header = Encoding.UTF8.GetString(hh.Response.Headers.ToByteArray());
                            var headers = header.Split('\n', '\r');
                            foreach (String str in headers) {
                                if (str.StartsWith("Content-Disposition:")) {
                                    title = str;
                                    title=title.Substring(title.IndexOf("filename=") + "filename=".Length).Trim('"');
                                }
                            }
                        }
                    };
                })
                .GetBytes();
            if (title == null) {
                if (res == null) return new ResponseEntity { status = unknownError, msg = "connection is broken" };
                return R(Json.Decode<ResponseEntity>(Encoding.UTF8.GetString(res)));
            } else return new ResponseEntity {
                status = successCode,
                msg = "download success",
                data = new object[] { title, res }
            };
        }

        public ResponseEntity Upload(string poststr, string filepath) {
            string boundary = "---------------------ad7d9fdf30d1a8";
            StringBuilder sb = new StringBuilder();
            sb.Append("--" + boundary + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"" + "userfile\";" + "filename=\"" + poststr + "\"\r\n");//"username=\""+username+ 
            sb.Append("Content-Type:text/plain" + "\r\n");
            sb.Append("\r\n");
            
            byte[] bsHeader = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] bsTail = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            byte[] body = File.ReadAllBytes(filepath);
            if (body.Length > MAX_FILE_LENGTH) return UnknownErrorEntity;
            byte[] md5 = Encoding.UTF8.GetBytes("MD5=" + A.MD5(body)+"\r\n");

            String res=Http.Create(url + "/upload").SetHttpDelegate(this)
                .SetContentType("multipart/form-data;boundary=" + boundary)
                .Add(bsHeader).Add(body).Add(bsTail)
                .Post();
            return R(Json.Decode<ResponseEntity>(res));
        }
    }
}