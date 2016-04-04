using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Extensions.VRageMath;
using SEGarden.Logging;
using SEGarden.Logic;
using SEGarden.Messaging;
using SEGarden.Notifications;

namespace SEGarden.Commons.Conceal {

    public abstract class ConcealMessageHandler : MessageHandlerBase {

        private static Logger Log =
            new Logger("SEGardens.Commons.Conceal.ConcealMessageHandler");

        public ConcealMessageHandler() : base((ushort)MessageDomain.Conceal) { }

        public override void HandleMessage(ushort messageTypeId, byte[] body,
            ulong senderSteamId, RunLocation sourceType) {

            Log.Trace("Received message typeId " + messageTypeId, "HandleMessage");
            MessageType messageType = (MessageType)messageTypeId;
            Log.Trace("Received message type " + messageType, "HandleMessage");

            switch (messageType) {
                case MessageType.ConcealQueuedMessage:
                    ConcealQueuedMessage msg1 = new ConcealQueuedMessage(body);
                    ConcealQueued(msg1.EntityId);
                    break;
                case MessageType.ConcealCancelledMessage:
                    ConcealCancelledMessage msg2 = new ConcealCancelledMessage(body);
                    ConcealCancelled(msg2.EntityId);
                    break;
            }

        }

        protected abstract void ConcealQueued(long entityId);

        protected abstract void ConcealCancelled(long entityId);

    }

}
