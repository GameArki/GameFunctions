using System;
using UnityEngine;

namespace GameFunctions.GridGeneratorInternal {

    [Serializable]
    public struct GridGenAreaOption {

        public GridGenCellType cellType;

        public int value;
        public int count;
        public int countMax;

        public GridGenStartType startType;
        public GridGenCellType baseOnCellType;
        public int FROM_DIR;

        public GridGenCellType closeToCellType;
        public int closeToManhattanDis;
        public GridGenCellType awayFromCellType;
        public int awayFromManhattanDis;

        // Loop
        public GridGenAlgorithmType algorithmType;
        public int erodeRate;
        public Vector2Int scatterMinMax;

    }

}