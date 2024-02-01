using System;

namespace GameFunctions {

    public interface IProtocolService {

        (byte serviceID, byte messageID) GetMessageID<T>() where T : IGFBufferEncoderMessage<T>;
        Func<T> GetGenerateHandle<T>() where T : IGFBufferEncoderMessage<T>;

    }

}