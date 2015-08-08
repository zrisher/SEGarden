/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden.Chat {
    public enum ChatEventType {
        OnChatReceived,
        OnChatSent,
    }

    public struct ChatEvent {
        public ChatEvent(ChatEventType type, DateTime timestamp, ulong sourceUserId, ulong remoteUserId, string message, ushort priority) {
            Type = type;
            Timestamp = timestamp;
            SourceUserId = sourceUserId;
            RemoteUserId = remoteUserId;
            Message = message;
            Priority = priority;
        }

        public ChatEventType Type;
        public DateTime Timestamp;
        public ulong SourceUserId;
        public ulong RemoteUserId;
        public string Message;
        public ushort Priority;

        public ChatEvent(DateTime timestamp, ulong remoteUserId, string message) {
            Timestamp = timestamp;
            RemoteUserId = remoteUserId;
            Message = message;

            //Defaults
            Type = ChatEventType.OnChatReceived;
            SourceUserId = 0;
            Priority = 0;
        }
    }
}
*/