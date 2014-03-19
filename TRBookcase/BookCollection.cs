using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using TRBookcase.Net;
using TXTReader.Interfaces;
using TXTReader.Plugins;
using Zlib.Algorithm;
using Zlib.Utility;

namespace TRBookcase {

    public class BookCollection : ObservableCollection<IBook> {

        private static BookCollection instance = null;
        public static BookCollection Instance {
            get {
                if (instance == null) 
                    instance = new BookCollection();
                return instance;
            }
        }

        protected override void InsertItem(int index, IBook item) {
            if (GetBook(item)!=null) return;
            if (item == null) return;
            base.InsertItem(index, item);
            item = Check(item);
            //item.MoreInfo();
            //item.Upload();
            if (PluginManager.Has("TRSpider")) {
                BookInfoManager.MoreInfo(item);
            }
        }


        

        protected override void RemoveItem(int index) {
            IBook item = this[index];
            base.RemoveItem(index);
            if (item.NotNull()) {
                String path = BookParser.GetBookPath(item);
                //if (File.Exists(path)) {
                //    File.Delete(path);
                //}
            }
        }

        public BookCaseItem GetBook(IBook item) {
            foreach (BookCaseItem b in this) {
                if (b == item) return b;
                if ((b.Source != null && b.Source == item.Source) || (b.Id != null && b.Id == item.Id)) {
                    return b;
                }
            }
            return null;
        }

        public IBook Check(IBook item, Comparison<IBook> com = null) {
            foreach (IBook b in this) {
                if (b == item) continue;
                if (com == null) {
                    if ((b.Source != null && b.Source == item.Source) || (b.Id != null && b.Id == item.Id)) {
                        Remove(item); 
                        item.Unbind(); 
                        return b;
                    }
                } else {
                    if (com(b, item) == 0) {
                        Remove(item);
                        item.Unbind();
                        return b;
                    }
                }
            }
            return item;
        }

        //public async void UploadAll() {
        //    var copy = this.ToList();
        //    foreach (Book b in copy) {
        //        if (b!=null)
        //            await b.Upload();
        //    }
        //}
    }
}
