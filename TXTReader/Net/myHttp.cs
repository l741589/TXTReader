/*
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
using Zlib.Net;
using Zlib.Text;
using Zlib.Utility;
using Zlib.Algorithm;

namespace TXTReader.Net {
    public class MyHttp : IHttpDelegate {
        public Dictionary<int,String[]> errorCodes = new Dictionary<int,String[]>();
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
        public const int fileExceedsLimitCode = 1012;
        public const int fileExceedsFormLimitCode = 1013;
        public const int resultNoFile = 1023;
        public const int unknownError = 9999;

        private void initErrorCodes() {
            errorCodes.Add(1000, new String[] { "success", "操作成功" });
            errorCodes.Add(1001, new String[] { "missing arguments", "参数不全" });
            errorCodes.Add(1002, new String[] { "database error", "数据库错误" });
            errorCodes.Add(1003, new String[] { "invalid username", "用户名不合法" });
            errorCodes.Add(1004, new String[] { "same username", "相同用户名" });
            errorCodes.Add(1005, new String[] { "passwords diffirent", "密码不一致" });
            errorCodes.Add(1006, new String[] { "user not exist", "用户不存在" });
            errorCodes.Add(1007, new String[] { "wrong password", "密码错误" });
            errorCodes.Add(1008, new String[] { "not login", "未登录" });
            errorCodes.Add(1009, new String[] { "cannot loggout", "无法登出" });
            errorCodes.Add(1010, new String[] { "no book matches", "书不存在" });
            errorCodes.Add(1011, new String[] { "file is not selected", "未选择文件" });
            errorCodes.Add(1012, new String[] { "file exceeds limit", "文件超过限制" });
            errorCodes.Add(1013, new String[] { "file exceeds form limit", "文件大小超过限制" });
            errorCodes.Add(1014, new String[] { "file is partial", "部分文件上传" });
            errorCodes.Add(1015, new String[] { "no temporary directory", "服务器没有临时文件夹" });
            errorCodes.Add(1016, new String[] { "unable to write file", "无法写文件" });
            errorCodes.Add(1017, new String[] { "upload stopped by extension", "上传被中断" });
            errorCodes.Add(1018, new String[] { "invalid filetype", "无效文件类型" });
            errorCodes.Add(1019, new String[] { "invalid filesize", "无效文件大小" });
            errorCodes.Add(1020, new String[] { "destination error", "目的地错误" });
            errorCodes.Add(1021, new String[] { "bad filename", "上传文件名有误" });
            errorCodes.Add(1022, new String[] { "no filepath", "找不到上传文件路径" });
            errorCodes.Add(1023, new String[] { "result no file", "该文件不存在" });
            errorCodes.Add(9999, new String[] { "unknown error", "未知错误" });
        }

        public String[] this[int i]{
            get{
                if (errorCodes.ContainsKey(i)) return errorCodes[i];
                else return errorCodes[unknownError];
            }
        }

        public static readonly ResponseEntity UnknownErrorEntity = new ResponseEntity { status = unknownError, msg = "Unknown Error" };

        private const long MAX_FILE_LENGTH = 16*1024*1000; //上传文件的大小上限
        private CookieContainer currentCookie; //用来保存用户登录成功后的cookie，登出后清空，再次登录会更新
        private string url; //服务器的开始url
        private const String S_USERAGENT = "Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0";
        //private const String S_USERAGENT = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
        private ResponseEntity R(ResponseEntity r) { return r != null ? r : UnknownErrorEntity; }

        public MyHttp(string url) {
            initErrorCodes();
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
            byte[] body = File.ReadAllBytes(filepath);
            if (body.Length > MAX_FILE_LENGTH) return new ResponseEntity { status = fileExceedsFormLimitCode, msg = errorCodes[fileExceedsFormLimitCode][1] };
            //
            //sb.Clear();
            //sb.Append("--" + boundary + "\r\n");
            //sb.Append("Content-Disposition: form-data; name=\"file_md5\";\r\n");
            //sb.Append("\r\n");
            //byte[] md5Header = Encoding.UTF8.GetBytes(sb.ToString());
            //byte[] md5 = Encoding.UTF8.GetBytes(A.MD5(body) + "\r\n");
            //
            //sb.Clear();
            //sb.Append("--" + boundary + "\r\n");
            //sb.Append("Content-Disposition: form-data; name=\"book_name\";\r\n");
            //sb.Append("\r\n");
            //byte[] nameHeader = Encoding.UTF8.GetBytes(sb.ToString());
            //byte[] name = Encoding.UTF8.GetBytes(poststr + "\r\n");
            //
            String res = Http.Create(url + "/upload").SetHttpDelegate(this).Add("file_md5", A.MD5(body)).Add("book_name", poststr).Post();
            var r = R(Json.Decode<ResponseEntity>(res));
            if (r.status != resultNoFile) return r;

            string boundary = "---------------------ad7d9fdf30d1a8";
            StringBuilder sb = new StringBuilder();
            sb.Append("--" + boundary + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"" + "userfile\";" + "filename=\"" + poststr + "\"\r\n");//"username=\""+username+ 
            sb.Append("Content-Type:text/plain" + "\r\n");
            sb.Append("\r\n");            
            byte[] bsHeader = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] bsTail = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");                        

            res=Http.Create(url + "/upload").SetHttpDelegate(this)
                .SetContentType("multipart/form-data;boundary=" + boundary)
                //.Add(md5Header).Add(md5).Add(nameHeader).Add(name)
                .Add(bsHeader).Add(body).Add(bsTail)
                .Post();
            return R(Json.Decode<ResponseEntity>(res));
        }
    }
}


/*/
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
using Zlib.Net;
using Zlib.Text;
using Zlib.Utility;
using Zlib.Algorithm;

