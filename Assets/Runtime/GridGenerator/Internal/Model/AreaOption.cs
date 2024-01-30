using System;
using UnityEngine;

namespace GameFunctions.GridGeneratorInternal {

    [Serializable]
    public struct AreaOption {

        public CellType cellType;

        public CellType baseOnCellType;
        public int closeToManhattanDis;
        public CellType awayFromCellType;
        public int awayFromManhattanDis;

        public int value;
        public int count;

        // Start
        public StartType startType;
        public int FROM_DIR;

        // Loop
        public AlgorithmType algorithmType;
        public int erodeRate;
        public Vector2Int scatterMinMax;

    }

}