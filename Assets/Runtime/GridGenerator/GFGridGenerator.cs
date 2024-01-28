using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using RD = System.Random;

// Rewrite 算法:
// 侵蚀算法(Erode)
// 描边算法(Outline)
// 洪泛算法(Flood)
// 播种算法(Scatter)
// 平滑算法(Smooth)
// 河流算法(River)
// 点缀算法(Decorate)
namespace GameFunctions {

    public static class GFGridGenerator {

        public const int DIR_TOP = 0;
        public const int DIR_RIGHT = 1;
        public const int DIR_BOTTOM = 2;
        public const int DIR_LEFT = 3;
        public const int DIR_COUNT = 4;

        [ThreadStatic] static HashSet<int> CELLS_land_grass_index_set;
        [ThreadStatic] static int[] CELLS_land_grass_indices;

        [ThreadStatic] static int[] CELLS_land_lake_indices;
        [ThreadStatic] static int CELLS_land_lake_count;

        [ThreadStatic] static int[] CELLS_sea_indices;
        [ThreadStatic] static int CELLS_sea_count;

        // ==== Land Init ====
        public static int[] GenAll(GFGenGridOption gridOption, GFGenSeaOption seaOption, GFGenLakeOption lakeOption) {

            // Random
            RD rd = new RD(gridOption.seed);
            for (int i = 0; i < gridOption.seedTimes; i++) {
                rd.Next();
            }

            int[] cells = new int[gridOption.width * gridOption.height];

            // Gen: Land(default)
            for (int i = 0; i < cells.Length; i++) {
                cells[i] = gridOption.defaultLandValue;
            }

            // Cache: Land(default) 
            CELLS_land_grass_index_set = new HashSet<int>(cells.Length);
            CELLS_land_grass_indices = new int[cells.Length];
            for (int i = 0; i < cells.Length; i++) {
                CELLS_land_grass_index_set.Add(i);
            }
            CELLS_land_grass_index_set.CopyTo(CELLS_land_grass_indices);

            // Cache: Land - Lake
            if (CELLS_land_lake_indices == null || CELLS_land_lake_indices.Length < cells.Length) {
                CELLS_land_lake_indices = new int[cells.Length];
            }

            // Cache: Sea
            if (CELLS_sea_indices == null || CELLS_sea_indices.Length < cells.Length) {
                CELLS_sea_indices = new int[cells.Length];
            }

            // Gen: Sea
            bool succ_sea = Gen_Sea(cells, rd, gridOption.width, gridOption.height, seaOption);
            if (!succ_sea) {
                Debug.LogError("Gen_Sea failed");
            }

            // Cache Update: Land
            // remove sea cells from land cells
            for (int i = 0; i < CELLS_sea_count; i += 1) {
                int seaIndex = CELLS_sea_indices[i];
                CELLS_land_grass_index_set.Remove(seaIndex);
            }

            // Gen: Land - Lake
            bool succ_lake = Gen_Land_Lake(cells, rd, gridOption.width, gridOption.height, seaOption.seaValue, lakeOption);
            if (!succ_lake) {
                Debug.LogError("Gen_Land_Lake failed");
            }

            return cells;

        }

        // ==== Sea ====
        // cells[index] = seaValue
        public static bool Gen_Sea(int[] cells, RD random, int width, int height, GFGenSeaOption option) {
            CELLS_sea_count = 0;
            option.DIR = Math.Abs(option.DIR) % DIR_COUNT;
            if (option.TYPE == GFGenSeaOption.TYPE_NORMAL) {
                return Gen_Sea_Normal(cells, random, width, height, option);
            } else if (option.TYPE == GFGenSeaOption.TYPE_SHARP) {
                return Gen_Sea_Normal(cells, random, width, height, option);
            } else {
                Debug.LogWarning("Unknown type: " + option.TYPE);
                return Gen_Sea_Normal(cells, random, width, height, option);
            }
        }

        // implement by rewrite:
        // 0 0 0 0
        // 0 1 0 0
        // 0 0 0 0
        // 0 0 0 0
        // 1. Find a random 0 => 1
        // or: 0 1 = 1 1
        // or: 0   = 1
        //     1     1
        // or: 1 0 = 1 1
        // or: 1   = 1
        //     0     1
        static bool Gen_Sea_Normal(int[] cells, RD random, int width, int height, GFGenSeaOption option) {

            int seaCount = option.seaCount;
            int seaValue = option.seaValue;

            if (seaCount >= cells.Length) {
                Debug.LogError("seaCount >= cells.Length");
                return false;
            }

            // Gen: start sea cell
            int start_x;
            int start_y;
            Pos_GetOnEdge(random, width, height, option.DIR, out start_x, out start_y);
            int startSeaIndex = Index_GetByPos(start_x, start_y, width);
            cells[startSeaIndex] = seaValue;
            seaCount--;
            CELLS_sea_indices[CELLS_sea_count++] = startSeaIndex;

            // Erode: sea
            return Algorithm_Erode(cells, CELLS_sea_indices, ref CELLS_sea_count, random, width, height, seaCount, option.erodeRate, seaValue, option.DIR, (int index) => {
                cells[index] = seaValue;
                CELLS_sea_indices[CELLS_sea_count++] = index;
            });

        }

        // ==== Land-Lake ====
        // cells[index] = landValue
        public static bool Gen_Land_Lake(int[] cells, RD random, int width, int height, int seaValue, GFGenLakeOption option) {
            CELLS_land_lake_count = 0;
            if (option.TYPE == GFGenLakeOption.TYPE_FLOOD) {
                return Gen_Land_Lake_Normal(cells, random, width, height, seaValue, option);
            } else {
                Debug.LogWarning("Unknown type: " + option.TYPE);
                return Gen_Land_Lake_Normal(cells, random, width, height, seaValue, option);
            }
        }

