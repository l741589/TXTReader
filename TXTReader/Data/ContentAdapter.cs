using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Data {
    public enum ContentStatus { None, TooLong, TooShort, ConfusingIndex }

    public interface ContentItemAdapter {

        ContentStatus ContentStatus { get; }
        String Title { get; }
        String TotalTitle { get; }
        List<String> Text { get; }
        List<String> TotalText { get; }
        int Level { get; }
        int Length { get; }
        ChapterCollection Children { get; }
        ContentItemAdapter Parent { get; }
        LinkedListNode<ContentItemAdapter> Node { get; set; }
        int AbsolutePosition { get; }
        int LineCount { get; }
        int TotalLineCount { get; }
    }

    public interface ContentAdapter : ContentItemAdapter { }
}
