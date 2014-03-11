using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zlib.Utility;

namespace Zlib.Text.ZSpiderScript {

    public class CommandDesc {
        public String Command;
        public List<ZSSSwitch> Switches = new List<ZSSSwitch>();
        public List<String> Arguments = new List<String>();
    }



    static class ZSSParser {

        public readonly static Dictionary<String, int> SwitchArgCountTable = new Dictionary<String, int>();
        public readonly static Dictionary<String, String> CommandEquality = new Dictionary<String, String>();
        public readonly static Dictionary<String, String> SwitchEquality = new Dictionary<String, String>();
        private static IParentCommand curNode = null;
        private static ZSSRoot rootNode = null;
        public static readonly Regex R_CMD = new Regex(
            @"((?<G>(?<=(\s|^)\"")) | ^ | (?<=\s))(?(G) 
            ([^\""]|\""(?!$|\s))* | [^\""\s]\S* )
            ((?<-G>(?=\""(\s|$))) | $ | (?=\s))(?(G)(?!))",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture| RegexOptions.Compiled);
            //new Regex(@"((?<G>\"")|(?<=\s|^))(?(G)[^\""]+|\S+)((?<-G>\"")|(?=\s|$))", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        static ZSSParser() {
            SwitchArgCountTable.Add("get-e", 1);
            SwitchArgCountTable.Add("get-h", 2);
            SwitchArgCountTable.Add("get-nr", 0);
            SwitchArgCountTable.Add("get-dh", 0);
            SwitchArgCountTable.Add("post-e", 1);
            SwitchArgCountTable.Add("post-h", 2);
            SwitchArgCountTable.Add("post-nr", 0);
            SwitchArgCountTable.Add("post-dh", 0);
            SwitchArgCountTable.Add("replace-r", 0);
            SwitchArgCountTable.Add("replace-e", 0);
            SwitchArgCountTable.Add("jsondecode-q", 0);
            SwitchArgCountTable.Add("multicapture-xp", 0);
            SwitchArgCountTable.Add("multicapture-j", 0);
            SwitchArgCountTable.Add("capture-xp", 0);
            SwitchArgCountTable.Add("capture-j", 0);
            SwitchArgCountTable.Add("capture-i", 1);
            SwitchArgCountTable.Add("capture-s", 2);
            SwitchArgCountTable.Add("if-not", 0);
            SwitchArgCountTable.Add("while-not", 0);
            SwitchArgCountTable.Add("urlencode-e", 1);
            SwitchArgCountTable.Add("urldecode-e", 1);

            CommandEquality.Add("std", "standard");
            CommandEquality.Add("sub", "substring");
            CommandEquality.Add("rep", "replace");
            CommandEquality.Add("cap", "capture");
            CommandEquality.Add("mcap", "multicapture");
            CommandEquality.Add("mcapture", "multicapture");
            CommandEquality.Add("multicap", "multicapture");
            CommandEquality.Add("htmle", "htmlencode");
            CommandEquality.Add("htmld", "htmldecode");
            CommandEquality.Add("urle", "urlencode");
            CommandEquality.Add("urld", "urldecode");
            CommandEquality.Add("jsencode", "jsencode");
            CommandEquality.Add("jsdecode", "jsdecode");            
            CommandEquality.Add("jse", "jsencode");
            CommandEquality.Add("jsd", "jsdecode");
            CommandEquality.Add("jsond", "jsondecode");

            SwitchEquality.Add("get--encoding", "get-e");
            SwitchEquality.Add("get--header", "get-h");
            SwitchEquality.Add("get--noredirect", "get-nr");
            SwitchEquality.Add("get--decodeheader", "get-dh");
            SwitchEquality.Add("post--encoding", "post-e");
            SwitchEquality.Add("post--header", "post-h");
            SwitchEquality.Add("post--noredirect", "post-nr");
            SwitchEquality.Add("post--decodeheader", "post-dh");
            SwitchEquality.Add("replace--regex", "replace-r");
            SwitchEquality.Add("replace--escape", "replace-e");
            SwitchEquality.Add("multicapture--xpath", "multicapture-xp");
            SwitchEquality.Add("multicapture--json", "multicapture-j");
            SwitchEquality.Add("capture--xpath", "capture-xp");
            SwitchEquality.Add("capture--json", "capture-j");            
            SwitchEquality.Add("capture--similar", "capture-s");
            SwitchEquality.Add("capture--index", "capture-i");
            SwitchEquality.Add("if--not", "if-not");
            SwitchEquality.Add("if-n", "if-not");
            SwitchEquality.Add("if-!", "if-not");
            SwitchEquality.Add("while-!", "while-not");
            SwitchEquality.Add("while-n", "while-not");
            SwitchEquality.Add("while--not", "while-not");
            SwitchEquality.Add("urlencode--encoding", "urlencode-e");
            SwitchEquality.Add("urldecode--encoding", "urldecode-e");
        }

        public static ZSSCommand CreateNode(CommandDesc cd,ZSS zss) {
            switch (cd.Command) {
                case "if": curNode = new IfCommand(curNode.GetReal(), cd); break;
                case "else": if (curNode is ParentCommandProxy<IfCommand>) curNode = (IfCommand)(curNode as ParentCommandProxy<IfCommand>);
                    if (curNode is IfCommand) ((IfCommand)curNode).CreateElse(); break;
                case "end": curNode = curNode.Parent; break;
                case "while": curNode = new WhileCommand(curNode.GetReal(), cd); break;
                case "call": new CallCommand(curNode.GetReal(), cd); break;
                case "case": curNode = new CaseCommand(curNode.GetReal(), cd); break;

                case "site": rootNode.Site = cd.Arguments[0]; break;
                case "name": rootNode.Name = cd.Arguments[0]; break;
                case "standard": rootNode.StandardLevel = int.Parse(cd.Arguments[0]); break;
                case "useragent": rootNode.UserAgent = cd.Arguments[0]; break;
                case "encoding": rootNode.Encoding = Encoding.GetEncoding(cd.Arguments[0]); break;
                case "keepcookie": rootNode.IsKeepCookie = true; break;
                case "timeout": rootNode.Timeout = int.Parse(cd.Arguments[0]); break;
                case "reconnect": rootNode.Reconnect = int.Parse(cd.Arguments[0]); break;
                
                case "get": new GetCommand(curNode.GetReal(), cd); break;
                case "post": new PostCommand(curNode.GetReal(), cd); break;
                case "capture": new CaptureCommand(curNode.GetReal(), cd); break;
                case "multicapture": curNode = new MultiCaptureCommand(curNode.GetReal(), cd); break;
                case "set": new SetCommand(curNode.GetReal(), cd); break;
                case "log": new LogCommand(curNode.GetReal(), cd); break;                
                case "replace": new ReplaceCommand(curNode.GetReal(), cd); break;
                case "substring": new SubStringCommand(curNode.GetReal(), cd); break;
                case "trim": new TrimNode(curNode.GetReal(), cd); break;
                case "htmlencode": new HtmlEncodeCommand(curNode.GetReal(), cd); break;
                case "htmldecode": new HtmlDecodeCommand(curNode.GetReal(), cd); break;
                case "urlencode": new UrlEncodeNode(curNode.GetReal(), cd); break;
                case "urldecode": new UrlDecodeNode(curNode.GetReal(), cd); break;
                case "jsencode": new JsEncodeCommand(curNode.GetReal(), cd); break;
                case "jsdecode": new JsDecodeCommand(curNode.GetReal(), cd); break;                
                case "jsondecode": new JsonDecodeCommand(curNode.GetReal(), cd); break;                
                
                default: if (zss.NodeCreator != null) {
                        ZSSCommand n = zss.NodeCreator(null, new NodeCreateEventArgs { Parent = curNode.GetReal(), CommandDesc = cd });
                        if (n == null) throw new ZSSParseException("Bad Command '" + cd.Command + "'");
                        if (n is IParentCommand) curNode = (IParentCommand)n;
                    } else throw new ZSSParseException("Bad Command '" + cd.Command + "'");
                    break;
            }
            return null;
        }

        private static String[] SplitLine(String input) {
            if (input.Trim().StartsWith("#")) return null;
            var ms=R_CMD.Matches(input);
            List<String> ss = new List<string>();
            foreach (Match m in ms) {
                var s = m.Value;
                /*if (s.First() == '"'&&s.Last()=='"') {
                    s = s.Substring(1, s.Length - 2);
                }*/
                ss.Add(s);
            }
            return ss.ToArray();
        }


        public static CommandDesc ParseLine(String input, ZSS zss) {
            //String[] ss = input.Split(' ', '\t', '\n', '\r').Where(s => !s.IsNullOrWhiteSpace()).ToArray();
            String[] ss = SplitLine(input);
            if (ss == null) return null;
            CommandDesc cd = new CommandDesc();
            cd.Command = ss[0].ToLower();
            if (CommandEquality.ContainsKey(cd.Command)) cd.Command = CommandEquality[cd.Command];
            else if (zss.CommandEquality.ContainsKey(cd.Command)) cd.Command = zss.CommandEquality[cd.Command];
            for (int i = 1; i < ss.Length; ++i) {
                if (ss[i] == "") {
                    cd.Arguments.Add(ss[i]);
                }else if (ss[i][0] == '-') {
                    if (ss[i] == "-") {
                        cd.Arguments.Add(ss[++i]);
                    } else if (Char.IsDigit(ss[i][1])) {
                        cd.Arguments.Add(ss[i]);
                    } else {
                        ZSSSwitch zs = new ZSSSwitch(cd.Command, ss, ref i, zss);
                        cd.Switches.Add(zs);
                    }
                } else if (ss[i] == "<" || ss[i] == ">") {
                    ZSSSwitch zs = new ZSSSwitch(cd.Command, ss, ref i, zss);
                    cd.Switches.Add(zs);
                } else {
                    cd.Arguments.Add(ss[i]);
                }
            }
            return cd;
        }

        public static ZSSRoot Parse(String[] lines, ZSS zss) {
            int l = 0;
            try {
                curNode = rootNode = new ZSSRoot();
                foreach (var s in lines) {
                    ++l;
                    if (!s.IsNullOrWhiteSpace()) {
                        var cd = ParseLine(s, zss);
                        if (cd == null) continue;
                        CreateNode(cd, zss);                        
                    }
                }
                var ret = rootNode;
                rootNode = null;
                curNode = null;
                return ret;
            } catch (ZSSParseException e) {
                throw new ZSSParseException(l + ": " + e.Message);
            } catch {
                throw new ZSSParseException("Unknown Exception Occured When Parsing Line " + l);
            }

        }
    }

    public class ZSSSwitch {
        public String OwnerCommand { get; set; }
        public String Name { get; set; }
        public List<String> Arguments { get; private set; }
        private ZSS zss;

        public String SysName {
            get {
                var s=(OwnerCommand + Name).ToLower();
                if (ZSSParser.SwitchEquality.ContainsKey(s)) return ZSSParser.SwitchEquality[s];
                if (zss.SwitchEquality.ContainsKey(s)) return zss.SwitchEquality[s];
                return s;
            }
        }

        /*public ZSSSwitch(String ownerCommand, String name) {
            OwnerCommand = ownerCommand;
            Name = name;
            Arguments = new List<string>();
        }*/

        public ZSSSwitch(String ownerCommand, String[] args, ref int index, ZSS zss) {
            this.zss = zss;
            OwnerCommand = ownerCommand;
            Name = args[index];
            Arguments = new List<string>();
            if (Name == "<" || Name == ">") {
                Arguments.Add(args[index + 1]);
                index += 1;
            } else {
                int l = 0;
                if (ZSSParser.SwitchArgCountTable.ContainsKey(SysName)) {
                    l = ZSSParser.SwitchArgCountTable[SysName];
                } else if (zss.SwitchArgCountTable.ContainsKey(SysName)) {
                    l = zss.SwitchArgCountTable[SysName];
                } else throw new ZSSParseException("Unrecognizable Switch:'" + SysName + "'");
                for (int i = 0; i < l; ++i) Arguments.Add(args[index + i + 1]);
                index += l;
            }
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (obj is ZSSSwitch) return SysName == (obj as ZSSSwitch).SysName;
            if (obj is String) return SysName == (String)obj;
            return false;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }

        public override string ToString() {
            if (Arguments == null) return SysName;
            return SysName + " " + String.Join(" ", Arguments);
        }
    }
}