        static bool Gen_Land_Lake_Normal(int[] cells, RD random, int width, int height, int seaValue, GFGenLakeOption option) {

            int failedTimes = width * height * 10;

            int lakeCount = option.lakeCount;
            int lakeValue = option.lakeValue;
            int awayFromSea = option.awayFromSea;

            // Gen: start lake cell
            GFVector4Int edgeOffset = new GFVector4Int();
            int start_x = 0;
            int start_y = 0;
            int start_index = 0;
            do {
                failedTimes--;
                if (failedTimes <= 0) {
                    break;
                }
                start_x = random.Next(width);
                start_y = random.Next(height);
                start_index = Index_GetByPos(start_x, start_y, width);
                int value = cells[start_index];
                if (value == seaValue) {
                    continue;
                }
                edgeOffset = Pos_DetectAwayFrom(cells, width, height, start_x, start_y, seaValue, awayFromSea);
            } while (edgeOffset.Min() < option.awayFromSea);

            if (failedTimes <= 0) {
                Debug.LogError("Gen_Land_Lake_Normal failed");
                return false;
            }

            cells[start_index] = lakeValue;
            CELLS_land_lake_indices[CELLS_land_lake_count++] = start_index;

            // Flood: lake
            return Algorithm_Flood(cells, CELLS_land_lake_indices, ref CELLS_land_lake_count, random, width, height, lakeCount, lakeValue, (int index) => {
                cells[index] = lakeValue;
                CELLS_land_lake_indices[CELLS_land_lake_count++] = index;
            });

        }

        // ==== Algorithm ====
        // 侵蚀算法: 0 1 = 1 1
        // 0 0 1 0 0 0      0 1 1 1 0 0     0 1 1 1 1 0
        // 0 0 0 0 0 0      0 0 1 0 0 0     0 0 1 1 0 0
        // 0 0 0 0 0 0  ==> 0 0 0 0 0 0 ==> 0 0 0 1 0 0
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 0 0 0
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 0 0 0
        static bool Algorithm_Erode(int[] cells, int[] cells_value_index, ref int cells_value_count, RD random, int width, int height, int erodeCount, int erodeRate, int erodeValue, int erodeFromDir, Action<int> onErode) {

            if (erodeRate <= 0) {
                erodeRate = 9;
                Debug.LogWarning($"erodeRate <= 0, use default: {erodeRate}");
            } else if (erodeRate >= 99) {
                erodeRate = 98;
                Debug.LogWarning($"erodeRate > 100, use default: {erodeRate}");
            }

            int failedTimes = width * height * 100;

            // Prepare: prefer direction
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

            while (erodeCount > 0) {
                for (int i = 0; i < cells_value_count; ++i) {
                    int seaIndex = cells_value_index[i];
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
        static bool Algorithm_Outline(int[] cells, RD random, int width, int height, int outlineCount, int outlineValue, int outlineFromDir, Action<int> onOutline) {
            throw new NotImplementedException();
        }

        // 洪泛算法: 0 1 0 = 0 1 1
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 1 0 0
        // 0 0 0 0 0 0      0 0 0 1 0 0     0 0 1 1 1 0
        // 0 0 0 1 0 0  ==> 0 0 0 1 1 0 ==> 0 0 1 1 1 0
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 1 0 0
        // 0 0 0 0 0 0      0 0 0 0 0 0     0 0 0 0 0 0
        static bool Algorithm_Flood(int[] cells, int[] cells_value_indices, ref int cells_value_count, RD random, int width, int height, int floodCount, int floodValue, Action<int> onFlood) {

            int failedTimes = width * height * 100;

            Span<int> prefer = stackalloc int[DIR_COUNT];
            for (int i = 0; i < DIR_COUNT; i += 1) {
                prefer[i] = i;
            }

            while (floodCount > 0) {
                for (int i = 0; i < cells_value_count; i += 1) {
                    int seaIndex = cells_value_indices[i];
                    int x = seaIndex % width;
                    int y = seaIndex / width;

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
        static int Dir_Reverse(int DIR) {
            return (DIR + 2) % DIR_COUNT;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Dir_ErodePrefer(int DIR, out int prefer1, out int prefer2) {
            prefer1 = (DIR + 1) % DIR_COUNT;
            prefer2 = (DIR + 3) % DIR_COUNT;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Index_GetByPos(int x, int y, int width, int height, int DIR) {
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
        static int Index_GetByPos(int x, int y, int width) {
            int index = y * width + x;
            if (index < 0 || index >= width * width) {
                return -1;
            }
            return index;
        }

        static GFVector4Int Pos_DetectAwayFrom(int[] cells, int width, int height, int x, int y, int awayValue, int awayFromSize) {
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
                    if (upIndex != -1 && cells[upIndex] == awayValue) {
                        edgeOffset.x = i;
                        findTop = true;
                    }
                }

                if (!findRight) {
                    int rightIndex = Index_GetByPos(x + i, y, width);
                    if (rightIndex != -1 && cells[rightIndex] == awayValue) {
                        edgeOffset.y = i;
                        findRight = true;
                    }
                }

                if (!findBottom) {
                    int downIndex = Index_GetByPos(x, y - i, width);
                    if (downIndex != -1 && cells[downIndex] == awayValue) {
                        edgeOffset.w = i;
                        findBottom = true;
                    }
                }

                if (!findLeft) {
                    int leftIndex = Index_GetByPos(x - i, y, width);
                    if (leftIndex != -1 && cells[leftIndex] == awayValue) {
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

        static void Pos_GetOnEdge(RD random, int width, int height, int DIR, out int x, out int y) {
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