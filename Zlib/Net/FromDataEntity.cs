using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Net {
    public class FromDataEntity : MimeEntity {
        private String value;
        public String Value { get { return value; } set { this.value = value; Data = e.GetBytes(value); } }
        private Encoding e;
        public override Encoding Encoding { get { return e; } set { e = value; Data = e.GetBytes(Value); } }


        public FromDataEntity(String name, String value,Encoding encoding=null) {
            ContentType = ContentTypes.FormData;
            ContentDisposition = String.Format("{0}; name=\"{1}\"",
                ContentTypes.FormData,
                name == null ? "file" : name);
            Value = value;
            Encoding = encoding == null ? Encoding.UTF8 : encoding;
        }            
    }
}
