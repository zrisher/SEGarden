using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;

namespace SEGarden.Extensions {

    static class IMyFactionExtensions {

        public static List<long> MemberIds(this IMyFaction faction) {
            return faction.Members.Select(kvp => kvp.Value.PlayerId).ToList();
        }

        
    }
}
