using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using NUnit.Framework;

namespace GameFunctions.Tests {

    public class Algorithm_AStar_Tests {

        [Test]
        public void BenchMark([NUnit.Framework.Range(1, 10, 2)] int seed,
                              [NUnit.Framework.Range(0, 2000, 200)] int blockCount) {
            // Run benchmarks for both SIMD and non-SIMD implementations
            BenchMark_SIMD(seed, blockCount);
            // BenchMark_NoSIMD(seed, blockCount);
        }

        const int width = 256;
        const int height = 256;

        void BenchMark_SIMD(int seed, int blockCount) {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            // SIMD
            int2 start = new int2(0, 0);
            int2 end = new int2(255, 255);
            int2 edge = new int2(width, height);
            NativeArray<int2> blocks = new NativeArray<int2>(blockCount, Allocator.Temp);
            // Fill blocks with some random positions
            System.Random rd = new System.Random(seed);
            for (int i = 0; i < blockCount; i++) {
                blocks[i] = new int2(rd.Next(0, width), rd.Next(0, height));
            }
            // Sort blocks to ensure they are in a consistent order
            blocks.Sort(new BlocksOrder());
            sw.Start();
            int pathCount = Algorithm_AStar.Go_8Dir_SIMD(start, end, edge, blocks, blocks.Length, out var path);
            Debug.Log($"SIMD Path Count: {pathCount}, Time: {sw.Elapsed.TotalMilliseconds} ms");

            blocks.Dispose();
        }

        void BenchMark_NoSIMD(int seed, int blockCount) {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            // No SIMD
            Vector2Int start = new Vector2Int(0, 0);
            Vector2Int end = new Vector2Int(255, 255);
            Vector2Int edge = new Vector2Int(width, height);
            HashSet<Vector2Int> blocks = new HashSet<Vector2Int>(blockCount);
            // Fill blocks with some random positions
            System.Random rd = new System.Random(seed);
            for (int i = 0; i < blockCount; i++) {
                blocks.Add(new Vector2Int(rd.Next(0, width), rd.Next(0, height)));
            }
            sw.Start();
            int pathCount = Algorithm_AStar_NoSIMD.Go_8Dir(start, end, edge, blocks, out var path);
            Debug.Log($"No SIMD Path Count: {pathCount}, Time: {sw.Elapsed.TotalMilliseconds} ms");
        }

    }

}