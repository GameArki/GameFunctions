using System;

namespace GameFunctions {

    [Serializable]
    public struct GFGenLakeOption {

        public const int TYPE_FLOOD = 1; // 扁平

        public int lakeValue;
        public int lakeCount;
        public int awayFromWater;
        public int TYPE;

    }

}