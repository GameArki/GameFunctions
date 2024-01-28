using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using RD = System.Random;

namespace GameFunctions {

    public static class GFCellGenerator {

        public const int DIR_TOP = 0;
        public const int DIR_RIGHT = 1;
        public const int DIR_BOTTOM = 2;
        public const int DIR_LEFT = 3;
        public const int DIR_COUNT = 4;

        public const int TYPE_LAND_LAKE_NORMAL = 1;

        [ThreadStatic] static HashSet<int> cells_land_grass_index_set;
        [ThreadStatic] static int[] cells_land_grass_indices;
        [ThreadStatic] static int[] cells_sea_indices;
        [ThreadStatic] static int cells_sea_count;

        // ==== Land Init ====
        public static int[] GenAll(GFGenCellOption cellOption, GFGenSeaOption seaOption) {

            RD rd = new RD(cellOption.seed);
            for (int i = 0; i < cellOption.seedTimes; i++) {
                rd.Next();
            }

            int[] cells = new int[cellOption.width * cellOption.height];
            cells_land_grass_index_set = new HashSet<int>(cells.Length);
            cells_land_grass_indices = new int[cells.Length];
            for (int i = 0; i < cells.Length; i++) {
                cells[i] = cellOption.defaultLandValue;
                cells_land_grass_index_set.Add(i);
            }

            cells_land_grass_index_set.CopyTo(cells_land_grass_indices);
            if (cells_sea_indices == null || cells_sea_indices.Length < cells.Length) {
                cells_sea_indices = new int[cells.Length];
                cells_sea_count = 0;
            }

            // Sea
            Gen_Sea(cells, rd, cellOption.width, seaOption);

            return cells;
        }

        // ==== Sea ====
        // cells[index] = seaValue
        public static bool Gen_Sea(int[] cells, RD random, int width, GFGenSeaOption option) {
            cells_sea_count = 0;
            bool succ = false;
            option.DIR = Math.Abs(option.DIR) % DIR_COUNT;
            if (option.TYPE == 1) {
                succ = Gen_Sea_Type1(cells, random, width, option);
            } else {
                succ = Gen_Sea_Type1(cells, random, width, option);
                Debug.LogError("Unknown type: " + option.TYPE);
            }

            // remove sea cells from land cells
            for (int i = 0; i < cells_sea_count; i += 1) {
                int seaIndex = cells_sea_indices[i];
                cells_land_grass_index_set.Remove(seaIndex);
            }
            return succ;
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
        static bool Gen_Sea_Type1(int[] cells, RD random, int width, GFGenSeaOption option) {
            // cells is a 2D array, but we use 1D array to store it
            int height = cells.Length / width;

            // Gen sea cells
            int start_x;
            int start_y;
            int fromDir = option.DIR;
            if (fromDir == DIR_TOP) {
                start_x = random.Next(width);
                start_y = height - 1;
            } else if (fromDir == DIR_RIGHT) {
                start_x = width - 1;
                start_y = random.Next(height);
            } else if (fromDir == DIR_BOTTOM) {
                start_x = random.Next(width);
                start_y = 0;
            } else if (fromDir == DIR_LEFT) {
                start_x = 0;
                start_y = random.Next(height);
            } else {
                throw new System.Exception("Unknown DIR: " + fromDir);
            }
            int seaIndex = Index_GetByPos(start_x, start_y, width);
            cells[seaIndex] = option.seaValue;
            option.seaCount--;
            cells_sea_indices[cells_sea_count++] = seaIndex;
            Gen_Sea_Rewrite(cells, random, width, option);
            return true;
        }

        static void Gen_Sea_Rewrite(int[] cells, RD random, int width, GFGenSeaOption option) {

            int seaCount = option.seaCount;
            if (seaCount >= cells.Length || cells_sea_count <= 0) {
                throw new System.Exception("seaCount >= cells.Length");
            }

            int seaValue = option.seaValue;

            int height = cells.Length / width;

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
        public static bool Gen_Land_Lake(int[] cells, RD random, int landValue, int width, int lakeCount, int preferRadius, int TYPE = TYPE_LAND_LAKE_NORMAL) {
            throw new System.NotImplementedException();
        }

        // ==== Generics ====
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Dir_Reverse(int DIR) {
            if (DIR == DIR_TOP) {
                return DIR_BOTTOM;
            } else if (DIR == DIR_RIGHT) {
                return DIR_LEFT;
            } else if (DIR == DIR_BOTTOM) {
                return DIR_TOP;
            } else if (DIR == DIR_LEFT) {
                return DIR_RIGHT;
            } else {
                throw new System.Exception("Unknown DIR: " + DIR);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Dir_Prefer(int DIR, out int prefer1, out int prefer2) {
            if (DIR == DIR_TOP) {
                prefer1 = DIR_LEFT;
                prefer2 = DIR_RIGHT;
            } else if (DIR == DIR_RIGHT) {
                prefer1 = DIR_TOP;
                prefer2 = DIR_BOTTOM;
            } else if (DIR == DIR_BOTTOM) {
                prefer1 = DIR_RIGHT;
                prefer2 = DIR_LEFT;
            } else if (DIR == DIR_LEFT) {
                prefer1 = DIR_BOTTOM;
                prefer2 = DIR_TOP;
            } else {
                throw new System.Exception("Unknown DIR: " + DIR);
            }
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
            return y * width + x;
        }

    }

}