using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader.Data;

namespace TXTReader.Utility
{
    //this class is designed to make the enum types capable for data binding with a customized display name.
    public class EnumTypesBinder
    {
        static EnumTypesBinder()
        {
            EffectsDict = new Dictionary<EffectType, string>() { 
            { EffectType.None, "无特效" }, 
            { EffectType.Shadow, "阴影" }, 
            { EffectType.Stroke, "描边" } 
            };

            BackGroundDict = new Dictionary<BackGroundType, string>() { 
            { BackGroundType.SolidColor, "纯色" }, 
            { BackGroundType.Image, "图片" } 
            };
        }

        public static Dictionary<EffectType, string> EffectsDict { get; private set; }
        public static Dictionary<BackGroundType, string> BackGroundDict { get; private set; }
    }
}
