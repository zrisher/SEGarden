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

            ArgNames = argNames;
            Logic = logic;

            if (argNames == null) argNames = new List<String>();
            ArgCount = argNames.Count;

            NoticeArgsError = new WindowNotification() {
                Text = "Expected " + ArgCount + " arguments."
            };
        }

        /// <summary>
        /// FullCommand can change depending on command's position in the Tree
        /// </summary>
        /// <param name="security"></param>
        public void refresh(int security) {
            base.refresh(security);

            String fullCommandWithArgs = FullCommand + " " + String.Join(" ", ArgNames);
            InfoAsTop = fullCommandWithArgs + "\n" + LongInfo;
            InfoAsChild = fullCommandWithArgs + " - " + ShortInfo;
        }


        public Notifications.Notification Invoke(List<String> inputs, int userSecurity) {
            if (inputs == null) inputs = new List<String>();
            if (userSecurity < Security) return NoticeUnAuthorized;
            if (ArgCount != inputs.Count) return NoticeArgsError;
            return Logic(inputs);
        }


    }

}
