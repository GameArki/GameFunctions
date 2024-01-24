using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using GameFunctions.PathfindingInternal;

namespace GameFunctions {

    public static class GFPathfinding2D {

        [ThreadStatic] public static SortedSet<GFCell> openSet = new SortedSet<GFCell>();
        [ThreadStatic] public static HashSet<I32I32_U64> openSetKey = new HashSet<I32I32_U64>(10000);
        [ThreadStatic] public static SortedSet<GFCell> closedSet = new SortedSet<GFCell>();
        [ThreadStatic] public static HashSet<I32I32_U64> closedSetKey = new HashSet<I32I32_U64>(10000);
        [ThreadStatic] public static Dictionary<I32I32_U64, GFCell> posToCell = new Dictionary<I32I32_U64, GFCell>(10000);

        [ThreadStatic]
        readonly static Dictionary<int, Vector2Int> neighbors = new Dictionary<int, Vector2Int> {
            { 0, new Vector2Int(0, 1) },
            { 1, new Vector2Int(1, 0) },
            { 2, new Vector2Int(0, -1) },
            { 3, new Vector2Int(-1, 0) }
        };

        /// <summary> ↑ → ↓ ←, returns -1 if not found. </summary>
        public static int AStar(Vector2Int start, Vector2Int end, int limitedCount, Predicate<Vector2Int> isWalkable, Vector2Int[] result) {

            // A* algorithm
            openSet.Clear();
            openSetKey.Clear();
            closedSet.Clear();
            closedSetKey.Clear();
            posToCell.Clear();

            float G_COST = 100;

            GFCell startCell = new GFCell(start, 0, G_COST, new I32I32_U64(start));
            openSet.Add(startCell);
            openSetKey.Add(startCell.key);
            closedSet.Add(startCell);
            closedSetKey.Add(startCell.key);
            posToCell.Add(startCell.key, startCell);

            int visited = 0;

            while (openSet.Count > 0) {

                GFCell q = openSet.Min;
                openSet.Remove(q);
                openSetKey.Remove(q.key);
                closedSet.Add(q);
                closedSetKey.Add(q.key);

                visited += 1;
                if (visited >= limitedCount) {
                    return -2;
                }

                for (int i = 0; i < 4; i++) {

                    Vector2Int neighborPos = q.pos + neighbors[i];
                    if (!isWalkable(neighborPos) || closedSetKey.Contains(new I32I32_U64(neighborPos))) {
                        continue;
                    }

                    // 找到了, 从end开始回溯
                    if (neighborPos == end) {
                        GFCell point = q;
                        int count = 0;
                        while (point.pos != start) {
                            result[count++] = point.pos;
                            point = posToCell[point.parent];
                        }
                        return count;
                    }

                    GFCell neighborCell = new GFCell();
                    neighborCell.pos = neighborPos;
                    neighborCell.parent = q.key;
                    neighborCell.gCost = q.gCost + Vector2.Distance(q.pos, neighborPos);
                    float h = H_Manhattan(neighborPos, end);
                    float f = neighborCell.gCost + h;
                    neighborCell.fCost = f;
                    openSet.Add(neighborCell);
                    openSetKey.Add(neighborCell.key);
                    posToCell.TryAdd(neighborCell.key, neighborCell);

                }
            }

            return -1;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float H_Manhattan(Vector2Int cur, Vector2Int end) {
            return 10 * Mathf.Abs(cur.x - end.x) + Mathf.Abs(cur.y - end.y);
        }

    }

}