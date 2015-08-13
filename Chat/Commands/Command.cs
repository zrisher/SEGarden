using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SEGarden.Logging;
using SEGarden.Notifications;

namespace SEGarden.Chat.Commands {

    public class Command : Node {

        private static Logger Log = new Logger("SEGarden.Chat.Commands");

        public delegate Notifications.Notification RunLogic(List<String> input);

        private List<String> ArgNames;
        private int ArgCount;
        private RunLogic Logic;
        private Notifications.Notification ArgsErrorNotice;
        private Notifications.Notification InvokeErrorNotice;


        public Command(
            String word,
            String shortInfo,
            String longInfo,
            RunLogic logic,
            List<String> argNames = null,
            int security = 0
            )
            : base(word, shortInfo, longInfo, security) {

            Log.Trace("Running constructor for command", "ctr");
            if (argNames == null) argNames = new List<String>();

            ArgNames = argNames;
            Logic = logic;
            ArgCount = argNames.Count;

            ArgsErrorNotice = new ChatNotification() {
                Text = "Expected " + ArgCount + " arguments."
            };

            InvokeErrorNotice = new ChatNotification() {
                Text = "Error invoking command.",
                Sender = "Server"
            };

            Log.Trace("Finished running constructor for command", "ctr");
        }

        /// <summary>
        /// FullCommand can change depending on command's position in the Tree
        /// </summary>
        /// <param name="security"></param>
        public override void Refresh(int security) {
            RefreshFullCommand();

            String fullCommandWithArgs = FullCommand + " " + String.Join(" ", ArgNames);
            InfoAsTop = fullCommandWithArgs + "\n\n" + LongInfo;
            //InfoBadusage = "Incorrect usage.\n\n" + InfoAsTop;
            InfoAsChild = fullCommandWithArgs + " - " + ShortInfo;
            InfoNotice = new WindowNotification() {
                Text = InfoAsTop,
                BigLabel = "Garden Performance",
                SmallLabel = FullCommand
            };

            ArgsErrorNotice = new ChatNotification() {
                Text = "Expected " + ArgCount + " arguments -\n" + fullCommandWithArgs
            };
        }


        public override Notification Invoke(List<String> inputs, int userSecurity) {
            Logger.Trace("Invoking " + FullCommand + " with inputs " + 
                String.Join("", inputs), "Invoke");
            if (userSecurity < Security) return NoticeUnAuthorized;
            if (inputs == null) inputs = new List<String>();
            if (ArgCount != inputs.Count) return ArgsErrorNotice;

            try {
                return Logic(inputs);
            }
            catch (Exception e) {
                Logger.Error("Error invoking provided logic for'" + FullCommand + "' with inputs " + 
                String.Join(", ", inputs) + ":", "handleChatInput");
                Logger.Error(e.ToString(), "handleChatInput");

                return InvokeErrorNotice;
            }
        }


    }

}
