using System;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace GameFunctions.Sample {

    public class Algorithm_AStar_Sample : MonoBehaviour {

        NativeArray<short2> path;
        int pathCount;

        [SerializeField] short2 start;
        [SerializeField] short2 end;

        [SerializeField] int seed = 0;
        [SerializeField] int width = 256;
        [SerializeField] int height = 256;

        [SerializeField] int blockCount;
        NativeArray<short2> blocks;

        void Awake() {
            short2 edge = new short2((short)width, (short)height);

            Algorithm_AStar.Init(width, height); // Initialize the algorithm with the grid size

            blocks = new NativeArray<short2>(blockCount, Allocator.Persistent);
            // Fill blocks with some random positions
            System.Random rd = new System.Random(seed);
            for (int i = 0; i < blockCount; i++) {
                blocks[i] = new short2((short)rd.Next(0, width), (short)rd.Next(0, height));
            }

            // Sort blocks to ensure they are in a consistent order
            blocks.Sort(new Comparer_short2());
            // Call the SIMD version of the A* algorithm
            pathCount = Algorithm_AStar.Go_8Dir_SIMD(start, end, edge, blocks, blocks.Length, out path);
            Debug.Log($"Path Count: {pathCount}");
            // Dispose of the blocks array after use

        }

        void OnDestroy() {
            if (blocks.IsCreated) {
                blocks.Dispose();
            }
            Algorithm_AStar.Dispose(); // Clean up the algorithm resources
        }

        void OnDrawGizmos() {
            if (pathCount <= 0) {
                return;
            }

            // Draw blocks
            Gizmos.color = Color.yellow;
            for (int i = 0; i < blockCount; i++) {
                Gizmos.DrawCube(new Vector3(blocks[i].x, blocks[i].y, 0), Vector3.one);
            }

            // Draw the path using Gizmos
            Gizmos.color = Color.green;
            for (int i = 0; i < pathCount - 1; i++) {
                Gizmos.DrawCube(new Vector3(path[i].x, path[i].y, 0), Vector3.one);
            }

            // Optionally, draw the start and end points
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(start.x, start.y, 0), Vector3.one); // Start point
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(new Vector3(end.x, end.y, 0), Vector3.one); // End point
            
        }

    }

}