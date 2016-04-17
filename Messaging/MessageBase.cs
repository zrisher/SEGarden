using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;

using VRage;
using VRage.Game.ModAPI;

using SEGarden.Messaging;
using SEGarden.Extensions;
using SEGarden.Logging;

namespace SEGarden.Messaging {

    public abstract class MessageBase {

        private static Logger Log = new Logger("SEGarden.Messaging.MessageBase");

        private ushort _DomainId;
        private ushort _TypeId;
        private bool _Reliable;

        /*
        protected ushort DomainId { get { return _DomainID; } }
        protected ushort TypeId { get { return _TypeId; } }
        */

        public MessageBase(ushort domainId, ushort typeId, bool reliable = true) {
            _DomainId = domainId;
            _TypeId = typeId;
            _Reliable = reliable;

            //Log.Trace("Constructed message base with domain " + _DomainId +
            //    " and type " + _TypeId, "Send");
        }
        
        protected abstract byte[] ToBytes();

        /*
        private ushort DomainId; // { get; }
        private ushort TypeId; // { get; }
        
        public MessageBase(ushort domainId, ushort typeId) {
            DomainId = domainId;
            TypeId = typeId;

        }
        */


        public void Send(ulong destSteamId, MessageDestinationType destType) {

            Log.Trace("Sending message with domain " + _DomainId + 
                " and type " + _TypeId, "Send");

            MessageContainer container = new MessageContainer() {
                Body = ToBytes(),
                DestinationId = destSteamId,
                DestinationType = destType,
                DomainId = _DomainId,
                TypeId = _TypeId,
                Reliable = _Reliable
            };

            container.Send();

        }

        public void SendToServer() {
            Send(0, MessageDestinationType.Server);
        }

        public void SendToPlayer(ulong steamId) {
            Send(steamId, MessageDestinationType.Player);
        }

        public void SendToAll() {
            Send(0, MessageDestinationType.All);
        }

        public void SendToFaction(long factionId) {
            IMyFaction faction = MyAPIGateway.Session.Factions.
                TryGetFactionById(factionId);

            if (faction == null) {
                Log.Error("Failed to find faction " + factionId +
                    " to send message to", "SendToFaction");
                return;
            }

            foreach (ulong steamId in faction.SteamIds()) {
                Send(steamId, MessageDestinationType.Player);
            }
        }
    }
}
