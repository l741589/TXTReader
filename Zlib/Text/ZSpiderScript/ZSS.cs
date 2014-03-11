using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    public class ZSpiderEventArgs : EventArgs { public IZSpider Spider;}

    public class HostVarEventArgs : ZSpiderEventArgs { public String Name; public String Value; public bool IsSet; }
    public delegate String HostVarHandler(object sender, HostVarEventArgs args);

    public class LogEventArgs : ZSpiderEventArgs { public String Text;}
    public delegate void LoggerHandler(object sender, LogEventArgs args);

    public class PipeInputEventArgs : ZSpiderEventArgs { public String DefaultInput; public String Source;}
    public delegate String PipeInputHandler(object sender, PipeInputEventArgs args);

    public class PipeOutputEventArgs : ZSpiderEventArgs { public String Data; public String Target;}
    public delegate bool PipeOutputHandler(object sender, PipeOutputEventArgs args);

    public class NodeCreateEventArgs : ZSpiderEventArgs { public ParentCommand Parent; public CommandDesc CommandDesc;}
    public delegate ZSSCommand NodeCreateHandler(object sender,NodeCreateEventArgs args);

    public class ZSS {
        public const String ERROR_HEADER = "##ERROR##";
        public HostVarHandler HostVarHandler { get; set; }
        public Dictionary<String, int> SwitchArgCountTable { get; private set; }
        public Dictionary<String, String> CommandEquality { get; private set; }
        public Dictionary<String, String> SwitchEquality { get; private set; }
        public NodeCreateHandler NodeCreator { get; set; }

        public LoggerHandler Logger { get; set; }
        public PipeInputHandler Input { get; set; }
        public PipeOutputHandler Output { get; set; }


        public ZSS() {
            SwitchArgCountTable = new Dictionary<string, int>();
            CommandEquality = new Dictionary<string, string>();
            SwitchEquality = new Dictionary<string, string>();
            NodeCreator = null;
            HostVarHandler = null;
        }

        public IZSpider LoadFile(String filename) {
            var ss = File.ReadAllLines(filename, Encoding.Default);
            var rn = ZSSParser.Parse(ss, this);
            rn.Context = this;
            return rn;
        }

        public IZSpider LoadStrings(IEnumerable<String> ss) {
            var rn = ZSSParser.Parse(ss.ToArray(), this);
            rn.Context = this;
            return rn;
        }
    }
}
