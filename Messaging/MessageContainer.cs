using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SEGarden.Extensions;

using Sandbox.ModAPI;

using SEGarden.Logging;

using SEGarden.Extensions;

using SEGarden.Messaging;
using SEGarden.Logic.Common;

namespace SEGarden.Messaging {
	
	/// <summary>
	/// Base class for all messages sent from client to server
	/// </summary>
	public class MessageContainer {

        private const int MAX_MESSAGE_SIZE = 4096; // Per SE

        private static Logger Log = new Logger("SEGarden.Messaging.MessageBase");

        //protected VRage.ByteStream CurrentStream;

        // The size of the header data, i.e. the properties below that we include
        // in the request.
        private const int HeaderSize = sizeof(ushort) * 3 + sizeof(long) * 2;
        //private int MessageSize { get { return HeaderSize + Body.Length; } }

        // Included in message:
        public ushort MessageTypeId; // { get; private set; }
		public ulong SourceId { get; private set; }
        public RunLocation SourceType{ get; private set; }
        public byte[] Body; //
        //public int BodySize; // send to stop us from having to resize the stream

        // Just used for sending:
        public ushort MessageDomainId;
        public ulong DestinationId; // { get; private set; }
        public MessageDestinationType DestinationType; //{ get; private set; }
        public bool Reliable = true; // { get; protected set; }


    /*
		public Message(ushort messageId, ulong destId, MessageDestinationType destType, bool reliable = true) {
            MessageId = messageId;
            SourceId = MyAPIGateway.Session.Player.SteamUserId;
            HeaderSize = sizeof(long);
		}
     * */

        //protected abstract byte[] ToBytes(); 
        
        /*
        {
            CurrentStream = new VRage.ByteStream(HeaderSize, true);
            CurrentStream.addUlong(SenderId);
            return CurrentStream.Data;
        }
        */

        private byte[] ToBytes() {
            VRage.ByteStream stream = 
                new VRage.ByteStream(HeaderSize + Body.Length); //, true);

            stream.addUShort(MessageTypeId);
            stream.addUlong(SourceId);
            stream.addUShort((ushort)SourceType);
            stream.addUShort((ushort)Body.Length);
            stream.Write(Body, 0, Body.Length);
            return stream.Data;
        }
     

        public static MessageContainer FromBytes(byte[] buffer) {
            VRage.ByteStream stream = new VRage.ByteStream(buffer, buffer.Length);

            MessageContainer message = new MessageContainer();
            message.MessageTypeId = stream.getUShort();
            message.SourceId = stream.getUlong();
            message.SourceType = (RunLocation)stream.getUShort();
            int length = (int)stream.getUShort();
            message.Body = stream.getByteArray(length);

            return message;
        }


        public void Send() {
            SourceId = (ulong)MyAPIGateway.Session.Player.PlayerID;
            SourceType = SEGarden.GardenGateway.RunningOn;

            byte[] buffer = ToBytes();

            switch (DestinationType) {

                case MessageDestinationType.Server:
                    MyAPIGateway.Multiplayer.SendMessageToServer(
                        MessageDomainId, buffer, Reliable);
                    break;

                case MessageDestinationType.Player:
                    MyAPIGateway.Multiplayer.SendMessageTo(
                        MessageDomainId, buffer, DestinationId, Reliable);
                    break;

                case MessageDestinationType.Faction:
                    IMyFaction faction = MyAPIGateway.Session.Factions.
                        TryGetFactionById((long)DestinationId);

                    if (faction == null) {
                        Log.Error("Failed to find faction " + DestinationId +
                            " to send message to", "SendMessage");
                        return;
                    }

                    foreach (long playerId in faction.MemberIds()) {
                        MyAPIGateway.Multiplayer.SendMessageTo(
                            MessageDomainId, buffer, (ulong)playerId, Reliable
                            );
                    }

                    break;
            }


            Log.Info("Sent packet of " + buffer.Length + " bytes", "SendMessage");
        }

    }

}
