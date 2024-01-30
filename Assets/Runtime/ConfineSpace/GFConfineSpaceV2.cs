using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions {

    public static class GFConfineSpaceV2 {

        [ThreadStatic] static HashSet<Vector2Int> closeSet = new HashSet<Vector2Int>();
        [ThreadStatic] static Stack<Vector2Int> openStack = new Stack<Vector2Int>();
        readonly static Dictionary<int, Vector2Int> neighbor4Dict = new Dictionary<int, Vector2Int>() {
            { 0, new Vector2Int(0, 1) }, // Up
            { 1, new Vector2Int(1, 0) }, // Right
            { 2, new Vector2Int(0, -1) }, // Down
            { 3, new Vector2Int(-1, 0) }, // Left
        };

        /// <summary> returns -1 if limitedCount is exceeded </summary>
        public static int Process(Vector2Int startWalkable, int limitedCount, Predicate<Vector2Int> isWalkable, Vector2Int[] result) {

            // BFS
            if (!isWalkable(startWalkable)) {
                return -1;
            }

            openStack.Clear();
            openStack.Push(startWalkable);

            closeSet.Clear();
            closeSet.Add(startWalkable);

            int walkedCount = 0;
            result[walkedCount++] = startWalkable;

            while (openStack.Count > 0) {

                Vector2Int current = openStack.Pop();
                if (walkedCount >= limitedCount) {
                    return -1;
                }

                for (int i = 0; i < 4; i++) {
                    Vector2Int neighbor = current + neighbor4Dict[i];
                    if (isWalkable(neighbor)) {
                        if (!closeSet.Contains(neighbor)) {
                            openStack.Push(neighbor);
                            closeSet.Add(neighbor);
                            if (walkedCount < limitedCount) {
                                result[walkedCount++] = neighbor;
                            } else {
                                return -1;
                            }
                        }
                    } else {
                        closeSet.Add(neighbor);
                    }
                }
            }

            return walkedCount;
        }

    }

}