using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SEGarden.Definitions {

    [XmlType("ItemCount")]
    public class ItemCountDefinition : DefinitionBase {

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

        protected override String ValidationName {
            get { return "ItemCount"; }
        }

        public override void Validate(ref List<ValidationError> errors) {
            ErrorIf(String.IsNullOrWhiteSpace(TypeName),
                "TypeName cannot be empty.", ref errors);
            ErrorIf(String.IsNullOrWhiteSpace(SubtypeName),
                "SubtypeName cannot be empty.", ref errors);
            ErrorIf(Count < 0,
                "Count cannot be < 0.", ref errors);
        }

    }

}
