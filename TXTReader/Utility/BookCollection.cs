using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using TXTReader.Data;
using TXTReader.WebApi;

namespace TXTReader.Utility {
    class BookCollection : ObservableCollection<Book>{
        protected override void InsertItem(int index, Book item) {
            if (Contains(item)) return;
            if (item == null || item.State == Book.BookState.Missing) return;
            base.InsertItem(index, item);
            if (Check(item)) return;
            item.MoreInfo();
            //item.Upload();
        }

        protected override void RemoveItem(int index) {
            Book item=this[index];
            File.Delete(BookParser.GetBookPath(item));
            base.RemoveItem(index);
        }

        public bool Check(Book item) {
            foreach (Book b in this) {
                if (b == item) continue;
                if (b.Source == item.Source) { Remove(item); return true; }
                if (!String.IsNullOrEmpty(item.Id) && item.Id == b.Id) {
                    if (b.State == Book.BookState.Remote && item.State == Book.BookState.Local) {
                        Remove(b);
                        return true;
                    } else {
                        Remove(item);
                        return true;
                    }
                }
            }
            return false;
        }

        public async void UploadAll() {
            var copy = this.ToList();
            foreach (Book b in copy) {
                if (b!=null)
                    await b.Upload();
            }
        }
    }
}
