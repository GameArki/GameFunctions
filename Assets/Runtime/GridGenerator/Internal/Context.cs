using System;
using System.Collections.Generic;

namespace GameFunctions.GridGeneratorInternal {

    public class Context {

        public Random random;
        public GridOption gridOption;

        public HashSet<int> waterValues;

        public int[] grid;

        public HashSet<int> grid_land_set;
        public int[] grid_land_indices;

        Dictionary<int, AreaEntity> all;
        Dictionary<int, AreaEntity> seaAreas;
        Dictionary<int, AreaEntity> lakeAreas;

        public Context() { }

        public void Init(GridOption gridOption, params AreaOption[] options) {

            this.gridOption = gridOption;

            random = new Random(gridOption.seed);
            for (int i = 0; i < gridOption.seedTimes; i++) {
                random.Next();
            }

            waterValues = new HashSet<int>();
            for (int i = 0; i < options.Length; i++) {
                CellType type = options[i].cellType;
                if (type.IsWater()) {
                    waterValues.Add(options[i].value);
                }
            }

            all = new Dictionary<int, AreaEntity>();
            lakeAreas = new Dictionary<int, AreaEntity>();
            seaAreas = new Dictionary<int, AreaEntity>();
            for (int i = 0; i < options.Length; i++) {
                var option = options[i];
                CellType type = option.cellType;
                AreaEntity entity = new AreaEntity(i, gridOption.width, gridOption.height, option);
                if (type == CellType.Sea) {
                    seaAreas.Add(i, entity);
                } else if (type == CellType.Lake) {
                    lakeAreas.Add(i, entity);
                } else {
                    throw new Exception("Unknown cell type: " + type);
                }
                all.Add(i, entity);
            }

            if (grid == null || grid.Length < gridOption.width * gridOption.height) {
                grid = new int[gridOption.width * gridOption.height];
            }

            // Cache: Land
            if (grid_land_set == null) {
                grid_land_set = new HashSet<int>();
            } else {
                grid_land_set.Clear();
            }
            if (grid_land_indices == null || grid_land_indices.Length < grid.Length) {
                grid_land_indices = new int[grid.Length];
            }

        }

        public void Land_Add(int index, int value) {
            grid[index] = value;
            bool succ = grid_land_set.Add(index);
            if (succ) {
                grid_land_indices[grid_land_set.Count - 1] = index;
            }
        }

        public void Land_Remove(int index) {
            grid_land_set.Remove(index);
        }

        public void Land_UpdateAll() {
            grid_land_set.CopyTo(grid_land_indices);
        }

        public void Sea_Foreach(Action<AreaEntity> action) {
            foreach (var pair in seaAreas) {
                action(pair.Value);
            }
        }

        public void Lake_Foreach(Action<AreaEntity> action) {
            foreach (var pair in lakeAreas) {
                action(pair.Value);
            }
        }

    }

}