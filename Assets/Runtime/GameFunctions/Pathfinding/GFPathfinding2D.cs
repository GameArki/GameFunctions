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
        [ThreadStatic] internal static SortedSet<GFRectCell> openSet = new SortedSet<GFRectCell>();
#endif

#if UNITY_EDITOR
        [ThreadStatic] public static SortedSet<GFRectCell> closedSet = new SortedSet<GFRectCell>();
#else
        [ThreadStatic] internal static SortedSet<GFRectCell> closedSet = new SortedSet<GFRectCell>();
#endif

        [ThreadStatic] internal static Dictionary<Vector2Int, GFRectCell> openSetKey = new Dictionary<Vector2Int, GFRectCell>(10000);
        [ThreadStatic] internal static HashSet<Vector2Int> closedSetKey = new HashSet<Vector2Int>(10000);
        [ThreadStatic] internal static Dictionary<Vector2Int, Vector2Int> childToParentDict = new Dictionary<Vector2Int, Vector2Int>(10000);

        readonly static Vector2Int[] neighbors = new Vector2Int[8] {
            new Vector2Int(1, 0) , // →
            new Vector2Int(0, -1) , // ↓
            new Vector2Int(-1, 0) , // ←
            new Vector2Int(0, 1),  // ↑
            new Vector2Int(1, 1) , // ↗
            new Vector2Int(-1, 1) , // ↖
            new Vector2Int(-1, -1) , // ↙
            new Vector2Int(1, -1)  // ↘
        };
        const int N_RIGHT = 0;
        const int N_DOWN = 1;
        const int N_LEFT = 2;
        const int N_UP = 3;
        const int N_UP_RIGHT = 4;
        const int N_UP_LEFT = 5;
        const int N_DOWN_LEFT = 6;
        const int N_DOWN_RIGHT = 7;

        /// <summary> ↑ → ↓ ←, returns -1 if not found, -2 over limited count. </summary>
        public static int AStar(bool is8Mode, Vector2Int start, Vector2Int end, int limitedCount, Predicate<Vector2Int> isWalkable, Vector2Int[] result, bool isManuallyProcess = false) {

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
                bool isDone = ManualProcess(is8Mode, ref visited, ref resultCount, limitedCount, start, end, isWalkable, result, out _);
                if (isDone) {
                    return resultCount;
                }
            }

            return -1;

        }

        public static bool ManualProcess(bool is8Mode, ref int visited, ref int count, int limitedCount, Vector2Int start, Vector2Int end, Predicate<Vector2Int> isWalkable, Vector2Int[] result, out Vector2Int cur) {

            GFRectCell q = openSet.Min;
            cur = q.pos;
            // Debug.Log("Cur: " + cur);
            bool succ = openSet.Remove(q);
            if (!succ) {
                Debug.LogError("Remove failed");
            }
            succ = openSetKey.Remove(q.pos);
            if (!succ) {
                Debug.LogError("Remove failed");
            }
            closedSet.Add(q);
            closedSetKey.Add(q.pos);

            visited += 1;
            if (visited >= limitedCount) {
                count = -2;
                return true;
            }

            Span<Vector2Int> allowNeighbors = stackalloc Vector2Int[8];
            int allowNeighborCount = 4;
            if (is8Mode) {
                allowNeighborCount = GetAllowNeighborWhen8Dir(cur, isWalkable, neighbors, ref allowNeighbors);
                // allowNeighborCount = 8;
                // allowNeighbors = neighbors;
            } else {
                allowNeighbors = neighbors;
            }

            for (int i = 0; i < allowNeighborCount; i++) {

                bool isSlope = i >= 4;

                Vector2Int neighborPos = q.pos + allowNeighbors[i];

                // 1. 已经在closedSet中
                if (closedSetKey.Contains(neighborPos)) {
                    continue;
                }

                // 2. 外部判定
                if (!isWalkable(neighborPos)) {
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

                float gCost = 10;
                if (isSlope) {
                    gCost = 14;
                }
                float hCost = H_Manhattan(neighborPos, end);
                float fCost = gCost + hCost;
                GFRectCell neighborCell;
                if (openSetKey.TryGetValue(neighborPos, out neighborCell)) {
                    if (fCost < neighborCell.fCost) {
                        succ = openSet.Remove(neighborCell);
                        if (!succ) {
                            Debug.LogError("openSet Remove failed");
                        }
                        succ = openSetKey.Remove(neighborPos);
                        if (!succ) {
                            Debug.LogError("openSetKey Remove failed");
                        }
                        succ = childToParentDict.Remove(neighborPos);
                        if (!succ) {
                            Debug.LogError("childToParentDict Remove failed");
                        }
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

        static HashSet<Vector2Int> temp_notAllowNeighbors = new HashSet<Vector2Int>(8);
        static int GetAllowNeighborWhen8Dir(Vector2Int cur, Predicate<Vector2Int> walkable, Span<Vector2Int> originNeighbors, ref Span<Vector2Int> allowNeighbors) {
            int count = 0;
            temp_notAllowNeighbors.Clear();

            // 处理斜方向
            for (int i = 0; i < 8; i++) {
                Vector2Int neighborPos = cur + originNeighbors[i];
                if (!walkable(neighborPos)) {
                    if (i == N_RIGHT) {
                        // → ↗ ↘ 不让走
                        temp_notAllowNeighbors.Add(originNeighbors[N_UP_RIGHT]);
                        temp_notAllowNeighbors.Add(originNeighbors[N_DOWN_RIGHT]);
                    } else if (i == N_UP) {
                        // ↗ ↖ 不让走
                        temp_notAllowNeighbors.Add(originNeighbors[N_UP_RIGHT]);
                        temp_notAllowNeighbors.Add(originNeighbors[N_UP_LEFT]);
                    } else if (i == N_LEFT) {
                        // ↖ ↙ 不让走
                        temp_notAllowNeighbors.Add(originNeighbors[N_UP_LEFT]);
                        temp_notAllowNeighbors.Add(originNeighbors[N_DOWN_LEFT]);
                    } else if (i == N_DOWN) {
                        // ↙ ↘ 不让走
                        temp_notAllowNeighbors.Add(originNeighbors[N_DOWN_LEFT]);
                        temp_notAllowNeighbors.Add(originNeighbors[N_DOWN_RIGHT]);
                    }
                    temp_notAllowNeighbors.Add(originNeighbors[i]);
                }
            }

            for (int i = 0; i < 8; i++) {
                Vector2Int neighborPos = originNeighbors[i];
                if (temp_notAllowNeighbors.Contains(neighborPos)) {
                    continue;
                }
                allowNeighbors[count++] = neighborPos;
            }
            return count;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float H_Manhattan(Vector2Int cur, Vector2Int end) {
            return 10 * (Mathf.Abs(cur.x - end.x) + Mathf.Abs(cur.y - end.y));
        }

    }

}