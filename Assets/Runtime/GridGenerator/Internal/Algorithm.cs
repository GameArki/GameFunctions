using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using RD = System.Random;

namespace GameFunctions.GridGeneratorInternal {

    public static class Algorithm {

        public const int DIR_TOP = 0;
        public const int DIR_RIGHT = 1;
        public const int DIR_BOTTOM = 2;
        public const int DIR_LEFT = 3;
        public const int DIR_COUNT = 4;

        public static bool Alg_FillAll(int[] cells, int fillValue, Action<int> onFill) {
            for (int i = 0; i < cells.Length; i += 1) {
                if (cells[i] == fillValue) {
                    continue;
                }
                onFill.Invoke(i);
            }
            return true;
        }

        // 侵蚀算法: 0 1 = 1 1
        // 0 0 1 0 0 0      0 1 1 1 0 0     0 1 1 1 1 0
        // 0 0 0 0 0 0      0 0 1 0 0 0     0 0 1 1 0 0
        // 0 0 0 0 0 0  ==> 0 0 0 0 0 0 ==> 0 0 0 1 0 0
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 0 0 0
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 0 0 0
        public static bool Alg_Erode_Loop(int[] cells, int[] indices, HashSet<int> sets, RD random, int width, int height, int erodeCount, int erodeRate, int erodeValue, int fromDir, HashSet<int> basedOnValues, HashSet<int> awayFromValues, Action<int> onErode) {

            if (erodeCount >= cells.Length) {
                return false;
            }

            // Erode: prefer direction
            if (erodeRate <= 0) {
                erodeRate = 9;
                Debug.LogWarning($"erodeRate <= 0, use default: {erodeRate}");
            } else if (erodeRate >= 99) {
                erodeRate = 98;
                Debug.LogWarning($"erodeRate > 100, use default: {erodeRate}");
            }

            int failedTimes = width * height * 100;

            int dir_from = fromDir;
            int d0 = Dir_Reverse(dir_from);
            int d1, d2;
            Dir_ErodePrefer(d0, out d1, out d2);

            // d0 + d1 + d2 = 100
            int d0Rate = erodeRate;
            int d1Rate = (100 - erodeRate) / 2;
            int d2Rate = (100 - erodeRate) / 2;
            int preferCount = d0Rate + d1Rate + d2Rate;
            Span<int> prefer = stackalloc int[preferCount];
            for (int i = 0; i < preferCount; i += 1) {
                if (i < d0Rate) {
                    prefer[i] = d0;
                } else if (i < d0Rate + d1Rate) {
                    prefer[i] = d1;
                } else if (i < d0Rate + d1Rate + d2Rate) {
                    prefer[i] = d2;
                }
            }

            // Erode: Loop
            while (erodeCount > 0) {
                int sets_count = sets.Count;
                for (int i = 0; i < sets_count; ++i) {
                    int seaIndex = indices[i];
                    int x = seaIndex % width;
                    int y = seaIndex / width;

                    // fill reverse direction
                    int reverseDirIndex = Index_GetByPos(x, y, width, height, dir_from);
                    if (reverseDirIndex != -1 && cells[reverseDirIndex] != erodeValue) {
                        onErode.Invoke(reverseDirIndex);
                        --erodeCount;
                        continue;
                    }

                    // fill prefer direction
                    int nextDir = prefer[random.Next(preferCount)];
                    int nextDirIndex = Index_GetByPos(x, y, width, height, nextDir);
                    if (nextDirIndex != -1 && AllowGen(basedOnValues, awayFromValues, cells[nextDirIndex], erodeValue)) {
                        onErode.Invoke(nextDirIndex);
                        --erodeCount;
                        continue;
                    }
                }
                --failedTimes;
                if (failedTimes <= 0) {
                    Debug.LogError("Algorithm_Erode failed");
                    return false;
                }
            }
            return true;
        }

        // 描边算法: 0 0 1 = 0 2 1
        // 0 0 0 0 0 0          0 0 0 0 0 0
        // 0 1 1 1 1 0          0 2 2 2 2 0
        // 0 1 1 1 1 0   ==>    0 2 1 1 2 0
        // 0 1 1 1 1 0   ==>    0 2 1 1 2 0
        // 0 1 1 1 1 0          0 2 2 2 2 0
        // 0 0 0 0 0 0          0 0 0 0 0 0
        static bool Alg_Outline(int[] cells, RD random, int width, int height, int outlineCount, int outlineValue, int outlineFromDir, Action<int> onOutline) {
            throw new NotImplementedException();
        }

