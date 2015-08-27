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
using SEGarden.Logic;

namespace SEGarden.Messaging {
	
	/// <summary>
	/// Base class for all messages sent from client to server
	/// </summary>
	public class MessageContainer {

        #region Static

        // The size of the header data, i.e. the properties below that we include
        private const int HEADER_SIZE = sizeof(ushort) * 3 + sizeof(long) * 2;
        // The max size message SE will send
        private const int MAX_MESSAGE_SIZE = 4096; 

        private static Logger Log = new Logger("SEGarden.Messaging.MessageContainer");

        public static MessageContainer FromBytes(byte[] buffer) {
            VRage.ByteStream stream = new VRage.ByteStream(buffer, buffer.Length);

            MessageContainer message = new MessageContainer();
            message.MessageTypeId = stream.getUShort();
            //Log.Trace("Deserialized MessageTypeId " + message.MessageTypeId, "ToBytes");
            message.SourceId = stream.getUlong();
            //Log.Trace("Deserialized SourceId " + message.SourceId, "ToBytes");
            message.SourceType = (RunLocation)stream.getUShort();
            //Log.Trace("Deserialized SourceTypeId " + message.SourceType, "ToBytes");
            int length = (int)stream.getUShort();
            //Log.Trace("Deserialized Body.Length " + length, "ToBytes");
            message.Body = stream.getByteArray(length);
            //Log.Trace("Deserialized Body of length " + message.Body.Length, "ToBytes");

            return message;
        }

        #endregion
        #region Instance

        // Included in message:
        public ushort MessageTypeId; // { get; private set; }
		public ulong SourceId { get; private set; }
        public RunLocation SourceType { get; private set; }
        public byte[] Body;

        // Just used for sending:
        public ushort MessageDomainId;
        public ulong DestinationId; // { get; private set; }
        public MessageDestinationType DestinationType; //{ get; private set; }
        public bool Reliable = true; // { get; protected set; }

        private byte[] ToBytes() {
            VRage.ByteStream stream = 
                new VRage.ByteStream(HEADER_SIZE + Body.Length, true);

            //Log.Trace("Serializing MessageTypeId " + MessageTypeId, "ToBytes");
            stream.addUShort(MessageTypeId);
            //Log.Trace("Serializing SourceId " + SourceId, "ToBytes");
            stream.addUlong(SourceId);
            //Log.Trace("Serializing SourceTypeID " + (ushort)SourceType, "ToBytes");
            stream.addUShort((ushort)SourceType);
            //Log.Trace("Serializing Body.Length " + (ushort)Body.Length, "ToBytes");
            stream.addUShort((ushort)Body.Length);
            //Log.Trace("Serializing Body", "ToBytes");
            stream.Write(Body, 0, Body.Length);
            return stream.Data;
        }
     
        public void Send() {
            //Log.Trace("Sending Message Container", "Send");

            SourceType = SEGarden.GardenGateway.RunningOn;
            //Log.Trace("SourceType : " + SourceType, "Send");

            if (SourceType == RunLocation.Client || SourceType == RunLocation.Singleplayer)
                SourceId = (ulong)MyAPIGateway.Session.Player.SteamUserId;
            else SourceId = 0;

            //Log.Trace("SourceId : " + SourceId, "Send");

            byte[] buffer = ToBytes();

            if (DestinationId == 0) DestinationType = MessageDestinationType.Server;

            //Log.Trace("DestinationType : " + DestinationType, "Send");

            switch (DestinationType) {

                case MessageDestinationType.Server:
                    //Log.Info("Sending to server " + DestinationId, "SendMessage");
                    MyAPIGateway.Multiplayer.SendMessageToServer(
                        MessageDomainId, buffer, Reliable);
                    break;

                case MessageDestinationType.Player:
                    //Log.Info("Sending to player " + DestinationId, "SendMessage");
                    MyAPIGateway.Multiplayer.SendMessageTo(
                        MessageDomainId, buffer, DestinationId, Reliable);
                        //Log.Info("Sent message to player " + DestinationId, "SendMessage");
                    break;
            }

            Log.Info("Sent packet of " + buffer.Length + " bytes", "SendMessage");
        }

        #endregion

    }

}
