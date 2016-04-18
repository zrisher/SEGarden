using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SEGarden.Definitions {

    [XmlType("ItemCountAggregate")]
    public class ItemCountAggregateDefinition : DefinitionBase {

        [XmlElement("ItemCount")]
        public List<ItemCountDefinition> Counts = 
            new List<ItemCountDefinition>();

        protected override String ValidationName {
            get { return "ItemCountAggregate"; }
        }

        public override void Validate(ref List<ValidationError> errors) {
            ValidateChildren(Counts, "Counts", ref errors);
        }

    }

}
