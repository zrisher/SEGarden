using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using VRage.Library;

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
    /// Thanks to StackCollision for writing most of this class:
    /// github.com/stackcollision/GardenConquest
    /// </summary>
    public static class ByteConverterExtension {

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

    }
}
