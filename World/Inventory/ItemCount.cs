using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Definitions; // from Sandbox.Game.dll
using Sandbox.Game.Entities; // from Sandbox.Game.dll
using Sandbox.ModAPI;

using VRage; // from VRage.dll and VRage.Library.dll
using VRage.Game; // from VRage.Game.dll
using VRage.Game.Entity; // from VRage.Game.dll
using VRage.Game.ModAPI; // from VRage.Game.dll
using VRage.ModAPI; // from VRage.Game.dll
using VRage.ObjectBuilders;

using SEGarden.Logging;

namespace SEGarden.World.Inventory {

    /// <summary>
    /// Represents a collection of item counts as a vector
    /// </summary>
    public class InventoryItem {

        MyFixedPoint Amount;
        String TypeName;
        String SubtypeName;



        /*
 * Need TypeName indicated if this is going to work in a general case
public static List<ItemCountDefinition> FromItemCounts(ItemCountsAggregate aggregate) {
    var result = new List<ItemCountDefinition>();

    foreach (var kvp in aggregate.Counts) {
        if (kvp.Value > 0) {
            result.Add(new ItemCountDefinition() {
                SubtypeName = kvp.Key.SubtypeName,
                Count = (double)kvp.Value,
            });
        }
    }

    return result;
}
 * 
 *         public static ItemCountsAggregate ToItemCounts(List<ItemCountDefinition> defs) {
    var result = new ItemCountsAggregate();

    foreach (var def in defs) {
        MyDefinitionId? defId;
        LicenseType.TryGetId(def.SubtypeName, out defId);
        if (defId != null) {         
            result.Set(defId.Value, (MyFixedPoint)def.Count);
        }
        else {
            Log.Error("Unrecognized license subtype " + def.SubtypeName, "ToItemCounts");
        }

    }

    return result;
}
*/
       
    }
}
