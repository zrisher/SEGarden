using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using BuilderDefs = Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using InGame = Sandbox.ModAPI.Ingame;
using Interfaces = Sandbox.ModAPI.Interfaces;

using SEGarden;

namespace SEGarden.Extensions {

    /// <summary>
    /// Helper functions for SE Players
    /// </summary>
    public static class PlayerExtension {

        public static bool IsAdmin(this IMyPlayer player) {
            if (GardenGateway.RunningOn == Logic.RunLocation.Singleplayer)
                return true;

            if (player.IsHost())
                return true;

            if (player.isAuthenticatedAdmin())
                return true;

            return false;
        }

        public static bool isAuthenticatedAdmin(this IMyPlayer player) {
            var clients = MyAPIGateway.Session.GetCheckpoint("null").Clients;
            if (clients != null) {
                var client = clients.FirstOrDefault(
                    c => c.SteamId == player.SteamUserId && c.IsAdmin);
                return (client != null);
            }

            return false;
        }

        public static bool IsHost(this IMyPlayer player) {
            return MyAPIGateway.Multiplayer.IsServerPlayer(player.Client);
        }

        public static IMyFaction GetFaction(this IMyPlayer player) {
            return MyAPIGateway.Session.Factions.TryGetPlayerFaction(player.PlayerID);
        }

    }

}
