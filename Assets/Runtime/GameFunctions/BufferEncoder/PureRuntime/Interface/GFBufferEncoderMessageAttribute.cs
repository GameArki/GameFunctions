using System;

namespace GameFunctions {

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class GFBufferEncoderMessageAttribute : Attribute {

        public GFBufferEncoderMessageAttribute() {

        }

    }

}