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

using SEGarden.Notifications;

namespace SEGarden.Chat.Commands {

	class Processor {

		//private static LogicComponents.Logger s_Logger = null;
		//private ClientMessenger m_MailMan;
        /*
		public Processor(ClientMessenger mailMan) {
			log("Started", "CommandProcessor");
			m_MailMan = mailMan;
		}
        */

        // TODO: track multiple trees and offer add/remove methods
        // that check for uniqueness of toplevel word and take care of refreshing
        private Tree CommandTree;
        private bool Loaded;


        public Processor() {
			//log("Started", "CommandProcessor");
			//m_MailMan = mailMan;
		}

        public virtual void LoadData() {
            if (MyAPIGateway.Utilities == null) return;

			MyAPIGateway.Utilities.MessageEntered += handleChatInput;
            Loaded = true;
            
			//log("Chat handler registered", "initialized");
		}

        public virtual void UnloadDataConditional() {
            if (Loaded) UnloadData();
        }

        public virtual void UnloadData() {
            MyAPIGateway.Utilities.MessageEntered -= handleChatInput;
		}

        private void handleChatInput(string messageText, ref bool sendToOthers) {

            WindowNotification test = new WindowNotification() {
                Text = "Help - Classifiers",
            };

            test.Raise();

            /*
            if (messageText[0] != '/')
                return;

            string[] inputs = messageText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);


			try {

				//log("Handling commands " + String.Join(" , ", cmd), "handleChatCommand");
				if (cmd[0].ToLower() != "/gc")
					return;

				sendToOthers = false;
				int numCommands = cmd.Length - 1;
				//log("numCommands " + numCommands, "handleChatCommand");
				if (numCommands > 0) {
					switch (cmd[1].ToLower()) {
						case "about":
						case "help":
							if (numCommands == 1)
                                Utility.General.showDialog("Help", s_HelpText, "Close");
							else
							{
								switch (cmd[2].ToLower())
								{
									case "classes":
                                        Utility.General.showDialog("Help - Classes", helpClassesText(), "Close");
										break;
									case "classifiers":
                                        Utility.General.showDialog("Help - Classifiers", helpClassifiersText(), "Close");
										break;
									case "cps":
                                        Utility.General.showDialog("Help - Control Points", helpCPsText(), "Close");
										break;
									case "licenses":
                                        Utility.General.showDialog("Help - Licenses", s_HelpLicensesText, "Close");
										break;
								}
							}
							break;

						case "fleet":
							if (numCommands == 1) {
								m_MailMan.requestFleet();
							} else {
								switch (cmd[2].ToLower()) {
									case "disown":
										String entityID = "";
										if (numCommands > 2)
											entityID = cmd[3];
										//m_MailMan.requestDisown(cmd[3]);
										break;
								}
							}
							break;

						case "violations":
							//m_MailMan.requestViolations();
							break;

						case "admin":
							// admin fleet listing
							break;
					}
				}
			} catch (Exception e) {
				//log("Exception occured: " + e, "handleChatCommand", LogicComponents.Logger.severity.ERROR);
			}
             * */
		}

        public static List<string> ParseInput(string input) {
            return new List<String>() { "" };
            /*
            bool quote = false;

            List<string> output = new List<string>();

            StringBuilder sb = new StringBuilder();

            for (int pos = 0; pos < input.Length; pos++) {
                char character = input[pos];

                if (character == '"') {
                    if (!quote) {
                        quote = true;
                    }
                    else if (pos + 1 < input.Length && input[pos + 1] == '"') {
                        sb.Append(character);
                        pos++;
                    }
                    else {
                        quote = false;
                    }
                }
                else if (char.IsWhiteSpace(character) && !quote) {
                    output.Add(sb.ToString());
                    sb.Clear();
                }
                else {
                    sb.Append(character);
                }
            }

            if (!quote && sb.Length > 0) {
                output.Add(sb.ToString());
            }
            return output;
             * */
        }

        /*
		private void log(String message, String method = null, LogicComponents.Logger.severity level = LogicComponents.Logger.severity.DEBUG) {
			if (s_Logger == null)
				s_Logger = new LogicComponents.Logger("Conquest Core", "Command Processor");

			s_Logger.log(level, method, message);
		}
         * */
	}
}
