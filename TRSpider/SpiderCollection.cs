using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Utility;

namespace TRSpider {
    public class SpiderCollection : ObservableCollection<ISpider>{
        private static SpiderCollection instance = null;
        public static SpiderCollection Instance {
            get {
                if (instance == null) instance = new SpiderCollection();
                return instance;
            }
        }

        private SpiderCollection() {
            IsLoaded = false;
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
