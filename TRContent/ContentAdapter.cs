using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRContent {
    public enum ContentStatus { None, TooLong, TooShort, LowLevelConfusingIndex, ConfusingIndex }

    public interface IContentItemAdapter {

        ContentStatus ContentStatus { get; }
        String Title { get; }
        String TotalTitle { get; }
        List<String> Text { get; }
        List<String> TotalText { get; }
        int Level { get; }
        int Length { get; }
        IEnumerable<IContentItemAdapter> Children { get; }
        IContentItemAdapter Parent { get; }
        LinkedListNode<IContentItemAdapter> Node { get; set; }
        int AbsolutePosition { get; }
        int LineCount { get; }
        int TotalLineCount { get; }
        int? Number { get; }
        int SerializeId { get; }
        void Notify();
    }

    public interface IContentAdapter : IContentItemAdapter {
        IContentItemAdapter CurrentChapter { get; set; }
    }
}
