using System;
using System.Collections.Generic;

namespace GameFunctions.GridGeneratorInternal {

    internal class GridGenAreaEntity {

        public int typeID;
        public List<int> indices;
        public HashSet<int> set;

        public GridGenAreaOption option;

        public GridGenAreaEntity(int typeID, int width, int height, GridGenAreaOption option) {
            this.typeID = typeID;
            int len = width * height;
            indices = new List<int>(len);
            set = new HashSet<int>(len);
            this.option = option;
            option.countMax = option.count;
        }

        public bool Add(int index) {
            bool succ = set.Add(index);
            if (succ) {
                indices.Add(index);
            }
            return succ;
        }

        public void Remove(int index) {
            set.Remove(index);
            indices.Remove(index);
        }

    }

}