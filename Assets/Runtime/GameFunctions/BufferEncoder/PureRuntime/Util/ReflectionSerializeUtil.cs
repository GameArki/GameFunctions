using System;
using System.IO;
using System.Buffers;
using System.Linq;
using System.Reflection;

namespace GameFunctions {

    public static class ReflectionSerializeUtil {

        static byte[] writeBuffer = new byte[1024 * 1024];

        public static byte[] Serialize<T>(T obj) {

            int offset = 0;

            var fields = GetFields<T>();
            for (int i = 0; i < fields.Length; i += 1) {
                var field = fields[i];
                var value = field.GetValue(obj);
                WriteField(writeBuffer, field.FieldType.Name, value, ref offset);
            }

            byte[] result = new byte[offset];
            Buffer.BlockCopy(writeBuffer, 0, result, 0, offset);
            return result;

        }

        public static T Deserialize<T>(byte[] data) {

            int offset = 0;

            var type = typeof(T);
            var fields = GetFields<T>();
            var obj = Activator.CreateInstance<T>();
            for (int i = 0; i < fields.Length; i += 1) {
                var field = fields[i];
                var value = ReadField(data, field.FieldType.Name, ref offset);
                field.SetValue(obj, value);
            }

            return obj;

        }

        static FieldInfo[] GetFields<T>() {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(value => value.Name).ToArray();
            return fields;
        }

        static void WriteField(byte[] dst, string fielType, object value, ref int offset) {

            // - Sign
            byte typeSign = GetTypeSign(fielType);
            GFBufferEncoderWriter.WriteUInt8(dst, typeSign, ref offset);

            // - Data
            switch (fielType) {
                case "Boolean":
                    GFBufferEncoderWriter.WriteBool(dst, (bool)value, ref offset);
                    break;
                case "Boolean[]":
                    GFBufferEncoderWriter.WriteBoolArr(dst, (bool[])value, ref offset);
                    break;
                case "Byte":
                    GFBufferEncoderWriter.WriteUInt8(dst, (byte)value, ref offset);
                    break;
                case "Byte[]":
                    GFBufferEncoderWriter.WriteUint8Arr(dst, (byte[])value, ref offset);
                    break;
                case "SByte":
                    GFBufferEncoderWriter.WriteInt8(dst, (sbyte)value, ref offset);
                    break;
                case "SByte[]":
                    GFBufferEncoderWriter.WriteInt8Arr(dst, (sbyte[])value, ref offset);
                    break;
                case "Int16":
                    GFBufferEncoderWriter.WriteInt16(dst, (short)value, ref offset);
                    break;
                case "Int16[]":
                    GFBufferEncoderWriter.WriteInt16Arr(dst, (short[])value, ref offset);
                    break;
                case "UInt16":
                    GFBufferEncoderWriter.WriteUInt16(dst, (ushort)value, ref offset);
                    break;
                case "UInt16[]":
                    GFBufferEncoderWriter.WriteUInt16Arr(dst, (ushort[])value, ref offset);
                    break;
                case "Int32":
                    GFBufferEncoderWriter.WriteInt32(dst, (int)value, ref offset);
                    break;
                case "Int32[]":
                    GFBufferEncoderWriter.WriteInt32Arr(dst, (int[])value, ref offset);
                    break;
                case "UInt32":
                    GFBufferEncoderWriter.WriteUInt32(dst, (uint)value, ref offset);
                    break;
                case "UInt32[]":
                    GFBufferEncoderWriter.WriteUInt32Arr(dst, (uint[])value, ref offset);
                    break;
                case "Int64":
                    GFBufferEncoderWriter.WriteInt64(dst, (long)value, ref offset);
                    break;
                case "Int64[]":
                    GFBufferEncoderWriter.WriteInt64Arr(dst, (long[])value, ref offset);
                    break;
                case "UInt64":
                    GFBufferEncoderWriter.WriteUInt64(dst, (ulong)value, ref offset);
                    break;
                case "UInt64[]":
                    GFBufferEncoderWriter.WriteUInt64Arr(dst, (ulong[])value, ref offset);
                    break;
                case "Single":
                    GFBufferEncoderWriter.WriteSingle(dst, (float)value, ref offset);
                    break;
                case "Single[]":
                    GFBufferEncoderWriter.WriteSingleArr(dst, (float[])value, ref offset);
                    break;
                case "Double":
                    GFBufferEncoderWriter.WriteDouble(dst, (double)value, ref offset);
                    break;
                case "Double[]":
                    GFBufferEncoderWriter.WriteDoubleArr(dst, (double[])value, ref offset);
                    break;
                case "Char":
                    GFBufferEncoderWriter.WriteChar(dst, (char)value, ref offset);
                    break;
                case "String":
                    GFBufferEncoderWriter.WriteUTF8String(dst, (string)value, ref offset);
                    break;
                case "String[]":
                    GFBufferEncoderWriter.WriteUTF8StringArr(dst, (string[])value, ref offset);
                    break;
                default:
                    System.Console.WriteLine("Unsupport type: " + fielType);
                    break;
            }

        }

