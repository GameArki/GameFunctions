using System;

namespace GameFunctions {

    [Serializable]
    public struct GFGenLakeOption {

        public const int TYPE_NORMAL = 1;

        public int lakeValue;
        public int lakeCount;
        public int awayFromSea;
        public int TYPE;

    }

}