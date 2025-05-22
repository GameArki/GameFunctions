using System;
using System.Collections.Generic;
using UnityEngine;
using RD = System.Random;

namespace GameFunctions.GridGeneratorInternal {

    internal class GridGenContext {

        public RD random;
        public GridGenGridOption gridOption;

        public int[] grid;

        Dictionary<int, GridGenAreaEntity> all;
        GridGenAreaEntity[] tempArray;

        public GridGenContext() {
            all = new Dictionary<int, GridGenAreaEntity>();
            tempArray = new GridGenAreaEntity[0];
        }

        public void Init(GridGenGridOption gridOption, params GridGenAreaOption[] options) {

            this.gridOption = gridOption;

            random = new RD(gridOption.seed);
            for (int i = 0; i < gridOption.seedTimes; i++) {
                random.Next();
            }

            for (int i = 0; i < options.Length; i++) {
                var option = options[i];
                int typeID = option.typeID;
                GridGenAreaEntity entity = new GridGenAreaEntity(typeID, gridOption.width, gridOption.height, option);
                all.Add(typeID, entity);
            }

            if (grid == null || grid.Length < gridOption.width * gridOption.height) {
                grid = new int[gridOption.width * gridOption.height];
            }

        }

        public void Grid_Set(int index, int value) {
            grid[index] = value;
        }

        public void Remove(int typeID, int index) {
            if (all.TryGetValue(typeID, out GridGenAreaEntity area)) {
                area.Remove(index);
            }
        }

        public int TakeAllArea(out GridGenAreaEntity[] results) {
            int count = all.Count;
            if (count > tempArray.Length) {
                tempArray = new GridGenAreaEntity[count];
            }
            all.Values.CopyTo(tempArray, 0);
            results = tempArray;
            return count;
        }

        public bool TryGetArea(int typeID, out GridGenAreaEntity area) {
            bool has = all.TryGetValue(typeID, out area);
            if (!has) {
                Debug.LogError($"GridGenContext: typeID {typeID} not found");
                return false;
            }
            return true;
        }

        public HashSet<int> GetCellTypeValues(int typeID) {
            bool has = all.TryGetValue(typeID, out var area);
            if (!has) {
                return null;
            }
            return area.set;
        }

        public bool GetRandomCell(RD rd, int typeID, out int res) {
            int failedTimes = 0;
            int index = 0;
            do {
                index = rd.Next(0, grid.Length);
                if (grid[index] == typeID) {
                    res = index;
                    return true;
                }
                failedTimes++;
            } while (failedTimes < 100);

            res = index;
            return false;
        }

    }

}