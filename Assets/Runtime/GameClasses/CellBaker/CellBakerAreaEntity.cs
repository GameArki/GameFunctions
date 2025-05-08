using System;
using System.Collections.Generic;

namespace GameClasses.CellBakerLib.Internal {

    public class CellBakerAreaEntity {

        int typeID;
        HashSet<int> indices;

        public CellBakerAreaEntity(int typeID) {
            this.typeID = typeID;
            indices = new HashSet<int>();
        }

        public void Add(int index) {
            indices.Add(index);
        }

        public void Remove(int index) {
            indices.Remove(index);
        }

        public bool Contains(int index) {
            return indices.Contains(index);
        }

        public int GetCenterIndex(int width) {
            int sumX = 0;
            int sumY = 0;
            int count = indices.Count;

            foreach (var index in indices) {
                var (x, y) = PositionFunctions.GetXY(index, width);
                sumX += x;
                sumY += y;
            }

            return PositionFunctions.GetIndex(sumX / count, sumY / count, width);
        }

        public int GetRandomIndex(Random rd) {
            int indexOfIndex = rd.Next(0, indices.Count);
            int i = 0;
            foreach (int value in indices) {
                if (i == indexOfIndex) {
                    return value;
                }
                i++;
            }
            return -1;
        }

    }

}