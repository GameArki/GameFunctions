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

        // ==== Algorithm ====
        // 侵蚀算法: 0 1 = 1 1
        // 0 0 1 0 0 0      0 1 1 1 0 0     0 1 1 1 1 0
        // 0 0 0 0 0 0      0 0 1 0 0 0     0 0 1 1 0 0
        // 0 0 0 0 0 0  ==> 0 0 0 0 0 0 ==> 0 0 0 1 0 0
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 0 0 0
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 0 0 0
        public static bool Alg_Erode(int[] cells, int[] erode_value_indices, HashSet<int> erode_index_set, RD random, int width, int height, int erodeCount, int erodeRate, int erodeValue, int erodeFromDir, Action<int> onErode) {

            if (erodeCount >= cells.Length) {
                return false;
            }

            // Erode: start cell
            int value = erodeValue;
            Algorithm.Pos_GetRandomPointOnEdge(random, width, height, erodeFromDir, out int start_x, out int start_y);
            int startIndex = Algorithm.Index_GetByPos(start_x, start_y, width);
            onErode.Invoke(startIndex);
            --erodeCount;

            // Erode: prefer direction
            if (erodeRate <= 0) {
                erodeRate = 9;
                Debug.LogWarning($"erodeRate <= 0, use default: {erodeRate}");
            } else if (erodeRate >= 99) {
                erodeRate = 98;
                Debug.LogWarning($"erodeRate > 100, use default: {erodeRate}");
            }

            int failedTimes = width * height * 100;

            int dir_from = erodeFromDir;
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
                int count = erode_index_set.Count;
                for (int i = 0; i < count; ++i) {
                    int seaIndex = erode_value_indices[i];
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
                    if (nextDirIndex != -1 && cells[nextDirIndex] != erodeValue) {
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
        public static bool Alg_Flood(int[] cells, int[] cells_value_indices, HashSet<int> cells_index_set, RD random, int width, int height, int floodCount, int floodValue, Action<int> onFlood) {

            int failedTimes = width * height * 100;

            Span<int> prefer = stackalloc int[DIR_COUNT];
            for (int i = 0; i < DIR_COUNT; i += 1) {
                prefer[i] = i;
            }

            while (floodCount > 0) {
                int count = cells_index_set.Count;
                for (int i = 0; i < count; i += 1) {
                    int index = cells_value_indices[i];
                    int x = index % width;
                    int y = index / width;

                    // fill prefer direction
                    int nextDir = prefer[random.Next(DIR_COUNT)];
                    int nextDirIndex = Index_GetByPos(x, y, width, height, nextDir);
                    if (nextDirIndex != -1 && cells[nextDirIndex] != floodValue) {
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
        public static int Index_GetByPos(int x, int y, int width) {
            int index = y * width + x;
            if (index < 0 || index >= width * width) {
                return -1;
            }
            return index;
        }

        public static GFVector4Int Pos_DetectAwayFrom(int[] cells, int width, int height, int x, int y, HashSet<int> awayValues, int awayFromSize) {
            // up down left right, walk awayFromSize steps
            // if found awayValue, return awayFromSize
            bool findLeft, findRight, findTop, findBottom;
            findTop = false; // x
            findRight = false; // y
            findBottom = false; // w
            findLeft = false; // z
            GFVector4Int edgeOffset = new GFVector4Int();
            for (int i = 0; i <= awayFromSize; i += 1) {
                if (!findTop) {
                    int upIndex = Index_GetByPos(x, y + i, width);
                    if (upIndex != -1 && awayValues.Contains(cells[upIndex])) {
                        edgeOffset.x = i;
                        findTop = true;
                    }
                }

                if (!findRight) {
                    int rightIndex = Index_GetByPos(x + i, y, width);
                    if (rightIndex != -1 && awayValues.Contains(cells[rightIndex])) {
                        edgeOffset.y = i;
                        findRight = true;
                    }
                }

                if (!findBottom) {
                    int downIndex = Index_GetByPos(x, y - i, width);
                    if (downIndex != -1 && awayValues.Contains(cells[downIndex])) {
                        edgeOffset.w = i;
                        findBottom = true;
                    }
                }

                if (!findLeft) {
                    int leftIndex = Index_GetByPos(x - i, y, width);
                    if (leftIndex != -1 && awayValues.Contains(cells[leftIndex])) {
                        edgeOffset.z = i;
                        findLeft = true;
                    }
                }

                if (findTop && findRight && findBottom && findLeft) {
                    break;
                }
            }
            if (!findTop) {
                edgeOffset.x = awayFromSize;
            }
            if (!findRight) {
                edgeOffset.y = awayFromSize;
            }
            if (!findBottom) {
                edgeOffset.w = awayFromSize;
            }
            if (!findLeft) {
                edgeOffset.z = awayFromSize;
            }
            return edgeOffset;
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