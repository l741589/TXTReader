using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    public interface IZSpider {
        String Site { get; }
        String Name { get; }
        int StandardLevel { get; }
        Encoding Encoding { get; }
        object Tag { get; }
        string Execute(String name, String input,object tag = null);
    }
}
