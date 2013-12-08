using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Utility {
    class RuleParser : Parser{
        private const String S_ROOT = "rule";
        private const String S_LINE = "p";
        private const String S_LEVEL = "l";
        private const String S_LIST = "list";
        private const String S_TREE = "tree";
        private const String S_SELECTION = "selection";
        private const String S_ENABLE = "enable";
        private const String S_TRUE = "true";
        private const String S_FALSE = "false";

        public static void Load() {
            String[] files = Directory.GetFiles(G.PATH_RULE);
            String optionfile = null;
            foreach (String file in files) {
                if (Path.GetExtension(file).ToLower() == G.EXT_LISTRULE) G.Rules.List.Add(file);
                else if (Path.GetExtension(file).ToLower() == G.EXT_TREERULE) G.Rules.Tree.Add(file);
                else if (Path.GetExtension(file).ToLower() == G.EXT_RULEOPTION) optionfile = file;
            }
            if (optionfile != null) {
                var r = new Reader(optionfile).Child(S_LIST)
                    .Read(S_SELECTION, (n) => { G.Rules.ListSelection = n.InnerText; })
                    .Read(S_ENABLE, (n) => { G.Rules.IsListEnable = n.InnerText == S_TRUE; })
                    .Parent.Child(S_TREE)
                    .Read(S_SELECTION, (n) => { G.Rules.TreeSelection = n.InnerText; })
                    .Read(S_ENABLE, (n) => { G.Rules.IsTreeEnable = n.InnerText == S_TRUE; })
                    .Parent;
            }
        }

        public static void Save() {
            try { SaveList(); } catch (IOException) { }
            try { SaveTree(); } catch (IOException) { } 
            new Writer(S_ROOT)
                .Start(S_LIST)
                    .Write(S_SELECTION, G.Rules.ListSelection)
                    .Write(S_ENABLE, G.Rules.IsListEnable ? S_TRUE : S_FALSE)
                .End
                .Start(S_TREE)
                    .Write(S_SELECTION, G.Rules.TreeSelection)
                    .Write(S_ENABLE, G.Rules.IsTreeEnable ? S_TRUE : S_FALSE)
                .End
            .WriteTo(G.PATH_RULEOPTION + S_ROOT + G.EXT_RULEOPTION);
        }

        public static bool Load(String filename) {
            if (!A.IsFullFilename(filename)) return false;
            if (Path.GetExtension(filename) == G.EXT_LISTRULE) {
                new Reader(filename).Read(S_LEVEL, (n) => { G.Rules.ListText = n.InnerText; });
            } else if (Path.GetExtension(filename) == G.EXT_TREERULE) {
                G.Rules.TreeText.Clear();
                new Reader(filename).ForChildren(S_LEVEL, (n) => { G.Rules.TreeText.Add(n.InnerText); });
            }
            return true;
        }

        public static String SaveList() {
            if (G.Rules == null) return null;
            if (G.Rules.ListRule == null) return null;
            String filename = null;
            foreach (var s in G.Rules.ListRule)
                if (s != null && s.Trim() != "") {
                    filename = s.Trim();
                    break;
                }
            if (filename == null || filename == "") return null;            
            filename = G.PATH_LISTRULE + A.EncodeFilename(filename) + G.EXT_LISTRULE;
            if (File.Exists(filename)) throw new IOException("doublicate filename");
            new Writer(S_ROOT).Write(S_LEVEL, G.Rules.ListText).WriteTo(filename);
            return filename;
        }

        public static String SaveTree() {
            if (G.Rules == null) return null;
            if (G.Rules.TreeRule == null) return null;
            String filename = null;
            foreach (var ss in G.Rules.TreeRule) {
                foreach (var s in ss) {
                    if (s != null && s.Trim() != "") filename = s.Trim();
                    break;
                }
                if (filename != null && filename != "") break;
            }
            if (filename==null||filename=="") return null;
            filename = G.PATH_TREERULE + A.EncodeFilename(filename) + G.EXT_TREERULE;
            if (File.Exists(filename)) throw new IOException("doublicate filename");
            var w = new Writer(S_ROOT);
            foreach (var s in G.Rules.TreeText) w = w.Write(S_LEVEL, s);
            w.WriteTo(filename);
            return filename;
        }
    }
}