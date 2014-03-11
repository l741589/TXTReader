using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace TRBook {
    class BookCollection : ObservableCollection<Book>{
        protected override void InsertItem(int index, Book item) {
            if (Contains(item)) return;
            if (item == null || item.State == Book.BookState.Missing) return;
            base.InsertItem(index, item);
            item = Check(item);
            //item.MoreInfo();
            //item.Upload();
        }

        protected override void RemoveItem(int index) {
            Book item=this[index];
            //File.Delete(BookParser.GetBookPath(item));
            base.RemoveItem(index);        
        }

        public Book Check(Book item) {
            foreach (Book b in this) {
                if (b == item) continue;
                if (b.Source == item.Source) { Remove(item); return b; }
                if (!String.IsNullOrEmpty(item.Id) && item.Id == b.Id) {
                    if (b.State == Book.BookState.Remote && item.State == Book.BookState.Local) {
                        Remove(b);
                        return item;
                    } else {
                        Remove(item);
                        return b;
                    }
                }
            }
            return item;
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
