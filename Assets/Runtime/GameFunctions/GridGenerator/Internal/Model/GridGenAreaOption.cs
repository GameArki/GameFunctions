using System;
using UnityEngine;

namespace GameFunctions.GridGeneratorInternal {

    [Serializable]
    public struct GridGenAreaOption {

        public int typeID; // Value
        public string areaName;

        public int count;
        public int countMax;

        // Start
        public GridGenStartType startType;
        public int baseOnCellTypeID;
        public int FROM_DIR;

        public int closeToCellTypeID;
        public int closeToManhattanDis;

        public int awayFromCellTypeID;
        public int awayFromManhattanDis;

        // Loop
        public GridGenAlgorithmType algorithmType;
        public int erodeRate;
        public Vector2Int scatterMinMax;

    }

}