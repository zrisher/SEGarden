using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Library.Utils;
using Interfaces = Sandbox.ModAPI.Interfaces;
using InGame = Sandbox.ModAPI.Ingame;

//using SEGarden.Core;

using SEGarden.Logging;

namespace SEGarden.Messaging {
	/// <summary>
	/// Client side message hooks.  Processed messages coming from the server.
	/// </summary>
	public abstract class MessengerBase {

		private static Logger Log = new Logger("SEGarden.Messaging.MessengerBase");

        private ushort MessageId;

        public MessengerBase(ushort messageId) {
            MessageId = messageId;
            SEGarden.GardenGateway.Messages.AddHandler(MessageId, ReceiveBytes);
        }


        /// <summary>
        /// Inheriting classes need to define their own, since we won't know how
        /// to deserialize their messages due to limitations in msg byte serialization
        /// (See ByteConverterExtenstions)
        /// </summary>
        /// <param name="buffer"></param>
		public virtual void ReceiveBytes(byte[] buffer) {
            Log.Info("Got message of size " + buffer.Length, "ReceiveBytes");
		}

        protected bool IntendedForUs(MessageBase msg) {
            /*
            // Is this message even intended for us?
			if (msg.DestType == BaseResponse.DEST_TYPE.FACTION) {
				IMyFaction fac = MyAPIGateway.Session.Factions.TryGetPlayerFaction(
					MyAPIGateway.Session.Player.PlayerID);
				if (fac == null || !msg.Destination.Contains(fac.FactionId)) {
					return; // Message not meant for us
				}
			} else if (msg.DestType == BaseResponse.DEST_TYPE.PLAYER) {
				long localUserId = (long)MyAPIGateway.Session.Player.PlayerID;
				if (!msg.Destination.Contains(localUserId)) {
					return; // Message not meant for us
				}
			}
             * */
            return true;
        }

        /// <summary>
        /// Inheriting classes should use this to parse received messages
        /// once they've been determined to 
        /// </summary>
        /// <param name="buffer"></param>
        protected abstract void ReceiveMessage(MessageBase msg);


	}
}
