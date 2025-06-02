using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public class BlocksOrder : IComparer<int2> {
    public int Compare(int2 x, int2 y) {
        return CompareStatic(x, y);
    }

    [BurstCompile]
    public static int CompareStatic(in int2 x, in int2 y) {
        if (x.x == y.x) {
            return x.y.CompareTo(y.y);
        }
        return x.x.CompareTo(y.x);
    }
}

[BurstCompile]
public static class Algorithm_AStar {

    [BurstCompile]
    public struct Node : IEquatable<Node> {
        public int2 pos;
        public half gCost; // Cost from start to this node
        public half hCost; // Heuristic cost to target
        public float fCost => gCost + hCost; // Total cost
        public int2 parent;
        public Node(int2 position, float g, float h, int2 parent) {
            pos = position;
            gCost = new half(g);
            hCost = new half(h);
            this.parent = parent;
        }

        bool IEquatable<Node>.Equals(Node other) {
            return pos.Equals(other.pos);
        }
    }

    const int DefaultLength = 256 * 256; // Initial size, can be resized
    [ThreadStatic] static NativeArray<Node> openSet = new NativeArray<Node>(DefaultLength, Allocator.Persistent); // Initial size, can be resized
    [ThreadStatic] static NativeArray<Node> closeSet = new NativeArray<Node>(DefaultLength, Allocator.Persistent); // Initial size, can be resized
    [ThreadStatic] static NativeArray<int2> path = new NativeArray<int2>(DefaultLength, Allocator.Persistent); // Initial size, can be resized
    public static int Go_8Dir_SIMD(in int2 start, in int2 end, in int2 edge, in NativeArray<int2> blocks, in int blockCount, out NativeArray<int2> result) {
        int len = edge.x * edge.y;
        if (openSet.Length < len) {
            openSet.Dispose();
            openSet = new NativeArray<Node>(len, Allocator.Persistent);
        }
        if (closeSet.Length < len) {
            closeSet.Dispose();
            closeSet = new NativeArray<Node>(len, Allocator.Persistent);
        }
        if (path.Length < len) {
            path.Dispose();
            path = new NativeArray<int2>(len, Allocator.Persistent);
        }
        result = path;
        int pathCount = Go_8Dir_SIMD(in start, in end, in edge, blocks, blockCount, ref openSet, ref closeSet, ref result);
        return pathCount; // Return the number of nodes in the path
    }

    // Call this method to find the path
    [BurstCompile]
    static int Go_8Dir_SIMD(in int2 start, in int2 end, in int2 edge, in NativeArray<int2> blocks, in int blockCount, ref NativeArray<Node> openSet, ref NativeArray<Node> closeSet, ref NativeArray<int2> path) {
        int pathCount = -1;
        int openCount = 0;
        int closedCount = 0;

        Node startNode = new Node(start, 0, ManhattenDis(start, end), start);
        OpenSet_AddAndSort(ref openSet, ref openCount, startNode);

        while (openCount > 0) {
            // PERF 最小堆: Find the node with the lowest fCost
            int lowestIndex = OpenSet_GetMinFCostIndex(in openSet, openCount);

            Node currentNode = openSet[lowestIndex];
            OpenSet_RemoveAtAndSort(ref openSet, ref openCount, lowestIndex);
            CloseSet_AddAndSort(ref closeSet, ref closedCount, currentNode);

            // If we reached the target
            if (ManhattenDis(currentNode.pos, end) < 2) {
                // Reconstruct path
                pathCount = 0;
                Node node = currentNode;
                while (node.pos.x != start.x || node.pos.y != start.y) {
                    path[pathCount++] = node.pos;
                    // Find parent in closed set
                    for (int i = 0; i < closedCount; i++) {
                        if (closeSet[i].pos.Equals(node.parent)) {
                            node = closeSet[i];
                            break;
                        }
                    }
                }
                path[pathCount++] = start; // Add start position
                return pathCount; // Return the number of nodes in the path
            }

            // Check neighbors
            for (int2 offset = new int2(-1, -1); offset.x <= 1; offset.x++) {
                for (offset.y = -1; offset.y <= 1; offset.y++) {
                    int2 neighborPos = currentNode.pos + offset;

                    // Check if neighbor is a block or already in closed set
                    if (Blocks_FindIndex(neighborPos, blocks, blockCount) != -1
                        || (CloseSet_FindIndex(neighborPos, closeSet, closedCount) != -1)
                        || IsOverEdge(neighborPos, edge)) {
                        continue;
                    }

                    float gCost = currentNode.gCost + ManhattenDis(currentNode.pos, neighborPos);
                    float hCost = ManhattenDis(neighborPos, end);
                    Node neighborNode = new Node(neighborPos, gCost, hCost, currentNode.pos);

                    // Check if neighbor is in open set
                    int existingIndex = OpenSet_FindIndex(neighborPos, openSet, openCount);
                    if (existingIndex >= 0) {
                        // If this path is better, update it
                        // - their parent is different
                        if (gCost < openSet[existingIndex].gCost) {
                            openSet[existingIndex] = neighborNode;
                        }
                    } else {
                        OpenSet_AddAndSort(ref openSet, ref openCount, neighborNode);
                    }
                }
            }
        }

        return pathCount;
    }

    [BurstCompile]
    static float ManhattenDis(in int2 start, in int2 end) {
        int2 diff = start - end;
        return math.abs(diff.x) + math.abs(diff.y);
    }

    [BurstCompile]
    static void OpenSet_AddAndSort(ref NativeArray<Node> openSet, ref int openCount, in Node node) {
        openSet[openCount++] = node;
    }

    [BurstCompile]
    static int OpenSet_GetMinFCostIndex(in NativeArray<Node> openSet, in int openCount) {
        int minIndex = 0;
        for (int i = 1; i < openCount; i++) {
            if (openSet[i].fCost < openSet[minIndex].fCost) {
                minIndex = i;
            }
        }
        return minIndex;
    }

    [BurstCompile]
    static int OpenSet_FindIndex(in int2 pos, in NativeArray<Node> openSet, in int openCount) {
        for (int i = 0; i < openCount; i++) {
            if (openSet[i].pos.Equals(pos)) {
                return i; // Return index if found
            }
        }
        return -1; // Not found
    }

    [BurstCompile]
    static void OpenSet_RemoveAtAndSort(ref NativeArray<Node> openSet, ref int openCount, int index) {
        if (index < 0 || index >= openCount) return; // Invalid index
        openSet[index] = openSet[--openCount]; // Replace with last element
    }

    [BurstCompile]
    static void CloseSet_AddAndSort(ref NativeArray<Node> closeSet, ref int closedCount, in Node node) {
        closeSet[closedCount++] = node;
    }

    [BurstCompile]
    static int CloseSet_FindIndex(in int2 pos, in NativeArray<Node> closeSet, in int closedCount) {
        for (int i = 0; i < closedCount; i++) {
            if (closeSet[i].pos.Equals(pos)) {
                return i; // Return index if found
            }
        }
        return -1; // Not found
    }

    [BurstCompile]
    static int Blocks_FindIndex(in int2 pos, in NativeArray<int2> blocks, in int blockCount) {
        // Binary search for performance
        int left = 0;
        int right = blockCount - 1;
        while (left <= right) {
            int mid = left + (right - left) / 2;
            int comparison = BlocksOrder.CompareStatic(blocks[mid], pos);
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
    static bool IsOverEdge(in int2 pos, in int2 edge) {
        return pos.x < 0 || pos.y < 0 || pos.x >= edge.x || pos.y >= edge.y;
    }

}