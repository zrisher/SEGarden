using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.ModAPI;
using VRage.Game.ModAPI;

using SEGarden;

namespace SEGarden.Extensions {

    /// <summary>
    /// Helper functions for SE Players
    /// </summary>
    public static class IMyPlayerExtensions {

        /*

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

        public static List<IMyPlayer> getPlayersNearPoint(this IMyPlayerCollection self, Vector3D point, float radius) {
            log("Getting players within " + radius + " of " + point, "getPlayersNearPoint");

            var allPlayers = new List<IMyPlayer>();
            self.GetPlayers(allPlayers);

            float distanceFromPoint = 0.0f;
            var nearbyPlayers = new List<IMyPlayer>();
            foreach (IMyPlayer player in allPlayers) {
                distanceFromPoint = VRageMath.Vector3.Distance(player.GetPosition(), point);
                if (distanceFromPoint < radius) {
                    nearbyPlayers.Add(player);
                }
            }

            log(nearbyPlayers.Count + " Nearby players.", "getPlayersNearPoint");
            return nearbyPlayers;
        }

        */

    }

}
