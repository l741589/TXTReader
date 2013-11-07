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



        public static void Load() {
            String[] files = Directory.GetFiles(G.PATH_RULE);
            String ls = Rules.S_ADD;
            String ts = Rules.S_ADD;
            String optionfile = null;
            foreach (String file in files) {

                if (Path.GetExtension(file).ToLower() == G.EXT_LISTRULE) G.Rules.List.Add(file);
                else if (Path.GetExtension(file).ToLower() == G.EXT_TREERULE) G.Rules.Tree.Add(file);
                else if (Path.GetExtension(file).ToLower() == G.EXT_RULEOPTION) optionfile = file;

            }
            if (optionfile != null) {
                var r = new Reader(optionfile).Child(S_LIST)
                    .Read(S_SELECTION, (n) => { G.Rules.ListSelection = n.InnerText; })
                    .Parent.Child(S_TREE)
                    .Read(S_SELECTION, (n) => { G.Rules.TreeSelection = n.InnerText; })
                    .Parent;
            }
        }

        public static void Save() {
            SaveList();
            SaveTree();
            new Writer(S_ROOT)
                .Start(S_LIST).Write(S_SELECTION, G.Rules.ListSelection).End
                .Start(S_TREE).Write(S_SELECTION, G.Rules.TreeSelection).End
            .WriteTo(G.PATH_RULEOPTION + S_ROOT + G.EXT_RULEOPTION);
        }

        public static bool Load(String filename) {
            if (!A.IsFullFilename(filename)) return false;
            if (Path.GetExtension(filename) == G.EXT_LISTRULE) {
                List<String> ss = new List<String>();
                new Reader(filename).ForChildren(S_LINE, (n) => {
                    ss.Add(n.InnerText);
                });
                G.Rules.CurrentList = ss;
            } else if (Path.GetExtension(filename) == G.EXT_TREERULE) {
                List<List<String>> sss = new List<List<String>>();
                new Reader(filename).ForChildren(S_LEVEL, (n) => {
                    List<String> ss = new List<String>();
                    new Reader(n).ForChildren(S_LINE, (m) => {
                        if (m.InnerText!="") ss.Add(m.InnerText);
                    });
                    if (ss.Count != 0) sss.Add(ss);
                });
                G.Rules.CurrentTree = sss;                
            }
            return true;
        }

        public static String SaveList() {
            if (G.Rules.CurrentList == null) return null;
            String filename = null;
            foreach (var s in G.Rules.CurrentList)
                if (s != null && s.Trim() != "") {
                    filename = s.Trim();
                    break;
                }
            if (filename == null || filename == "") return null;            
            filename = G.PATH_LISTRULE + A.EncodeFilename(filename) + G.EXT_LISTRULE;
            if (File.Exists(filename)) throw new IOException("doublicate filename");
            var w=new Writer(S_ROOT);
            foreach (var s in G.Rules.CurrentList) w = w.Write(S_LINE, s);
            w.WriteTo(filename);
            return filename;
        }

        public static String SaveTree() {
            if (G.Rules.CurrentTree==null) return null;
            String filename = null;
            foreach (var ss in G.Rules.CurrentTree) {
                foreach (var s in ss) {
                    if (s != null && s.Trim() != "") filename = s.Trim();
                    break;
                }
                if (filename != null && filename != "") break;
            }
            if (filename==null||filename=="") return null;            
            filename = G.PATH_TREERULE + filename + G.EXT_TREERULE;
            if (File.Exists(filename)) throw new IOException("doublicate filename");
            var w = new Writer(S_ROOT);
            foreach (var ss in G.Rules.CurrentTree) {
                w = w.Start(S_LEVEL);
                foreach (var s in ss)
                {
                    w.Write(S_LINE, s);
                }
                w = w.End;
            }
            w.WriteTo(filename);
            return filename;
        }

    }
}