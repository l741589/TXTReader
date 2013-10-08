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
        int Length { get; }
        ContentItemAdapter FirstChild { get; }
        ContentItemAdapter LastChild { get; }
        ContentItemAdapter Parent { get; }
        ContentItemAdapter Prevous { get; }
        ContentItemAdapter Next { get; }
    }

    interface ContentAdapter : ContentItemAdapter { }
}
