using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TXTReader.Interfaces {
    public delegate void PluginEventHandler(object sender,PluginEventArgs e);
    public class PluginEventArgs : EventArgs {
        public PluginEventArgs() {
            Args = new Dictionary<string, object>();
        }

        public PluginEventArgs(params KeyValuePair<String,object>[] pair) {
            Args = pair.ToDictionary(p => p.Key, p => p.Value);
        }

        public PluginEventArgs(Action<Dictionary<String,object>> init) {
            Args = new Dictionary<string, object>();
            init(Args);
        }
        public Dictionary<String, object> Args { get; private set; }

        public PluginEventArgs Add(String key,object value){
            Args.Add(key, value);
            return this;
        }
    }
}
