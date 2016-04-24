using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Definitions;

using VRage.Game;
using VRage.ObjectBuilders;


namespace SEGarden.World {

    public class CubeBlockType : DefinedObjectType {

        public CubeBlockType(String typeName, String subtypeName)
            : base(typeName, subtypeName) 
        {
            if (!(Definition is MyCubeBlockDefinition))
                throw new InvalidOperationException(
                    "Passed type/subtype is not a CubeBlock."
                );
        }

        public CubeBlockType(MyDefinitionId definitionId) :
            base(definitionId) 
        {
            if (!(Definition is MyCubeBlockDefinition))
                throw new InvalidOperationException(
                    "Passed type/subtype is not a CubeBlock."
                );
        }

    }

}
