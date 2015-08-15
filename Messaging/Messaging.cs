using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden.Messaging {

    public enum MessageDestinationType : ushort {
        None,
        Faction,
        Player,
        Server
    }

}
