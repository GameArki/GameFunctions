using System;
using System.Collections.Generic;
using UnityEngine;
using RD = System.Random;

namespace GameFunctions.GridGeneratorInternal {

    internal class GridGenContext {

        public RD random;
        public GridGenGridOption gridOption;

        Dictionary<GridGenCellType, HashSet<int>> cellTypeValues;

        public int[] grid;

        Dictionary<int, GridGenAreaEntity> all;
        Dictionary<int, GridGenAreaEntity> landAreas;
        Dictionary<int, GridGenAreaEntity> seaAreas;
        Dictionary<int, GridGenAreaEntity> lakeAreas;
        Dictionary<int, GridGenAreaEntity> forestAreas;

        public GridGenContext() { }

        public void Init(GridGenGridOption gridOption, params GridGenAreaOption[] options) {

            this.gridOption = gridOption;

            random = new RD(gridOption.seed);
            for (int i = 0; i < gridOption.seedTimes; i++) {
                random.Next();
            }

            cellTypeValues = new Dictionary<GridGenCellType, HashSet<int>>();
            for (int i = 0; i < options.Length; i++) {
                var option = options[i];
                GridGenCellType type = option.cellType;
                if (!cellTypeValues.ContainsKey(type)) {
                    cellTypeValues.Add(type, new HashSet<int>());
                }
                cellTypeValues[type].Add(option.value);

                GridGenCellType waterFlag = GridGenCellType.Water;
                if (waterFlag.HasFlag(type)) {
                    if (!cellTypeValues.ContainsKey(waterFlag)) {
                        cellTypeValues.Add(waterFlag, new HashSet<int>());
                    }
                    cellTypeValues[waterFlag].Add(option.value);
                }
            }

            all = new Dictionary<int, GridGenAreaEntity>();
            landAreas = new Dictionary<int, GridGenAreaEntity>();
            lakeAreas = new Dictionary<int, GridGenAreaEntity>();
            seaAreas = new Dictionary<int, GridGenAreaEntity>();
            forestAreas = new Dictionary<int, GridGenAreaEntity>();
            for (int i = 0; i < options.Length; i++) {
                var option = options[i];
                GridGenCellType type = option.cellType;
                GridGenAreaEntity entity = new GridGenAreaEntity(i, gridOption.width, gridOption.height, option);
                if (type == GridGenCellType.Land) {
                    landAreas.Add(i, entity);
                } else if (type == GridGenCellType.Sea) {
                    seaAreas.Add(i, entity);
                } else if (type == GridGenCellType.Lake) {
                    lakeAreas.Add(i, entity);
                } else if (type == GridGenCellType.Forest) {
                    forestAreas.Add(i, entity);
                } else {
                    throw new Exception("Unknown cell type: " + type);
                }
                all.Add(i, entity);
            }

            if (grid == null || grid.Length < gridOption.width * gridOption.height) {
                grid = new int[gridOption.width * gridOption.height];
            }

        }

        public void Grid_Set(int index, int value) {
            grid[index] = value;
        }

        public void Land_Remove(int index) {
            foreach (var pair in landAreas) {
                pair.Value.Remove(index);
            }
        }

        public void Land_UpdateAll() {
            foreach (var pair in landAreas) {
                pair.Value.UpdateAll();
            }
        }

        public void Land_Foreach(Action<GridGenAreaEntity> action) {
            foreach (var pair in landAreas) {
                action(pair.Value);
            }
        }

        public void Sea_Foreach(Action<GridGenAreaEntity> action) {
            foreach (var pair in seaAreas) {
                action(pair.Value);
            }
        }

        public void Lake_Foreach(Action<GridGenAreaEntity> action) {
            foreach (var pair in lakeAreas) {
                action(pair.Value);
            }
        }

        public void Forest_Foreach(Action<GridGenAreaEntity> action) {
            foreach (var pair in forestAreas) {
                action(pair.Value);
            }
        }

        public HashSet<int> GetCellTypeValues(GridGenCellType awayFromType) {
            bool has = cellTypeValues.TryGetValue(awayFromType, out HashSet<int> values);
            if (!has) {
                return null;
            }
            return values;
        }

        public bool GetRandomCell(RD rd, GridGenCellType cellType, out int index) {
            if (cellType == GridGenCellType.Land) {
                return GetRandomCell(rd, landAreas, out index);
            } else if (cellType == GridGenCellType.Sea) {
                return GetRandomCell(rd, seaAreas, out index);
            } else if (cellType == GridGenCellType.Lake) {
                return GetRandomCell(rd, lakeAreas, out index);
            } else if (cellType == GridGenCellType.Forest) {
                return GetRandomCell(rd, forestAreas, out index);
            } else {
                index = rd.Next(0, grid.Length);
                return true;
            }
        }

        bool GetRandomCell(RD rd, Dictionary<int, GridGenAreaEntity> areas, out int index) {
            foreach (var pair in areas) {
                var area = pair.Value;
                if (area.set.Count > 0) {
                    index = area.indices[rd.Next(0, area.set.Count)];
                    return true;
                }
            }
            index = -1;
            return false;
        }

    }

}