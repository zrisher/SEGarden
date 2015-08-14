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
using SEGarden.Logging;
using SEGarden.Chat.Commands;

namespace SEGarden.Chat {

	class ChatManager {

		private static Logger Log = new Logger("SEGarden.Chat.ChatManager");
        private List<Tree> CommandTrees = new List<Tree>();
        private bool Ready;
        private int _LocalSecurity;

        int LocalSecurity {
            get { return _LocalSecurity; }
            set {
                _LocalSecurity = value;
                RefreshCommandTreeInfo();
            }
        }

        public ChatManager(int localSecurity = 0) {
            Log.Trace("New command processor", "ctr");
            LocalSecurity = localSecurity;
		}


        /// <summary>
        /// This shouldn't be called until MyAPIGateway is ready!
        /// </summary>
        public virtual void Initialize() {
            if (!Ready) {
                MyAPIGateway.Utilities.MessageEntered += handleChatInput;
                Ready = true;
                Log.Trace("Initialized chat handler", "Initialize");
            }
		}

        public void addCommands(Tree commandTree) {

            Log.Trace("Adding command tree " + commandTree.Word, "addCommands");

            if (commandTree == null) return;
            foreach (Tree storedTree in CommandTrees) {
                if (storedTree.Word == commandTree.Word) {
                    Log.Warning("Failed to register new chat command tree " + 
                        commandTree.Word +", collides with existing.", 
                        "addCommands");
                    return;
                }
            }
            commandTree.Refresh(LocalSecurity);
            CommandTrees.Add(commandTree);
        }


        public virtual void Terminate() {
            if (Ready) {
                MyAPIGateway.Utilities.MessageEntered -= handleChatInput;
                Ready = false;
                Log.Trace("Terminating chat handler", "Terminate");
            }
		}

        private void handleChatInput(string messageText, ref bool sendToOthers) {

            Log.Trace("Handling chat input " + messageText, "handleChatInput");

            WindowNotification test;

            if (messageText[0] != '/' || messageText.Length < 2)
                return;

            List<string> inputs = ParseInput(messageText.Substring(1));

            List<String> remainingInputs;
            Notification resultNotification;

            foreach (Tree commandTree in CommandTrees) {
                if (commandTree.Matches(inputs[0])) {

                    sendToOthers = false;
                    remainingInputs = inputs;
                    remainingInputs.RemoveAt(0);

                    resultNotification = commandTree.Invoke(
                        remainingInputs, LocalSecurity);

                    if (resultNotification != null) {
                        resultNotification.Raise();
                    }
                    else {
                        Log.Warning("Null command invoke result for " +
                            commandTree.Word, "handleChatInput");
                    }     

                }
            }
		}

        public void RefreshCommandTreeInfo() {
            if (CommandTrees == null) return;
            foreach (Tree commandTree in CommandTrees) {
                commandTree.Refresh(LocalSecurity);
            }
        }

        public static List<string> ParseInput(string input) {

            bool escape = false;
            bool quote = false;

            StringBuilder part = new StringBuilder();
            List<string> parts = new List<string>();

            for (int pos = 0; pos < input.Length; pos++) {
                char character = input[pos];

                if (escape) {
                    escape = false;
                    goto AddToPart;
                }
                
                if (character == '\\') {
                    escape = true;
                    goto Skip;
                }
                
                if (quote) {
                    if (character == '"') {
                        quote = false;
                        goto FinishedPart;
                    } 
                    else {
                        goto AddToPart;
                    }
                }
                
                if (character == '"') {
                    quote = true;
                    goto Skip;
                }
                
                if (char.IsWhiteSpace(character) ) {
                    goto FinishedPart;
                }

            AddToPart:
                part.Append(character);
                continue;
            FinishedPart:
                if (part.Length > 0) {
                    parts.Add(part.ToString());
                    part.Clear();
                }
                continue;
            Skip:
                continue;
            }

            // Add last part if was building
            if (part.Length > 0) parts.Add(part.ToString());

            return parts;
        }

	}
}
