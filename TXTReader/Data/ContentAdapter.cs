using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Data {
    enum ContentStatus { None, TooLong, TooShort, ConfusingIndex }

    interface ContentItemAdapter {
        ContentStatus ContentStatus { get; }
        String Title { get; }
        String TotalTitle { get; }
        List<String> Text { get; }
        List<String> TotalText { get; }
        int Level { get; }
        int Length { get; }
        LinkedList<ContentItemAdapter> Children { get; }
        ContentItemAdapter Parent { get; }
    }

    interface ContentAdapter : ContentItemAdapter { }
}
