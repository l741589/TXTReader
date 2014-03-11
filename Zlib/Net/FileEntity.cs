using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zlib.Net {
    public class FileEntity : MimeEntity {
        private String _filename;

        public FileEntity(String filepath,String name=null,String filename = null) {
            Data = File.ReadAllBytes(filepath);
            _filename = Path.GetFileName(filepath);
            var ext = Path.GetExtension(filepath);
            String ct = ContentTypes.GetContentType(ext);
            ContentType = ct;
            ContentDisposition = String.Format("{0}; name=\"{1}\"; filename=\"{2}\"", 
                ContentTypes.FormData,
                name == null ? "file" : name, 
                HttpUtility.UrlEncode(filename == null ? _filename : filename));
        }
    }
}
