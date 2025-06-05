using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;

[BurstCompile]
public class Comparer_short2 : IComparer<short2> {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(short2 x, short2 y) {
        return CompareStatic(x, y);
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareStatic(in short2 a, in short2 b) {
        int res = a.x.CompareTo(b.x);
        if (res == 0) {
            res = a.y.CompareTo(b.y);
        }
        return res;
    }
}

[BurstCompile]
public static class Algorithm_AStar {

    [BurstCompile]
    public struct Node {
        public short2 pos; // 4 
        public short2 parent; // 4
        public float gCost; // 4 // Cost from start to this node
        public float hCost; // 4 // Heuristic cost to target
        public Node(short2 position, float g, float h, short2 parent) {
            pos = position;
            gCost = g;
            hCost = h;
            this.parent = parent;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float FCost() => gCost + hCost; // Total cost

    }

    const int DefaultWidth = 256; // Initial width, can be resized
    const int DefaultHeight = 256; // Initial height, can be resized
    const int DefaultArea = DefaultWidth * DefaultHeight; // Initial size, can be resized
    const int DefaultPerimeter = (DefaultWidth + DefaultHeight) * 2 * 8; // Initial size, can be resized
    [ThreadStatic] static NativeArray<Node> openSet;
    [ThreadStatic] static NativeArray<Node> closeSet;
    [ThreadStatic] static NativeArray<short2> path;
    public static void Init(int width, int height) {
        int area = width * height;
        int perimeter = (width + height) * 2 * 8; // 周长
        openSet = new NativeArray<Node>(perimeter, Allocator.Persistent);
        closeSet = new NativeArray<Node>(area, Allocator.Persistent);
        path = new NativeArray<short2>(area, Allocator.Persistent);
    }

    public static void Dispose() {
        if (openSet.IsCreated) {
            openSet.Dispose();
        }
        if (closeSet.IsCreated) {
            closeSet.Dispose();
        }
        if (path.IsCreated) {
            path.Dispose();
        }
    }

    public static int Go_8Dir_SIMD(in short2 start, in short2 end, in short2 edge, in NativeArray<short2> blocks, in int blockCount, out NativeArray<short2> result) {
        int area = edge.x * edge.y; // 面积
        int perimeter = (edge.x + edge.y) * 2 * 8; // 周长
        if (openSet.Length < perimeter) {
            openSet.Dispose();
            openSet = new NativeArray<Node>(perimeter, Allocator.Persistent);
        }
        if (closeSet.Length < area) {
            closeSet.Dispose();
            closeSet = new NativeArray<Node>(area, Allocator.Persistent);
        }
        if (path.Length < area) {
            path.Dispose();
            path = new NativeArray<short2>(area, Allocator.Persistent);
        }
        result = path;
        int pathCount = Go_8Dir_SIMD(in start, in end, in edge, blocks, blockCount, ref openSet, ref closeSet, ref result);
        return pathCount; // Return the number of nodes in the path
    }

