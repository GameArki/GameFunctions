using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using GameFunctions.ConfinSpaceInternal;

namespace GameFunctions {

    public static class GFConfineSpaceV2 {

        static HashSet<I32I32_U64> closeSet = new HashSet<I32I32_U64>();
        static Stack<Vector2Int> openStack = new Stack<Vector2Int>();
        static Dictionary<int, Vector2Int> neighbor8Dict = new Dictionary<int, Vector2Int>() {
            { 0, new Vector2Int(0, 1) }, // Up
            { 1, new Vector2Int(1, 1) }, // RightUp
            { 2, new Vector2Int(1, 0) }, // Right
            { 3, new Vector2Int(1, -1) }, // RightDown
            { 4, new Vector2Int(0, -1) }, // Down
            { 5, new Vector2Int(-1, -1) }, // LeftDown
            { 6, new Vector2Int(-1, 0) }, // Left
            { 7, new Vector2Int(-1, 1) }, // LeftUp
        };

        static Dictionary<int, Vector2Int> neighbor4Dict = new Dictionary<int, Vector2Int>() {
            { 0, new Vector2Int(0, 1) }, // Up
            { 1, new Vector2Int(1, 0) }, // Right
            { 2, new Vector2Int(0, -1) }, // Down
            { 3, new Vector2Int(-1, 0) }, // Left
        };

        /// <summary> returns -1 if limitedCount is exceeded </summary>
        public static int Process(Vector2Int startWalkable, int limitedCount, Predicate<Vector2Int> isWalkable, Vector2Int[] result) {
            // Based Djikstra
            if (!isWalkable(startWalkable)) {
                return -1;
            }

            openStack.Clear();
            openStack.Push(startWalkable);

            closeSet.Clear();
            closeSet.Add(new I32I32_U64(startWalkable));

            int walkedCount = 0;
            result[walkedCount++] = startWalkable;

            while (openStack.Count > 0) {

                Vector2Int current = openStack.Pop();
                if (walkedCount >= limitedCount) {
                    return -1;
                }

                for (int i = 0; i < 4; i++) {
                    Vector2Int neighbor = current + neighbor4Dict[i];
                    I32I32_U64 neighborKey = new I32I32_U64(neighbor);
                    if (isWalkable(neighbor)) {
                        if (!closeSet.Contains(neighborKey)) {
                            openStack.Push(neighbor);
                            closeSet.Add(neighborKey);
                            if (walkedCount < limitedCount) {
                                result[walkedCount++] = neighbor;
                            } else {
                                return -1;
                            }
                        }
                    } else {
                        closeSet.Add(neighborKey);
                    }
                }
            }

            return walkedCount;
        }

    }

}