using System;

namespace GameFunctions.GridGeneratorInternal {

    [Serializable]
    public struct AreaOption {

        public CellType cellType;

        public CellType baseOnCellType;

        public int value;
        public int count;

        public int FROM_DIR;

        public int awayFromWater;

        public AlgorithmType algorithmType;
        public int erodeRate;

    }

}