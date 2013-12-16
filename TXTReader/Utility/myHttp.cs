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
        /// ��ʾ���������صĸ���״̬��
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

        private const long MAX_FILE_LENGTH = 1045650; //�ϴ��ļ��Ĵ�С����
        private CookieCollection currentCookie; //���������û���¼�ɹ����cookie���ǳ�����գ��ٴε�¼�����
        private Encoding encoding = Encoding.Default;
        
        private string startUrl; //�������Ŀ�ʼurl
        private string currentWebPage;
        private HttpWebResponse currentResponce;


        /// <summary>
        /// ���캯����
        /// </summary>
        /// <param name="newUrl">newUrlΪ�������Ŀ�ʼ��ַ�������������ܺ�����
        /// url�������url�Ļ����Ͻ����޸ģ�����login��url��newUrl+"/login"</param>
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
        /// ע�ắ��
        /// </summary>
        /// <param name="username">ע���û���</param>
        /// <param name="password">ע������</param>
        /// <returns>
        /// ����û����Ѿ����ڣ�����һ��string��ֵΪ"error:username existed."
        /// ���û����д�û����������룬����һ��string��ֵΪ"error:the username and password should be supplied."
        /// ���������������������ע��ɹ�������һ��string��ֵΪ"signip successfully."
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
        /// ��¼����
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="password">����</param>
        /// <returns>
        /// ���������󣨰���û���������룩������һ��string��ֵΪ"error:wrong password."
        /// ����û��������ڣ�����û�����û�����������һ��string��ֵΪ"error:the username not exist."
        /// ��������������������ǵ�½�ɹ�������һ��string��ֵΪ"login successfully."
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
        /// ע���������Ὣcookie���
        /// </summary>
        /// <returns>
        /// ����û���û�е�½�����ô˺����󣬷���һ��string��ֵΪ"error:no login."
        /// ����û��Ѿ���½�����ô˺����󣬷���һ��string��ֵΪ"logout successfully."
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
        /// ��ѯ����
        /// </summary>
        /// <param name="bookname">����</param>
        /// <param name="searchType">�ò���Ϊstring��������һ��bool��ֵΪ��false��ʱ����ѯ���е��飻ֵΪ��true��ʱ����ѯ�û�����</param>
        /// <returns>
        /// ��ѯ�ɹ��󣬷��ص�ֵ����ʽ��book_id,book_name&book_id,book_name,���� 2,test1.txt&3,test2.txt&4,test3.txt
        /// ��ѯʧ�ܺ󣬷��� "unknown error."
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
        /// ���غ���
        /// </summary>
        /// <param name="bookid">��ı�ţ�����ͨ����ѯ֪��ĳ�����Ӧ�ı��</param>
        /// <returns>
        /// û�е�½������ "error:no login."
        /// ��Ų����ڻ���û�������ţ����� "error:the book_id isn't existed."
        /// ���سɹ������صĸ�ʽΪ��
        /// ��bookname��
        ///  �ı�
        ///  ���磺
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
        /// �ϴ��ı�
        /// </summary>
        /// <param name="poststr">txt�ļ������ڷ������ϵ�����</param>
        /// <param name="filepath">Ҫ�ϴ���txt�ļ��ڱ��ص�·��</param>
        /// <returns>
        /// �ϴ��ɹ������� "The file \""+filepath+"\" uploaded successfully."
        /// ���͵��ļ�̫�󣬷��� "this file \"" + filepath + "\" is too large to be uploaded."
        /// ����ǰû�е�¼������ "not login."
        /// ·��Ϊ�գ����� "filepath is empty." 
        /// </returns>
        public string uploadFile(string poststr, string filepath)
        {
            if (filepath != ""&currentCookie.Count!=0)
            {
                int errorType = 0;
                try
                {
                    // ��������Ǹı�ģ�Ҳ��������������̶����ַ��� 
                    string boundary = "---------------------ad7d9fdf30d1a8";

                    // ����request���� 
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

                    // ���췢������ 
                    StringBuilder sb = new StringBuilder();

                    // �ı�������ݣ���user=eking&pass=123456  ��ʽ���ı����� ��Ȼ���� 


                    //string[] item = poststr.Split('=');
                    //string userfile = item[0];
                    //string value = item[1];
                    //sb.Append("�C" + boundary); 
                    sb.Append("--"+boundary + "\r\n");
                    sb.Append("Content-Disposition: form-data; name=\"" + "userfile\";" + "filename=\"" + poststr + "\"\r\n");//"username=\""+username+ 
                    sb.Append("Content-Type:text/plain" + "\r\n");
                    sb.Append("\r\n");
                    string postHeader = sb.ToString();
                    testStr = "";////////////////////////////////-------------------
                    testStr += postHeader;////////////////////////////---------------
                    byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

                    //����β������ 

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

                    // ����ͷ������ 
                    requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                    // �����ļ������� 
                    byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        requestStream.Write(buffer, 0, bytesRead);

                    // ����β������ 

                    requestStream.Write(postHeaderBytes1, 0, postHeaderBytes1.Length);
                    testStr += sb1.ToString();//////////////////////---------------------
                    HttpWebResponse responce = (HttpWebResponse)webrequest.GetResponse();
                    Stream s = responce.GetResponseStream();
                    StreamReader sr = new StreamReader(s);

                    // ����������(Դ��) 
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
        /// ��������ͽ�����Ӧ����login��logout��download��search����
        /// </summary>
        /// <param name="Url">Url</param>
        /// <param name="methord">"POST"����"GET"</param>
        /// <param name="sendStr">POST����GETҪ���͵�����</param>
        /// <returns>����HTML����</returns>
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