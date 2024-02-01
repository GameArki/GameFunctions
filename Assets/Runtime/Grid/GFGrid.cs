using System;
using UnityEngine;

namespace GameFunctions {

    public static class GFGrid {

        /*
           o o + o o 
           o + + + o 
           + + + + + 
           o + + + o 
           o o + o o 
        */
        #region CircleCycle
        public static int CircleCycle_GetCells(Vector2Int center, float radius, Vector2Int[] cells) {
            if (radius <= 0) {
                cells[0] = center;
                return 1;
            }
            int count = 0;
            int range = Mathf.CeilToInt(radius);
            float rangeSqr = radius * radius;
            for (int x = center.x - range; x <= center.x + range; x++) {
                for (int y = center.y - range; y <= center.y + range; y++) {
                    Vector2 pos = new Vector2(x, y);
                    if (Vector2.SqrMagnitude(pos - center) <= rangeSqr) {
                        cells[count] = new Vector2Int(x, y);
                        count++;
                    }
                }
            }
            return count;
        }
        #endregion CircleCycle

        #region CrossCycle
        public static int CrossCycle_GetCells(Vector2Int center, int radius, Vector2Int[] cells) {
            if (radius <= 0) {
                cells[0] = center;
                return 1;
            }
            int count = 0;
            for (int x = center.x - radius; x <= center.x + radius; x++) {
                for (int y = center.y - radius; y <= center.y + radius; y++) {
                    int dis = ManhattanDistance(center, new Vector2Int(x, y));
                    if (dis <= radius) {
                        cells[count] = new Vector2Int(x, y);
                        count++;
                    }
                }
            }
            return count;
        }
        #endregion CrossCycle

        /*
           gridCount == 0
           o

           gridCount == '/' count == 1
           o o / 
           o o o
           o o o

           gridCount == '/' count == 2
           o o o o / 
           o o o / o 
           o o o o o 
           o o o o o 
           o o o o o 
        */
        #region RectCycle
        public static int RectCycle_GetCellCount(int gridCount) {
            int v = 2 * gridCount + 1;
            return v * v;
        }

        /// <summary>
        /// 从中心点开始, 按给定的圈数, 螺旋向外查找格子. 
        /// 离中心点近的位置, 排在数组的前面
        /// </summary>
        public static int RectCycle_GetCellsBySpirals(Vector2Int center, int cycleCount, Vector2Int[] results) {
            // means: gridCount == 1
            // 2 1 8
            // 3 0 7
            // 4 5 6

            // 九宫格 center(05,10)
            // 04,11 05,11 06,11
            // 04,10 05,10 06,10
            // 04,09 05,09 06,09
            int curCycle = 0;
            int count = 0;
            int cx = center.x;
            int cy = center.y;
            while (curCycle <= cycleCount) {
                RectCycle_GetOneCellBySpiral(center, curCycle, false, (pos) => {
                    results[count++] = pos;
                    return false;
                });
                curCycle += 1;
            }
            return count;
        }

        public static void RectCycle_GetOneCellBySpirals(Vector2Int center, int cycleCount, Predicate<Vector2Int> condition) {
            int curCycle = 0;
            int cx = center.x;
            int cy = center.y;
            while (curCycle <= cycleCount) {
                RectCycle_GetOneCellBySpiral(center, curCycle, true, condition);
                curCycle += 1;
            }
        }

        /// <summary> 
        /// cycle = 第几圈
        /// </summary>
        // 2 2 2 2 2
        // 2 1 1 1 2
        // 2 1 0 1 2
        // 2 1 1 1 2
        // 2 2 2 2 2
        public static void RectCycle_GetOneCellBySpiral(in Vector2Int center, int cycle, bool isEndWhenFound, Predicate<Vector2Int> condition) {
            int cx = center.x;
            int cy = center.y;
            int x;
            int y;
            // x ← o
            // o o o
            // o o o
            for (x = cx, y = cy + cycle; x >= cx - cycle; x -= 1) {
                if (condition(new Vector2Int(x, y)) && isEndWhenFound) {
                    return;
                }
            }
            // o o o
            // ↓ o o
            // x o o
            for (x = cx - cycle, y = cy + cycle - 1; y >= cy - cycle; y -= 1) {
                if (condition(new Vector2Int(x, y)) && isEndWhenFound) {
                    return;
                }
            }
            // o o o
            // o o o
            // o → x
            for (x = cx - cycle + 1, y = cy - cycle; x <= cx + cycle; x += 1) {
                if (condition(new Vector2Int(x, y)) && isEndWhenFound) {
                    return;
                }
            }
            // o o x
            // o o ↑
            // o o o
            for (x = cx + cycle, y = cy - cycle + 1; y <= cy + cycle; y += 1) {
                if (condition(new Vector2Int(x, y)) && isEndWhenFound) {
                    return;
                }
            }
            // o x ←
            // o o o
            // o o o
            for (x = cx + cycle - 1, y = cy + cycle; x >= cx + 1; x -= 1) {
                if (condition(new Vector2Int(x, y)) && isEndWhenFound) {
                    return;
                }
            }
        }
        #endregion RectCycle

        // 斜线
        #region Slant
        public static int Slant_GetCells(Vector2 start, Vector2 end, Vector2Int[] result) {

            Vector2 bottom = start.y < end.y ? start : end;
            Vector2 top = start.y < end.y ? end : start;
            Vector2 left = start.x < end.x ? start : end;
            Vector2 right = start.x < end.x ? end : start;

            Vector2 diff = right - left;
            bool isReverse = false;
            float tx;
            float ty;
            if (Math.Abs(diff.y) > Math.Abs(diff.x)) {
                // y as x, bottom as start
                isReverse = true;
                diff = top - bottom;
                Vector2Int diffInt = diff.RoundToVector2Int();
                if (diff == Vector2.zero || diffInt == Vector2Int.zero) {
                    return 0;
                }
                tx = diff.y;
                ty = diff.x;
            } else {
                // x as x, left as start
                tx = diff.x;
                ty = diff.y;
            }

            int times = Mathf.RoundToInt(Math.Abs(tx));

            int count = 0;
            float k = ty / tx;
            for (int i = 0; i < times; i += 1) {
                if (isReverse) {
                    Vector2 res = new Vector2(bottom.x + i * k, bottom.y + i);
                    result[count++] = res.CeilToVector2Int();
                } else {
                    Vector2 res = new Vector2(left.x + i, left.y + i * k);
                    result[count++] = res.CeilToVector2Int();
                }
            }

            return count;
        }
        #endregion Slant

        static int ManhattanDistance(Vector2Int a, Vector2Int b) {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

    }

}