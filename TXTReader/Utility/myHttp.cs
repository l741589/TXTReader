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

namespace Http
{
    class MyHttp
    {
        public string testStr;
        private int requestTimeout = 1000;
        /// <summary>
        /// 表示服务器返回的各种状态码
        /// </summary>
        private int successCode = 1000;
        private int missArgumentsCode = 1001;
        private int databaseErrorCode = 1002;
        private int invalidUsernameCode = 1003;
        private int usernameExistCode = 1004;
        private int notExitsUsernameCode = 1005;
        private int wrongPasswordCode = 1006;
        private int notLoginCode = 1007;
        private int uploadErrorCode = 1008;
        private int notExistBookCode = 1009;

        private const long MAX_FILE_LENGTH = 1045650; //上传文件的大小上限
        private CookieCollection currentCookie; //用来保存用户登录成功后的cookie，登出后清空，再次登录会更新
        private Encoding encoding = Encoding.Default;
        
        private string startUrl; //服务器的开始url
        private string currentWebPage;
        private HttpWebResponse currentResponce;


        /// <summary>
        /// 构造函数，
        /// </summary>
        /// <param name="newUrl">newUrl为服务器的开始地址，其他几个功能函数的
        /// url是在这个url的基础上进行修改，比如login的url是newUrl+"/login"</param>
        public MyHttp(string newUrl)
        {
            startUrl = newUrl;
            
            currentWebPage = "";
            currentCookie = new CookieCollection();
        }

        public string mainPage()
        {
            currentWebPage = sendData(startUrl, "POST", "");
            return "success";
        }

        /// <summary>
        /// 注册函数
        /// </summary>
        /// <param name="username">注册用户名</param>
        /// <param name="password">注册密码</param>
        /// <returns>
        /// 如果用户名已经存在，返回一个string，值为"error:username existed."
        /// 如果没有填写用户名或者密码，返回一个string，值为"error:the username and password should be supplied."
        /// 除了以上两种情况，就是注册成功，返回一个string，值为"signip successfully."
        /// </returns>
        public String signup(string username, string password)
        {
            string sendStr = "username=" + username + "&password=" + password;
            string signupUrl = startUrl + "/signup";
            string tempStr = sendData(signupUrl, "POST",sendStr);
            currentWebPage = tempStr;
            if (tempStr.IndexOf("{\"status\":" + usernameExistCode + ",\"msg\":\"username exist\"}") != -1)
            {         
                return "error:username existed.";
            }
            else if (tempStr.IndexOf("{\"status\":" + missArgumentsCode + ",\"msg\":\"missing args\"}") != -1)
            {
                return "error:the username and password should be supplied.";
            }
            else
            {
                return "signip successfully.";
            }
        }

        /// <summary>
        /// 登录函数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>
        /// 如果密码错误（包括没有输入密码），返回一个string，值为"error:wrong password."
        /// 如果用户名不存在（包括没输入用户名），返回一个string，值为"error:the username not exist."
        /// 除了以上两种情况，就是登陆成功，返回一个string，值为"login successfully."
        /// </returns>
        public string login(string username, string password)
        {
            string sendStr = "username=" + username + "&password=" + password;
            string loginUrl = startUrl + "/login";
            string tempStr = sendData(loginUrl, "POST", sendStr);
            currentWebPage = tempStr;
            if (tempStr.IndexOf("{\"status\":" + wrongPasswordCode + ",\"msg\":\"wrong password\"}") != -1)
            {
                return "error:wrong password.";
            }
            else if (tempStr.IndexOf("{\"status\":" + notExitsUsernameCode + ",\"msg\":\"user not exist\"}") != -1)
            {
                return "error:the username not exist.";
            }
            else
            {
                return "login successfully.";
            }
        }

