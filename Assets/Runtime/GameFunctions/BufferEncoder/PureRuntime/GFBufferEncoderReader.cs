using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameFunctions {

    public static class GFBufferEncoderReader {

        public static bool ReadBool(byte[] src, ref int offset) {
            return ReadUInt8(src, ref offset) == 1;
        }

        public static sbyte ReadInt8(byte[] src, ref int offset) {
            return (sbyte)ReadUInt8(src, ref offset);
        }

        public static byte ReadUInt8(byte[] src, ref int offset) {
            return src[offset++];
        }

        public static char ReadChar(byte[] src, ref int offset) {
            char data = (char)ReadUInt16(src, ref offset);
            return data;
        }

        public static short ReadInt16(byte[] src, ref int offset) {
            return (short)ReadUInt16(src, ref offset);
        }

        public static ushort ReadUInt16(byte[] src, ref int offset) {
            Bit16 content = new Bit16();
            content.b0 = src[offset++];
            content.b1 = src[offset++];
            return content.ushortValue;
        }

        public static int ReadInt32(byte[] src, ref int offset) {
            return (int)ReadUInt32(src, ref offset);
        }

        public static uint ReadUInt32(byte[] src, ref int offset) {
            Bit32 content = new Bit32();
            content.b0 = src[offset++];
            content.b1 = src[offset++];
            content.b2 = src[offset++];
            content.b3 = src[offset++];
            return content.uintValue;
        }

        public static long ReadInt64(byte[] src, ref int offset) {
            return (long)ReadUInt64(src, ref offset);
        }

        public static ulong ReadUInt64(byte[] src, ref int offset) {
            Bit64 content = new Bit64();
            content.b0 = src[offset++];
            content.b1 = src[offset++];
            content.b2 = src[offset++];
            content.b3 = src[offset++];
            content.b4 = src[offset++];
            content.b5 = src[offset++];
            content.b6 = src[offset++];
            content.b7 = src[offset++];
            return content.ulongValue;
        }

        public static float ReadSingle(byte[] src, ref int offset) {
            Bit32 content = new Bit32();
            content.b0 = src[offset++];
            content.b1 = src[offset++];
            content.b2 = src[offset++];
            content.b3 = src[offset++];
            return content.floatValue;
        }

        public static double ReadDouble(byte[] src, ref int offset) {
            Bit64 content = new Bit64();
            content.b0 = src[offset++];
            content.b1 = src[offset++];
            content.b2 = src[offset++];
            content.b3 = src[offset++];
            content.b4 = src[offset++];
            content.b5 = src[offset++];
            content.b6 = src[offset++];
            content.b7 = src[offset++];
            return content.doubleValue;
        }

        public static decimal ReadDecimal(byte[] src, ref int offset) {
            Bit128 content = new Bit128();
            content.b0 = src[offset++];
            content.b1 = src[offset++];
            content.b2 = src[offset++];
            content.b3 = src[offset++];
            content.b4 = src[offset++];
            content.b5 = src[offset++];
            content.b6 = src[offset++];
            content.b7 = src[offset++];
            content.b8 = src[offset++];
            content.b9 = src[offset++];
            content.b10 = src[offset++];
            content.b11 = src[offset++];
            content.b12 = src[offset++];
            content.b13 = src[offset++];
            content.b14 = src[offset++];
            content.b15 = src[offset++];
            return content.decimalValue;
        }

        public static bool[] ReadBoolArr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            bool[] data = new bool[count];
            Buffer.BlockCopy(src, offset, data, 0, count);
            offset += count;
            return data;
        }

        public static List<bool> ReadBoolList(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<bool> data = new List<bool>(count);
            for (int i = 0; i < count; i += 1) {
                bool d = ReadBool(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static byte[] ReadUInt8Arr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            byte[] data = new byte[count];
            Buffer.BlockCopy(src, offset, data, 0, count);
            offset += count;
            return data;
        }

        public static List<byte> ReadUInt8List(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<byte> data = new List<byte>(count);
            for (int i = 0; i < count; i += 1) {
                byte d = ReadUInt8(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static sbyte[] ReadInt8Arr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            sbyte[] data = new sbyte[count];
            Buffer.BlockCopy(src, offset, data, 0, count);
            offset += count;
            return data;
        }

        public static List<sbyte> ReadInt8List(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<sbyte> data = new List<sbyte>(count);
            for (int i = 0; i < count; i += 1) {
                sbyte d = ReadInt8(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static short[] ReadInt16Arr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            short[] data = new short[count];
            Buffer.BlockCopy(src, offset, data, 0, count * 2);
            offset += count * 2;
            return data;
        }

        public static List<short> ReadInt16List(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<short> data = new List<short>(count);
            for (int i = 0; i < count; i += 1) {
                short d = ReadInt16(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static ushort[] ReadUInt16Arr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            ushort[] data = new ushort[count];
            Buffer.BlockCopy(src, offset, data, 0, count * 2);
            offset += count * 2;
            return data;
        }

        public static List<ushort> ReadUInt16List(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<ushort> data = new List<ushort>(count);
            for (int i = 0; i < count; i += 1) {
                ushort d = ReadUInt16(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static int[] ReadInt32Arr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            int[] data = new int[count];
            Buffer.BlockCopy(src, offset, data, 0, count * 4);
            offset += count * 4;
            return data;
        }

        public static List<int> ReadInt32List(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<int> data = new List<int>(count);
            for (int i = 0; i < count; i += 1) {
                int d = ReadInt32(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static uint[] ReadUInt32Arr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            uint[] data = new uint[count];
            Buffer.BlockCopy(src, offset, data, 0, count * 4);
            offset += count * 4;
            return data;
        }

        public static List<uint> ReadUInt32List(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<uint> data = new List<uint>(count);
            for (int i = 0; i < count; i += 1) {
                uint d = ReadUInt32(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static long[] ReadInt64Arr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            long[] data = new long[count];
            Buffer.BlockCopy(src, offset, data, 0, count * 8);
            offset += count * 8;
            return data;
        }

        public static List<long> ReadInt64List(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<long> data = new List<long>(count);
            for (int i = 0; i < count; i += 1) {
                long d = ReadInt64(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static ulong[] ReadUInt64Arr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            ulong[] data = new ulong[count];
            Buffer.BlockCopy(src, offset, data, 0, count * 8);
            offset += count * 8;
            return data;
        }

        public static List<ulong> ReadUInt64List(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<ulong> data = new List<ulong>(count);
            for (int i = 0; i < count; i += 1) {
                ulong d = ReadUInt64(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static float[] ReadSingleArr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            float[] data = new float[count];
            Buffer.BlockCopy(src, offset, data, 0, count * 4);
            offset += count * 4;
            return data;
        }

        public static List<float> ReadSingleList(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<float> data = new List<float>(count);
            for (int i = 0; i < count; i += 1) {
                float d = ReadSingle(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static double[] ReadDoubleArr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            double[] data = new double[count];
            Buffer.BlockCopy(src, offset, data, 0, count * 8);
            offset += count * 8;
            return data;
        }

        public static List<double> ReadDoubleList(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<double> data = new List<double>(count);
            for (int i = 0; i < count; i += 1) {
                double d = ReadDouble(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static decimal[] ReadDecimalArr(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            decimal[] data = new decimal[count];
            Buffer.BlockCopy(src, offset, data, 0, count * 16);
            offset += count * 16;
            return data;
        }

        public static List<decimal> ReadDecimalList(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            List<decimal> data = new List<decimal>(count);
            for (int i = 0; i < count; i += 1) {
                decimal d = ReadDecimal(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        public static string ReadUTF8String(byte[] src, ref int offset) {
            ushort count = ReadUInt16(src, ref offset);
            string data = Encoding.UTF8.GetString(src, offset, count);
            offset += count;
            return data;
        }

        public static string[] ReadUTF8StringArr(byte[] src, ref int offset) {
            ushort totalCount = ReadUInt16(src, ref offset);
            string[] data = new string[totalCount];
            for (int i = 0; i < totalCount; i += 1) {
                string d = ReadUTF8String(src, ref offset);
                data[i] = d;
            }
            return data;
        }

        public static List<string> ReadUTF8StringList(byte[] src, ref int offset) {
            ushort totalCount = ReadUInt16(src, ref offset);
            List<string> data = new List<string>(totalCount);
            for (int i = 0; i < totalCount; i += 1) {
                string d = ReadUTF8String(src, ref offset);
                data.Add(d);
            }
            return data;
        }

        // ==== VARINT ====
        public static ulong ReadVarint(byte[] src, ref int offset) {
            ulong data = 0;
            byte b = 0;
            for (int i = 0, j = 0; ; i += 1, j += 7) {
                b = src[offset++];
                if ((b & 0x80) != 0) {
                    data |= (ulong)(b & 0x7F) << j;
                } else {
                    data |= (ulong)b << j;
                    break;
                }
                if (i >= 9 && b > 0) {
                    throw new Exception("overflow");
                }
            }
            return data;
        }

        public static ulong ReadVarintWithZigZag(byte[] src, ref int offset) {
            ulong udata = (ulong)ReadVarint(src, ref offset);
            ulong data = ReadZigZag(udata);
            return data;
        }

        static ulong ReadZigZag(ulong value) {
            bool isNegative = value % 2 != 0;
            unchecked {
                if (isNegative) {
                    return (~(value >> 1) + 1ul) | 0x8000_0000_0000_0000ul;
                } else {
                    return value >> 1;
                }
            }
        }

        public static T ReadMessage<T>(byte[] src, Func<T> createHandle, ref int offset) where T : struct, IGFBufferEncoderMessage<T> {
            T msg = createHandle.Invoke();
            msg.FromBytes(src, ref offset);
            return msg;
        }

        public static T[] ReadMessageArr<T>(byte[] src, Func<T> createHandle, ref int offset) where T : struct, IGFBufferEncoderMessage<T> {
            ushort totalCount = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            T[] arr = new T[totalCount];
            for (int i = 0; i < arr.Length; i += 1) {
                arr[i] = ReadMessage(src, createHandle, ref offset);
            }
            return arr;
        }

        public static List<T> ReadMessageList<T>(byte[] src, Func<T> createHandle, ref int offset) where T : struct, IGFBufferEncoderMessage<T> {
            ushort totalCount = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            List<T> list = new List<T>(totalCount);
            for (int i = 0; i < totalCount; i += 1) {
                list.Add(ReadMessage(src, createHandle, ref offset));
            }
            return list;
        }

        public static Vector2 ReadVector2(byte[] src, ref int offset) {
            return new Vector2(GFBufferEncoderReader.ReadSingle(src, ref offset),
                               GFBufferEncoderReader.ReadSingle(src, ref offset));
        }

        public static Vector2[] ReadVector2Array(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            Vector2[] data = new Vector2[length];
            for (int i = 0; i < length; i++) {
                data[i] = new Vector2(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                      GFBufferEncoderReader.ReadSingle(src, ref offset));
            }
            return data;
        }

        public static List<Vector2> ReadVector2List(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            List<Vector2> data = new List<Vector2>(length);
            for (int i = 0; i < length; i++) {
                data.Add(new Vector2(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                     GFBufferEncoderReader.ReadSingle(src, ref offset)));
            }
            return data;
        }

        public static Vector2Int ReadVector2Int(byte[] src, ref int offset) {
            return new Vector2Int(GFBufferEncoderReader.ReadInt32(src, ref offset),
                                  GFBufferEncoderReader.ReadInt32(src, ref offset));
        }

        public static Vector2Int[] ReadVector2IntArray(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            Vector2Int[] data = new Vector2Int[length];
            for (int i = 0; i < length; i++) {
                data[i] = new Vector2Int(GFBufferEncoderReader.ReadInt32(src, ref offset),
                                         GFBufferEncoderReader.ReadInt32(src, ref offset));
            }
            return data;
        }

        public static List<Vector2Int> ReadVector2IntList(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            List<Vector2Int> data = new List<Vector2Int>(length);
            for (int i = 0; i < length; i++) {
                data.Add(new Vector2Int(GFBufferEncoderReader.ReadInt32(src, ref offset),
                                        GFBufferEncoderReader.ReadInt32(src, ref offset)));
            }
            return data;
        }

        public static Vector3 ReadVector3(byte[] src, ref int offset) {
            return new Vector3(GFBufferEncoderReader.ReadSingle(src, ref offset),
                               GFBufferEncoderReader.ReadSingle(src, ref offset),
                               GFBufferEncoderReader.ReadSingle(src, ref offset));
        }

        public static Vector3[] ReadVector3Array(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            Vector3[] data = new Vector3[length];
            for (int i = 0; i < length; i++) {
                data[i] = new Vector3(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                      GFBufferEncoderReader.ReadSingle(src, ref offset),
                                      GFBufferEncoderReader.ReadSingle(src, ref offset));
            }
            return data;
        }

        public static List<Vector3> ReadVector3List(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            List<Vector3> data = new List<Vector3>(length);
            for (int i = 0; i < length; i++) {
                data.Add(new Vector3(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                     GFBufferEncoderReader.ReadSingle(src, ref offset),
                                     GFBufferEncoderReader.ReadSingle(src, ref offset)));
            }
            return data;
        }

        public static Vector3Int ReadVector3Int(byte[] src, ref int offset) {
            return new Vector3Int(GFBufferEncoderReader.ReadInt32(src, ref offset),
                                  GFBufferEncoderReader.ReadInt32(src, ref offset),
                                  GFBufferEncoderReader.ReadInt32(src, ref offset));
        }

        public static Vector3Int[] ReadVector3IntArray(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            Vector3Int[] data = new Vector3Int[length];
            for (int i = 0; i < length; i++) {
                data[i] = new Vector3Int(GFBufferEncoderReader.ReadInt32(src, ref offset),
                                         GFBufferEncoderReader.ReadInt32(src, ref offset),
                                         GFBufferEncoderReader.ReadInt32(src, ref offset));
            }
            return data;
        }

        public static List<Vector3Int> ReadVector3IntList(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            List<Vector3Int> data = new List<Vector3Int>(length);
            for (int i = 0; i < length; i++) {
                data.Add(new Vector3Int(GFBufferEncoderReader.ReadInt32(src, ref offset),
                                        GFBufferEncoderReader.ReadInt32(src, ref offset),
                                        GFBufferEncoderReader.ReadInt32(src, ref offset)));
            }
            return data;
        }

        public static Vector4 ReadVector4(byte[] src, ref int offset) {
            return new Vector4(GFBufferEncoderReader.ReadSingle(src, ref offset),
                               GFBufferEncoderReader.ReadSingle(src, ref offset),
                               GFBufferEncoderReader.ReadSingle(src, ref offset),
                               GFBufferEncoderReader.ReadSingle(src, ref offset));
        }

        public static Vector4[] ReadVector4Array(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            Vector4[] data = new Vector4[length];
            for (int i = 0; i < length; i++) {
                data[i] = new Vector4(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                      GFBufferEncoderReader.ReadSingle(src, ref offset),
                                      GFBufferEncoderReader.ReadSingle(src, ref offset),
                                      GFBufferEncoderReader.ReadSingle(src, ref offset));
            }
            return data;
        }

        public static List<Vector4> ReadVector4List(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            List<Vector4> data = new List<Vector4>(length);
            for (int i = 0; i < length; i++) {
                data.Add(new Vector4(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                     GFBufferEncoderReader.ReadSingle(src, ref offset),
                                     GFBufferEncoderReader.ReadSingle(src, ref offset),
                                     GFBufferEncoderReader.ReadSingle(src, ref offset)));
            }
            return data;
        }

        public static Color ReadColor(byte[] src, ref int offset) {
            return new Color(GFBufferEncoderReader.ReadSingle(src, ref offset),
                             GFBufferEncoderReader.ReadSingle(src, ref offset),
                             GFBufferEncoderReader.ReadSingle(src, ref offset),
                             GFBufferEncoderReader.ReadSingle(src, ref offset));
        }

        public static Color[] ReadColorArray(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            Color[] data = new Color[length];
            for (int i = 0; i < length; i++) {
                data[i] = new Color(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                    GFBufferEncoderReader.ReadSingle(src, ref offset),
                                    GFBufferEncoderReader.ReadSingle(src, ref offset),
                                    GFBufferEncoderReader.ReadSingle(src, ref offset));
            }
            return data;
        }

        public static List<Color> ReadColorList(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            List<Color> data = new List<Color>(length);
            for (int i = 0; i < length; i++) {
                data.Add(new Color(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                   GFBufferEncoderReader.ReadSingle(src, ref offset),
                                   GFBufferEncoderReader.ReadSingle(src, ref offset),
                                   GFBufferEncoderReader.ReadSingle(src, ref offset)));
            }
            return data;
        }

        public static Color32 ReadColor32(byte[] src, ref int offset) {
            return new Color32(GFBufferEncoderReader.ReadUInt8(src, ref offset),
                               GFBufferEncoderReader.ReadUInt8(src, ref offset),
                               GFBufferEncoderReader.ReadUInt8(src, ref offset),
                               GFBufferEncoderReader.ReadUInt8(src, ref offset));
        }

        public static Color32[] ReadColor32Array(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            Color32[] data = new Color32[length];
            for (int i = 0; i < length; i++) {
                data[i] = new Color32(GFBufferEncoderReader.ReadUInt8(src, ref offset),
                                      GFBufferEncoderReader.ReadUInt8(src, ref offset),
                                      GFBufferEncoderReader.ReadUInt8(src, ref offset),
                                      GFBufferEncoderReader.ReadUInt8(src, ref offset));
            }
            return data;
        }

        public static List<Color32> ReadColor32List(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            List<Color32> data = new List<Color32>(length);
            for (int i = 0; i < length; i++) {
                data.Add(new Color32(GFBufferEncoderReader.ReadUInt8(src, ref offset),
                                     GFBufferEncoderReader.ReadUInt8(src, ref offset),
                                     GFBufferEncoderReader.ReadUInt8(src, ref offset),
                                     GFBufferEncoderReader.ReadUInt8(src, ref offset)));
            }
            return data;
        }

        public static Quaternion ReadQuaternion(byte[] src, ref int offset) {
            return new Quaternion(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                  GFBufferEncoderReader.ReadSingle(src, ref offset),
                                  GFBufferEncoderReader.ReadSingle(src, ref offset),
                                  GFBufferEncoderReader.ReadSingle(src, ref offset));
        }

        public static Quaternion[] ReadQuaternionArray(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            Quaternion[] data = new Quaternion[length];
            for (int i = 0; i < length; i++) {
                data[i] = new Quaternion(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                         GFBufferEncoderReader.ReadSingle(src, ref offset),
                                         GFBufferEncoderReader.ReadSingle(src, ref offset),
                                         GFBufferEncoderReader.ReadSingle(src, ref offset));
            }
            return data;
        }

        public static List<Quaternion> ReadQuaternionList(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            List<Quaternion> data = new List<Quaternion>(length);
            for (int i = 0; i < length; i++) {
                data.Add(new Quaternion(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                        GFBufferEncoderReader.ReadSingle(src, ref offset),
                                        GFBufferEncoderReader.ReadSingle(src, ref offset),
                                        GFBufferEncoderReader.ReadSingle(src, ref offset)));
            }
            return data;
        }

        public static Rect ReadRect(byte[] src, ref int offset) {
            return new Rect(GFBufferEncoderReader.ReadSingle(src, ref offset),
                            GFBufferEncoderReader.ReadSingle(src, ref offset),
                            GFBufferEncoderReader.ReadSingle(src, ref offset),
                            GFBufferEncoderReader.ReadSingle(src, ref offset));
        }

        public static Rect[] ReadRectArray(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            Rect[] data = new Rect[length];
            for (int i = 0; i < length; i++) {
                data[i] = new Rect(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                   GFBufferEncoderReader.ReadSingle(src, ref offset),
                                   GFBufferEncoderReader.ReadSingle(src, ref offset),
                                   GFBufferEncoderReader.ReadSingle(src, ref offset));
            }
            return data;
        }

        public static List<Rect> ReadRectList(byte[] src, ref int offset) {
            int length = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            List<Rect> data = new List<Rect>(length);
            for (int i = 0; i < length; i++) {
                data.Add(new Rect(GFBufferEncoderReader.ReadSingle(src, ref offset),
                                   GFBufferEncoderReader.ReadSingle(src, ref offset),
                                   GFBufferEncoderReader.ReadSingle(src, ref offset),
                                   GFBufferEncoderReader.ReadSingle(src, ref offset)));
            }
            return data;
        }

    }

}