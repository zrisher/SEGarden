using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRage.Game.Components;
using VRage.ModAPI;

namespace SEGarden.Extensions {

    using Logging;

    public static class EntityExtensions {

        private static Logger Log = new Logger("SEGarden.Extensions.EntityExtensions");

        public enum EntityType {
            Unknown,
            Character,
            FloatingObject,
            LargeShip,
            Station,
            SmallShip,
            Meteor,
            Voxel,
        };

        public static bool IsMoving(this IMyEntity entity) {
            MyPhysicsComponentBase physics = entity.Physics;

            if (physics == null) return false;

            // acceleration seems to not be zero'd out when characters enter cockpits,
            // etc. It's more accurate to just use velocity
            if (//physics.AngularAcceleration.AbsMax() == 0 &&
                physics.AngularVelocity.AbsMax() == 0 &&
                //physics.LinearAcceleration.AbsMax() == 0 &&
                physics.LinearVelocity.AbsMax() == 0) 
            {
                return false;
            }

            //Log.Trace("Physics for entity " + entity.EntityId + " :", "IsMoving");
            //Log.Trace("AngularAcceleration: " + physics.AngularAcceleration, "IsMoving");
            //Log.Trace("AngularVelocity: " + physics.AngularVelocity, "IsMoving");
            //Log.Trace("LinearAcceleration: " + physics.LinearAcceleration, "IsMoving");
            //Log.Trace("LinearVelocity: " + physics.LinearVelocity, "IsMoving");

            return true;
        }


    }

}
