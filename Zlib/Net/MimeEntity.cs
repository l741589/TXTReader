using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Zlib.Net {
    public abstract class MimeEntity {
        public virtual Encoding Encoding { get; set; }
        public String ContentType { get { return Headers[S_ContentType]; } set { Headers[S_ContentType] = value; } }
        public TransferEncoding? ContentTransferEncoding { get { return (TransferEncoding)Enum.Parse(typeof(TransferEncoding), Headers[S_ContentTransferEncoding], true); } set { Headers[S_ContentTransferEncoding] = value.ToString(); } }
        public string ContentDescription { get { return Headers[S_ContentDescription]; } set { Headers[S_ContentDescription] = value; } }
        public string ContentDisposition { get { return Headers[S_ContentDisposition]; } set { Headers[S_ContentDisposition] = value; } }
        public Dictionary<string, String> Headers { get; private set; }

        
        public const String S_ContentType ="Content-Type: ";
        public const String S_ContentTransferEncoding="Content-Transfer-Encoding: ";
        public const String S_ContentID ="Content-ID: ";
        public const String S_ContentDescription="Content-Description: ";
        public const String S_ContentDisposition = "Content-Disposition: ";

        public MimeEntity() {
            Headers = new Dictionary<string, string>();
            Encoding = Encoding.UTF8;
        }

        public string HeaderString() {
            StringBuilder sb=new StringBuilder();
            foreach (var e in Headers) sb.Append(e.Key).Append(e.Value).Append("\r\n");
            sb.Append("\r\n");
            return sb.ToString();
        }

        public byte[] HeaderBytes() {
            if (Encoding != null) return Encoding.GetBytes(HeaderString());
            return Encoding.UTF8.GetBytes(HeaderString());
        }

        private byte[] data = null;
        public virtual byte[] Data { get { return data; } protected set { data = value; alldata = null; } }

        private byte[] alldata = null;

        public byte[] AllData {
            get {
                if (alldata != null) return alldata;
                MemoryStream stream = new MemoryStream();
                var bs = HeaderBytes();
                stream.Write(bs, 0, bs.Length);
                bs = Data;
                stream.Write(bs, 0, bs.Length);
                return alldata = stream.ToArray();
            }
            set {
                alldata = value;
            }
        }

        public bool CheckBoundary(String boundary) {
            return !Encoding.UTF8.GetString(AllData).Contains(boundary);
        }
    }
}
