using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using GameFunctions.PathfindingInternal;

namespace GameFunctions {

    public static class GFPathfinding2D {

        [ThreadStatic] public static SortedSet<GFCell> openSet = new SortedSet<GFCell>();
        [ThreadStatic] public static Dictionary<I32I32_U64, GFCell> openSetKey = new Dictionary<I32I32_U64, GFCell>(10000);
        [ThreadStatic] public static SortedSet<GFCell> closedSet = new SortedSet<GFCell>();
        [ThreadStatic] public static HashSet<I32I32_U64> closedSetKey = new HashSet<I32I32_U64>(10000);
        [ThreadStatic] static Stack<GFCell> pool = new Stack<GFCell>(10000);

        const float G_COST = 10;

        [ThreadStatic]
        readonly static Dictionary<int, Vector2Int> neighbors = new Dictionary<int, Vector2Int> {
            { 0, new Vector2Int(1, 0) }, // →
            { 1, new Vector2Int(0, -1) }, // ↓
            { 2, new Vector2Int(-1, 0) }, // ←
            { 3, new Vector2Int(0, 1) }, // ↑
        };

        /// <summary> ↑ → ↓ ←, returns -1 if not found. </summary>
        public static int AStar(Vector2Int start, Vector2Int end, int limitedCount, Predicate<Vector2Int> isWalkable, Vector2Int[] result, bool isManuallyProcess = false) {

            // ==== Begin ====
            // A* algorithm
            foreach (GFCell cell in openSet) {
                PoolReturn(cell);
            }
            foreach (GFCell cell in closedSet) {
                PoolReturn(cell);
            }
            openSet.Clear();
            openSetKey.Clear();
            closedSet.Clear();
            closedSetKey.Clear();

            GFCell startCell = PoolGet();
            startCell.Init(start, 0, 0, 0, null);

            openSet.Add(startCell);
            openSetKey.Add(startCell.key, startCell);

            int visited = 0;
            int resultCount = 0;
            // ===============

            if (isManuallyProcess) {
                return 0;
            }

            while (openSet.Count > 0) {
                bool isDone = Process(ref visited, ref resultCount, limitedCount, start, end, isWalkable, result, out _);
                if (isDone) {
                    return resultCount;
                }
            }

            return -1;

        }

        public static bool Process(ref int visited, ref int count, int limitedCount, Vector2Int start, Vector2Int end, Predicate<Vector2Int> isWalkable, Vector2Int[] result, out Vector2Int cur) {

            GFCell q = openSet.Min;
            cur = q.pos;
            openSet.Remove(q);
            openSetKey.Remove(q.key);
            closedSet.Add(q);
            closedSetKey.Add(q.key);

            visited += 1;
            if (visited >= limitedCount) {
                count = -2;
                return true;
            }

            for (int i = 0; i < 4; i++) {

                Vector2Int neighborPos = q.pos + neighbors[i];
                I32I32_U64 neighborKey = new I32I32_U64(neighborPos);
                if (!isWalkable(neighborPos) || closedSetKey.Contains(neighborKey)) {
                    continue;
                }

                // 找到了, 从end开始回溯
                if (neighborPos == end) {
                    GFCell point = q;
                    count = 0;
                    result[count++] = end;
                    while (point != null) {
                        result[count++] = point.pos;
                        point = point.parent;
                    }
                    return true;
                }

                float gCost = q.gCost + Vector2.Distance(q.pos, neighborPos);
                float hCost = H_Manhattan(neighborPos, end);
                float fCost = gCost + hCost;
                GFCell neighborCell;
                if (openSetKey.TryGetValue(neighborKey, out neighborCell)) {
                    if (fCost < neighborCell.fCost) {
                        openSet.Remove(neighborCell);
                        neighborCell.Init(neighborPos, fCost, gCost, hCost, q);
                        openSet.Add(neighborCell);
                    }
                } else {
                    neighborCell = PoolGet();
                    neighborCell.Init(neighborPos, fCost, gCost, hCost, q);
                    openSet.Add(neighborCell);
                    openSetKey.Add(neighborCell.key, neighborCell);
                }

            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float H_Manhattan(Vector2Int cur, Vector2Int end) {
            return 10 * (Mathf.Abs(cur.x - end.x) + Mathf.Abs(cur.y - end.y));
        }

        static GFCell PoolGet() {
            if (pool.Count > 0) {
                return pool.Pop();
            } else {
                return new GFCell();
            }
        }

        static void PoolReturn(GFCell cell) {
            pool.Push(cell);
        }

    }

}