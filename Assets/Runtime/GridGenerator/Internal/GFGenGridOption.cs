using System;

namespace GameFunctions {

    [Serializable]
    public struct GFGenGridOption {
        public int seed;
        public int seedTimes;
        public int width;
        public int height;
        public int defaultLandValue;
    }
}