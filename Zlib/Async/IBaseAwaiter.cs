using System.Runtime.CompilerServices;
using System;
namespace Zlib.Async {
    public interface IAwaiter : IAwaiter<object> { }
    public interface IAwaiter<T> : INotifyCompletion{
        T GetResult();
        bool IsCompleted { get; }
    }
}
