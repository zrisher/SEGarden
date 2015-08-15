using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;

using SEGarden.Messaging;

using SEGarden.Extensions;
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


        public void Send(ulong destSteamId, MessageDestinationType destType, 
            bool reliable = true) {

            Log.Trace("Sending message with domain " + DomainId + 
                " and type " + TypeId, "Send");

            MessageContainer container = new MessageContainer() {
                Body = ToBytes(),
                DestinationId = destSteamId,
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

        public void SendToPlayer(ulong steamId, bool reliable = true) {
            Send(steamId, MessageDestinationType.Player, reliable);
        }

        public void SendToFaction(long factionId, bool reliable = true) {
            IMyFaction faction = MyAPIGateway.Session.Factions.
                TryGetFactionById(factionId);

            if (faction == null) {
                Log.Error("Failed to find faction " + factionId +
                    " to send message to", "SendToFaction");
                return;
            }

            foreach (ulong steamId in faction.SteamIds()) {
                Send(steamId, MessageDestinationType.Player, reliable);
            }
        }
    }
}