    // Call this method to find the path
    [BurstCompile]
    static int Go_8Dir_SIMD(in short2 start, in short2 end, in short2 edge, in NativeArray<short2> blocks, in int blockCount, ref NativeArray<Node> openSet, ref NativeArray<Node> closeSet, ref NativeArray<short2> path) {
        int pathCount = -1;
        int openCount = 0;

        Node startNode = new Node(start, 0, ManhattenDis(start, end), start);
        OpenSet_Add(ref openSet, ref openCount, startNode);
        CloseSet_Clear(ref closeSet);

        while (openCount > 0) {
            int lowestIndex = OpenSet_GetMinFCostIndex(in openSet, openCount, out Node currentNode);
            OpenSet_RemoveAt(ref openSet, ref openCount, lowestIndex);
            CloseSet_Add(ref closeSet, edge.x, currentNode);

            // If we reached the target
            if (ManhattenDis(currentNode.pos, end) < 2) {
                // Reconstruct path
                pathCount = 0;
                Node node = currentNode;
                while (node.pos.x != start.x || node.pos.y != start.y) {
                    Array_SetValue(ref path, pathCount++, node.pos); // Store the current node in the path
                    // Find parent in closed set
                    int parentIndex = node.parent.x + node.parent.y * edge.x; // Convert 2D position to 1D index
                    if (parentIndex < 0 || parentIndex >= closeSet.Length) {
                        break; // Out of bounds
                    }
                    Array_GetValue(in closeSet, parentIndex, out Node parent);
                    if (parent.pos.x == node.parent.x && parent.pos.y == node.parent.y) {
                        node = parent;
                    } else {
                        break; // Parent not found
                    }
                }
                Array_SetValue(ref path, pathCount++, start); // Store the current node in the path
                return pathCount; // Return the number of nodes in the path
            }

            // Check neighbors
            for (short2 offset = new short2(-1, -1); offset.x <= 1; offset.x++) {
                for (offset.y = -1; offset.y <= 1; offset.y++) {
                    short2.Add(currentNode.pos, offset, out short2 neighborPos);

                    // Check if neighbor is a block or already in closed set
                    if (CloseSet_FindIndex(neighborPos, closeSet, edge.x) != -1
                        || Blocks_FindIndex(neighborPos, blocks, blockCount) != -1
                        || IsOverEdge(neighborPos, edge)) {
                        continue;
                    }

                    float gCost = currentNode.gCost + ManhattenDis(currentNode.pos, neighborPos);
                    float hCost = ManhattenDis(neighborPos, end);
                    Node neighborNode = new Node(neighborPos, gCost, hCost, currentNode.pos);

                    // Check if neighbor is in open set
                    int existingIndex = OpenSet_FindIndex_Reverse(neighborPos, openSet, openCount, out Node existingNode);
                    if (existingIndex >= 0) {
                        // If this path is better, update it
                        // - their parent is different
                        if (gCost < existingNode.gCost) {
                            Array_SetValue(ref openSet, existingIndex, neighborNode);
                        }
                    } else {
                        OpenSet_Add(ref openSet, ref openCount, neighborNode);
                    }
                }
            }
        }

        return pathCount;
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float ManhattenDis(in short2 start, in short2 end) {
        short2.Minus(in start, in end, out short2 diff);
        return math.abs(diff.x) + math.abs(diff.y); // Manhattan distance
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void OpenSet_Add(ref NativeArray<Node> openSet, ref int openCount, in Node node) {
        Array_SetValue(ref openSet, openCount++, node); // Add the node to the open set
    }

    [BurstCompile]
    static int OpenSet_GetMinFCostIndex(in NativeArray<Node> openSet, in int openCount, out Node result) {
        int minIndex = 0;
        Array_GetValue(in openSet, in minIndex, out Node minNode);
        for (int i = 1; i < openCount; i++) {
            Array_GetValue(in openSet, in i, out Node node);
            Array_GetValue(in openSet, in minIndex, out minNode);
            if (node.FCost() < minNode.FCost()) {
                minIndex = i;
            }
        }
        result = minNode;
        return minIndex; // Return index of the node with the lowest fCost
    }

    [BurstCompile]
    static int OpenSet_FindIndex(in short2 pos, in NativeArray<Node> openSet, in int openCount) {
        for (int i = 0; i < openCount; i++) {
            Array_GetValue(in openSet, in i, out Node node);
            if (node.pos.x == pos.x && node.pos.y == pos.y) {
                return i; // Return index if found
            }
        }
        return -1; // Not found
    }

    [BurstCompile]
    static int OpenSet_FindIndex_Reverse(in short2 pos, in NativeArray<Node> openSet, in int openCount, out Node result) {
        for (int i = openCount - 1; i >= 0; i--) {
            Array_GetValue(in openSet, in i, out Node node);
            if (node.pos.x == pos.x && node.pos.y == pos.y) {
                result = node; // Return the found node
                return i; // Return index if found
            }
        }
        result = new Node(); // Default value if not found
        return -1; // Not found
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void OpenSet_RemoveAt(ref NativeArray<Node> openSet, ref int openCount, int index) {
        openCount--;
        Array_GetValue(in openSet, openCount, out Node node);
        Array_SetValue(ref openSet, index, node); // Set the node at the calculated index
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void CloseSet_Add(ref NativeArray<Node> closeSet, in int gridWidth, in Node node) {
        int index = node.pos.x + node.pos.y * gridWidth; // Convert 2D position to 1D index
        if (index < 0 || index >= closeSet.Length) {
            return;
        }
        Array_SetValue(ref closeSet, index, node); // Set the node at the calculated index
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void CloseSet_Clear(ref NativeArray<Node> closeSet) {
        for (int i = 0; i < closeSet.Length; i++) {
            Array_SetValue(ref closeSet, i, new Node() { pos = new short2(-1, -1) }); // Reset all nodes
        }
    }

    [BurstCompile]
    static unsafe int CloseSet_FindIndex(in short2 pos, in NativeArray<Node> closeSet, in int gridWidth) {
        int index = pos.x + pos.y * gridWidth; // Convert 2D position to 1D index
        if (index < 0 || index >= closeSet.Length) {
            return -1; // Out of bounds
        }
        Array_GetValue(in closeSet, in index, out Node node);
        if (node.pos.x == pos.x && node.pos.y == pos.y) {
            return index; // Return index if found
        }
        return -1; // Not found
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe static void Array_GetValue(in NativeArray<Node> array, in int index, out Node value) {
        value = Unsafe.Read<Node>((byte*)array.GetUnsafeReadOnlyPtr() + index * sizeof(Node));
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe static void Array_GetValue(in NativeArray<short2> array, in int index, out short2 value) {
        value = Unsafe.Read<short2>((byte*)array.GetUnsafeReadOnlyPtr() + index * sizeof(short2));
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe static void Array_SetValue(ref NativeArray<Node> array, in int index, in Node value) {
        Unsafe.Write((byte*)array.GetUnsafePtr() + index * sizeof(Node), value);
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe static void Array_SetValue(ref NativeArray<short2> array, in int index, in short2 value) {
        Unsafe.Write((byte*)array.GetUnsafePtr() + index * sizeof(short2), value);
    }

    [BurstCompile]
    static int Blocks_FindIndex(in short2 pos, in NativeArray<short2> blocks, in int blockCount) {
        // Binary search for performance
        int left = 0;
        int right = blockCount - 1;
        while (left <= right) {
            int mid = left + (right - left) / 2;
            Array_GetValue(in blocks, mid, out short2 blockPos);
            int comparison = Comparer_short2.CompareStatic(blockPos, pos);
            if (comparison == 0) {
                return mid; // Found
            } else if (comparison < 0) {
                left = mid + 1; // Search in the right half
            } else {
                right = mid - 1; // Search in the left half
            }
        }
        // If not found, return -1
        return -1;
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsOverEdge(in short2 pos, in short2 edge) {
        return pos.x < 0 || pos.y < 0 || pos.x >= edge.x || pos.y >= edge.y;
    }

}