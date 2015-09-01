using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Logging;

namespace SEGarden.Commons.Conceal {
    abstract class ConcealMessageBase : SEGarden.Messaging.MessageBase {

        public static readonly int SIZE = 0;
        protected static readonly Logger Log =
            new Logger("SEGardens.Commons.Conceal.ConcealMessageBase");

        public ConcealMessageBase(ushort typeId = (ushort)MessageType.ConcealMessageBase) :
            base((ushort)MessageDomain.Conceal, typeId) { }

    }
}

