using System;

namespace GameFunctions {

    [Serializable]
    public struct GFGenCellOption {
        public int seed;
        public int seedTimes;
        public int width;
        public int height;
        public int defaultLandValue;
    }
}