        /// <summary>
        /// 注销函数，会将cookie清空
        /// </summary>
        /// <returns>
        /// 如果用户还没有登陆，调用此函数后，返回一个string，值为"error:no login."
        /// 如果用户已经登陆，调用此函数后，返回一个string，值为"logout successfully."
        /// </returns>
        public string logout()
        {
            string loginUrl = startUrl + "/logout";
            string tempStr = sendData(loginUrl, "POST","");
            currentWebPage = tempStr;
            currentCookie = new CookieCollection();
            if (tempStr.IndexOf("{\"status\":" + notLoginCode + ",\"msg\":\"no login\"}") != -1)
            {
                return "error:no login.";
            }
            else
            {
                return "logout successfully.";
            }
        }

        /// <summary>
        /// 查询函数
        /// </summary>
        /// <param name="bookname">书名</param>
        /// <param name="searchType">该参数为string，而不是一个bool，值为“false”时，查询所有的书；值为“true”时，查询用户的书</param>
        /// <returns>
        /// 查询成功后，返回的值的形式：book_id,book_name&book_id,book_name,例如 2,test1.txt&3,test2.txt&4,test3.txt
        /// 查询失败后，返回 "unknown error."
        /// </returns>
        public string search(string bookname,string searchType)
        {
            string sendStr = "";
            if (searchType == "false" || currentCookie.Count == 0)
            {
                sendStr = "book_name=" + bookname;
            }
            else
            {
                sendStr = "book_name=" + bookname + "&own=" + searchType;
            }
            string uploadUrl = startUrl + "/search";
            string tempStr = sendData(uploadUrl, "GET", sendStr);
            currentWebPage = tempStr;
            if (tempStr.IndexOf("{\"status\":" + successCode + ",\"msg\":\"success\"") != -1)
            {
                int index = tempStr.IndexOf("[[")+1;
                string bookInformation = "";
                string tempInfor = tempStr.Substring(index, tempStr.LastIndexOf("]]") - index);
                string[] bookInformations = tempInfor.Split(']');
                for (int i = 0; i < bookInformations.Length;i++ )
                {
                    index = bookInformations[i].IndexOf("[\"") + 2;
                    string book_id = bookInformations[i].Substring(index, bookInformations[i].IndexOf(",\"") - index-1);
                    index = bookInformations[i].IndexOf(",\"") + 2;
                    string book_name = bookInformations[i].Substring(index, bookInformations[i].IndexOf("txt\"") + 3 - index);
                    bookInformation += book_id + "," + book_name + "&";
                }
                bookInformation = bookInformation.Substring(0, bookInformation.Length - 1);
                return bookInformation;
            }
            else
            {
                return "unknown error.";
            }
        }

        /// <summary>
        /// 下载函数
        /// </summary>
        /// <param name="bookid">书的编号，可以通过查询知晓某本书对应的编号</param>
        /// <returns>
        /// 没有登陆，返回 "error:no login."
        /// 编号不存在或者没有输入编号，返回 "error:the book_id isn't existed."
        /// 下载成功，返回的格式为：
        /// （bookname）
        ///  文本
        ///  例如：
        ///  (test222.txt)
        ///  2312sdfsf
        ///  wq213
        ///  1
        /// </returns>
        public string download(string bookid)
        {
            string downloadUrl = startUrl + "/download";
            string sendStr = "id=" + bookid;
            string tempStr = sendData(downloadUrl, "GET",sendStr);
            currentWebPage = tempStr;
            
            if (tempStr.IndexOf("-downloadsuccess-")==-1)
            {
                if (tempStr.IndexOf("{\"status\":"+missArgumentsCode+",\"msg\":\"missing args\"}")!=-1)
                {
                    return "error:the book_id should be supplied.";
                }
                else if (tempStr.IndexOf("{\"status\":" + notExistBookCode + ",\"msg\":\"user has no books\"}") != -1)
                {
                    return "error:the book_id isn't existed.";
                }
                else if (tempStr.IndexOf("{\"status\":" + notLoginCode + ",\"msg\":\"no login\"}") != -1)
                {
                    return "error:no login.";
                }
            }
            else
            {
                string fname = "";
                fname = tempStr.Substring(tempStr.IndexOf("("), tempStr.IndexOf(")") - tempStr.IndexOf("(")+1);
                tempStr = fname + "\r\n" + tempStr.Substring(tempStr.LastIndexOf("-downloadsuccess-") + "-downloadsuccess-".Length);   
            }
            return tempStr;
        }

