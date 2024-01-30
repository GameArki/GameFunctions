using System;
using System.Collections.Generic;
using UnityEngine;
using RD = System.Random;

namespace GameFunctions.GridGeneratorInternal {

    public class Context {

        public RD random;
        public GridOption gridOption;

        Dictionary<CellType, HashSet<int>> cellTypeValues;

        public int[] grid;

        Dictionary<int, AreaEntity> all;
        Dictionary<int, AreaEntity> landAreas;
        Dictionary<int, AreaEntity> seaAreas;
        Dictionary<int, AreaEntity> lakeAreas;
        Dictionary<int, AreaEntity> forestAreas;

        public Context() { }

        public void Init(GridOption gridOption, params AreaOption[] options) {

            this.gridOption = gridOption;

            random = new RD(gridOption.seed);
            for (int i = 0; i < gridOption.seedTimes; i++) {
                random.Next();
            }

            cellTypeValues = new Dictionary<CellType, HashSet<int>>();
            for (int i = 0; i < options.Length; i++) {
                var option = options[i];
                CellType type = option.cellType;
                if (!cellTypeValues.ContainsKey(type)) {
                    cellTypeValues.Add(type, new HashSet<int>());
                }
                cellTypeValues[type].Add(option.value);

                CellType waterFlag = CellType.Water;
                if (waterFlag.HasFlag(type)) {
                    if (!cellTypeValues.ContainsKey(waterFlag)) {
                        cellTypeValues.Add(waterFlag, new HashSet<int>());
                    }
                    cellTypeValues[waterFlag].Add(option.value);
                }
            }

            all = new Dictionary<int, AreaEntity>();
            landAreas = new Dictionary<int, AreaEntity>();
            lakeAreas = new Dictionary<int, AreaEntity>();
            seaAreas = new Dictionary<int, AreaEntity>();
            forestAreas = new Dictionary<int, AreaEntity>();
            for (int i = 0; i < options.Length; i++) {
                var option = options[i];
                CellType type = option.cellType;
                AreaEntity entity = new AreaEntity(i, gridOption.width, gridOption.height, option);
                if (type == CellType.Land) {
                    landAreas.Add(i, entity);
                } else if (type == CellType.Sea) {
                    seaAreas.Add(i, entity);
                } else if (type == CellType.Lake) {
                    lakeAreas.Add(i, entity);
                } else if (type == CellType.Forest) {
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

        public void Land_Foreach(Action<AreaEntity> action) {
            foreach (var pair in landAreas) {
                action(pair.Value);
            }
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

        public void Forest_Foreach(Action<AreaEntity> action) {
            foreach (var pair in forestAreas) {
                action(pair.Value);
            }
        }

        public HashSet<int> GetCellTypeValues(CellType awayFromType) {
            bool has = cellTypeValues.TryGetValue(awayFromType, out HashSet<int> values);
            if (!has) {
                return null;
            }
            return values;
        }

        public bool GetRandomCell(RD rd, CellType cellType, out int index) {
            if (cellType == CellType.Land) {
                return GetRandomCell(rd, landAreas, out index);
            } else if (cellType == CellType.Sea) {
                return GetRandomCell(rd, seaAreas, out index);
            } else if (cellType == CellType.Lake) {
                return GetRandomCell(rd, lakeAreas, out index);
            } else if (cellType == CellType.Forest) {
                return GetRandomCell(rd, forestAreas, out index);
            } else {
                index = rd.Next(0, grid.Length);
                return true;
            }
        }

        bool GetRandomCell(RD rd, Dictionary<int, AreaEntity> areas, out int index) {
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