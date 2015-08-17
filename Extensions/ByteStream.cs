using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Runtime.InteropServices;

using VRage.Library;
using VRageMath;

using SEGarden.Extensions.VRageMath;

namespace SEGarden.Extensions {

    /// <summary>
    /// Currently, the only way to serialize objects to binary data is to maually 
    /// define their ToBytes and FromBytes methods and store in a VRage.ByteStream.
    /// 
    /// Normally this could be done using System.ComponentModel.ByteConverter and 
    /// System.IO.MemoryStream, but these are not whitelisted.
    /// 
    /// Space Engineers itselfs uses Google's ProtoBuf library, which we can actually
    /// use as well, but critically the Protobuf.Serializer is missing.
    /// Keen has wrappered this with VRage.Serialization.ProtoSerializer<T>, but 
    /// that's not available either.
    /// 
    /// TODO: Fix this in core
    /// 
    /// Thanks to StackCollision for writing most of the primitives:
    /// github.com/stackcollision/GardenConquest
    /// 
    /// Thanks to Rynchodon for the ByteUnions:
    /// github.com/rynchodon/Autopilot
    /// </summary>
    public static class ByteConverterExtension {

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatByteUnion {
            [FieldOffset(0)]
            private byte b0;
            [FieldOffset(8)]
            private byte b1;
            [FieldOffset(16)]
            private byte b2;
            [FieldOffset(24)]
            private byte b3;

            [FieldOffset(0)]
            public float Float;

            public byte[] Bytes { 
                get {return new Byte[4] { b0, b1, b2, b3}; }
                set { b0 = value[0]; b1 = value[1]; b2 = value[2]; b3 = value[3]; }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct DoubleByteUnion {
            [FieldOffset(0)]
            private byte b0;
            [FieldOffset(8)]
            private byte b1;
            [FieldOffset(16)]
            private byte b2;
            [FieldOffset(24)]
            private byte b3;
            [FieldOffset(32)]
            private byte b4;
            [FieldOffset(40)]
            private byte b5;
            [FieldOffset(48)]
            private byte b6;
            [FieldOffset(54)]
            private byte b7;

            [FieldOffset(0)]
            public double Double;

            public byte[] Bytes {
                get { return new Byte[8] { b0, b1, b2, b3, b4, b5, b6, b7 }; }
                set { 
                    b0 = value[0]; b1 = value[1]; b2 = value[2]; b3 = value[3];
                    b4 = value[4]; b5 = value[5]; b6 = value[6]; b7 = value[7]; 
                }
            }
        }


        public static void addByteArray(this VRage.ByteStream stream, byte[] bytes) {
            stream.Write(bytes, 0, bytes.Length);
        }

        public static byte[] getByteArray(this VRage.ByteStream stream, int count) {
            byte[] bytes = new byte[count];
            stream.Read(bytes, 0, count);
            return bytes;
        }

        public static void addUShort(this VRage.ByteStream stream, ushort v) {
            for (byte i = 0; i < sizeof(ushort); ++i)
                stream.WriteByte((byte)((v >> (i * 8)) & 0xFF));
        }

        public static ushort getUShort(this VRage.ByteStream stream) {
            ushort v = 0;
            for (byte i = 0; i < sizeof(ushort); ++i)
                v |= (ushort)((ushort)(stream.ReadByte()) << (i * 8));
            return v;
        }

        public static void addLong(this VRage.ByteStream stream, long v) {
            ulong u = (ulong)v;
            for (byte i = 0; i < sizeof(ulong); ++i)
                stream.WriteByte((byte)((u >> (i * 8)) & 0xFF));
        }

        public static long getLong(this VRage.ByteStream stream) {
            ulong v = 0;
            for (byte i = 0; i < sizeof(ulong); ++i)
                v |= (ulong)((ulong)(stream.ReadByte()) << (i * 8));
            return (long)v;
        }

        public static void addUlong(this VRage.ByteStream stream, ulong u) {
            for (byte i = 0; i < sizeof(ulong); ++i)
                stream.WriteByte((byte)((u >> (i * 8)) & 0xFF));
        }

        public static ulong getUlong(this VRage.ByteStream stream) {
            ulong v = 0;
            for (byte i = 0; i < sizeof(ulong); ++i)
                v |= (ulong)((ulong)(stream.ReadByte()) << (i * 8));
            return v;
        }

        public static void addDouble(this VRage.ByteStream stream, double v) {
            var union = new DoubleByteUnion();
            union.Double = v;
            byte[] bytes = union.Bytes;
            stream.Write(bytes, 0, bytes.Length);
        }

        public static double getDouble(this VRage.ByteStream stream) {
            var union = new DoubleByteUnion();
            union.Bytes = stream.getByteArray(8);
            return union.Double;
        }

        public static void addString(this VRage.ByteStream stream, string s) {
            if (s.Length > ushort.MaxValue) {
                stream.addUShort(0);
                return;
            }

            // Write length
            stream.addUShort((ushort)s.Length);

            // Write data
            char[] sarray = s.ToCharArray();
            for (ushort i = 0; i < s.Length; ++i)
                stream.WriteByte((byte)sarray[i]);
        }

        public static string getString(this VRage.ByteStream stream) {
            // Read length
            ushort len = stream.getUShort();

            // Read data
            char[] cstr = new char[len];
            for (ushort i = 0; i < len; ++i)
                cstr[i] = (char)stream.ReadByte();
            return new string(cstr);
        }

        public static void addBoolean(this VRage.ByteStream stream, bool b) {
            stream.WriteByte(Convert.ToByte(b));
        }

        public static bool getBoolean(this VRage.ByteStream stream) {
            return Convert.ToBoolean(stream.ReadByte());
        }

        public static void addLongList(this VRage.ByteStream stream, List<long> L) {
            if (L == null) {
                stream.addUShort(0);
                return;
            }

            // Write length
            stream.addUShort((ushort)L.Count);

            // Write data
            foreach (long l in L)
                stream.addLong(l);
        }

        public static List<long> getLongList(this VRage.ByteStream stream) {
            List<long> L = new List<long>();

            // Read length
            ushort len = stream.getUShort();
            if (len == 0)
                return null;

            // Read data
            for (ushort i = 0; i < len; ++i)
                L.Add(stream.getLong());

            return L;
        }

        public static void addVector3D(this VRage.ByteStream stream, Vector3D v) {
            v.AddToByteStream(stream);
        }

        public static Vector3D getVector3D(this VRage.ByteStream stream) {
            Vector3D v = new Vector3D();
            v.RemoveFromByteStream(stream);
            return v;
        }

    }
}
