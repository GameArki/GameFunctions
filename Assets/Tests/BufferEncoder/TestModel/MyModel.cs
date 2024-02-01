using System;
using GameFunctions;

namespace GameFunctions.Tests
{
    [GFBufferEncoderMessage]
    public struct MyModel : IGFBufferEncoderMessage<MyModel>
    {
        public bool boolValue;
        public char charValue;
        public byte byteValue;
        public sbyte sbyteValue;
        public short shortValue;
        public ushort ushortValue;
        public int intValue;
        public uint uintValue;
        public long longValue;
        public ulong ulongValue;
        public float floatValue;
        public double doubleValue;
        public bool[] boolArr;
        public byte[] byteArr;
        public sbyte[] sbyteArr;
        public short[] shortArr;
        public ushort[] ushortArr;
        public int[] intArr;
        public uint[] uintArr;
        public long[] longArr;
        public ulong[] ulongArr;
        public float[] floatArr;
        public double[] doubleArr;
        public string strValue;
        public string[] strArr;
        public HerModel herModel;
        public HerModel[] herModelArr;
        public string otherStr;
        public void WriteTo(byte[] dst, ref int offset)
        {
            GFBufferEncoderWriter.WriteBool(dst, boolValue, ref offset);
            GFBufferEncoderWriter.WriteChar(dst, charValue, ref offset);
            GFBufferEncoderWriter.WriteUInt8(dst, byteValue, ref offset);
            GFBufferEncoderWriter.WriteInt8(dst, sbyteValue, ref offset);
            GFBufferEncoderWriter.WriteInt16(dst, shortValue, ref offset);
            GFBufferEncoderWriter.WriteUInt16(dst, ushortValue, ref offset);
            GFBufferEncoderWriter.WriteInt32(dst, intValue, ref offset);
            GFBufferEncoderWriter.WriteUInt32(dst, uintValue, ref offset);
            GFBufferEncoderWriter.WriteInt64(dst, longValue, ref offset);
            GFBufferEncoderWriter.WriteUInt64(dst, ulongValue, ref offset);
            GFBufferEncoderWriter.WriteSingle(dst, floatValue, ref offset);
            GFBufferEncoderWriter.WriteDouble(dst, doubleValue, ref offset);
            GFBufferEncoderWriter.WriteBoolArr(dst, boolArr, ref offset);
            GFBufferEncoderWriter.WriteUint8Arr(dst, byteArr, ref offset);
            GFBufferEncoderWriter.WriteInt8Arr(dst, sbyteArr, ref offset);
            GFBufferEncoderWriter.WriteInt16Arr(dst, shortArr, ref offset);
            GFBufferEncoderWriter.WriteUInt16Arr(dst, ushortArr, ref offset);
            GFBufferEncoderWriter.WriteInt32Arr(dst, intArr, ref offset);
            GFBufferEncoderWriter.WriteUInt32Arr(dst, uintArr, ref offset);
            GFBufferEncoderWriter.WriteInt64Arr(dst, longArr, ref offset);
            GFBufferEncoderWriter.WriteUInt64Arr(dst, ulongArr, ref offset);
            GFBufferEncoderWriter.WriteSingleArr(dst, floatArr, ref offset);
            GFBufferEncoderWriter.WriteDoubleArr(dst, doubleArr, ref offset);
            GFBufferEncoderWriter.WriteUTF8String(dst, strValue, ref offset);
            GFBufferEncoderWriter.WriteUTF8StringArr(dst, strArr, ref offset);
            GFBufferEncoderWriter.WriteMessage(dst, herModel, ref offset);
            GFBufferEncoderWriter.WriteMessageArr(dst, herModelArr, ref offset);
            GFBufferEncoderWriter.WriteUTF8String(dst, otherStr, ref offset);
        }

        public void FromBytes(byte[] src, ref int offset)
        {
            boolValue = GFBufferEncoderReader.ReadBool(src, ref offset);
            charValue = GFBufferEncoderReader.ReadChar(src, ref offset);
            byteValue = GFBufferEncoderReader.ReadUInt8(src, ref offset);
            sbyteValue = GFBufferEncoderReader.ReadInt8(src, ref offset);
            shortValue = GFBufferEncoderReader.ReadInt16(src, ref offset);
            ushortValue = GFBufferEncoderReader.ReadUInt16(src, ref offset);
            intValue = GFBufferEncoderReader.ReadInt32(src, ref offset);
            uintValue = GFBufferEncoderReader.ReadUInt32(src, ref offset);
            longValue = GFBufferEncoderReader.ReadInt64(src, ref offset);
            ulongValue = GFBufferEncoderReader.ReadUInt64(src, ref offset);
            floatValue = GFBufferEncoderReader.ReadSingle(src, ref offset);
            doubleValue = GFBufferEncoderReader.ReadDouble(src, ref offset);
            boolArr = GFBufferEncoderReader.ReadBoolArr(src, ref offset);
            byteArr = GFBufferEncoderReader.ReadUInt8Arr(src, ref offset);
            sbyteArr = GFBufferEncoderReader.ReadInt8Arr(src, ref offset);
            shortArr = GFBufferEncoderReader.ReadInt16Arr(src, ref offset);
            ushortArr = GFBufferEncoderReader.ReadUInt16Arr(src, ref offset);
            intArr = GFBufferEncoderReader.ReadInt32Arr(src, ref offset);
            uintArr = GFBufferEncoderReader.ReadUInt32Arr(src, ref offset);
            longArr = GFBufferEncoderReader.ReadInt64Arr(src, ref offset);
            ulongArr = GFBufferEncoderReader.ReadUInt64Arr(src, ref offset);
            floatArr = GFBufferEncoderReader.ReadSingleArr(src, ref offset);
            doubleArr = GFBufferEncoderReader.ReadDoubleArr(src, ref offset);
            strValue = GFBufferEncoderReader.ReadUTF8String(src, ref offset);
            strArr = GFBufferEncoderReader.ReadUTF8StringArr(src, ref offset);
            herModel = GFBufferEncoderReader.ReadMessage(src, () => new HerModel(), ref offset);
            herModelArr = GFBufferEncoderReader.ReadMessageArr(src, () => new HerModel(), ref offset);
            otherStr = GFBufferEncoderReader.ReadUTF8String(src, ref offset);
        }
    }
}