/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Library.Utils;
using Interfaces = Sandbox.ModAPI.Interfaces;
using InGame = Sandbox.ModAPI.Ingame;

using SEGarden.Core;
using SEGarden.Records;

namespace SEGarden.Messaging {

	/// <summary>
	/// Server side messaging hooks.  Recieves requests from the clients and sends
	/// responses.
	/// </summary>
	public class ServerMessenger {

		static LogicComponents.Logger s_Logger = null;

		private Action<byte[]> localMsgSend;
		public event Action<byte[]> localMsgSent {
			add { localMsgSend += value; }
			remove { localMsgSend -= value; }
		}
		private void sendLocalMsg(byte[] buffer) {
			if (localMsgSend != null)
				localMsgSend(buffer);
		}

		public ServerMessenger() {
			if (s_Logger == null)
				s_Logger = new LogicComponents.Logger("Conquest Core", "RequestProcessor");

			if (MyAPIGateway.Multiplayer != null) {
				MyAPIGateway.Multiplayer.RegisterMessageHandler(Constants.GCMessageId, incomming);
				log("Message handler registered", "ctor");
			} else {
				log("Multiplayer null.  No message handler registered", "ctor");
			}
		}

		public void unload() {
			if (MyAPIGateway.Multiplayer != null)
				MyAPIGateway.Multiplayer.UnregisterMessageHandler(Constants.GCMessageId, incomming);
		}

		public void incomming(byte[] buffer) {
			log("Got message of size " + buffer.Length, "incomming");

			try {
				//Deserialize message
				BaseRequest msg = BaseRequest.messageFromBytes(buffer);

				// Process type
				switch (msg.MsgType) {
					case BaseRequest.TYPE.FLEET:
						processFleetRequest(msg as FleetRequest);
						break;
					case BaseRequest.TYPE.SETTINGS:
						processSettingsRequest(msg as SettingsRequest);
						break;
				}
			} catch (Exception e) {
				log("Exception occured: " + e, "incomming");
			}
		}

		public void send(BaseResponse msg) {
			log("Sending " + msg.MsgType + " response", "send");

			if (msg == null)
				return;

			byte[] buffer = msg.serialize();
			MyAPIGateway.Multiplayer.SendMessageToOthers(Constants.GCMessageId, buffer);

			try {
				sendLocalMsg(buffer);
				log("Sent packet of " + buffer.Length + " bytes", "send");
			}
			catch (Exception e) {
				log("Error in onSend(buffer) " + e, "send", LogicComponents.Logger.severity.ERROR);
			}
		}

		private void processFleetRequest(FleetRequest req) {
			// Get an Owner object from the player ID of the request
			GridOwner.OWNER owner = GridOwner.ownerFromPlayerID(req.ReturnAddress);

			// Retrieve that owner's fleet
			FactionFleet fleet = SEGarden.Core.StateTracker.
				getInstance().getFleet(owner.FleetID, owner.OwnerType);

			// Get the fleet's juicy description
			String body = fleet.classesToString();

			// build the title
			String title = "";
			switch (owner.OwnerType) {
				case GridOwner.OWNER_TYPE.FACTION:
					title = "Your Faction's Fleet:";
					break;
				case GridOwner.OWNER_TYPE.PLAYER:
					title = "Your Fleet";
					break;
			}

			// send the response
			DialogResponse resp = new DialogResponse() {
				Body = body,
				Title = title,
				Destination = new List<long>() { req.ReturnAddress },
				DestType = BaseResponse.DEST_TYPE.PLAYER
			};

			send(resp);
		}

		private void processSettingsRequest(SettingsRequest req) {
			log("", "processSettingsRequest");
			SettingsResponse resp = new SettingsResponse() {
				Settings = ConquestSettings.getInstance().Settings,
				Destination = new List<long>() { req.ReturnAddress },
				DestType = BaseResponse.DEST_TYPE.PLAYER
			};

			send(resp);
		}


		private void log(String message, String method = null, LogicComponents.Logger.severity level = LogicComponents.Logger.severity.DEBUG) {
			if (s_Logger != null)
				s_Logger.log(level, method, message);
		}

	}
}
*/