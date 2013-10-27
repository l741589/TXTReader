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

namespace TXTReader.Utility
{
    class Http {
        private String path;
        private Dictionary<String, String> args;
        private MemoryStream ms;
        private WebRequest request;

        public static Http Create(String path) {
            Http r = new Http();
            r.path = path;
            WebRequest request = null;
            if (path.StartsWith("http")) request = WebRequest.Create(path);
            else
#if DEBUG
                request = WebRequest.Create("http://" + Properties.Settings.Default.DEBUG_ADDR + ":" + Properties.Settings.Default.DEBUG_PORT + "/" + path);
#else
                WebRequest request = WebRequest.Create("http://"+Properties.Settings.Default.SERVERADDR+":" + Properties.Settings.Default.PORT + "/" + path);
#endif
            r.request = request;
            return r;
        }

        public Http Add(String key, String value) {
            if (args == null) args = new Dictionary<string, string>();
            args.Add(key, value);
            return this;
        }

        public Http Add(byte[] data) {
            if (ms == null) ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            return this;
        }

        public async Task<String> Post() {
            try {
                request.Method = "POST";
                if (args != null) await RequestString(args);
                else await RequestBytes(ms);
                return await ResponseString();
            } catch (Exception e) {
                MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                return null;
            }
        }

        public async Task<String> Get() {
            try {
                request.Method = "GET";
                if (args != null) await RequestString(args);
                else await RequestBytes(ms);
                return await ResponseString();
            } catch (Exception e) {
                MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                return null;
            }
        }

        public async Task<byte[]> PostBytes() {
            try {
                request.Method = "POST";
                if (args != null) await RequestString(args);
                else await RequestBytes(ms);
                return await ResponseBytes();
            } catch (Exception) {
                MessageBox.Show(App.Current.MainWindow, "无法连接到服务器。");
                return null;
            }
        }

        private String BuildArgs(Dictionary<String, String> args)
        {
            if (args == null) return "";
            String s = "";
            Boolean start = true;
            Dictionary<String, String>.KeyCollection keys = args.Keys;
            foreach (String key in keys)
            {
                if (start) start = false;
                else s += "&";
                s += key + "=" + args[key];
            }
            return s;
        }

        public async Task RequestString(Dictionary<String, String> args = null) {            
            string postData = BuildArgs(args);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = await request.GetRequestStreamAsync();
            await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
            dataStream.Close();
        }

        public async Task RequestBytes(MemoryStream data) {
            if (data != null) {
                Stream dataStream = await request.GetRequestStreamAsync();
                byte[] byteArray = data.ToArray();
                await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }            
        }

        public async Task<String> ResponseString() {
            WebResponse response = await request.GetResponseAsync();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = await reader.ReadToEndAsync();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;            
        }

        public async Task<byte[]> ResponseBytes() {
            WebResponse response = await request.GetResponseAsync();
            Stream dataStream = response.GetResponseStream();
            MemoryStream reader = new MemoryStream();
            int num;
            byte[] buffer = new byte[0x400];
            do {
                num = await dataStream.ReadAsync(buffer, 0, buffer.Length);
                if (num > 0)
                    await reader.WriteAsync(buffer, 0, num);
            }while (num > 0);
            reader.Close();
            dataStream.Close();
            response.Close();
            return reader.ToArray();
        }
    }
}
