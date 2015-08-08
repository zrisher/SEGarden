//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using Sandbox.Common;
//using Sandbox.ModAPI;
//using VRage;

namespace SEModGarden.Logging {

    public static class Severity {

        public enum Level : byte { 
            Off, Fatal, Error, Warning, Info, Debug, Trace, All
        }

        public static MyFontEnum font(Level level = Level.Trace) {
            switch (level) {
                case Level.Info:
                    return MyFontEnum.Green;
                case Level.Trace:
                    return MyFontEnum.White;
                case Level.Debug:
                    return MyFontEnum.Debug;
                case Level.Warning:
                    return MyFontEnum.Red;
                case Level.Error:
                    return MyFontEnum.Red;
                case Level.Fatal:
                    return MyFontEnum.Red;
                default:
                    return MyFontEnum.White;
            }
        }

    }

}
