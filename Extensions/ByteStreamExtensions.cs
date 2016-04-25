using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Runtime.InteropServices;

using VRage;
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
    public static class ByteStreamExtensions {

        //private static SEGarden.Logging.Logger Log = new Logging.Logger("ByteConverterExtension");

        [StructLayout(LayoutKind.Explicit)]
        private struct FourByteUnion {
            [FieldOffset(0)]
            private byte b0;
            [FieldOffset(1)]
            private byte b1;
            [FieldOffset(2)]
            private byte b2;
            [FieldOffset(3)]
            private byte b3;

            [FieldOffset(0)]
            public float Float;

            public byte[] Bytes { 
                get {return new Byte[4] { b0, b1, b2, b3}; }
                set { b0 = value[0]; b1 = value[1]; b2 = value[2]; b3 = value[3]; }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct EightByteUnion {
            [FieldOffset(0)]
            private byte b0;
            [FieldOffset(1)]
            private byte b1;
            [FieldOffset(2)]
            private byte b2;
            [FieldOffset(3)]
            private byte b3;
            [FieldOffset(4)]
            private byte b4;
            [FieldOffset(5)]
            private byte b5;
            [FieldOffset(6)]
            private byte b6;
            [FieldOffset(7)]
            private byte b7;

            [FieldOffset(0)]
            public double Double;

            [FieldOffset(0)]
            public DateTime DateTime;

            public byte[] Bytes {
                get { return new Byte[8] { b0, b1, b2, b3, b4, b5, b6, b7 }; }
                set { 
                    b0 = value[0]; b1 = value[1]; b2 = value[2]; b3 = value[3];
                    b4 = value[4]; b5 = value[5]; b6 = value[6]; b7 = value[7]; 
                }
            }
        }


        public static void addByte(this ByteStream stream, byte theByte) {
            stream.WriteByte(theByte);
        }

        public static byte getByte(this ByteStream stream) {
            return stream.ReadByte();
        }


        public static void addByteArray(this ByteStream stream, byte[] bytes) {
            stream.Write(bytes, 0, bytes.Length);
        }

        public static byte[] getByteArray(this ByteStream stream, int count) {
            byte[] bytes = new byte[count];
            stream.Read(bytes, 0, count);
            return bytes;
        }

        public static void addUShort(this ByteStream stream, ushort v) {
            for (byte i = 0; i < sizeof(ushort); ++i)
                stream.WriteByte((byte)((v >> (i * 8)) & 0xFF));
        }

        public static ushort getUShort(this ByteStream stream) {
            ushort v = 0;
            for (byte i = 0; i < sizeof(ushort); ++i)
                v |= (ushort)((ushort)(stream.ReadByte()) << (i * 8));
            return v;
        }

        public static void addLong(this ByteStream stream, long v) {
            ulong u = (ulong)v;
            for (byte i = 0; i < sizeof(ulong); ++i)
                stream.WriteByte((byte)((u >> (i * 8)) & 0xFF));
        }

        public static long getLong(this ByteStream stream) {
            ulong v = 0;
            for (byte i = 0; i < sizeof(ulong); ++i) {
                //Log.Trace("Removing one byte from steam at position: " + stream.Position + " / " + stream.Length, "getLong");
                v |= (ulong)((ulong)(stream.ReadByte()) << (i * 8));
            }

            return (long)v;
        }

        public static void addUlong(this ByteStream stream, ulong u) {
            for (byte i = 0; i < sizeof(ulong); ++i)
                stream.WriteByte((byte)((u >> (i * 8)) & 0xFF));
        }

        public static ulong getUlong(this ByteStream stream) {
            ulong v = 0;
            for (byte i = 0; i < sizeof(ulong); ++i)
                v |= (ulong)((ulong)(stream.ReadByte()) << (i * 8));
            return v;
        }

        public static void addDouble(this ByteStream stream, double v) {
            var union = new EightByteUnion();
            union.Double = v;
            byte[] bytes = union.Bytes;
            stream.Write(bytes, 0, bytes.Length);
        }

        public static double getDouble(this ByteStream stream) {
            var union = new EightByteUnion();
            union.Bytes = stream.getByteArray(8);
            return union.Double;
        }

        public static void addString(this ByteStream stream, string s) {
            //Log.Trace(String.Format("Addings string \"{0}\"", s), "addString");
            //Log.Trace(String.Format("Current stream pos: {0}", stream.Position), "addString");

            if (s == null || s.Length > ushort.MaxValue) {
                stream.addUShort(0);
                //Log.Trace(String.Format("Writing string length: {0}", 0), "addString");
                //Log.Trace(String.Format("Final stream pos: {0}", stream.Position), "addString");
                return;
            }

            // Write length
            stream.addUShort((ushort)s.Length);
            //Log.Trace(String.Format("Writing string length: {0}", (ushort)s.Length), "addString");

            // Write data
            char[] sarray = s.ToCharArray();
            for (ushort i = 0; i < s.Length; ++i)
                stream.WriteByte((byte)sarray[i]);

            //Log.Trace(String.Format("Final stream pos: {0}", stream.Position), "addString");
        }

        public static string getString(this ByteStream stream) {
            //Log.Trace(String.Format("Deserializing stream at pos {0}", stream.Position), "getString");
            // Read length
            ushort len = stream.getUShort();
            //Log.Trace(String.Format("Deserialized string length as {0}", len), "getString");

            // Read data
            char[] cstr = new char[len];

            if (len > 0)
                for (ushort i = 0; i < len; ++i)
                    cstr[i] = (char)stream.ReadByte();

            //Log.Trace(String.Format("Finished deserializing stream at pos {0}", stream.Position), "getString");
            return new string(cstr);
        }

        public static void addStringList(this ByteStream stream, List<String> list) {
            stream.addUShort((ushort)list.Count);
            foreach (String element in list) {
                stream.addString(element);
            }
        }

        public static List<String> getStringList(this ByteStream stream) {
            List<String> result = new List<String>();

            ushort len = stream.getUShort();

            for (ushort i = 0; i < len; ++i) {
                result.Add(stream.getString());
            }

            return result;
        }

        public static void addBoolean(this ByteStream stream, bool b) {
            stream.WriteByte(Convert.ToByte(b));
        }

        public static bool getBoolean(this ByteStream stream) {
            return Convert.ToBoolean(stream.ReadByte());
        }

        public static void addLongList(this ByteStream stream, List<long> L) {
            if (L == null) {
                stream.addUShort(0);
                //Log.Info("Adding long list of " + 0 + " at " + stream.Position + " / " + stream.Length, "addLongList");
                return;
            }

            // Write length
            stream.addUShort((ushort)L.Count);
            //Log.Info("Adding long list of " + L.Count + " at " + stream.Position + " / " + stream.Length, "addLongList");

            // Write data
            foreach (long l in L)
                stream.addLong(l);
        }

        public static List<long> getLongList(this ByteStream stream) {
            List<long> L = new List<long>();

            // Read length
            ushort len = stream.getUShort();

            //Log.Info("Getting long list of " + L.Count + " at " + stream.Position + " / " + stream.Length, "getLongList");

            if (len == 0)
                return L;

            // Read data
            for (ushort i = 0; i < len; ++i)
                L.Add(stream.getLong());

            //Log.Info("Finished getting long list from stream at pos: " + stream.Position + " / " + stream.Length, "getLongList");
            return L;
        }

        public static void addVector3D(this ByteStream stream, Vector3D v) {
            stream.addDouble(v.X);
            stream.addDouble(v.Y);
            stream.addDouble(v.Z);
        }

        public static Vector3D getVector3D(this ByteStream stream) {
            Vector3D v = new Vector3D();
            v.X = stream.getDouble();
            v.Y = stream.getDouble();
            v.Z = stream.getDouble();
            //Log.Trace("getVector3D got vector " + v, "AddToByteStream");
            return v;
        }

        public static void addDateTime(this ByteStream stream, DateTime v) {
            var union = new EightByteUnion();
            union.DateTime = v;
            byte[] bytes = union.Bytes;
            stream.Write(bytes, 0, bytes.Length);
        }

        public static DateTime getDateTime(this ByteStream stream) {
            var union = new EightByteUnion();
            union.Bytes = stream.getByteArray(8);
            return union.DateTime;
        }

    }
}
