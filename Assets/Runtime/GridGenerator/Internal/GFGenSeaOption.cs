using System;

namespace GameFunctions {

    [Serializable]
    public struct GFGenSeaOption {

        public const int TYPE_NORMAL = 1;

        public int seaValue;
        public int seaCount;
        public int DIR;
        public int TYPE;
    }

}