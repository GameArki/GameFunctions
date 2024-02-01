using System;
using GameFunctions;

namespace GameFunctions.Sample
{
    [GFBufferEncoderMessage]
    public struct HerExtraModel : IGFBufferEncoderMessage<HerExtraModel>
    {
        public string name;
        public int value;
        public void WriteTo(byte[] dst, ref int offset)
        {
            GFBufferEncoderWriter.WriteUTF8String(dst, name, ref offset);
            GFBufferEncoderWriter.WriteInt32(dst, value, ref offset);
        }

        public void FromBytes(byte[] src, ref int offset)
        {
            name = GFBufferEncoderReader.ReadUTF8String(src, ref offset);
            value = GFBufferEncoderReader.ReadInt32(src, ref offset);
        }
    }
}