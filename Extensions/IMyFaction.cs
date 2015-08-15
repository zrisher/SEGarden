using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;

namespace SEGarden.Extensions {

    static class IMyFactionExtensions {

        public static List<long> PlayerIds(this IMyFaction faction) {
            return faction.Members.Select(kvp => kvp.Value.PlayerId).ToList();
        }

        public static List<ulong> SteamIds(this IMyFaction faction) {
            List<long> playerIds = faction.PlayerIds();

            List<IMyPlayer> allPlayers = new List<IMyPlayer>();
            MyAPIGateway.Multiplayer.Players.GetPlayers(allPlayers, (x) => {
                return playerIds.Contains(x.PlayerID);
            });

            return allPlayers.Select((x) => x.SteamUserId).ToList();
        }
        
    }
}
