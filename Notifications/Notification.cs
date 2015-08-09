using System;
using System.Collections.Generic;
using System.Text;

using Sandbox.Common;
using Sandbox.ModAPI;

namespace SEGarden.Notifications {

    public abstract class Notification {
        //public NotificationDestination Destination;
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
        public override void Raise() {
        }
    }

    public class WindowNotification : Notification {
        public String BigLabel;
        public String SmallLabel;
        public String Text;
        public Action<ResultEnum> Callback;
        public String ButtonLabel;

        public override void Raise() {
            MyAPIGateway.Utilities.ShowMissionScreen(
                BigLabel, "", SmallLabel, Text, Callback, ButtonLabel);
        }

    }

    /*
    public enum NotificationDestination {
        None,
        Alert,
        Chat,
        Window,
    }
     * */


}
