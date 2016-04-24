using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Definitions;

using VRage.Game;
using VRage.ObjectBuilders;


namespace SEGarden.World {

    public class PhysicalItemType : DefinedObjectType {

        public PhysicalItemType(String typeName, String subtypeName)
            : base(typeName, subtypeName) 
        {
            if (!(Definition is MyPhysicalItemDefinition))
                throw new InvalidOperationException(
                    "Passed type/subtype is not a PhysicalItem."
                );
        }

        public PhysicalItemType(MyDefinitionId definitionId) : 
            base(definitionId) 
        {
            if (!(Definition is MyPhysicalItemDefinition))
                throw new InvalidOperationException(
                    "Passed type/subtype is not a PhysicalItem."
                );
        }

    }

}
