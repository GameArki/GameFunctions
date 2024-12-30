using UnityEngine;

namespace GameFunctions.Editors {

    public class BufferIOExtraGeneratorSampleTool {
#if GAMEARKI_DEV
        [UnityEditor.MenuItem("GameFunctions/Sample/GenBufferIO")]
#endif
        public static void GenBufferIO() {

            GFBufferEncoderGenerator.GenModel(Application.dataPath + "/com.GameFunctions/Tests/TestModel");
            GFBufferEncoderGenerator.GenModel(Application.dataPath + "/Tests/BufferEncoderGenerator/TestModel");

        }

    }

}