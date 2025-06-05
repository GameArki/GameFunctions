using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;

[BurstCompile]
public struct NodeSet {

    public NativeArray<float> gArr;
    public NativeArray<float> hArr;
    public NativeArray<short2> posArr;
    public NativeArray<short2> parentArr;
    public int count;
    public int length;

    public NodeSet(int size, Allocator allocator) {
        gArr = new NativeArray<float>(size, allocator);
        hArr = new NativeArray<float>(size, allocator);
        posArr = new NativeArray<short2>(size, allocator);
        parentArr = new NativeArray<short2>(size, allocator);
        count = 0;
        length = size;
    }

    public void Dispose() {
        if (gArr.IsCreated) gArr.Dispose();
        if (hArr.IsCreated) hArr.Dispose();
        if (posArr.IsCreated) posArr.Dispose();
        if (parentArr.IsCreated) parentArr.Dispose();
    }

    [BurstCompile]
    public void Clear() {
        for (int i = 0; i < length; i++) {
            posArr[i] = new short2(-1, -1); // Reset position to an invalid value
        }
    }

    [BurstCompile]
    public void SetNode(in int index, in short2 pos, in float g, in float h, in short2 parent) {
        if (index < 0 || index >= posArr.Length) {
            throw new IndexOutOfRangeException("Index out of bounds for NodeSet.");
        }
        posArr[index] = pos;
        gArr[index] = g;
        hArr[index] = h;
        parentArr[index] = parent;
    }

    [BurstCompile]
    public void SetG(in int index, in float gCost) {
        gArr[index] = gCost;
    }

    [BurstCompile]
    public void SetH(in int index, in float hCost) {
        hArr[index] = hCost;
    }

    [BurstCompile]
    public void SetParent(in int index, in short2 parent) {
        parentArr[index] = parent;
    }

    [BurstCompile]
    public void SetPosition(in int index, in short2 pos) {
        posArr[index] = pos;
    }

    [BurstCompile]
    public void GetNode(in int index, out short2 pos, out float g, out float h, out short2 parent) {
        if (index < 0 || index >= posArr.Length) {
            throw new IndexOutOfRangeException("Index out of bounds for NodeSet.");
        }
        pos = posArr[index];
        g = gArr[index];
        h = hArr[index];
        parent = parentArr[index];
    }

    [BurstCompile]
    public int FindIndexReverse_OutG(in short2 position, out float foundG) {
        for (int i = count - 1; i >= 0; i--) {
            if (posArr[i].x == position.x && posArr[i].y == position.y) {
                foundG = gArr[i]; // Return the g cost of the found node
                return i; // Return index if found
            }
        }
        foundG = float.MaxValue; // Default value if not found
        return -1; // Not found
    }

    [BurstCompile]
    public int GetMinFCostIndex() {
        int minIndex = 0;
        float minFCost = gArr[0] + hArr[0];
        for (int i = 1; i < count; i++) {
            float fCost = gArr[i] + hArr[i];
            if (fCost < minFCost) {
                minFCost = fCost;
                minIndex = i;
            }
        }
        return minIndex; // Return index of the node with the lowest fCost
    }

    [BurstCompile]
    public void RemoveAt(int index) {
        if (index < 0 || index >= posArr.Length) {
            throw new IndexOutOfRangeException("Index out of bounds for NodeSet.");
        }
        int lastIndex = count - 1;
        posArr[index] = posArr[lastIndex];
        gArr[index] = gArr[lastIndex];
        hArr[index] = hArr[lastIndex];
        parentArr[index] = parentArr[lastIndex];
        count--;
    }

    [BurstCompile]
    public void AddNode(in short2 position, in float gCost, in float hCost, in short2 parentPos) {
        posArr[count] = position;
        gArr[count] = gCost;
        hArr[count] = hCost;
        parentArr[count] = parentPos;
        count++;
    }

    [BurstCompile]
    public float GetFCost(int index) {
        return gArr[index] + hArr[index]; // Total cost
    }

    [BurstCompile]
    public float GetGCost(int index) {
        return gArr[index]; // Cost from start to this node
    }

    [BurstCompile]
    public float GetHCost(int index) {
        return hArr[index]; // Heuristic cost to target
    }

    [BurstCompile]
    public void GetPosition(int index, out short2 position) {
        position = posArr[index]; // Get the position of the node
    }

