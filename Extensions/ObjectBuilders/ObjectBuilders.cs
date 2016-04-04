using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using VRage.Game;
using VRage.ObjectBuilders;

using SEGarden.Logging;

namespace SEGarden.Extensions.Objectbuilders {

    /// <summary>
    /// </summary>
    public static class ObjectBuildersExtension {

        private static Logger Log = 
            new Logger("SEGarden.Extensions.Objectbuilders.ObjectBuildersExtension");


        /// <summary>
        /// TODO: Check the rest of the nulls recursively
        /// </summary>
        /// <param name="b"></param>
        public static void FillNullsWithDefaults( this MyObjectBuilder_EntityBase b) {
            if (b.ComponentContainer == null) {
                Log.Warning("ConcealableGrid builder had a null ComponentContainer and we can't fill it (type not allowed in script). So trying to allow.", "FillNullsWithDefaults");
                // MyObjectBuilder_ComponentContainer not allowed in script : /
                //Builder.ComponentContainer = new VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer();
            }
            if (b.EntityDefinitionId == null) {
                Log.Warning("ConcealableGrid builder had a null EntityDefinitionId, filling with default", "FillNullsWithDefaults");
                b.EntityDefinitionId = new SerializableDefinitionId();
                // TODO: Recurse
            }
            if (b.Name == null) {
                Log.Warning("ConcealableGrid builder had a null Name, filling with default", "FillNullsWithDefaults");
                b.Name = "";

            }
            if (b.PositionAndOrientation == null) {
                Log.Warning("ConcealableGrid builder had a null PositionAndOrientation. It's nullable attempt to leave as is.", "FillNullsWithDefaults");
            }
            if (b.SubtypeId == null) {
                Log.Warning("ConcealableGrid builder had a null SubtypeId, but it's readonly so trying to allow.", "FillNullsWithDefaults");
            }
            if (b.SubtypeName == null) {
                Log.Warning("ConcealableGrid builder had a null SubtypeName, filling with default.", "FillNullsWithDefaults");
                b.SubtypeName = "";
            }
            if (b.TypeId == (System.Type)null) {
            }
        }


        public static void FillNullsWithDefaults( this MyObjectBuilder_CubeGrid b) {
            ((MyObjectBuilder_EntityBase)b).FillNullsWithDefaults();

            if (b.AngularVelocity == null) {
                Log.Warning("ConcealableGrid builder had a null AngularVelocity, can't save.", "FillNullsWithDefaults");
                //return false;
            }
            if (b.BlockGroups == null) {
                Log.Warning("ConcealableGrid builder had a null BlockGroups, can't save.", "FillNullsWithDefaults");
                //return false;
            }
            if (b.ConveyorLines == null) {
                Log.Warning("ConcealableGrid builder had a null ConveyorLines, can't save but trying allow.", "FillNullsWithDefaults");
                //return false;
            }
            if (b.CubeBlocks == null) {
                Log.Warning("ConcealableGrid builder had a null CubeBlocks, can't save.", "FillNullsWithDefaults");
                //return false;
            }
            if (b.DisplayName == null) {
                Log.Warning("ConcealableGrid builder had a null DisplayName, can't save.", "FillNullsWithDefaults");
                //return false;
            }
            if (b.JumpDriveDirection == null) {
                Log.Warning("ConcealableGrid builder had a null JumpDriveDirection, can't save.", "FillNullsWithDefaults");
                //return false;
            }
            if (b.LinearVelocity == null) {
                Log.Warning("ConcealableGrid builder had a null LinearVelocity, can't save.", "FillNullsWithDefaults");
                //return false;
            }
            if (b.OxygenAmount == null) {
                Log.Warning("ConcealableGrid builder had a null OxygenAmount, can't save.", "FillNullsWithDefaults");
                //return false;
            }
            if (b.Skeleton == null) {
                Log.Warning("ConcealableGrid builder had a null Skeleton, can't save.", "FillNullsWithDefaults");
                //return false;
            }

        }

    }
}