        /// <summary>
        /// 上传文本
        /// </summary>
        /// <param name="poststr">txt文件保存在服务器上的名字</param>
        /// <param name="filepath">要上传的txt文件在本地的路径</param>
        /// <returns>
        /// 上传成功，返回 "The file \""+filepath+"\" uploaded successfully."
        /// 发送的文件太大，返回 "this file \"" + filepath + "\" is too large to be uploaded."
        /// 下载前没有登录，返回 "not login."
        /// 路径为空，返回 "filepath is empty." 
        /// </returns>
        public string uploadFile(string poststr, string filepath)
        {
            if (filepath != ""&currentCookie.Count!=0)
            {
                int errorType = 0;
                try
                {
                    // 这个可以是改变的，也可以是下面这个固定的字符串 
                    string boundary = "---------------------ad7d9fdf30d1a8";

                    // 创建request对象 
                    HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(startUrl + "/upload");
                    webrequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                    //webrequest.Accept = "Accept-Language:zh-cn"; 
                    webrequest.ContentType = "multipart/form-data;boundary=" + boundary;
                    //webrequest.Accept = "text/html, application/xhtml+xml, */*";
                    webrequest.Method = "POST";
                    webrequest.CookieContainer = new CookieContainer();
                    for (int i = 0; i < currentCookie.Count; i++)
                    {
                        webrequest.CookieContainer.Add(currentCookie[i]);
                    }

                    // 构造发送数据 
                    StringBuilder sb = new StringBuilder();

                    // 文本域的数据，将user=eking&pass=123456  格式的文本域拆分 ，然后构造 


                    //string[] item = poststr.Split('=');
                    //string userfile = item[0];
                    //string value = item[1];
                    //sb.Append("–" + boundary); 
                    sb.Append("--"+boundary + "\r\n");
                    sb.Append("Content-Disposition: form-data; name=\"" + "userfile\";" + "filename=\"" + poststr + "\"\r\n");//"username=\""+username+ 
                    sb.Append("Content-Type:text/plain" + "\r\n");
                    sb.Append("\r\n");
                    string postHeader = sb.ToString();
                    testStr = "";////////////////////////////////-------------------
                    testStr += postHeader;////////////////////////////---------------
                    byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

                    //构造尾部数据 

                    StringBuilder sb1 = new StringBuilder();
                    sb1.Append("\r\n--"+boundary + "--\r\n");
                    byte[] postHeaderBytes1 = Encoding.UTF8.GetBytes(sb1.ToString());

                    FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);

                    byte[] tempBytes = new byte[fileStream.Length];///////////////-----------

                    /////////////////////////////////////////////////////////

                    StreamReader objReader = new StreamReader(filepath);
                    string sLine = "";///////////

                    while (sLine != null)////////////
                    {
                       sLine = objReader.ReadLine();////////////
                        testStr += sLine + "\r\n";//////////
                    }////////////
                    objReader.Close();

                    /////////////////////////////////////////////////////////

                    //testStr += fileStream.Read(tempBytes, 0, tempBytes.Length);////////////////-------------
                    if (fileStream.Length > MAX_FILE_LENGTH)
                    {
                        errorType = 1;
                        throw new Exception();
                    }
                    long length = postHeaderBytes.Length + fileStream.Length + postHeaderBytes1.Length;
                    webrequest.ContentLength = length;

                    Stream requestStream = webrequest.GetRequestStream();

                    // 输入头部数据 
                    requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                    // 输入文件流数据 
                    byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        requestStream.Write(buffer, 0, bytesRead);

                    // 输入尾部数据 

                    requestStream.Write(postHeaderBytes1, 0, postHeaderBytes1.Length);
                    testStr += sb1.ToString();//////////////////////---------------------
                    HttpWebResponse responce = (HttpWebResponse)webrequest.GetResponse();
                    Stream s = responce.GetResponseStream();
                    StreamReader sr = new StreamReader(s);

                    // 返回数据流(源码) 
                    string tempStr = sr.ReadToEnd();
                    currentWebPage = tempStr;
                    currentResponce = responce;

                    return "The file \"" + filepath + "\" uploaded successfully.";
                }
                catch (WebException we)
                {
                    return "Fail to upload the file \"" + filepath + "\".";
                }
                catch (Exception e)
                {
                    if (errorType == 1)
                    {
                        return "this file \"" + filepath + "\" is too large to be uploaded.";
                    }
                    else
                    {
                        return "Fail to upload the file \"" + filepath + "\".";
                    }
                }
            }
            else if (currentCookie.Count == 0)
            {
                return "not login.";
            }
            else
            {
                return "filepath is empty.";
            }
        }

