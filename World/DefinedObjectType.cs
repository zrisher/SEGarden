using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Definitions;

using VRage.Game;
using VRage.ObjectBuilders;

using SEGarden.Logging;

namespace SEGarden.World {

    public class DefinedObjectType {

        static Logger Log = new Logger("SEGarden.World.DefinedObjectType");

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

        public DefinedObjectType(String typeName, String subtypeName) {
            MyObjectBuilderType builderType;
            try {
                builderType = MyObjectBuilderType.Parse(typeName);
            }
            catch (Exception e) {
                Log.Error(String.Format(
                    "Failed to find builder type \"{0}\"", typeName
                ), "ctr");
                return;
            }

            MyDefinitionId id;
            try {
                id = new MyDefinitionId(builderType, subtypeName);
            }
            catch (Exception e) {
                Log.Error(String.Format(
                    "Failed to find definitionId for \"{0}/{1}\"", builderType, subtypeName
                ), "ctr");
                return;
            }

            try {
                Definition = MyDefinitionManager.Static.GetDefinition(id);
            }
            catch (Exception e) {
                Log.Error(String.Format(
                    "Failed to find Definition for \"{0}\"", id
                ), "ctr");
                return;
                
                

                /*
                Log.Trace("Logging existing cb defs", "ctr");
                var entities = Sandbox.Game.Entities.MyEntities.GetEntities(); //.Select(x => x as Sandbox.Game.Entities.Cube.MySlimBlock).Where(x => x != null).ToList();

                foreach (var entity in entities) {
                    if (entity is Sandbox.Game.Entities.MyCubeGrid) {
                        var grid = entity as Sandbox.Game.Entities.MyCubeGrid;
                        foreach (VRage.Game.ModAPI.IMySlimBlock block in grid.CubeBlocks) {
                            if (block.FatBlock != null)
                                Log.Trace(String.Format("{0}", block.FatBlock.BlockDefinition), "ctr");
                        }
                    }
                }
                 * */
            }

           
        }

        public DefinedObjectType(MyDefinitionId definitionId) {
            Definition = MyDefinitionManager.Static.GetDefinition(definitionId);
        }

    }

}
