using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using VRage;
using VRage.Game;

using SEGarden.Logging;
using SEGarden.World.Inventory;

using GC.World.PhysicalObjects;

namespace SEGarden.Definitions {

    [XmlType("ItemCountAggregate")]
    public class ItemCountAggregateDefinition : DefinitionBase {

        static readonly Logger Log =
            new Logger("SEGarden.Definitions.ItemCountAggregateDefinition");

        [XmlElement("ItemCount")]
        public List<ItemCountDefinition> Counts = 
            new List<ItemCountDefinition>();

        public ItemCountAggregateDefinition() { }

        public override bool Validate() {
            bool result = true;

            if (Counts == null) {
                Log.Warning("Counts cannot be null.", "Validate");
                result = false;
            }
            else {
                foreach (var count in Counts) {
                    result &= count.Validate();
                }
            }

            return result;
        }

    }

}
