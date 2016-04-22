using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using VRage;

using SEGarden.Extensions;
using SEGarden.Logging;

namespace SEGarden.Definitions {

    [XmlType("ItemCount")]
    public class ItemCountDefinition : DefinitionBase {

        //static Logger Log = new Logger("SEGarden.Definitions.ItemCountDefinition");

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

        public ItemCountDefinition() { }

        public ItemCountDefinition(ByteStream stream) {
            if (stream == null) throw new ArgumentException("null stream");

            
            TypeName = stream.getString();
            //Log.Trace(String.Format("deserialized TypeName \"{0}\"", TypeName), "ctr");
            SubtypeName = stream.getString();
            //Log.Trace(String.Format("deserialized SubtypeName \"{0}\"", SubtypeName), "ctr");
            Count = stream.getDouble();
            //Log.Trace(String.Format("deserialized Count \"{0}\"", Count), "ctr");
        }

         public void AddToByteSteam(ByteStream stream) {
             if (stream == null) throw new ArgumentException("null stream");

             //Log.Trace(String.Format("serializing TypeName \"{0}\"",TypeName), "AddToByteSteam");
             stream.addString(TypeName);
             //Log.Trace(String.Format("serializing SubtypeName \"{0}\"", SubtypeName), "AddToByteSteam");
             stream.addString(SubtypeName);
             //Log.Trace(String.Format("serializing Count \"{0}\"", Count), "AddToByteSteam");
             stream.addDouble(Count);
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
