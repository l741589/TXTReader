using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Zlib.Utility;

namespace Zlib.Net {
    public class UrlBuilder<T> {
        public String BaseUrl { get; set; }
        public Dictionary<String, String> Args { get; set; }
        private MemoryStream data;
        private T owner;


        public UrlBuilder(String url = null, T owner = default(T)) {
            BaseUrl = url;
            this.owner = owner;
        }

        public static UrlBuilder<T> Create(String url = null, T owner = default(T)) {
            return new UrlBuilder<T>(url, owner);
        }

        public UrlBuilder<T> Add(String key, String value) {
            if (Args == null) Args = new Dictionary<string, string>();
            Args.Add(key, value);
            return this;
        }

        public UrlBuilder<T> Add(Byte[] bytes) {
            if (data == null) data = new MemoryStream();
            data.Write(bytes, 0, bytes.Length);
            return this;
        }

        public byte[] Data { get { return data.ToArray(); } }

        public String ArgString {
            get {
                if (Args == null) return "";
                String s = "";
                Boolean start = true;
                Dictionary<String, String>.KeyCollection keys = Args.Keys;
                foreach (String key in keys) {
                    if (start) start = false;
                    else s += "&";
                    s += key + "=" + Args[key];
                }
                return s;
            }
        }

        public String Url {
            get {
                if (ArgString.IsNullOrWhiteSpace()) return BaseUrl;
                else return BaseUrl + "?" + ArgString;
            }
        }

        public T Done() {
            return owner;
        }
    }

    public class UrlBuilder : UrlBuilder<object>{ }
}
