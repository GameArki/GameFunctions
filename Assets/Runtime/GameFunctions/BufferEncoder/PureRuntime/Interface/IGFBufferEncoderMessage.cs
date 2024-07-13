using System;

namespace GameFunctions {

    public interface IGFBufferEncoderMessage<T> {

        void WriteTo(byte[] dst, ref int offset);
        void FromBytes(byte[] src, ref int offset);

    }

}