        /// <summary>
        /// 发送请求和接收响应，被login，logout，download，search调用
        /// </summary>
        /// <param name="Url">Url</param>
        /// <param name="methord">"POST"或者"GET"</param>
        /// <param name="sendStr">POST或者GET要发送的数据</param>
        /// <returns>返回HTML内容</returns>
        private string sendData(string Url,string methord,string sendStr)
        {
            try
            {
                HttpWebRequest request;
                if (methord == "POST")
                {
                    request = (HttpWebRequest)WebRequest.Create(Url);
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(Url + (sendStr == "" ? "" : "?") + sendStr);
                }
                request.ContentType = "text/html;charset=UTF-8";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                request.Method = methord;
                request.Accept = "Accept-Language:zh-cn";            
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = requestTimeout;
                request.CookieContainer = new CookieContainer();

                if ((Url.IndexOf("/logout") != -1 || Url.IndexOf("/upload") != -1 || Url.IndexOf("/download") != -1 || Url.IndexOf("/search") != -1) && currentCookie.Count > 0)
                {
                    for (int i = 0; i < currentCookie.Count; i++)
                    {
                        request.CookieContainer.Add(currentCookie[i]);
                    }
                }

                if (methord == "POST")
                {
                    request.ContentLength = sendStr.Length;
                    Stream myRequestStream = request.GetRequestStream();
                    StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                    myStreamWriter.Write(sendStr);
                    myStreamWriter.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                
                StreamReader myStreamReader;
                if (Url.IndexOf("/download") != -1)
                {
                    myStreamReader = new StreamReader(myResponseStream, Encoding.Default);
                }
                else
                {
                    myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                }
                
                string retString = myStreamReader.ReadToEnd();/////ReadToEnd()

                byte[] headerByte = response.Headers.ToByteArray();
                string headerStr = Encoding.GetEncoding("GBK").GetString(headerByte) ;
                if (retString.IndexOf(""+successCode) != -1 && retString.IndexOf("success") != -1 && Url.IndexOf("/login") != -1)
                {
                    currentCookie = response.Cookies;
                }
                if (response.ContentType == "\"text/plain\"")
                {
                    int index = headerStr.IndexOf("filename=\"") + 10;

                    retString = "(" + headerStr.Substring(index, headerStr.IndexOf(".txt\"") - index + 4) + ")-downloadsuccess-" + retString;
                }
                currentResponce = response;
                return retString;
            }
            catch (WebException we)
            {
                MessageBox.Show(we.ToString());
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public string CurrentWebPage
        {
            get { return currentWebPage; }
        }

        public HttpWebResponse CurrentResponce
        {
            get { return currentResponce; }
        }

        public CookieCollection CurrentCookie
        {
            get { return currentCookie; }
        }     
    }
}