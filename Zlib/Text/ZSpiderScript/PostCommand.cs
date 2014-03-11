using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zlib.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Zlib.Text.ZSpiderScript {
    class PostCommand : ZSSCommand {

        public override string Command { get { return "post"; } }
        public Encoding Encoding { get; private set; }

        public static Regex R_SUMBOL=new Regex("[^A-Za-z0-9_]",RegexOptions.Compiled);

        public Dictionary<String, String> Header = new Dictionary<String, String>();

        public PostCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
            var sw = GetSwich("-e");
            if (sw == null) Encoding = Root.Encoding; else Encoding = Encoding.GetEncoding(sw.Arguments[0]);

            var sws = GetSwiches("-h");
            foreach (var s in sws) {
                Header.Add(s.Arguments[0], s.Arguments[1]);
            }
        }

        public override String DoExecute(String input) {
            WebException ex = null;
            for (int i = 0; i < Root.Reconnect; ++i) {
                try {
                    W.Encoding = Encoding;
                    String res = null;
                    foreach (var p in Header) W.Headers[p.Key] = p.Value;
                    if (ContainsSwich("-nr")) W.AllowAutoRedirect = false; else W.AllowAutoRedirect = true;
                    if (this[0] != null && this[1] != null) res = W.UploadString(GenVar(this[0]), GenVar(this[1]));
                    else if (this[0] != null) res = W.UploadString(input, GenVar(this[0]));
                    else res = W.UploadString(input, "");
                    W.AllowAutoRedirect = true;
                    if (ContainsSwich("-dh")) {
                        var keys = W.ResponseHeaders.AllKeys;
                        foreach (var key in keys) {
                            this[R_SUMBOL.Replace(key, "_")] = W.ResponseHeaders[key];
                        }
                    }
                    return res;
                } catch (WebException e) {
                    ex = e;
                } catch (Exception e) {
                    throw new ZSSRuntimeException(e.Message);
                }
            } 
            throw new ZSSRuntimeException("Web Error", ex);
        }
    }
}
