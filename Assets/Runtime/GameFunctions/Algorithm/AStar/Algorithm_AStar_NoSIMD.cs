using System;
using System.Collections.Generic;
using UnityEngine;

public static class Algorithm_AStar_NoSIMD {

    public struct Node : IEquatable<Node> {
        public Vector2Int pos;
        public float gCost; // Cost from start to this node
        public float hCost; // Heuristic cost to target
        public float fCost => gCost + hCost; // Total cost
        public Vector2Int parent;
        public Node(Vector2Int position, float g, float h, Vector2Int parent) {
            pos = position;
            gCost = g;
            hCost = h;
            this.parent = parent;
        }

        bool IEquatable<Node>.Equals(Node other) {
            return pos.Equals(other.pos);
        }
    }

    const int DefaultLength = 256 * 256; // Initial size, can be resized
    [ThreadStatic] static Node[] openSet = new Node[DefaultLength]; // Initial size, can be resized
    [ThreadStatic] static Node[] closeSet = new Node[DefaultLength]; // Initial size, can be resized
    [ThreadStatic] static Vector2Int[] path = new Vector2Int[DefaultLength]; // Initial size, can be resized
    public static int Go_8Dir(in Vector2Int start, in Vector2Int end, in Vector2Int edge, in HashSet<Vector2Int> blocks, out Vector2Int[] result) {
        int len = edge.x * edge.y;
        if (openSet.Length < len) {
            openSet = new Node[len];
        }
        if (closeSet.Length < len) {
            closeSet = new Node[len];
        }
        if (path.Length < len) {
            path = new Vector2Int[len];
        }
        result = path;
        int pathCount = Go_8Dir(in start, in end, in edge, blocks, ref openSet, ref closeSet, ref result);
        return pathCount; // Return the number of nodes in the path
    }

    // Call this method to find the path
    public static int Go_8Dir(in Vector2Int start, in Vector2Int end, in Vector2Int edge, in HashSet<Vector2Int> blocks, ref Node[] openSet, ref Node[] closeSet, ref Vector2Int[] result) {
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
                    result[pathCount++] = node.pos;
                    // Find parent in closed set
                    for (int i = 0; i < closedCount; i++) {
                        if (closeSet[i].pos.Equals(node.parent)) {
                            node = closeSet[i];
                            break;
                        }
                    }
                }
                result[pathCount++] = start; // Add start position
                return pathCount; // Return the number of nodes in the path
            }

            // Check neighbors
            for (Vector2Int offset = new Vector2Int(-1, -1); offset.x <= 1; offset.x++) {
                for (offset.y = -1; offset.y <= 1; offset.y++) {
                    Vector2Int neighborPos = currentNode.pos + offset;

                    // Check if neighbor is a block or already in closed set
                    if (Blocks_Contains(neighborPos, blocks)
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

    static int ManhattenDis(in Vector2Int start, in Vector2Int end) {
        Vector2Int diff = start - end;
        return Math.Abs(diff.x) + Math.Abs(diff.y);
    }

    static void OpenSet_AddAndSort(ref Node[] openSet, ref int openCount, in Node node) {
        openSet[openCount++] = node;
    }

    static int OpenSet_GetMinFCostIndex(in Node[] openSet, in int openCount) {
        int minIndex = 0;
        for (int i = 1; i < openCount; i++) {
            if (openSet[i].fCost < openSet[minIndex].fCost) {
                minIndex = i;
            }
        }
        return minIndex;
    }

    static int OpenSet_FindIndex(in Vector2Int pos, in Node[] openSet, in int openCount) {
        for (int i = 0; i < openCount; i++) {
            if (openSet[i].pos.Equals(pos)) {
                return i; // Return index if found
            }
        }
        return -1; // Not found
    }

    static void OpenSet_RemoveAtAndSort(ref Node[] openSet, ref int openCount, int index) {
        if (index < 0 || index >= openCount) return; // Invalid index
        openSet[index] = openSet[--openCount]; // Replace with last element
    }

    static void CloseSet_AddAndSort(ref Node[] closeSet, ref int closedCount, in Node node) {
        closeSet[closedCount++] = node;
    }

    static int CloseSet_FindIndex(in Vector2Int pos, in Node[] closeSet, in int closedCount) {
        for (int i = 0; i < closedCount; i++) {
            if (closeSet[i].pos.Equals(pos)) {
                return i; // Return index if found
            }
        }
        return -1; // Not found
    }

    static bool Blocks_Contains(in Vector2Int pos, in HashSet<Vector2Int> blocks) {
        return blocks.Contains(pos);
    }

    static bool IsOverEdge(in Vector2Int pos, in Vector2Int edge) {
        return pos.x < 0 || pos.y < 0 || pos.x >= edge.x || pos.y >= edge.y;
    }

}