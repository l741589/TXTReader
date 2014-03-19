using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zlib.Algorithm;
using Zlib.Text.ZSpiderScript;
using Zlib.Utility;

namespace TRSpider {
    public class SpiderCollection : ObservableCollection<ISpider>{
        private String WorkDir { get { return A.CheckDir(TXTReader.G.PATH + @"spider\"); } }

        private static SpiderCollection instance = null;
        public static SpiderCollection Instance {
            get {
                if (instance == null) instance = new SpiderCollection();
                return instance;
            }
        }

        private SpiderCollection() {
            IsLoaded = false;
            Load();
        }

        public void Load() {
            Clear();
            String[] files = Directory.GetFiles(WorkDir, "*.zss");
            foreach (var f in files) {
                try {
                    CustomSpider cs = TRZSS.Instance.Load(f);
                    Add(cs);
                } catch (ZSSParseException ee) {
                    MessageBox.Show("Exception Occurs When Parsing '" + f + "':\n" + ee.Message);
                }
            }
            OnLoaded(this, EventArgs.Empty);
        }

        public bool IsLoaded { get; private set; }
        public event EventHandler Loaded;
        internal void OnLoaded(object sender, EventArgs e){
            this.QuickSort((l, r) => l.StandardLevel - r.StandardLevel);
            IsLoaded = true;
            if (Loaded!=null) Loaded(sender,e);            
        }

        public ISpider this[String name] {
            get {
                return GetSpiderByName(name);
            }
        }

        public ISpider GetSpiderByName(String name) {
            foreach (var e in this) {
                if (e.Name == name) return e;
            }
            return null;
        }
    }
}