namespace TXTReader.Net {
    public class MyHttp {
        public Dictionary<int, String[]> errorCodes = new Dictionary<int, String[]>();
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
        public const int fileExceedsLimitCode = 1012;
        public const int fileExceedsFormLimitCode = 1013;
        public const int resultNoFile = 1023;
        public const int unknownError = 9999;

        private void initErrorCodes() {
            errorCodes.Add(1000, new String[] { "success", "操作成功" });
            errorCodes.Add(1001, new String[] { "missing arguments", "参数不全" });
            errorCodes.Add(1002, new String[] { "database error", "数据库错误" });
            errorCodes.Add(1003, new String[] { "invalid username", "用户名不合法" });
            errorCodes.Add(1004, new String[] { "same username", "相同用户名" });
            errorCodes.Add(1005, new String[] { "passwords diffirent", "密码不一致" });
            errorCodes.Add(1006, new String[] { "user not exist", "用户不存在" });
            errorCodes.Add(1007, new String[] { "wrong password", "密码错误" });
            errorCodes.Add(1008, new String[] { "not login", "未登录" });
            errorCodes.Add(1009, new String[] { "cannot loggout", "无法登出" });
            errorCodes.Add(1010, new String[] { "no book matches", "书不存在" });
            errorCodes.Add(1011, new String[] { "file is not selected", "未选择文件" });
            errorCodes.Add(1012, new String[] { "file exceeds limit", "文件超过限制" });
            errorCodes.Add(1013, new String[] { "file exceeds form limit", "文件大小超过限制" });
            errorCodes.Add(1014, new String[] { "file is partial", "部分文件上传" });
            errorCodes.Add(1015, new String[] { "no temporary directory", "服务器没有临时文件夹" });
            errorCodes.Add(1016, new String[] { "unable to write file", "无法写文件" });
            errorCodes.Add(1017, new String[] { "upload stopped by extension", "上传被中断" });
            errorCodes.Add(1018, new String[] { "invalid filetype", "无效文件类型" });
            errorCodes.Add(1019, new String[] { "invalid filesize", "无效文件大小" });
            errorCodes.Add(1020, new String[] { "destination error", "目的地错误" });
            errorCodes.Add(1021, new String[] { "bad filename", "上传文件名有误" });
            errorCodes.Add(1022, new String[] { "no filepath", "找不到上传文件路径" });
            errorCodes.Add(1023, new String[] { "result no file", "该文件不存在" });
            errorCodes.Add(9999, new String[] { "unknown error", "未知错误" });
        }

        public String[] this[int i] {
            get {
                if (errorCodes.ContainsKey(i)) return errorCodes[i];
                else return errorCodes[unknownError];
            }
        }

        public static readonly ResponseEntity UnknownErrorEntity = new ResponseEntity { status = unknownError, msg = "Unknown Error" };

