using System;
using System.Collections.Generic;

namespace GameFunctions.GridGeneratorInternal {

    public class AreaEntity {

        public int id;
        public int[] indices;
        public HashSet<int> set;

        public AreaOption option;

        public AreaEntity(int id, int width, int height, AreaOption option) {
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