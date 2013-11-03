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
            base.InsertItem(index, item);
            item.MoreInfo();
            
        }

        protected override void RemoveItem(int index) {
            Book item=this[index];
            File.Delete(BookcaseParser.GetBookPath(item));
            base.RemoveItem(index);
        }
    }
}
