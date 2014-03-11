using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TXTReader.Plugins;

namespace TRSpider {
    class Entry : PluginEntry{

        public override string[] Dependency {
            get { return new String[]{"TXTReader"}; }
        }

        public override string Description {
            get {
                return "提供小说爬虫支持，\n" +
                    "爬虫文件放置在spider目录下\n" +
                    "使用ZSpiderScript语言";
            }
        }

        public override void OnLoad(StartupEventArgs e) {
            //throw new Exception("The method or operation is not implemented.");
        }

        public override void OnWindowCreate(Window window) {
            AddToolTab("爬虫", new SpiderPanel());
        }

        public override void OnUnload(ExitEventArgs e) {
            //throw new Exception("The method or operation is not implemented.");
        }
    }
}
