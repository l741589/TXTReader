using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Utility {
    class OptionsParser : Parser{
        public const String S_ROOT = "option";
        public const String S_MINCHAPTERLENGTH = "minchapterlength";
        public const String S_MAXCHAPTERLENGTH = "maxchapterlength";
        public const String S_SPEED = "speed";
        public const String S_FLOATMESSAGE = "floatmessage";
        public const String S_OPEN = "open";
        public const String S_FPS = "fps";
        public const String S_CHAPTERTITLE = "chaptertitle";
        public const String S_TIME = "time";
        public const String S_PROGRESS = "progress";
        public const String S_BOOK = "book";
        public const String S_ISBORDERED = "isbordered";
        public const String S_FULLSCREEN = "isfullscreen";
        public const String S_LOG = "log";
        public const String S_FILTERSPACE = "filterspace";


        public static void Save() {
            new Writer(S_ROOT)
                .Write(S_MINCHAPTERLENGTH, G.Options.MinChapterLength)
                .Write(S_MAXCHAPTERLENGTH, G.Options.MaxChapterLength)
                .Write(S_SPEED, G.Options.Speed)
                .Start(S_FLOATMESSAGE).Attr(S_OPEN, G.Options.IsFloatMessageOpen)
                    .Write(S_CHAPTERTITLE, G.Options.FloatMessage.ChapterTitle, new bool[0])
                    .Write(S_FPS, G.Options.FloatMessage.Fps, new bool[0])
                    .Write(S_TIME, G.Options.FloatMessage.Time, new bool[0])
                    .Write(S_SPEED, G.Options.FloatMessage.Speed, new bool[0])
                    .Write(S_PROGRESS, G.Options.FloatMessage.Progress, new bool[0])
                    .Write(S_LOG, G.Options.FloatMessage.Log, new bool[0])
                .End
                .Write(S_BOOK, (App.Current as App).FileName)
                .Write(S_ISBORDERED, G.Options.IsBordered, new bool[0])
                .Write(S_FULLSCREEN, G.Options.IsFullScreen, new bool[0])
                .Write(S_FILTERSPACE, G.Options.IsFilterSpace, new bool[0])
                .WriteTo(G.NAME_OPTION);
        }

        public static void Load() {
            new Reader(G.NAME_OPTION)
                .Read(S_MINCHAPTERLENGTH, (n) => { G.Options.MinChapterLength = int.Parse(n.InnerText); })
                .Read(S_MAXCHAPTERLENGTH, (n) => { G.Options.MaxChapterLength = int.Parse(n.InnerText); })
                .Read(S_SPEED, (n) => { G.Options.Speed = int.Parse(n.InnerText); })
                .Child(S_FLOATMESSAGE).Do((n) => { G.Options.IsFloatMessageOpen = bool.Parse(n.Attributes[S_OPEN].Value); })
                    .Read(S_CHAPTERTITLE, (n) => { G.Options.FloatMessage.ChapterTitle = bool.Parse(n.InnerText); })
                    .Read(S_FPS, (n) => { G.Options.FloatMessage.Fps = bool.Parse(n.InnerText); })
                    .Read(S_TIME, (n) => { G.Options.FloatMessage.Time = bool.Parse(n.InnerText); })
                    .Read(S_SPEED, (n) => { G.Options.FloatMessage.Speed = bool.Parse(n.InnerText); })
                    .Read(S_PROGRESS, (n) => { G.Options.FloatMessage.Progress = bool.Parse(n.InnerText); })
                    .Read(S_LOG, (n) => { G.Options.FloatMessage.Log = bool.Parse(n.InnerText); })
                .Parent
                .Read(S_BOOK, (n) => { (App.Current as App).FileName = n.InnerText; })
                .Read(S_ISBORDERED, (n) => { G.Options.IsBordered = bool.Parse(n.InnerText); })
                .Read(S_FULLSCREEN, (n) => { G.Options.IsFullScreen = bool.Parse(n.InnerText); })
                .Read(S_FILTERSPACE, (n) => { G.Options.IsFilterSpace = bool.Parse(n.InnerText); });
        }
    }
}
