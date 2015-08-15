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

        private static Logger Log = new Logger("SEGarden.Messaging.MessageContainer");

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

            Log.Trace("Serializing MessageTypeId " + MessageTypeId, "ToBytes");
            stream.addUShort(MessageTypeId);
            Log.Trace("Serializing SourceId " + SourceId, "ToBytes");
            stream.addUlong(SourceId);
            Log.Trace("Serializing SourceTypeID " + (ushort)SourceType, "ToBytes");
            stream.addUShort((ushort)SourceType);
            Log.Trace("Serializing Body.Length " + (ushort)Body.Length, "ToBytes");
            stream.addUShort((ushort)Body.Length);
            Log.Trace("Serializing Body", "ToBytes");
            stream.Write(Body, 0, Body.Length);
            return stream.Data;
        }
     

        public static MessageContainer FromBytes(byte[] buffer) {
            VRage.ByteStream stream = new VRage.ByteStream(buffer, buffer.Length);

            MessageContainer message = new MessageContainer();
            message.MessageTypeId = stream.getUShort();
            Log.Trace("Deserialized MessageTypeId " + message.MessageTypeId, "ToBytes");
            message.SourceId = stream.getUlong();
            Log.Trace("Deserialized SourceId " + message.SourceId, "ToBytes");
            message.SourceType = (RunLocation)stream.getUShort();
            Log.Trace("Deserialized SourceTypeId " + message.SourceType, "ToBytes");
            int length = (int)stream.getUShort();
            Log.Trace("Deserialized Body.Length " + length, "ToBytes");
            message.Body = stream.getByteArray(length);
            Log.Trace("Deserialized Body of length " + message.Body.Length, "ToBytes");

            return message;
        }


        public void Send() {
            Log.Trace("Sending Message Container", "Send");

            SourceType = SEGarden.GardenGateway.RunningOn;
            Log.Trace("SourceType : " + SourceType, "Send");

            if (SourceType == RunLocation.Client)
                SourceId = (ulong)MyAPIGateway.Session.Player.SteamUserId;
            else SourceId = 0;

            Log.Trace("SourceId : " + SourceId, "Send");

            byte[] buffer = ToBytes();

            if (DestinationId == 0) DestinationType = MessageDestinationType.Server;

            Log.Trace("DestinationType : " + DestinationType, "Send");

            switch (DestinationType) {

                case MessageDestinationType.Server:
                    Log.Info("Sending to server " + DestinationId, "SendMessage");
                    MyAPIGateway.Multiplayer.SendMessageToServer(
                        MessageDomainId, buffer, Reliable);
                    break;

                case MessageDestinationType.Player:

                    Log.Info("Sending to player " + DestinationId, "SendMessage");
                    //Log.Info("Local player Id is " + MyAPIGateway.Session.Player.SteamUserId, "SendMessage");

                    // TODO: Seems there is a problem with this on singleplayer
                    // hopefully it's not a problem in multiplayer...
                    /*
                    if (GardenGateway.RunningOn == RunLocation.Singleplayer) {
                        Log.Warning("Sending to server instead of player since " + 
                            "this is singleplayer, but need to make sure this isn't broken!", "SendMessage");

                        MyAPIGateway.Multiplayer.SendMessageToServer(
                            MessageDomainId, buffer, Reliable);
                        break;
                    }
                     * */

                    MyAPIGateway.Multiplayer.SendMessageTo(
                        MessageDomainId, buffer, DestinationId, Reliable);
                        Log.Info("Sent message to player " + DestinationId, "SendMessage");
                    break;
                

            }


            Log.Info("Sent packet of " + buffer.Length + " bytes", "SendMessage");
        }

    }

}
