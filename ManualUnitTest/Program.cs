using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Zlib.Text;
using System.IO;
using Zlib.Text.ZMatchExpression;

namespace ManualUnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //String[] ss = { "*\"第\\#卷 >\" \"[测试添加]第#{章} *\"","*\"第#章 =\" \"[测试添加]第#{节} *\"" };
            //Trmex t = Trmex.Compile(ss);
            //Console.WriteLine(t.regex.ToString());
            //Console.WriteLine(t.LC);
            //foreach (var e in t.inserts)
            //{
            //    Console.WriteLine("[{0}]{1}", e.Key, e.Value);
            //}
            //
            //String s = "VIP章节目录 第#卷你好 第五百二十三章 再见 第五百二十三节 不再见";
            //Regex r = t.regex;
            //Match m=r.Match(s);
            //foreach (Group e in m.Groups)
            //{
            //    Console.WriteLine("[{0}]{1}", e.Index, e.Value);
            //}
            //ChapterDesc cd = t.Match(s);
            //Console.WriteLine(cd.Title);
            //Console.WriteLine(String.Join("\n",cd.SubTitle));
            //Console.ReadKey();

            //String[] ss = new String[] { "\"第#卷\" \"第#章*\"" };
            //Trmex t = Trmex.Compile(ss);
            //t.Match()
            //String s = @"""%"", ""\\"", ""/"", ""*"", ""?"", ""\"""", ""<"", "">"", ""|"", "":"",""胡思公司收""";
            //Console.WriteLine(s);
            //s=A.EncodeFilename(s);
            //Console.WriteLine(s);
            //s=A.DecodeFilename(s);
            //Console.WriteLine(s);
            //Console.WriteLine(Trmex.ToNumber("四亿五"));
            //Console.ReadKey();

            //Task t;
            //try {
            //    var js = File.ReadAllText("e:/test/inf.js");
            //    var xml = Json.ToXml(js);
            //    File.WriteAllText("e:/test/ouf.xml", xml);
            //    var js2 = Json.FromXml(xml);
            //    File.WriteAllText("e:/test/ouf.js", js2);
            //
            //   // Console.ReadKey();
            //} catch(FileNotFoundException) { }

            String s = "\"第#{卷}[章]   *\" \"第#{章}[节]   *\"";
            var zme=ZME.Compile(s);
            var zm = zme.Match("第十卷     比雷亚内战篇     第426章    未能完成的奇袭");
            
        }

        static void printmatch(Match m)
        {
            foreach (Group e in m.Groups)
            {
                Console.WriteLine("[{0}]{1}", e.Index, e.Value);                
            }
        }

       
    }
}

class MyWaiter{

}

class Waitable {
    MyWaiter GetAwaiter() {
        TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

        var t=tcs.Task;
        //tcs.set
        return new MyWaiter();
    }
}