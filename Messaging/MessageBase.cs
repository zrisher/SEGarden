using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Messaging;

using SEGarden.Logging;

namespace SEGarden.Messaging {
    abstract class MessageBase {

        private static Logger Log = new Logger("SEGarden.Messaging.MessageBase");

        protected abstract ushort DomainId { get; }
        protected abstract ushort TypeId { get; }
        
        protected abstract byte[] ToBytes();

        /*
        private ushort DomainId; // { get; }
        private ushort TypeId; // { get; }
        
        public MessageBase(ushort domainId, ushort typeId) {
            DomainId = domainId;
            TypeId = typeId;

        }
        */


        public void Send(long destId, MessageDestinationType destType, 
            bool reliable = true) {

            Log.Trace("Sending message with domain " + DomainId + 
                " and type " + TypeId, "Send");

            MessageContainer container = new MessageContainer() {
                Body = ToBytes(),
                DestinationId = (ulong)destId,
                DestinationType = destType,
                MessageDomainId = DomainId,
                MessageTypeId = TypeId,
                Reliable = reliable
            };

            container.Send();
        }

        public void SendToServer(bool reliable = true) {
            Send(0, MessageDestinationType.Server, reliable);
        }

        public void SendToPlayer(long destId, bool reliable = true) {
            Send(destId, MessageDestinationType.Player, reliable);
        }

        public void SendToFaction(long destId, bool reliable = true) {
            Send(destId, MessageDestinationType.Faction, reliable);
        }
    }
}
