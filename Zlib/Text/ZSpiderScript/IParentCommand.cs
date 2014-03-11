using System;
namespace Zlib.Text.ZSpiderScript {
    interface IParentCommand : IZSSCommand{
        void AddChild(ZSSCommand node);
        ParentCommand GetReal();
    }
}
