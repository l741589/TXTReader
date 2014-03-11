using System;
using System.Collections.Generic;
namespace Zlib.Text.ZSpiderScript {
    interface IZSSCommand  {
        List<string> Arguments { get; }
        string Case { get; }
        string Command { get; }
        ZSS Context { get; }
        string DoExecute(string input);
        void Execute(string input);
        ZSSSwitch GetSwich(string name);
        ParentCommand Parent { get; }
        ZSSRoot Root { get; }
        List<ZSSSwitch> Switches { get; }
        string this[int argIndex] { get; }
        string this[string varName] { get; set; }
        object Clone(ParentCommand parent);
    }
}
