using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRageMath;

namespace SEGarden.Extensions.VRageMath {
    static class Vector3DExtensions {

        public static byte[] ToBytes(this Vector3D vector) {
            VRage.ByteStream stream = new VRage.ByteStream(sizeof(long)*6);
            stream.addDouble(vector.X);
            stream.addDouble(vector.Y);
            stream.addDouble(vector.Z);
            return stream.Data;
        }

        public static Vector3D FromBytes(byte[] bytes) {
            VRage.ByteStream stream = new VRage.ByteStream(bytes, bytes.Length);

            double[] pos = new double[3];
            pos[0] = stream.getDouble();
            pos[1] = stream.getDouble();
            pos[2] = stream.getDouble();

            return new Vector3D(pos[0], pos[1], pos[2]);
        }

    }
}
