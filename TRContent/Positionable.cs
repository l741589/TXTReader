using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRContent;
using Zlib.Utility;

namespace TRContent {
    public interface IPositionable {
        IContentItemAdapter Chapter { get; }
        int Position { get; set; }
        double Offset { get; set; }
    }

    public class Positionable : IPositionable{
        public IContentItemAdapter Chapter { get; private set; }
        public int Position { get; set; }
        public double Offset { get; set; }

        public Positionable(IPositionable src){
            this.Position = src.Position;
            this.Offset = src.Offset;
            this.Chapter = src.Chapter;
        }
    }

    public static class PositionableExtension {
        public static void AssignTo(this IPositionable source,IPositionable target) {
            if (target.IsNull()) return;
            //if (target == Book.Empty) return;
            if (source.Chapter == null) {
                target.Position = source.Position;
                target.Offset = source.Offset;
            } else {
                target.Position = source.Chapter.AbsolutePosition + source.Position;
                target.Offset = source.Offset;
            }
        }

        public static IPositionable Clone(this IPositionable source) {
            return new Positionable(source);
        }
    }
}
