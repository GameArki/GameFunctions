using System;
using System.Collections.Generic;

namespace GameFunctions {

    public class GFGridGeneratorContext {

        public Random random;
        public GFGenGridOption gridOption;
        public GFGenSeaOption seaOption;
        public GFGenLakeOption lakeOption;

        public HashSet<int> waterValues;

        public int[] grid;

        public HashSet<int> grid_land_set;
        public int[] grid_land_indices;

        public HashSet<int> grid_sea_set;
        public int[] grid_sea_indices;

        public HashSet<int> grid_lake_set;
        public int[] grid_lake_indices;

        public GFGridGeneratorContext() { }

        public void Init(GFGenGridOption gridOption, GFGenSeaOption seaOption, GFGenLakeOption lakeOption) {

            random = new Random(gridOption.seed);
            for (int i = 0; i < gridOption.seedTimes; i++) {
                random.Next();
            }

            this.gridOption = gridOption;
            this.seaOption = seaOption;
            this.lakeOption = lakeOption;

            waterValues = new HashSet<int>();
            waterValues.Add(seaOption.seaValue);
            waterValues.Add(lakeOption.lakeValue);

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

            // Cache: Sea
            if (grid_sea_set == null) {
                grid_sea_set = new HashSet<int>();
            } else {
                grid_sea_set.Clear();
            }
            if (grid_sea_indices == null || grid_sea_indices.Length < grid.Length) {
                grid_sea_indices = new int[grid.Length];
            }

            // Cache: Lake
            if (grid_lake_set == null) {
                grid_lake_set = new HashSet<int>();
            } else {
                grid_lake_set.Clear();
            }
            if (grid_lake_indices == null || grid_lake_indices.Length < grid.Length) {
                grid_lake_indices = new int[grid.Length];
            }

        }

        public void Land_Add(int index, int value) {
            grid[index] = value;
            grid_land_indices[grid_land_set.Count] = index;
            grid_land_set.Add(index);
        }

        public void Land_Remove(int index) {
            grid_land_set.Remove(index);
        }

        public void Land_UpdateAll() {
            grid_land_set.CopyTo(grid_land_indices);
        }

        public void Sea_Add(int index, int value) {
            grid[index] = value;
            grid_sea_indices[grid_sea_set.Count] = index;
            grid_sea_set.Add(index);
        }

        public void Sea_Update() {
            grid_sea_set.CopyTo(grid_sea_indices);
        }

        public void Lake_Add(int index, int value) {
            grid[index] = value;
            grid_lake_indices[grid_lake_set.Count] = index;
            grid_lake_set.Add(index);
        }

        public void Lake_UpdateAll() {
            grid_lake_set.CopyTo(grid_lake_indices);
        }

    }

}