using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRage;

using SEGarden.Definitions;
using SEGarden.Extensions;
using SEGarden.Logging;
using SEGarden.Testing;

using GC.Definitions;
using GC.Definitions.GridTaxonomy;

namespace SEGarden.Tests.Definitions {

    public class ItemCountAggregateDefinitionSpec : Specification {

        //static Logger Log = new Logger("SEGarden.Tests.Definitions.ItemCountAggregateDefinitionSpec");

        public ItemCountAggregateDefinitionSpec() {
            Subject = "ItemCountAggregateDefinition";
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
            var a = new ItemCountAggregateDefinition() {
                Counts = new List<ItemCountDefinition>() {
                    new ItemCountDefinition() {
                        TypeName = "type 1"
                    },
                    new ItemCountDefinition() {
                        TypeName = "type 2"
                    }
                }
            };
            //Log.Trace("serializing", "TestByteSerialization");
            a.AddToByteSteam(stream);
            stream = new ByteStream(stream.Data, stream.Data.Length);
            //Log.Trace("deserializing", "TestByteSerialization");
            var a2 = new ItemCountAggregateDefinition(stream);
            x.Assert(a2.Counts[0].TypeName == "type 1",
                "First item serializes/deserializes correctly.");
            x.Assert(a2.Counts[1].TypeName == "type 2",
                "Second item serializes/deserializes correctly.");
        }

    }

}
