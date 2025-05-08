using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GameClasses.CellBakerLib.Internal {

    public class CellBakerRepository {

        int[] cells; // value is typeID

        Dictionary<int/*typeID*/, CellBakerAreaEntity> areaEntities;

        public CellBakerRepository(int width, int height) {
            cells = new int[width * height];
            areaEntities = new Dictionary<int, CellBakerAreaEntity>();
        }

        public void Set(int index, int typeID) {
            int fromTypeID = cells[index];
            if (fromTypeID == typeID) {
                return;
            }

            if (areaEntities.TryGetValue(fromTypeID, out var fromAreaEntity)) {
                fromAreaEntity.Remove(index);
            }

            cells[index] = typeID;

            if (!areaEntities.TryGetValue(typeID, out var areaEntity)) {
                areaEntity = new CellBakerAreaEntity(typeID);
                areaEntities.Add(typeID, areaEntity);
            }
            areaEntity.Add(index);
        }

        public void Set(int x, int y, int typeID, int width) {
            int index = PositionFunctions.GetIndex(x, y, width);
            Set(index, typeID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Get(int index) {
            return cells[index];
        }

        public int Get(int x, int y, int width) {
            int index = PositionFunctions.GetIndex(x, y, width);
            return Get(index);
        }

        public int[] GetCells() {
            return cells;
        }

        public int GetRandomIndex(Random rd) {
            int randomIndex = rd.Next(0, cells.Length);
            return randomIndex;
        }

        public int GetAreaEdge(Random rd, int areaTypeID, int width, out int outDirection) {
            int resultEdgeIndex = -1;
            outDirection = 0;

            if (areaEntities.TryGetValue(areaTypeID, out var areaEntity)) {
                // 找到区域中的随机点
                int randomCellIndex = areaEntity.GetRandomIndex(rd);
                // 往一个随机方向一直走, 直到碰到边界
                outDirection = PositionFunctions.RandomDirection(rd, width);
                int step = 1;
                int nextIndex;
                do {
                    nextIndex = randomCellIndex + outDirection * step;
                    // 如果越界了, 直接返回
                    if (nextIndex < 0 || nextIndex >= cells.Length) {
                        resultEdgeIndex = randomCellIndex;
                        break;
                    }

                    // 如果碰到的点不是同一个区域, 直接返回
                    int nextTypeID = Get(nextIndex);
                    if (nextTypeID != areaTypeID) {
                        resultEdgeIndex = nextIndex;
                        break;
                    }
                    step++;
                } while (step < width);
            }

            if (resultEdgeIndex == -1) {
                // 没找到
                UnityEngine.Debug.LogWarning($"CellBakerRepository.GetAreaEdge: areaTypeID {areaTypeID} not found");
            }

            return resultEdgeIndex;
        }

    }

}