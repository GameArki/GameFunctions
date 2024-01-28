using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using RD = System.Random;

namespace GameFunctions {

    public static class GFCellGenerator {

        public const int DIR_TOP = 0;
        public const int DIR_RIGHT = 1;
        public const int DIR_BOTTOM = 2;
        public const int DIR_LEFT = 3;

        public const int TYPE_SEA_NORMAL = 1;

        [ThreadStatic] static int[] cells_sea_indices;
        [ThreadStatic] static int seaIndexExistCount;

        // ==== Land ====
        public static int[] NewCells(int width, int height, int landValue) {
            int[] cells = new int[width * height];
            for (int i = 0; i < cells.Length; i++) {
                cells[i] = landValue;
            }
            if (cells_sea_indices == null || cells_sea_indices.Length < cells.Length) {
                cells_sea_indices = new int[cells.Length];
                seaIndexExistCount = 0;
            }
            return cells;
        }

        // ==== Sea ====
        // cells[index] = seaValue
        public static bool Gen_Sea(int[] cells, RD random, int seaValue, int width, int seaCount, int DIR, int TYPE = TYPE_SEA_NORMAL) {
            seaIndexExistCount = 0;
            if (TYPE == 1) {
                return Gen_Sea_Type1(cells, random, seaValue, width, seaCount, DIR);
            } else {
                throw new System.Exception("Unknown type: " + TYPE);
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
        static bool Gen_Sea_Type1(int[] cells, RD random, int seaValue, int width, int seaCount, int DIR) {
            // cells is a 2D array, but we use 1D array to store it
            int height = cells.Length / width;

            // Gen sea cells
            int start_x;
            int start_y;
            if (DIR == DIR_TOP) {
                start_x = random.Next(width);
                start_y = height - 1;
            } else if (DIR == DIR_RIGHT) {
                start_x = width - 1;
                start_y = random.Next(height);
            } else if (DIR == DIR_BOTTOM) {
                start_x = random.Next(width);
                start_y = 0;
            } else if (DIR == DIR_LEFT) {
                start_x = 0;
                start_y = random.Next(height);
            } else {
                throw new System.Exception("Unknown DIR: " + DIR);
            }
            int seaIndex = GetIndex(start_x, start_y, width);
            cells[seaIndex] = seaValue;
            cells_sea_indices[seaIndexExistCount++] = seaIndex;
            Sea_Rewrite(cells, random, seaValue, width, seaCount - 1, DIR);
            return true;
        }

        static void Sea_Rewrite(int[] cells, RD random, int seaValue, int width, int seaCount, int DIR_FROM) {

            if (seaCount >= cells.Length || seaIndexExistCount <= 0) {
                throw new System.Exception("seaCount >= cells.Length");
            }

            int height = cells.Length / width;

            int d0 = Dir_Reverse(DIR_FROM);
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
                for (int i = 0; i < seaIndexExistCount; i += 1) {
                    int seaIndex = cells_sea_indices[i];
                    int x = seaIndex % width;
                    int y = seaIndex / width;

                    // fill reverse direction
                    int reverseDirIndex = GetIndexByDir(x, y, width, height, DIR_FROM);
                    if (reverseDirIndex != -1 && cells[reverseDirIndex] != seaValue) {
                        cells[reverseDirIndex] = seaValue;
                        cells_sea_indices[seaIndexExistCount++] = reverseDirIndex;
                        seaCount--;
                        continue;
                    }

                    // fill prefer direction
                    int nextDir = prefer[random.Next(preferCount)];
                    int nextDirIndex = GetIndexByDir(x, y, width, height, nextDir);
                    if (nextDirIndex != -1 && cells[nextDirIndex] != seaValue) {
                        cells[nextDirIndex] = seaValue;
                        cells_sea_indices[seaIndexExistCount++] = nextDirIndex;
                        seaCount--;
                        continue;
                    }
                }
            }

        }

        // ==== Generics ====
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
        static int GetIndexByDir(int x, int y, int width, int height, int DIR) {
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
            return GetIndex(x, y, width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int GetIndex(int x, int y, int width) {
            return y * width + x;
        }

    }

}