    [BurstCompile]
    public void GetParent(int index, out short2 parentPos) {
        parentPos = parentArr[index]; // Get the parent position of the node
    }

}

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

    [ThreadStatic] static NodeSet openSet;
    [ThreadStatic] static NodeSet closeSet;
    [ThreadStatic] static NativeArray<short2> path;
    public static void Init(int width, int height) {
        int area = width * height;
        int perimeter = (width + height) * 2 * 8; // 周长
        openSet = new NodeSet(perimeter, Allocator.Persistent);
        closeSet = new NodeSet(area, Allocator.Persistent);
        path = new NativeArray<short2>(area, Allocator.Persistent);
    }

    public static void Dispose() {
        openSet.Dispose();
        closeSet.Dispose();
        if (path.IsCreated) {
            path.Dispose();
        }
    }

    public static int Go_8Dir_SIMD(in short2 start, in short2 end, in short2 edge, in NativeArray<short2> blocks, in int blockCount, out NativeArray<short2> result) {
        int area = edge.x * edge.y; // 面积
        int perimeter = (edge.x + edge.y) * 2 * 8; // 周长
        if (openSet.length < perimeter) {
            openSet.Dispose();
            openSet = new NodeSet(perimeter, Allocator.Persistent);
        }
        if (closeSet.length < area) {
            closeSet.Dispose();
            closeSet = new NodeSet(area, Allocator.Persistent);
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
    static int Go_8Dir_SIMD(in short2 start, in short2 end, in short2 edge, in NativeArray<short2> blocks, in int blockCount, ref NodeSet openSet, ref NodeSet closeSet, ref NativeArray<short2> path) {
        int pathCount = -1;

        openSet.AddNode(start, 0, ManhattenDis(start, end), start); // Add start node to open set
        closeSet.Clear();

        while (openSet.count > 0) {
            int lowestIndex = openSet.GetMinFCostIndex();
            openSet.RemoveAt(lowestIndex);
            openSet.GetNode(lowestIndex, out short2 cur_pos, out float cur_g, out float cur_h, out short2 cur_parent);
            CloseSet_SetNode(ref closeSet, cur_pos, cur_g, cur_h, cur_parent, edge.x); // Add current node to closed set

            // If we reached the target
            if (ManhattenDis(cur_pos, end) < 2) {
                // Reconstruct path
                pathCount = 0;
                short2 nodePos = cur_pos;
                short2 nodeParent = cur_parent;
                while (nodePos.x != start.x || nodePos.y != start.y) {
                    path[pathCount++] = nodePos; // Store the current node in the path
                    // Find parent in closed set
                    int parentIndex = nodeParent.x + nodeParent.y * edge.x; // Convert 2D position to 1D index
                    if (parentIndex < 0 || parentIndex >= closeSet.length) {
                        break; // Out of bounds
                    }
                    closeSet.GetPosition(parentIndex, out nodePos); // Get the position of the parent node
                    closeSet.GetParent(parentIndex, out nodeParent); // Get the parent position
                }
                return pathCount; // Return the number of nodes in the path
            }

            // Check neighbors
            for (short2 offset = new short2(-1, -1); offset.x <= 1; offset.x++) {
                for (offset.y = -1; offset.y <= 1; offset.y++) {
                    short2.Add(cur_pos, offset, out short2 neighborPos);

                    // Check if neighbor is a block or already in closed set
                    if (CloseSet_FindIndex(neighborPos, closeSet, edge.x) != -1
                        || Blocks_FindIndex(neighborPos, blocks, blockCount) != -1
                        || IsOverEdge(neighborPos, edge)) {
                        continue;
                    }

                    float gCost = cur_g + ManhattenDis(cur_pos, neighborPos);
                    float hCost = ManhattenDis(neighborPos, end);

                    // Check if neighbor is in open set
                    int existingIndex = openSet.FindIndexReverse_OutG(neighborPos, out float existingG);
                    if (existingIndex >= 0) {
                        // If this path is better, update it
                        // - their parent is different
                        if (gCost < existingG) {
                            openSet.SetNode(existingIndex, neighborPos, gCost, hCost, cur_pos);
                        }
                    } else {
                        openSet.AddNode(neighborPos, gCost, hCost, cur_pos); // Add neighbor to open set
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
    static unsafe int CloseSet_SetNode(ref NodeSet closeSet, in short2 pos, in float g, in float h, in short2 parent, in int gridWidth) {
        int index = pos.x + pos.y * gridWidth; // Convert 2D position to 1D index
        if (index < 0 || index >= closeSet.length) {
            return -1; // Out of bounds
        }
        closeSet.SetNode(index, pos, g, h, parent); // Set the node in the close set
        return index; // Return the index where the node was added
    }

    [BurstCompile]
    static unsafe int CloseSet_FindIndex(in short2 pos, in NodeSet closeSet, in int gridWidth) {
        int index = pos.x + pos.y * gridWidth; // Convert 2D position to 1D index
        if (index < 0 || index >= closeSet.length) {
            return -1; // Out of bounds
        }
        closeSet.GetPosition(index, out short2 nodePos);
        if (nodePos.x == pos.x && nodePos.y == pos.y) {
            return index; // Return index if found
        }
        return -1; // Not found
    }

    [BurstCompile]
    static int Blocks_FindIndex(in short2 pos, in NativeArray<short2> blocks, in int blockCount) {
        // Binary search for performance
        int left = 0;
        int right = blockCount - 1;
        while (left <= right) {
            int mid = left + (right - left) / 2;
            short2 blockPos = blocks[mid];
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