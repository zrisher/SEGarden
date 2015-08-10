using System;
using System.Collections.Generic;
using System.Text;

using Sandbox.Common;
using Sandbox.ModAPI;

namespace SEGarden.Notifications {

    public abstract class Notification {
        //public NotificationType Type = NotificationType.None;
        public String Text;

        public abstract void Raise();
    }

    public class AlertNotification : Notification {
        public MyFontEnum Color = MyFontEnum.White;
        public int DisplaySeconds = 2;

        public override void Raise() {
            MyAPIGateway.Utilities.ShowNotification(
                Text, DisplaySeconds * 1000, Color);
        }
    }

    public class ChatNotification : Notification {
        public String Sender;

        public override void Raise() {
            if (MyAPIGateway.Utilities == null) return;

            MyAPIGateway.Utilities.ShowMessage(Sender, Text);
        }
    }

    public class WindowNotification : Notification {
        public String BigLabel = "SE Garden";
        public String SmallLabel;
        public Action<ResultEnum> Callback;
        public String ButtonLabel;

        public override void Raise() {
            if (MyAPIGateway.Utilities == null) return;

            MyAPIGateway.Utilities.ShowMissionScreen(
                BigLabel, "", SmallLabel, Text, Callback, ButtonLabel);
        }

    }



    /*
    public enum NotificationType {
        None,
        Alert,
        Chat,
        Window,
    }

    */

}
