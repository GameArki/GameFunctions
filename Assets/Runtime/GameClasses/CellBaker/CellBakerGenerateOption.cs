using System;

namespace GameClasses.CellBakerLib.Internal {

    public struct CellBakerGenerateOption {

        // Select Start Index
        public CellBakerPositionDescription[] start_positionDesc;

        // Generate Algorithm
        public CellBakerGenerateAlgorithmType gen_AlgorithmType;
        public int gen_basedOnTypeID;
        public int gen_typeID;
        public int gen_count;
        public int gen_failedTimes;

    }

}