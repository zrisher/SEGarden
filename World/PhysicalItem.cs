using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Definitions;

using VRage.Game;
using VRage.ObjectBuilders;


namespace SEGarden.World {

    public class PhysicalItem {

        public readonly MyDefinitionBase Definition;

        public String TypeName { 
            get { return DefinitionId.TypeId.ToString(); } 
        }

        public String SubtypeName {
            get { return DefinitionId.SubtypeName; }
        }

        public MyObjectBuilderType BuilderType {
            get { return DefinitionId.TypeId; }
        }

        public MyDefinitionId DefinitionId {
            get { return Definition.Id; }
        }

        public PhysicalItem(String typeName, String subtypeName) {
            var builderType = MyObjectBuilderType.Parse(typeName);
            var definitionId = new MyDefinitionId(builderType, subtypeName);
            Definition = MyDefinitionManager.Static.GetDefinition(definitionId);

            if (!(Definition is MyPhysicalItemDefinition))
                throw new InvalidOperationException(
                    "Passed type/subtype is not a PhysicalItem."
                );
        }

        public PhysicalItem(MyDefinitionId definitionId) {
            Definition = MyDefinitionManager.Static.GetDefinition(definitionId);

            if (!(Definition is MyPhysicalItemDefinition))
                throw new InvalidOperationException(
                    "Passed type/subtype is not a PhysicalItem."
                );
        }

    }

}
