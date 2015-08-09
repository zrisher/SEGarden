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

		private static Logging.Logger Logger = 
            new Logging.Logger("SEGarden.Chat.Commands.Processor");
        private List<Tree> CommandTrees = new List<Tree>();
        private bool Loaded;
        private int _LocalSecurity;

        int LocalSecurity {
            get { return _LocalSecurity; }
            set {
                _LocalSecurity = value;
                RefreshCommandTreeInfo();
            }
        }

        public Processor(int localSecurity = 0) {
            LocalSecurity = localSecurity;
		}

        public virtual void LoadData() {
            if (MyAPIGateway.Utilities == null) return;

			MyAPIGateway.Utilities.MessageEntered += handleChatInput;
            Loaded = true;

            Logger.Info("Chat handler registered", "LoadData");
		}

        public void addCommands(Tree commandTree) {
            if (commandTree == null) return;
            foreach (Tree storedTree in CommandTrees) {
                if (storedTree.Word == commandTree.Word) {
                    Logger.Warning("Failed to register new chat command tree " + 
                        commandTree.Word +", collides with existing.", 
                        "addCommands");
                    return;
                }
            }
            commandTree.refresh(LocalSecurity);
            CommandTrees.Add(commandTree);
        }


        public virtual void UnloadDataConditional() {
            if (Loaded) UnloadData();
        }

        public virtual void UnloadData() {
            MyAPIGateway.Utilities.MessageEntered -= handleChatInput;
		}

        private void handleChatInput(string messageText, ref bool sendToOthers) {

            if (messageText[0] != '/')
                return;

            List<string> inputs = ParseInput(messageText.Substring(1));

            WindowNotification test = new WindowNotification() {
                BigLabel = "GardenPerformance",
                SmallLabel = "ChatCommand Debug",
                Text = String.Format(
                    "Received command\nString: {0}\nParts: {1}",
                    messageText, String.Join(", ",  inputs)
                    )
            };

            test.Raise();

            List<String> remainingInputs;
            Notification resultNotification;

            foreach (Tree commandTree in CommandTrees) {
                if (commandTree.Matches(inputs[0])) {
                    sendToOthers = false;
                    remainingInputs = inputs;
                    remainingInputs.RemoveAt(0);
                    try {
                        resultNotification = commandTree.Invoke(
                            remainingInputs, LocalSecurity);
                        resultNotification.Raise();
                    }
                    catch {
                        // TODO:
                        // log error
                        // make note in logger that Type is indeed depedenant on 
                        // top-level mod, and since this must always be below
                        // it's invariant and can stay there
                        // also make filemanager a session component to
                        // ensure it gets cleaned up at the end
                        // and maybe to help with loading and call earlyness too
                    }
                    return;
                }
            }
		}

        public void RefreshCommandTreeInfo() {
            if (CommandTrees == null) return;
            foreach (Tree commandTree in CommandTrees) {
                commandTree.refresh(LocalSecurity);
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
