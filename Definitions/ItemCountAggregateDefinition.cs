using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using VRage;

using SEGarden.Extensions;
using SEGarden.Logging;

namespace SEGarden.Definitions {

    [XmlType("ItemCountAggregate")]
    public class ItemCountAggregateDefinition : DefinitionBase {

        //static Logger Log = new Logger("SEGarden.Definitions.ItemCountAggregateDefinition");


        [XmlElement("ItemCount")]
        public List<ItemCountDefinition> Counts = 
            new List<ItemCountDefinition>();

        protected override String ValidationName {
            get { return "ItemCountAggregate"; }
        }

        public ItemCountAggregateDefinition() { }

        public ItemCountAggregateDefinition(ByteStream stream) {
            if (stream == null) throw new ArgumentException("null stream");

            ushort len = stream.getUShort();
            for (ushort i = 0; i < len; ++i) {
                Counts.Add(new ItemCountDefinition(stream));
            }
        }

         public void AddToByteSteam(ByteStream stream) {
             if (stream == null) throw new ArgumentException("null stream");

             //Log.Trace("serializing Counts count " + Counts.Count, "AddToByteSteam");
             stream.addUShort((ushort)Counts.Count);
             foreach (var count in Counts) {
                 count.AddToByteSteam(stream);
             }
         }

        public override void Validate(ref List<ValidationError> errors) {
            ValidateChildren(Counts, "Counts", ref errors);
        }

    }

}
