using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SEGarden.Notifications;

namespace SEGarden.Chat.Commands {

    public abstract class Node {

        /*
        private static ChatNotification EmptyNotice = 
            new ChatNotification() { 
                Text = "Empty biatches" 
            };
        */

        protected static Logging.Logger Logger =
            new Logging.Logger("SEGarden.Chat.Commands.Node");

        protected static ChatNotification NoticeUnAuthorized = 
            new ChatNotification() {
                Sender = "Server",
                Text = "You are not authorized to use this command",
            };

        public String Word { get; private set; }
        public String Alias { get; private set; }
        public String ShortInfo { get; private set; }
        public String LongInfo {
            get {
                if (LongInfoFunc == null) return LongInfoString;
                try {
                    return LongInfoFunc.Invoke();
                }
                catch (Exception e) {
                    // TODO Log error
                    return "";
                }
            }
        }
        public String LongInfoString { get; private set; }
        public Func<String> LongInfoFunc { get; private set; }
        public String FullCommand { get; private set; }
        public int Security { get; private set; }
        public String DomainTitle { get; private set; }

        public Node Parent;
        public String InfoAsChild; //{ get; protected set; }
        protected String InfoAsTop; // { get; protected set; }
        //protected String InfoBadusage;
        protected Notification InfoNotice = new ChatNotification() {
            Sender = "Server",
            Text = "Command not ready."
        };
        //protected Notification BadUsageNotice;

    /*
        private Notification BadUsageNotice(String input) {
            return new WindowNotification(){
                Text = "Unknown usage, did you mean " + "\n\n" InfoAsTop,
                BigLabel = ModName,
                SmallLabel = FullCommand
            }
        };

        */
        protected Node(
            String word,
            String shortInfo = "",
            String longInfo = "",
            int security = 0,
            String alias = null,
            String domainTitle = "Chat Commands") {

            Word = word;
            Alias = alias;
            ShortInfo = shortInfo;
            LongInfoString = longInfo;
            Security = security;
            DomainTitle = domainTitle;
        }

        protected Node(
            String word,
            String shortInfo,
            Func<String> longInfo,
            int security = 0,
            String alias = null,
            String domainTitle = "Chat Commands") {

            Word = word;
            Alias = alias;
            ShortInfo = shortInfo;
            LongInfoFunc = longInfo;
            Security = security;
            DomainTitle = domainTitle;
        }


        public bool Matches(String word) {
            if (word == Word) return true;
            if (Alias != null && word == Alias) return true;
            return false;
        }

        public abstract void Refresh(int security);

        /// <summary>
        /// Nodes are dependent on the Tree and local Security for some of their info
        /// If these have changed, refresh the tree for the latest info
        /// 
        /// Child classes are likely to have more complex logic after this
        /// </summary>
        protected void RefreshFullCommand(int security = 0) {
            if (Parent == null) FullCommand = "/" + Word;
            else FullCommand = Parent.FullCommand + " " + Word;
        }

        /// <summary>
        /// This should always be overridden
        /// </summary>
        public abstract Notification Invoke(List<String> inputs, int security);

    }

}
