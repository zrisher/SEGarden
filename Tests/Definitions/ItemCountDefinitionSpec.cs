using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRage;

using SEGarden.Definitions;
using SEGarden.Logging;
using SEGarden.Testing;

using GC.Definitions;
using GC.Definitions.GridTaxonomy;

namespace SEGarden.Tests.Definitions {

    public class ItemCountDefinitionSpec : Specification {

        //static Logger Log = new Logger("SEGarden.Tests.Definitions.ItemCountDefinitionSpec");

        public ItemCountDefinitionSpec() {
            Subject = "ItemCountDefinition";
            Describe(
                "It should serialize to & from XML", 
                TestXMLSerialization);
            Describe(
                "It should serialize to & from Bytes",
                TestByteSerialization);
        }

        private void TestXMLSerialization(SpecCase x) {
            // TODO, but seems to be working fine
        }

        private void TestByteSerialization(SpecCase x) {

            var stream = new ByteStream(0, true);
            var a = new ItemCountDefinition() {
                TypeName = "some type",
                SubtypeName = "some subtype",
                Count = 47
            };
            a.AddToByteSteam(stream);
            stream = new ByteStream(stream.Data, stream.Data.Length);
            var a2 = new ItemCountDefinition(stream);
            x.Assert(a2.TypeName == "some type",
                "Name serializes/deserializes correctly.");
            x.Assert(a2.SubtypeName == "some subtype",
                "SubtypeName serializes/deserializes correctly.");
            x.Assert(a2.Count == 47,
                "Count serializes/deserializes correctly.");
        }

    }

}
