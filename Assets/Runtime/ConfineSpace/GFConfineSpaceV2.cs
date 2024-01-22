using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

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

    [StructLayout(LayoutKind.Explicit)]
    struct I32I32_U64 {

        [FieldOffset(0)] public int i32_0;
        [FieldOffset(4)] public int i32_1;
        [FieldOffset(0)] public ulong u64;

        public I32I32_U64(Vector2Int v) {
            this.u64 = 0;
            this.i32_0 = v.x;
            this.i32_1 = v.y;
        }

        public I32I32_U64(int i32_0, int i32_1) {
            this.u64 = 0;
            this.i32_0 = i32_0;
            this.i32_1 = i32_1;
        }

        public override bool Equals(object obj) {
            if (obj is I32I32_U64) {
                I32I32_U64 other = (I32I32_U64)obj;
                return this.u64 == other.u64;
            }
            return false;
        }

        public override int GetHashCode() => this.u64.GetHashCode();
        public static bool operator ==(I32I32_U64 a, I32I32_U64 b) => a.u64 == b.u64;
        public static bool operator !=(I32I32_U64 a, I32I32_U64 b) => a.u64 != b.u64;

    }

}