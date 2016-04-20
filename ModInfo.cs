using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Versioning;

namespace SEGarden {

    class ModInfo {

        public static readonly AppVersion Version = 
            new AppVersion(0, 9, 3, "#GITSHA");

        public static bool DebugMode;

    }
}
