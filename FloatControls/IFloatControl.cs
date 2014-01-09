using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FloatControls {
    public enum FloatPosition { Left, Right, Top, Bottom, LeftTop, LeftBottom, RightTop, RightBottom, Canvas, Root }
    public interface IFloatControl {
        FloatPosition Position { get; }
        String Name { get; }
        Visibility Visibility { get; set; }
        object Tag { get; }
    }
}
