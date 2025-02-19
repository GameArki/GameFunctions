using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameFunctions {

    public static class GFBufferEncoderWriter {

        public static void WriteBool(byte[] dst, bool data, ref int offset) {
            byte b = data ? (byte)1 : (byte)0;
            WriteUInt8(dst, b, ref offset);
        }

        public static void WriteInt8(byte[] dst, sbyte data, ref int offset) {
            WriteUInt8(dst, (byte)data, ref offset);
        }

        public static void WriteUInt8(byte[] dst, byte data, ref int offset) {
            dst[offset++] = data;
        }

        public static void WriteChar(byte[] dst, char data, ref int offset) {
            Bit16 content = new Bit16();
            content.charValue = data;
            dst[offset++] = content.b0;
            dst[offset++] = content.b1;
        }

        public static void WriteInt16(byte[] dst, short data, ref int offset) {
            WriteUInt16(dst, (ushort)data, ref offset);
        }

        public static void WriteUInt16(byte[] dst, ushort data, ref int offset) {
            Bit16 content = new Bit16();
            content.ushortValue = data;
            dst[offset++] = content.b0;
            dst[offset++] = content.b1;
        }

        public static void WriteSingle(byte[] dst, float data, ref int offset) {
            Bit32 content = new Bit32();
            content.floatValue = data;
            dst[offset++] = content.b0;
            dst[offset++] = content.b1;
            dst[offset++] = content.b2;
            dst[offset++] = content.b3;
        }

        public static void WriteInt32(byte[] dst, int data, ref int offset) {
            WriteUInt32(dst, (uint)data, ref offset);
        }

        public static void WriteUInt32(byte[] dst, uint data, ref int offset) {
            Bit32 content = new Bit32();
            content.uintValue = data;
            dst[offset++] = content.b0;
            dst[offset++] = content.b1;
            dst[offset++] = content.b2;
            dst[offset++] = content.b3;
        }

        public static void WriteDouble(byte[] dst, double data, ref int offset) {
            Bit64 content = new Bit64();
            content.doubleValue = data;
            dst[offset++] = content.b0;
            dst[offset++] = content.b1;
            dst[offset++] = content.b2;
            dst[offset++] = content.b3;
            dst[offset++] = content.b4;
            dst[offset++] = content.b5;
            dst[offset++] = content.b6;
            dst[offset++] = content.b7;
        }

        public static void WriteDecimal(byte[] dst, decimal data, ref int offset) {
            Bit128 content = new Bit128();
            content.decimalValue = data;
            dst[offset++] = content.b0;
            dst[offset++] = content.b1;
            dst[offset++] = content.b2;
            dst[offset++] = content.b3;
            dst[offset++] = content.b4;
            dst[offset++] = content.b5;
            dst[offset++] = content.b6;
            dst[offset++] = content.b7;
            dst[offset++] = content.b8;
            dst[offset++] = content.b9;
            dst[offset++] = content.b10;
            dst[offset++] = content.b11;
            dst[offset++] = content.b12;
            dst[offset++] = content.b13;
            dst[offset++] = content.b14;
            dst[offset++] = content.b15;
        }

        public static void WriteInt64(byte[] dst, long data, ref int offset) {
            WriteUInt64(dst, (ulong)data, ref offset);
        }

        public static void WriteUInt64(byte[] dst, ulong data, ref int offset) {
            Bit64 content = new Bit64();
            content.ulongValue = data;
            dst[offset++] = content.b0;
            dst[offset++] = content.b1;
            dst[offset++] = content.b2;
            dst[offset++] = content.b3;
            dst[offset++] = content.b4;
            dst[offset++] = content.b5;
            dst[offset++] = content.b6;
            dst[offset++] = content.b7;
        }

        public static void WriteUTF8String(byte[] dst, string data, ref int offset) {
            if (!string.IsNullOrEmpty(data)) {
                byte[] d = Encoding.UTF8.GetBytes(data);
                ushort count = (ushort)d.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(d, 0, dst, offset, count);
                offset += count;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteUTF8StringArr(byte[] dst, string[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Length; i += 1) {
                    WriteUTF8String(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteUTF8StringList(byte[] dst, List<string> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteUTF8String(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteBoolArr(byte[] dst, bool[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count);
                offset += count;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteBoolList(byte[] dst, List<bool> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteBool(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteInt8Arr(byte[] dst, sbyte[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count);
                offset += count;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteInt8List(byte[] dst, List<sbyte> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteInt8(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteUint8Arr(byte[] dst, byte[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count);
                offset += count;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteUint8List(byte[] dst, List<byte> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteUInt8(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteInt16Arr(byte[] dst, short[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count * 2);
                offset += count * 2;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteInt16List(byte[] dst, List<short> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteInt16(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteUInt16Arr(byte[] dst, ushort[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count * 2);
                offset += count * 2;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteUInt16List(byte[] dst, List<ushort> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteUInt16(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteInt32Arr(byte[] dst, int[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count * 4);
                offset += count * 4;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteInt32List(byte[] dst, List<int> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteInt32(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteUInt32Arr(byte[] dst, uint[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count * 4);
                offset += count * 4;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteUInt32List(byte[] dst, List<uint> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteUInt32(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteSingleArr(byte[] dst, float[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count * 4);
                offset += count * 4;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteSingleList(byte[] dst, List<float> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteSingle(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteInt64Arr(byte[] dst, long[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count * 8);
                offset += count * 8;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteInt64List(byte[] dst, List<long> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteInt64(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteUInt64Arr(byte[] dst, ulong[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count * 8);
                offset += count * 8;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteUInt64List(byte[] dst, List<ulong> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteUInt64(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteDoubleArr(byte[] dst, double[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count * 8);
                offset += count * 8;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteDoubleList(byte[] dst, List<double> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteDouble(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteDecimalArr(byte[] dst, decimal[] data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Length;
                WriteUInt16(dst, count, ref offset);
                Buffer.BlockCopy(data, 0, dst, offset, count * 16);
                offset += count * 16;
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteDecimalList(byte[] dst, List<decimal> data, ref int offset) {
            if (data != null) {
                ushort count = (ushort)data.Count;
                WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteDecimal(dst, data[i], ref offset);
                }
            } else {
                WriteUInt16(dst, 0, ref offset);
            }
        }

        // ==== VARINT ====
        public static void WriteVarint(byte[] dst, ulong data, ref int offset) {
            while (data >= 0x80) {
                dst[offset++] = (byte)(data | 0x80);
                data >>= 7;
            }
            dst[offset++] = (byte)data;
        }

        public static void WriteVarintWithZigZag(byte[] dst, long data, ref int offset) {
            ulong udata = WriteZigZag(data);
            WriteVarint(dst, udata, ref offset);
        }

        static ulong WriteZigZag(long value) {
            bool isNegative = value < 0;
            ulong uv = (ulong)value;
            unchecked {
                if (isNegative) {
                    return ((~uv + 1ul) << 1) + 1ul;
                } else {
                    return uv << 1;
                }
            }
        }

        public static void WriteMessage<T>(byte[] dst, T data, ref int offset) where T : struct, IGFBufferEncoderMessage<T> {
            data.WriteTo(dst, ref offset);
        }

        public static void WriteMessageArr<T>(byte[] dst, T[] data, ref int offset) where T : struct, IGFBufferEncoderMessage<T> {
            if (data != null) {
                ushort count = (ushort)data.Length;
                GFBufferEncoderWriter.WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Length; i += 1) {
                    WriteMessage(dst, data[i], ref offset);
                }
            } else {
                GFBufferEncoderWriter.WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteMessageList<T>(byte[] dst, List<T> data, ref int offset) where T : struct, IGFBufferEncoderMessage<T> {
            if (data != null) {
                ushort count = (ushort)data.Count;
                GFBufferEncoderWriter.WriteUInt16(dst, count, ref offset);
                for (int i = 0; i < data.Count; i += 1) {
                    WriteMessage(dst, data[i], ref offset);
                }
            } else {
                GFBufferEncoderWriter.WriteUInt16(dst, 0, ref offset);
            }
        }

        public static void WriteVector2(byte[] dst, Vector2 data, ref int offset) {
            GFBufferEncoderWriter.WriteSingle(dst, data.x, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.y, ref offset);
        }

        public static void WriteVector2Array(byte[] dst, Vector2[] data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Length, ref offset);
            for (int i = 0; i < data.Length; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].y, ref offset);
            }
        }

        public static void WriteVector2List(byte[] dst, List<Vector2> data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Count, ref offset);
            for (int i = 0; i < data.Count; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].y, ref offset);
            }
        }

        public static void WriteVector2Int(byte[] dst, Vector2Int data, ref int offset) {
            GFBufferEncoderWriter.WriteInt32(dst, data.x, ref offset);
            GFBufferEncoderWriter.WriteInt32(dst, data.y, ref offset);
        }

        public static void WriteVector2IntArray(byte[] dst, Vector2Int[] data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Length, ref offset);
            for (int i = 0; i < data.Length; i++) {
                GFBufferEncoderWriter.WriteInt32(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteInt32(dst, data[i].y, ref offset);
            }
        }

        public static void WriteVector2IntList(byte[] dst, List<Vector2Int> data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Count, ref offset);
            for (int i = 0; i < data.Count; i++) {
                GFBufferEncoderWriter.WriteInt32(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteInt32(dst, data[i].y, ref offset);
            }
        }

        public static void WriteVector3(byte[] dst, Vector3 data, ref int offset) {
            GFBufferEncoderWriter.WriteSingle(dst, data.x, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.y, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.z, ref offset);
        }

        public static void WriteVector3Array(byte[] dst, Vector3[] data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Length, ref offset);
            for (int i = 0; i < data.Length; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].y, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].z, ref offset);
            }
        }

        public static void WriteVector3List(byte[] dst, List<Vector3> data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Count, ref offset);
            for (int i = 0; i < data.Count; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].y, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].z, ref offset);
            }
        }

        public static void WriteVector3Int(byte[] dst, Vector3Int data, ref int offset) {
            GFBufferEncoderWriter.WriteInt32(dst, data.x, ref offset);
            GFBufferEncoderWriter.WriteInt32(dst, data.y, ref offset);
            GFBufferEncoderWriter.WriteInt32(dst, data.z, ref offset);
        }

        public static void WriteVector3IntArray(byte[] dst, Vector3Int[] data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Length, ref offset);
            for (int i = 0; i < data.Length; i++) {
                GFBufferEncoderWriter.WriteInt32(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteInt32(dst, data[i].y, ref offset);
                GFBufferEncoderWriter.WriteInt32(dst, data[i].z, ref offset);
            }
        }

        public static void WriteVector3IntList(byte[] dst, List<Vector3Int> data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Count, ref offset);
            for (int i = 0; i < data.Count; i++) {
                GFBufferEncoderWriter.WriteInt32(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteInt32(dst, data[i].y, ref offset);
                GFBufferEncoderWriter.WriteInt32(dst, data[i].z, ref offset);
            }
        }

        public static void WriteVector4(byte[] dst, Vector4 data, ref int offset) {
            GFBufferEncoderWriter.WriteSingle(dst, data.x, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.y, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.z, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.w, ref offset);
        }

        public static void WriteVector4Array(byte[] dst, Vector4[] data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Length, ref offset);
            for (int i = 0; i < data.Length; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].y, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].z, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].w, ref offset);
            }
        }

        public static void WriteVector4List(byte[] dst, List<Vector4> data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Count, ref offset);
            for (int i = 0; i < data.Count; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].y, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].z, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].w, ref offset);
            }
        }

        public static void WriteColor(byte[] dst, Color data, ref int offset) {
            GFBufferEncoderWriter.WriteSingle(dst, data.r, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.g, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.b, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.a, ref offset);
        }

        public static void WriteColorArray(byte[] dst, Color[] data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Length, ref offset);
            for (int i = 0; i < data.Length; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].r, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].g, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].b, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].a, ref offset);
            }
        }

        public static void WriteColorList(byte[] dst, List<Color> data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Count, ref offset);
            for (int i = 0; i < data.Count; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].r, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].g, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].b, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].a, ref offset);
            }
        }

        public static void WriteColor32(byte[] dst, Color32 data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt8(dst, data.r, ref offset);
            GFBufferEncoderWriter.WriteUInt8(dst, data.g, ref offset);
            GFBufferEncoderWriter.WriteUInt8(dst, data.b, ref offset);
            GFBufferEncoderWriter.WriteUInt8(dst, data.a, ref offset);
        }

        public static void WriteColor32Array(byte[] dst, Color32[] data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Length, ref offset);
            for (int i = 0; i < data.Length; i++) {
                GFBufferEncoderWriter.WriteUInt8(dst, data[i].r, ref offset);
                GFBufferEncoderWriter.WriteUInt8(dst, data[i].g, ref offset);
                GFBufferEncoderWriter.WriteUInt8(dst, data[i].b, ref offset);
                GFBufferEncoderWriter.WriteUInt8(dst, data[i].a, ref offset);
            }
        }

        public static void WriteColor32List(byte[] dst, List<Color32> data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Count, ref offset);
            for (int i = 0; i < data.Count; i++) {
                GFBufferEncoderWriter.WriteUInt8(dst, data[i].r, ref offset);
                GFBufferEncoderWriter.WriteUInt8(dst, data[i].g, ref offset);
                GFBufferEncoderWriter.WriteUInt8(dst, data[i].b, ref offset);
                GFBufferEncoderWriter.WriteUInt8(dst, data[i].a, ref offset);
            }
        }

        public static void WriteQuaternion(byte[] dst, Quaternion data, ref int offset) {
            GFBufferEncoderWriter.WriteSingle(dst, data.x, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.y, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.z, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.w, ref offset);
        }

        public static void WriteQuaternionArray(byte[] dst, Quaternion[] data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Length, ref offset);
            for (int i = 0; i < data.Length; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].y, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].z, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].w, ref offset);
            }
        }

        public static void WriteQuaternionList(byte[] dst, List<Quaternion> data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Count, ref offset);
            for (int i = 0; i < data.Count; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].y, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].z, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].w, ref offset);
            }
        }

        public static void WriteRect(byte[] dst, Rect data, ref int offset) {
            GFBufferEncoderWriter.WriteSingle(dst, data.x, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.y, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.width, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, data.height, ref offset);
        }

        public static void WriteRectArray(byte[] dst, Rect[] data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Length, ref offset);
            for (int i = 0; i < data.Length; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].y, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].width, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].height, ref offset);
            }
        }

        public static void WriteRectList(byte[] dst, List<Rect> data, ref int offset) {
            GFBufferEncoderWriter.WriteUInt16(dst, (ushort)data.Count, ref offset);
            for (int i = 0; i < data.Count; i++) {
                GFBufferEncoderWriter.WriteSingle(dst, data[i].x, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].y, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].width, ref offset);
                GFBufferEncoderWriter.WriteSingle(dst, data[i].height, ref offset);
            }
        }

    }

}