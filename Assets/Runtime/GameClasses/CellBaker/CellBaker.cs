using System;
using UnityEngine;
using GameClasses.CellBakerLib.Internal;
using RD = System.Random;

namespace GameClasses.CellBakerLib {

    public class CellBaker {

        CellBakerRepository repo;
        public int width { get; private set; }
        public int height { get; private set; }

        RD random;

        public CellBaker(int seed, int width, int height) {
            random = new RD(seed);
            this.width = width;
            this.height = height;
            repo = new CellBakerRepository(width, height);
        }

        public ReadOnlyMemory<int> GetReadonlyCells() {
            return repo.GetCells();
        }

        public int[] GetCells() {
            return repo.GetCells();
        }

        public void Fill(int typeID) {
            int length = width * height;
            for (int i = 0; i < length; i++) {
                repo.Set(i, typeID);
            }
        }

        public void Gen(CellBakerGenerateOption option) {

        }

        bool TrySelectIndex(CellBakerPositionDescription[] descs, out int outCellIndex) {
            if (descs == null || descs.Length == 0) {
                outCellIndex = repo.GetRandomIndex(random);
                Debug.Log($"TrySelectIndex: descs is null or empty, select random index {outCellIndex}");
                return false;
            }

            bool isMeet = false;
            outCellIndex = -1;

            int curCellIndex = -1;
            int curCellTypeID = -1;
            for (int i = 0; i < descs.Length; i++) {
                CellBakerPositionDescription desc = descs[i];
                PositionRelationType relationType = desc.relationType;
                int relationTypeID = desc.relationTypeID;
                int relationDistance_x = desc.relationDistance_x;
                int relationDistance_y = desc.relationDistance_y;
                if (relationType == PositionRelationType.AwayFrom) {
                    int cellIndex = repo.GetAreaEdge(random, relationTypeID, width, out int direction);
                }
            }

            return isMeet;

        }

    }

}