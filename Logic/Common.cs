using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden.Logic.Common {

    public enum RunLocation : ushort {
        None,
        Any,
        Client,
        Server,
        Singleplayer,
    }

    public enum RunStatus : byte {
        NotInitialized, 
        Initialized, 
        Terminated
    }

}
