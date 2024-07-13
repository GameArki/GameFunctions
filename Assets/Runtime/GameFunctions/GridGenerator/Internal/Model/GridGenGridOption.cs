using System;

namespace GameFunctions.GridGeneratorInternal {

    [Serializable]
    public struct GridGenGridOption {
        public int seed;
        public int seedTimes;
        public int width;
        public int height;
    }
}