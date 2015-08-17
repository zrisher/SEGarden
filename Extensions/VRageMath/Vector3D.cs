using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRageMath;

namespace SEGarden.Extensions.VRageMath {
    static class Vector3DExtensions {

        public const int SizeInBytes = 8;

        /*
        public static byte[] ToBytes(this Vector3D vector) {
            VRage.ByteStream stream = new VRage.ByteStream(sizeof(long)*6);
            AddToByteStream(vector, stream);
            return stream.Data;
        }

        public static Vector3D FromBytes(byte[] bytes) {
            VRage.ByteStream stream = new VRage.ByteStream(bytes, bytes.Length);
            return RemoveFromByteStream(stream);
        }
        */

        public static void AddToByteStream(this Vector3D vector, VRage.ByteStream stream) {
            stream.addDouble(vector.X);
            stream.addDouble(vector.Y);
            stream.addDouble(vector.Z);
        }

        public static void RemoveFromByteStream(this Vector3D vector, VRage.ByteStream stream) {
            vector.X = stream.getDouble();
            vector.Y = stream.getDouble();
            vector.Z = stream.getDouble();
        }
    }
}
