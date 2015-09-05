using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRageMath;

namespace SEGarden.Extensions.VRageMath {
    static class Vector3DExtensions {

        public static String ToRoundedString(this Vector3D vector) {
            return System.Math.Truncate(vector.X) + ", " +
                System.Math.Truncate(vector.Y) + ", " +
                System.Math.Truncate(vector.Z);
        }

    }
}
