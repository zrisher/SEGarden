using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Sandbox.ModAPI;
using VRage;
using VRage.Game;

using SEGarden.Logging;
using SEGarden.World.Inventory;

namespace SEGarden.Definitions {

    [XmlType("ItemCount")]
    public class ItemCountDefinition : DefinitionBase {

        static readonly Logger Log = 
            new Logger("SEGarden.Definitions.ItemCountDefinition");

        [XmlAttribute("Type")]
        public String TypeName;

        [XmlAttribute("Subtype")]
        public String SubtypeName;

        /// <remarks>
        /// "XmlAttribute/XmlText cannot be used to encode types implementing 
        /// IXmlSerializable." So we can't encode directly as a MyFixedPoint 
        /// if we want to do in attribute.
        /// </remarks>
        [XmlAttribute("Count")]
        public double Count;

        public ItemCountDefinition() { }

        public ItemCountDefinition(
            String typeName, String subtypeName, double count
        ) {
            TypeName = typeName;
            SubtypeName = subtypeName;
            Count = count;
        }

        public ItemCountDefinition(ItemCount count) {
            TypeName = count.Item.TypeName;
            SubtypeName = count.Item.SubtypeName;
            Count = (double)count.Amount;
        }

        public override bool Validate() {
            bool result = true;

            if (String.IsNullOrWhiteSpace(TypeName)) {
                Log.Warning("TypeName cannot be blank.","Validate");
                result = false;
            }

            if (String.IsNullOrWhiteSpace(SubtypeName)) {
                Log.Warning("SubtypeName cannot be blank.","Validate");
                result = false;
            }

            if (Count < 0) {
                Log.Warning("Count cannot be less than zero.","Validate");
                result = false;
            }

            /* Is it really necessary to test this? Assuredly the user will later
             * if calling validate, and doing it at each level will replicate
             * the check many times.
            try {
                MyAPIGateway.Utilities.SerializeToXML<ItemCountDefinition>(this);
            }
            catch (Exception e) {
                Log.Warning("Failed serialization with error: " + e, "Validate");
                result = false;
            }
            */

            return result;
        }

    }

}