        // 洪泛算法: 0 1 0 = 0 1 1
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 1 0 0
        // 0 0 0 0 0 0      0 0 0 1 0 0     0 0 1 1 1 0
        // 0 0 0 1 0 0  ==> 0 0 0 1 1 0 ==> 0 0 1 1 1 0
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 1 0 0
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 0 0 0
        public static bool Alg_Flood_Loop(int[] cells, int[] indices, HashSet<int> sets, RD random, int width, int height, int floodCount, int floodValue, HashSet<int> basedOnValues, HashSet<int> awayFromValues, Action<int> onFlood) {

            int failedTimes = width * height * 100;

            Span<int> prefer = stackalloc int[DIR_COUNT];
            for (int i = 0; i < DIR_COUNT; i += 1) {
                prefer[i] = i;
            }

            while (floodCount > 0) {
                int sets_count = sets.Count;
                for (int i = 0; i < sets_count; i += 1) {
                    int index = indices[i];
                    int x = index % width;
                    int y = index / width;

                    // fill prefer direction
                    int nextDir = prefer[random.Next(DIR_COUNT)];
                    int nextDirIndex = Index_GetByPos(x, y, width, height, nextDir);
                    if (nextDirIndex != -1 && AllowGen(basedOnValues, awayFromValues, cells[nextDirIndex], floodValue)) {
                        onFlood.Invoke(nextDirIndex);
                        --floodCount;
                        continue;
                    }
                }
                --failedTimes;
                if (failedTimes <= 0) {
                    Debug.LogError("Algorithm_Flood failed");
                    return false;
                }
            }
            return true;
        }

        // 播种算法(Scatter):   0100    = 0101
        public static bool Alg_Scatter_Loop(int[] cells, int[] indices, HashSet<int> sets, RD random, int width, int height, int scatterCount, int scatterValue, Vector2Int scatterMinMax, HashSet<int> basedOnValues, HashSet<int> awayFromValues, Action<int> onScatter) {

            int failedTimes = width * height * 100;

            int lastCout = 0;
            while (scatterCount > 0) {

                int sets_count = sets.Count;
                for (int i = lastCout; i < sets_count; i += 1) {
                    int index = indices[i];
                    int x = index % width;
                    int y = index / width;


                    // fill random direction
                    int step = random.Next(scatterMinMax.x, scatterMinMax.y + 1);
                    int nextDir = random.Next(DIR_COUNT);
                    int nextDirIndex = Index_GetByPosStep(x, y, width, height, step, nextDir);
                    if (nextDirIndex != -1 && AllowGen(basedOnValues, awayFromValues, cells[nextDirIndex], scatterValue)) {
                        onScatter.Invoke(nextDirIndex);
                        --scatterCount;
                        continue;
                    }
                }
                lastCout = sets.Count - 1;

                --failedTimes;
                if (failedTimes <= 0) {
                    Debug.LogError("Algorithm_Scatter failed");
                    return false;
                }

            }

            return true;
        }

        static bool AllowGen(HashSet<int> basedOnValues, HashSet<int> awayFromValues, int value, int targetValue) {
            if (basedOnValues != null && !basedOnValues.Contains(value)) {
                return false;
            }
            if (awayFromValues != null && awayFromValues.Contains(value)) {
                return false;
            }
            if (value == targetValue) {
                return false;
            }
            return true;
        }

        // ==== Generics ====
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Dir_Reverse(int DIR) {
            return (DIR + 2) % DIR_COUNT;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dir_ErodePrefer(int DIR, out int prefer1, out int prefer2) {
            prefer1 = (DIR + 1) % DIR_COUNT;
            prefer2 = (DIR + 3) % DIR_COUNT;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index_GetByPos(int x, int y, int width, int height, int DIR) {
            if (DIR == DIR_TOP) {
                y += 1;
            } else if (DIR == DIR_RIGHT) {
                x += 1;
            } else if (DIR == DIR_BOTTOM) {
                y -= 1;
            } else if (DIR == DIR_LEFT) {
                x -= 1;
            } else {
                throw new System.Exception("Unknown DIR: " + DIR);
            }
            if (x < 0 || x >= width || y < 0 || y >= height) {
                return -1;
            }
            return Index_GetByPos(x, y, width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index_GetByPosStep(int x, int y, int width, int height, int step, int DIR) {
            if (DIR == DIR_TOP) {
                y += step;
            } else if (DIR == DIR_RIGHT) {
                x += step;
            } else if (DIR == DIR_BOTTOM) {
                y -= step;
            } else if (DIR == DIR_LEFT) {
                x -= step;
            } else {
                throw new System.Exception("Unknown DIR: " + DIR);
            }
            if (x < 0 || x >= width || y < 0 || y >= height) {
                return -1;
            }
            return Index_GetByPos(x, y, width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index_GetByPos(int x, int y, int width) {
            int index = y * width + x;
            if (index < 0 || index >= width * width) {
                return -1;
            }
            return index;
        }

        public static void Pos_GetRandomPointOnEdge(RD random, int width, int height, int DIR, out int x, out int y) {
            if (DIR == DIR_TOP) {
                x = random.Next(width);
                y = height - 1;
            } else if (DIR == DIR_RIGHT) {
                x = width - 1;
                y = random.Next(height);
            } else if (DIR == DIR_BOTTOM) {
                x = random.Next(width);
                y = 0;
            } else if (DIR == DIR_LEFT) {
                x = 0;
                y = random.Next(height);
            } else {
                throw new System.Exception("Unknown DIR: " + DIR);
            }
        }
    }
}