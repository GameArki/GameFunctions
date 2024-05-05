using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using GameFunctions.PathfindingInternal;

namespace GameFunctions {

    public static class GFPathfinding2D {

#if UNITY_EDITOR
        [ThreadStatic] public static SortedSet<GFRectCell> openSet = new SortedSet<GFRectCell>();
#else
        [ThreadStatic] internal static SortedSet<GFCell> openSet = new SortedSet<GFCell>();
#endif

#if UNITY_EDITOR
        [ThreadStatic] public static SortedSet<GFRectCell> closedSet = new SortedSet<GFRectCell>();
#else
        [ThreadStatic] internal static SortedSet<GFCell> closedSet = new SortedSet<GFCell>();
#endif

        [ThreadStatic] internal static Dictionary<Vector2Int, GFRectCell> openSetKey = new Dictionary<Vector2Int, GFRectCell>(10000);
        [ThreadStatic] internal static HashSet<Vector2Int> closedSetKey = new HashSet<Vector2Int>(10000);
        [ThreadStatic] internal static Dictionary<Vector2Int, Vector2Int> childToParentDict = new Dictionary<Vector2Int, Vector2Int>(10000);

        const float G_COST = 10;

        readonly static Vector2Int[] neighbors = new Vector2Int[4] {
            new Vector2Int(1, 0) , // →
            new Vector2Int(0, -1) , // ↓
            new Vector2Int(-1, 0) , // ←
            new Vector2Int(0, 1)  // ↑
        };

        /// <summary> ↑ → ↓ ←, returns -1 if not found. </summary>
        public static int AStar(Vector2Int start, Vector2Int end, int limitedCount, Predicate<Vector2Int> isWalkable, Vector2Int[] result, bool isManuallyProcess = false) {

            if (!isWalkable(start) || !isWalkable(end)) {
                return -1;
            }

            // ==== Begin ====
            // A* algorithm
            openSet.Clear();
            openSetKey.Clear();
            closedSet.Clear();
            closedSetKey.Clear();
            childToParentDict.Clear();

            GFRectCell startCell = new GFRectCell();
            startCell.Init(start, 0, 0, 0);

            openSet.Add(startCell);
            openSetKey.Add(startCell.pos, startCell);

            int visited = 0;
            int resultCount = 0;
            // ===============

            if (isManuallyProcess) {
                return 0;
            }

            while (openSet.Count > 0) {
                bool isDone = ManualProcess(ref visited, ref resultCount, limitedCount, start, end, isWalkable, result, out _);
                if (isDone) {
                    return resultCount;
                }
            }

            return -1;

        }

        public static bool ManualProcess(ref int visited, ref int count, int limitedCount, Vector2Int start, Vector2Int end, Predicate<Vector2Int> isWalkable, Vector2Int[] result, out Vector2Int cur) {

            GFRectCell q = openSet.Min;
            cur = q.pos;
            openSet.Remove(q);
            openSetKey.Remove(q.pos);
            closedSet.Add(q);
            closedSetKey.Add(q.pos);

            visited += 1;
            if (visited >= limitedCount) {
                count = -2;
                return true;
            }

            for (int i = 0; i < 4; i++) {

                Vector2Int neighborPos = q.pos + neighbors[i];
                if (!isWalkable(neighborPos) || closedSetKey.Contains(neighborPos)) {
                    continue;
                }

                // 找到了, 从end开始回溯
                if (neighborPos == end) {
                    Vector2Int p = q.pos;
                    count = 0;
                    result[count++] = end;
                    result[count++] = p;
                    while (childToParentDict.TryGetValue(p, out var parent)) {
                        result[count++] = parent;
                        p = parent;
                    }
                    return true;
                }

                float gCost = G_COST;
                float hCost = H_Manhattan(neighborPos, end);
                float fCost = gCost + hCost;
                GFRectCell neighborCell;
                if (openSetKey.TryGetValue(neighborPos, out neighborCell)) {
                    if (fCost < neighborCell.fCost) {
                        openSet.Remove(neighborCell);
                        openSetKey.Remove(neighborPos);
                        childToParentDict.Remove(neighborPos);
                        neighborCell.Init(neighborPos, fCost, gCost, hCost);
                        openSet.Add(neighborCell);
                        openSetKey.Add(neighborPos, neighborCell);
                        childToParentDict.Add(neighborPos, q.pos);
                    }
                } else {
                    neighborCell = new GFRectCell();
                    neighborCell.Init(neighborPos, fCost, gCost, hCost);
                    openSet.Add(neighborCell);
                    openSetKey.Add(neighborPos, neighborCell);
                    childToParentDict.Add(neighborPos, q.pos);
                }

            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float H_Manhattan(Vector2Int cur, Vector2Int end) {
            return 10 * (Mathf.Abs(cur.x - end.x) + Mathf.Abs(cur.y - end.y));
        }

    }

}