        private const long MAX_FILE_LENGTH = 16 * 1024 * 1000; //上传文件的大小上限
        private string url; //服务器的开始url
        public Zlib.Net.ZWeb W;
        private const String S_USERAGENT = "Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0";
        //private const String S_USERAGENT = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
        private ResponseEntity R(ResponseEntity r) { return r != null ? r : UnknownErrorEntity; }

        public MyHttp(string url) {
            initErrorCodes();
            this.url = url;
            W = ZWeb.CreateNew();
            W.BaseAddress = url;
            //W.Headers[HttpRequestHeader.UserAgent] = S_USERAGENT;
            W.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        }

        public async Task<ResponseEntity> SignUp(string username, string password) {
            string tempStr = await W.UploadStringTaskAsync("signup",
                "username".eq(username) &
                "password".eq(password) &
                "password_comfirmation".eq(password));
            return R(Json.Decode<ResponseEntity>(tempStr));
        }

        public async Task<ResponseEntity> Login(string username, string password) {
            string tempStr = await W.UploadStringTaskAsync("login",
               "username".eq(username) &
               "password".eq(password));
            return R(Json.Decode<ResponseEntity>(tempStr));
        }

        public async Task<ResponseEntity> Logout() {
            string loginUrl = url + "/logout";
            String tempStr = await W.DownloadStringTaskAsync("logout");
            W.Cookies = null;
            return R(Json.Decode<ResponseEntity>(tempStr));
        }

        public async Task<ResponseEntity> Search(string bookname, string searchType) {
            var t = await W.DownloadStringTaskAsync("search?" + ("own".eq(0) & "book_name".eq(HttpUtility.UrlEncode(bookname))));
            var ret = R(Json.Decode<ResponseEntity>(t));
            if (ret.status == 1010) return new ResponseEntity { status = successCode, msg = "success", data = new String[0][] };
            return ret;
        }

        public ResponseEntity Download(string bookid) {
            //String title = null;
            //byte[] res = Http.Create(url + "/download?id=" + bookid).SetHttpDelegate(this)
            //    .Do((h) => {
            //        h.AfterResponse = (hh) => {
            //            if (hh.Response.ContentType == "\"text/plain\"") {
            //                //var header = Encoding.GetEncoding(String.IsNullOrEmpty(hh.Response.CharacterSet) ? "iso-8859-1" : hh.Response.CharacterSet).GetString(hh.Response.Headers.ToByteArray());
            //                var header = Encoding.UTF8.GetString(hh.Response.Headers.ToByteArray());
            //                var headers = header.Split('\n', '\r');
            //                foreach (String str in headers) {
            //                    if (str.StartsWith("Content-Disposition:")) {
            //                        title = str;
            //                        title = title.Substring(title.IndexOf("filename=") + "filename=".Length).Trim('"');
            //                    }
            //                }
            //            }
            //        };
            //    })
            //    .GetBytes();
            //if (title == null) {
            //    if (res == null) return new ResponseEntity { status = unknownError, msg = "connection is broken" };
            //    return R(Json.Decode<ResponseEntity>(Encoding.UTF8.GetString(res)));
            //} else return new ResponseEntity {
            //    status = successCode,
            //    msg = "download success",
            //    data = new object[] { title, res }
            //};
            return null;
        }

        public async Task<ResponseEntity> Upload(string filename, string filepath, UploadProgressChangedEventHandler e = null) {
            byte[] body = File.ReadAllBytes(filepath);
            if (body.Length > MAX_FILE_LENGTH) return new ResponseEntity { status = fileExceedsFormLimitCode, msg = errorCodes[fileExceedsFormLimitCode][1] };            
            string res = await W.UploadStringTaskAsync("upload", "file_md5".eq(A.MD5(body)) & "book_name".eq(HttpUtility.UrlEncode(filename)));
            var r = R(Json.Decode<ResponseEntity>(res));
            if (r.status != resultNoFile) return r;

            //W.Headers[HttpRequestHeader.ContentType] = "text/plain";
            //W.Headers["Content-Disposition"] = "form-data; name=\"file\"; filename=\""+filename+"\"";

            var bs = await W.UploadFileTaskAsync("upload", "userfile", filepath, null);
            if (bs == null) return R(null);
            res = Encoding.UTF8.GetString(bs);
            return R(Json.Decode<ResponseEntity>(res));
        }
    }
}//*/