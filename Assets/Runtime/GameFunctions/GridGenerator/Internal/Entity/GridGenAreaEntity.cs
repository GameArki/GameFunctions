using System;
using System.Collections.Generic;

namespace GameFunctions.GridGeneratorInternal {

    internal class GridGenAreaEntity {

        public int id;
        public int[] indices;
        public HashSet<int> set;

        public GridGenAreaOption option;

        public GridGenAreaEntity(int id, int width, int height, GridGenAreaOption option) {
            this.id = id;
            indices = new int[width * height];
            set = new HashSet<int>(indices.Length);
            this.option = option;
            option.countMax = option.count;
        }

        public bool Add(int index) {
            bool succ = set.Add(index);
            if (succ) {
                indices[set.Count - 1] = index;
            }
            return succ;
        }

        public void Remove(int index) {
            set.Remove(index);
        }

        public void UpdateAll() {
            set.CopyTo(indices);
        }

    }

}