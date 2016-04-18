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

using SEGarden.Definitions;
using SEGarden.Logging;

namespace SEGarden.World.Inventory {

    public class ItemCount {

        public readonly MyFixedPoint Amount;
        public readonly PhysicalItem Item;

        public ItemCount(MyFixedPoint amount, PhysicalItem item) {
            Amount = amount;
            Item = item;
        }

        public ItemCount(ItemCountDefinition definition) {
            Amount = (MyFixedPoint)definition.Count;
            Item = new PhysicalItem(definition.TypeName, definition.SubtypeName);
        }

        public ItemCountDefinition GetDefinition() {
            return new ItemCountDefinition(
                Item.TypeName, Item.SubtypeName, (double)Amount
            );
        }

    }
}
