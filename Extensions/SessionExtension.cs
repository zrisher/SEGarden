/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden.Extensions {
    class SessionExtension {

        private static bool s_WaitingForGateway = true;
        private static bool s_ServerIsOffline;

        /// <summary>
        /// Checks if this session is the server
        /// </summary>
        /// <returns>True if this is a server</returns>
        /// <exception cref="NullReferenceException">Thrown is Multiplayer pointer is null.</exception>
        public static bool isServer() {
            if (MyAPIGateway.Multiplayer.MultiplayerActive == false)
                return true;
            else
                return MyAPIGateway.Multiplayer.IsServer;
        }

        public static bool isOffline() {
            if (s_WaitingForGateway) {
                try {
                    s_ServerIsOffline = (MyAPIGateway.Session.OnlineMode == MyOnlineModeEnum.OFFLINE);
                    s_WaitingForGateway = false;
                }
                catch { }
            }

            return s_ServerIsOffline;
        }

    }
}
*/
