using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SEGarden.Notifications;

namespace SEGarden.Chat.Commands {

    public class Command : Node {

        public delegate Notifications.Notification RunLogic(List<String> input);

        private List<String> ArgNames;
        private int ArgCount;
        private RunLogic Logic;
        private Notifications.Notification NoticeArgsError;


        public Command(
            String word,
            String shortInfo,
            String longInfo,
            RunLogic logic,
            List<String> argNames = null,
            int security = 0
            )
            : base(word, shortInfo, longInfo, security) {
            
            if (argNames == null) argNames = new List<String>();

            ArgNames = argNames;
            Logic = logic;
            ArgCount = argNames.Count;

            NoticeArgsError = new WindowNotification() {
                Text = "Expected " + ArgCount + " arguments."
            };
        }

        /// <summary>
        /// FullCommand can change depending on command's position in the Tree
        /// </summary>
        /// <param name="security"></param>
        public override void Refresh(int security) {
            RefreshFullCommand();

            String fullCommandWithArgs = FullCommand + " " + String.Join(" ", ArgNames);
            InfoAsTop = fullCommandWithArgs + "\n\n" + LongInfo;
            InfoAsChild = fullCommandWithArgs + " - " + ShortInfo;
            InfoNotice = new WindowNotification() {
                Text = InfoAsTop,
                BigLabel = "Garden Performance",
                SmallLabel = FullCommand
            };


        }


        public override Notification Invoke(List<String> inputs, int userSecurity) {
            Logger.Trace("Invoking " + FullCommand + " with inputs " + 
                String.Join("", inputs), "Invoke");
            if (userSecurity < Security) return NoticeUnAuthorized;
            if (inputs == null) inputs = new List<String>();
            if (ArgCount != inputs.Count) return InfoNotice; //NoticeArgsError;

            try {
                return Logic(inputs);
            }
            catch (Exception e) {
                Logger.Error("Error invoking provided logic for'" + FullCommand + "' with inputs " + 
                String.Join(", ", inputs) + ":", "handleChatInput");
                Logger.Error(e.ToString(), "handleChatInput");
            }

            return null;
        }


    }

}
