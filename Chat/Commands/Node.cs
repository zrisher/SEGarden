﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SEGarden.Notifications;

namespace SEGarden.Chat.Commands {

    public abstract class Node {

        private static ChatNotification EmptyNotice = 
            new ChatNotification() { 
                Text = "Empty biatches" 
            };

        protected static ChatNotification NoticeUnAuthorized = 
            new ChatNotification() {
                Text = "You are not authorized to use this command",
            };

        public String Word { get; private set; }
        public String ShortInfo { get; private set; }
        public String LongInfo { get; private set; }
        public String FullCommand { get; private set; }
        public int Security { get; private set; }

        public Node Parent;
        public String InfoAsChild; //{ get; protected set; }
        protected String InfoAsTop; // { get; protected set; }
        protected Notifications.Notification InfoNotice;


        protected Node(
            String word,
            String shortInfo = "",
            String longInfo = "",
            int security = 0) {

            Word = word;
            ShortInfo = shortInfo;
            LongInfo = longInfo;
            Security = security;
        }

        public bool Matches(String word) {
            return word == Word;
        }

        /// <summary>
        /// Nodes are dependent on the Tree and local Security for some of their info
        /// If these have changed, refresh the tree for the latest info
        /// 
        /// Child classes are likely to have more complex logic after this
        /// </summary>
        public void refresh(int security = 0) {
            if (Parent == null) FullCommand = "/" + Word;
            else FullCommand = Parent.FullCommand + " " + Word;
        }

        /// <summary>
        /// This should always be overridden
        /// </summary>
        public Notifications.Notification Invoke(List<String> inputs, int security) {
            return EmptyNotice;
        }

    }

}
