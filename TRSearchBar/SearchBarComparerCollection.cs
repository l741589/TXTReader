using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRSearchBar {
    public class SearchBarComparerCollection : ObservableCollection<ISearchBarComparer> {
        private static SearchBarComparerCollection instance = null;
        public static SearchBarComparerCollection Instance { get { if (instance == null) instance = new SearchBarComparerCollection(); return instance; } }
    }
}
