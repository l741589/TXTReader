using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zlib.Text.Xml {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class XmlEntity : Attribute {
        public bool IsParsable { get; set; }

        public XmlEntity() {
            IsParsable = true;
        }

        public XmlEntity(bool parsable) {
            IsParsable = parsable;
        }
    }
}
