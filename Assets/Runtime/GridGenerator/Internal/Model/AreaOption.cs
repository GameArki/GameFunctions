using System;
using UnityEngine;

namespace GameFunctions.GridGeneratorInternal {

    [Serializable]
    public struct AreaOption {

        public CellType cellType;

        public int value;
        public int count;
        public int countMax;

        public StartType startType;
        public CellType baseOnCellType;
        public int FROM_DIR;

        public CellType closeToCellType;
        public int closeToManhattanDis;
        public CellType awayFromCellType;
        public int awayFromManhattanDis;

        // Loop
        public AlgorithmType algorithmType;
        public int erodeRate;
        public Vector2Int scatterMinMax;

    }

}