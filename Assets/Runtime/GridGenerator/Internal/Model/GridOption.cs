using System;

namespace GameFunctions.GridGeneratorInternal {

    [Serializable]
    public struct GridOption {
        public int seed;
        public int seedTimes;
        public int width;
        public int height;
    }
}