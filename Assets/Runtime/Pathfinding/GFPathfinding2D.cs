using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using GameFunctions.PathfindingInternal;

namespace GameFunctions {

    public static class GFPathfinding2D {

        [ThreadStatic] public static SortedSet<GFCell> openSet = new SortedSet<GFCell>();
        [ThreadStatic] public static SortedSet<GFCell> closedSet = new SortedSet<GFCell>();
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
            closedSet.Clear();
            posToCell.Clear();

            float G_COST = 10;

            GFCell startCell = new GFCell(start, 0, G_COST, new I32I32_U64(start));
            openSet.Add(startCell);
            posToCell.Add(startCell.key, startCell);

            int visited = 0;

            while (openSet.Count > 0) {

                GFCell q = openSet.Min;
                openSet.Remove(q);

                visited += 1;
                if (visited >= limitedCount) {
                    return -2;
                }

                for (int i = 0; i < 4; i++) {

                    Vector2Int neighbor = q.pos + neighbors[i];

                    // 找到了, 从end开始回溯
                    if (neighbor == end) {
                        GFCell point = q;
                        int count = 0;
                        while (point.pos != start) {
                            result[count++] = point.pos;
                            point = posToCell[point.parent];
                        }
                        return count;
                    }

                    if (isWalkable(neighbor)) {
                        float g = q.gCost + D_Distance(neighbor, q.pos);
                        float h = H_Manhattan(neighbor, end);
                        float f = g + h;
                        if (neighbor == openSet.Min.pos && f > openSet.Min.fCost) {
                            continue;
                        }
                        if (neighbor == closedSet.Min.pos && f > closedSet.Min.fCost) {
                            continue;
                        }
                        GFCell neighborCell = new GFCell(neighbor, f, g, q.key);
                        openSet.Add(neighborCell);
                        posToCell.TryAdd(neighborCell.key, neighborCell);
                    } else {
                        GFCell neighborCell = new GFCell(neighbor, float.MaxValue, float.MaxValue, q.key);
                        closedSet.Add(neighborCell);
                        posToCell.TryAdd(neighborCell.key, neighborCell);
                    }
                }
                closedSet.Add(q);
            }

            return -1;

        }

        static float D_Distance(Vector2 neighbor, Vector2 cur) {
            return Vector2.Distance(neighbor, cur);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float H_Manhattan(Vector2Int cur, Vector2Int end) {
            return Mathf.Abs(cur.x - end.x) + Mathf.Abs(cur.y - end.y);
        }

    }

}