        static object ReadField(byte[] data, string fielType, ref int offset) {

            // - Sign
            byte typeSign = GFBufferEncoderReader.ReadUInt8(data, ref offset);

            // - Data
            switch (fielType) {
                case "Boolean":
                    return GFBufferEncoderReader.ReadBool(data, ref offset);
                case "Boolean[]":
                    return GFBufferEncoderReader.ReadBoolArr(data, ref offset);
                case "Byte":
                    return GFBufferEncoderReader.ReadUInt8(data, ref offset);
                case "Byte[]":
                    return GFBufferEncoderReader.ReadUInt8Arr(data, ref offset);
                case "SByte":
                    return GFBufferEncoderReader.ReadInt8(data, ref offset);
                case "SByte[]":
                    return GFBufferEncoderReader.ReadInt8Arr(data, ref offset);
                case "Int16":
                    return GFBufferEncoderReader.ReadInt16(data, ref offset);
                case "Int16[]":
                    return GFBufferEncoderReader.ReadInt16Arr(data, ref offset);
                case "UInt16":
                    return GFBufferEncoderReader.ReadUInt16(data, ref offset);
                case "UInt16[]":
                    return GFBufferEncoderReader.ReadUInt16Arr(data, ref offset);
                case "Int32":
                    return GFBufferEncoderReader.ReadInt32(data, ref offset);
                case "Int32[]":
                    return GFBufferEncoderReader.ReadInt32Arr(data, ref offset);
                case "UInt32":
                    return GFBufferEncoderReader.ReadUInt32(data, ref offset);
                case "UInt32[]":
                    return GFBufferEncoderReader.ReadUInt32Arr(data, ref offset);
                case "Int64":
                    return GFBufferEncoderReader.ReadInt64(data, ref offset);
                case "Int64[]":
                    return GFBufferEncoderReader.ReadInt64Arr(data, ref offset);
                case "UInt64":
                    return GFBufferEncoderReader.ReadUInt64(data, ref offset);
                case "UInt64[]":
                    return GFBufferEncoderReader.ReadUInt64Arr(data, ref offset);
                case "Single":
                    return GFBufferEncoderReader.ReadSingle(data, ref offset);
                case "Single[]":
                    return GFBufferEncoderReader.ReadSingleArr(data, ref offset);
                case "Double":
                    return GFBufferEncoderReader.ReadDouble(data, ref offset);
                case "Double[]":
                    return GFBufferEncoderReader.ReadDoubleArr(data, ref offset);
                case "Char":
                    return GFBufferEncoderReader.ReadChar(data, ref offset);
                case "String":
                    return GFBufferEncoderReader.ReadUTF8String(data, ref offset);
                case "String[]":
                    return GFBufferEncoderReader.ReadUTF8StringArr(data, ref offset);
                default:
                    System.Console.WriteLine("Unsupport type: " + fielType);
                    return null;
            }

        }

        static byte GetTypeSign(string type) {
            switch (type) {
                case "Boolean":
                    return 1;
                case "Boolean[]":
                    return 2;
                case "Byte":
                    return 3;
                case "Byte[]":
                    return 4;
                case "SByte":
                    return 5;
                case "SByte[]":
                    return 6;
                case "Int16":
                    return 7;
                case "Int16[]":
                    return 8;
                case "UInt16":
                    return 9;
                case "UInt16[]":
                    return 10;
                case "Int32":
                    return 11;
                case "Int32[]":
                    return 12;
                case "UInt32":
                    return 13;
                case "UInt32[]":
                    return 14;
                case "Int64":
                    return 15;
                case "Int64[]":
                    return 16;
                case "UInt64":
                    return 17;
                case "UInt64[]":
                    return 18;
                case "Single":
                    return 19;
                case "Single[]":
                    return 20;
                case "Double":
                    return 21;
                case "Double[]":
                    return 22;
                case "Char":
                    return 23;
                case "Char[]":
                    return 24;
                case "String":
                    return 25;
                case "String[]":
                    return 26;
                default:
                    return 0;
            }
        }

    }

}