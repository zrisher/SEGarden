using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SEGarden.Extensions;

using Sandbox.ModAPI;

using SEGarden.Logging;

namespace SEGarden.Messaging {
	
	/// <summary>
	/// Base class for all messages sent from client to server
	/// </summary>
	public abstract class MessageBase {

        protected static Logger Log = new Logger("SEGarden.Messaging.MessageBase");

        public enum MessageDestinationType { None, Faction, Player, Server }

        public const int MAX_MESSAGE_SIZE = 4096; // Per SE


        protected VRage.ByteStream CurrentStream;
        protected int HeaderSize;

        public ushort DomainId { get; private set; }
		public ulong SourceId { get; private set; }
        public MessageDestinationType SourceType { get; private set; }
        public ulong DestinationId { get; private set; }
        public MessageDestinationType DestinationType { get; private set; }
        public bool Reliable { get; protected set; }


		public MessageBase(ushort domainId) {
            DomainId = domainId;
            SourceId = MyAPIGateway.Session.Player.SteamUserId;
            HeaderSize = sizeof(long);
		}

        protected abstract byte[] ToBytes(); 
        
        /*
        {
            CurrentStream = new VRage.ByteStream(HeaderSize, true);
            CurrentStream.addUlong(SenderId);
            return CurrentStream.Data;
        }
        */

        public void Send() {
            byte[] buffer = ToBytes();

            switch (DestinationType) {
                case MessageBase.MessageDestinationType.Server:
                    MyAPIGateway.Multiplayer.SendMessageToServer(
                        DomainId, buffer, Reliable);
                    break;
                case MessageBase.MessageDestinationType.Player:
                    MyAPIGateway.Multiplayer.SendMessageTo(
                        DomainId, buffer, DestinationId, Reliable);
                    break;
                case MessageBase.MessageDestinationType.Faction:
                    IMyFaction faction = MyAPIGateway.Session.Factions.
                        TryGetFactionById((long)DestinationId);

                    if (faction == null) {
                        Log.Error("Failed to find faction " + DestinationId +
                            " to send message to", "SendMessage");
                        return;
                    }
                    faction.Members.Select(xa => xa).ToList();
                    break;
            }


            Log.Info("Sent packet of " + buffer.Length + " bytes", "SendMessage");
        }
          
        public abstract MessageBase FromBytes(byte[] bytes);

        /*
        public static abstract MessageBase FromBytes(byte[] bytes);

               			VRage.ByteStream bs =
				new VRage.ByteStream(HeaderSize, true);
			bs.addUShort((ushort)MsgType);
			bs.addLong(SenderId);
			return bs.Data;

                public virtual void FromtBytes()(VRage.ByteStream stream) {
                    MsgType = (TYPE)stream.getUShort();
                    SenderId = stream.getLong();
                }

                public static BaseRequest messageFromBytes(byte[] buffer) {
                    VRage.ByteStream stream = new VRage.ByteStream(buffer, buffer.Length);
                    TYPE t = (TYPE)stream.getUShort();
                    stream.Seek(0, System.IO.SeekOrigin.Begin);

                    BaseRequest msg = null;
                    switch (t) {
                        case TYPE.FLEET:
                            msg = new FleetRequest();
                            break;
                        case TYPE.SETTINGS:
                            msg = new SettingsRequest();
                            break;
                    }

                    if (msg != null)
                        msg.deserialize(stream);
                    return msg;
                }
         *  */
    }

}
