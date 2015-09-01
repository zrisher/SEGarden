using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden.Commons.Conceal {

    public enum MessageType : ushort {
        ConcealMessageBase,
        ConcealQueuedMessage,
        ConcealCancelledMessage,
    }

}
