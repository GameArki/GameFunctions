using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using RD = System.Random;

namespace GameFunctions {

    public static class GFGridGenerator {

        public const int DIR_TOP = 0;
        public const int DIR_RIGHT = 1;
        public const int DIR_BOTTOM = 2;
        public const int DIR_LEFT = 3;
        public const int DIR_COUNT = 4;

        [ThreadStatic] static HashSet<int> cells_land_grass_index_set;
        [ThreadStatic] static int[] cells_land_grass_indices;
        [ThreadStatic] static int[] cells_sea_indices;
        [ThreadStatic] static int cells_sea_count;

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
            cells_land_grass_index_set = new HashSet<int>(cells.Length);
            cells_land_grass_indices = new int[cells.Length];
            for (int i = 0; i < cells.Length; i++) {
                cells_land_grass_index_set.Add(i);
            }
            cells_land_grass_index_set.CopyTo(cells_land_grass_indices);

            // Cache: Sea
            if (cells_sea_indices == null || cells_sea_indices.Length < cells.Length) {
                cells_sea_indices = new int[cells.Length];
            }

            // Gen: Sea
            Gen_Sea(cells, rd, gridOption.width, gridOption.height, seaOption);

            // Cache Update: Land
            // remove sea cells from land cells
            for (int i = 0; i < cells_sea_count; i += 1) {
                int seaIndex = cells_sea_indices[i];
                cells_land_grass_index_set.Remove(seaIndex);
            }

            // Gen: Land - Lake
            Gen_Land_Lake(cells, rd, gridOption.width, gridOption.height, seaOption.seaValue, lakeOption);

            return cells;
        }

        // ==== Sea ====
        // cells[index] = seaValue
        public static void Gen_Sea(int[] cells, RD random, int width, int height, GFGenSeaOption option) {
            cells_sea_count = 0;
            option.DIR = Math.Abs(option.DIR) % DIR_COUNT;
            if (option.TYPE == GFGenSeaOption.TYPE_NORMAL) {
                Gen_Sea_Normal(cells, random, width, height, option);
            } else {
                Debug.LogWarning("Unknown type: " + option.TYPE);
                Gen_Sea_Normal(cells, random, width, height, option);
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
        static void Gen_Sea_Normal(int[] cells, RD random, int width, int height, GFGenSeaOption option) {

            int seaCount = option.seaCount;
            int seaValue = option.seaValue;

            if (seaCount >= cells.Length) {
                throw new System.Exception("seaCount >= cells.Length");
            }

            // Gen: start sea cell
            int start_x;
            int start_y;
            Pos_GetOnEdge(random, width, height, option.DIR, out start_x, out start_y);
            int startSeaIndex = Index_GetByPos(start_x, start_y, width);
            cells[startSeaIndex] = seaValue;
            seaCount--;
            cells_sea_indices[cells_sea_count++] = startSeaIndex;

            // Prepare: prefer direction
            int dir_from = option.DIR;
            int d0 = Dir_Reverse(dir_from);
            int d1, d2;
            Dir_Prefer(d0, out d1, out d2);

            // d0: 10% d1: 45%, d2: 45% = 100%
            // d0: 2%  d1: 9%,  d2: 9%  = 20%
            int d0Rate = 2;
            int d1Rate = 9;
            int d2Rate = 9;
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

            while (seaCount > 0) {
                for (int i = 0; i < cells_sea_count; i += 1) {
                    int seaIndex = cells_sea_indices[i];
                    int x = seaIndex % width;
                    int y = seaIndex / width;

                    // fill reverse direction
                    int reverseDirIndex = Index_GetByPos(x, y, width, height, dir_from);
                    if (reverseDirIndex != -1 && cells[reverseDirIndex] != seaValue) {
                        cells[reverseDirIndex] = seaValue;
                        cells_sea_indices[cells_sea_count++] = reverseDirIndex;
                        seaCount--;
                        continue;
                    }

                    // fill prefer direction
                    int nextDir = prefer[random.Next(preferCount)];
                    int nextDirIndex = Index_GetByPos(x, y, width, height, nextDir);
                    if (nextDirIndex != -1 && cells[nextDirIndex] != seaValue) {
                        cells[nextDirIndex] = seaValue;
                        cells_sea_indices[cells_sea_count++] = nextDirIndex;
                        seaCount--;
                        continue;
                    }
                }
            }

        }

        // ==== Land-Lake ====
        // cells[index] = landValue
        public static bool Gen_Land_Lake(int[] cells, RD random, int width, int height, int seaValue, GFGenLakeOption option) {
            if (option.TYPE == GFGenLakeOption.TYPE_NORMAL) {
                return Gen_Land_Lake_Normal(cells, random, width, height, seaValue, option);
            } else {
                Debug.LogWarning("Unknown type: " + option.TYPE);
                return Gen_Land_Lake_Normal(cells, random, width, height, seaValue, option);
            }
        }

        static bool Gen_Land_Lake_Normal(int[] cells, RD random, int width, int height, int seaValue, GFGenLakeOption option) {

            int failedTimes = 10000;

            int lakeCount = option.lakeCount;
            int lakeValue = option.lakeValue;
            int awayFromSea = option.awayFromSea;

            GFVector4Int edgeOffset = new GFVector4Int();
            int start_x = 0;
            int start_y = 0;
            do {
                failedTimes--;
                if (failedTimes <= 0) {
                    break;
                }
                start_x = random.Next(width);
                start_y = random.Next(height);
                int value = cells[Index_GetByPos(start_x, start_y, width)];
                if (value == seaValue) {
                    continue;
                }
                edgeOffset = Pos_DetectAwayFrom(cells, width, height, start_x, start_y, seaValue, awayFromSea);
            } while (edgeOffset.Min() < option.awayFromSea);

            if (failedTimes <= 0) {
                Debug.LogError("Gen_Land_Lake_Normal failed");
                return false;
            }

            cells[Index_GetByPos(start_x, start_y, width)] = lakeValue;

            return true;
        }

        // ==== Generics ====
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Dir_Reverse(int DIR) {
            return (DIR + 2) % DIR_COUNT;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Dir_Prefer(int DIR, out int prefer1, out int prefer2) {
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