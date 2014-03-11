using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Zlib.Algorithm;

namespace Zlib.Net {
    public class MultipartEntity : MimeEntity {

        public List<MimeEntity> Entities { get; private set; }

        public MultipartEntity() {
            ContentType = ContentTypes.MultipartFormData;
            Entities = new List<MimeEntity>();
        }

        public MultipartEntity(params MimeEntity[] entities) {
            ContentType = ContentTypes.MultipartFormData;
            Entities = new List<MimeEntity>();
            Entities.AddRange(entities);
        }



        public bool CheckChildBoundary(String boundary) {
            foreach (var e in Entities) if (!e.CheckBoundary(boundary)) return false;
            return true;
        }

        public override byte[] Data {
            get {
                if (base.Data != null) return base.Data;
                MemoryStream os = new MemoryStream();
                var b=A.RandomString(16,A.DICT_NUMBER,A.DICT_UPPER,A.DICT_LOWER);
                while (!CheckChildBoundary(b)) b = A.RandomString(16, A.DICT_NUMBER, A.DICT_UPPER, A.DICT_NUMBER);
                ContentType += "; boundary=" + b;
                byte[] bs = null;
                foreach (var e in Entities) {
                    bs = Encoding.GetBytes("--" + b + "\r\n");
                    os.Write(bs,0,bs.Length);
                    bs = e.AllData;
                    os.Write(bs, 0, bs.Length);
                }
                bs = Encoding.GetBytes("\r\n--" + b + "--\r\n");
                os.Write(bs, 0, bs.Length);
                return base.Data = os.ToArray();
            }
            protected set {
                base.Data = value;
            }
        }
    }
}
