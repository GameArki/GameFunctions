using System;
using UnityEngine;
using RD = System.Random;

namespace GameFunctions {

    public static class GFCellGenerator {

        public const int DIR_FROM_TOP = 0;
        public const int DIR_FROM_RIGHT = 1;
        public const int DIR_FROM_BOTTOM = 2;
        public const int DIR_FROM_LEFT = 3;
        public const int DRI_COUNT = 4;

        public static int[] NewCells(int width, int height) {
            return new int[width * height];
        }

        // cells[index] = seaValue
        public static bool Gen_Sea(int[] cells, RD random, int seaValue, int width, int seaCount, int DIR, int type = 1) {
            if (type == 1) {
                return Gen_Sea_Type1(cells, random, seaValue, width, seaCount, DIR);
            } else {
                throw new System.Exception("Unknown type: " + type);
            }
        }

        // implement by rewrite:
        // 0 0 0 0
        // 0 0 0 0
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
            if (DIR == DIR_FROM_TOP) {
                start_x = random.Next(width);
                start_y = height - 1;
            } else if (DIR == DIR_FROM_RIGHT) {
                start_x = width - 1;
                start_y = random.Next(height);
            } else if (DIR == DIR_FROM_BOTTOM) {
                start_x = random.Next(width);
                start_y = 0;
            } else if (DIR == DIR_FROM_LEFT) {
                start_x = 0;
                start_y = random.Next(height);
            } else {
                throw new System.Exception("Unknown DIR: " + DIR);
            }
            cells[GetIndex(start_x, start_y, width)] = seaValue;
            Sea_Rewrite(cells, random, seaValue, width, seaCount, DIR);
            return true;
        }

        static int Dir_Reverse(int DIR) {
            if (DIR == DIR_FROM_TOP) {
                return DIR_FROM_BOTTOM;
            } else if (DIR == DIR_FROM_RIGHT) {
                return DIR_FROM_LEFT;
            } else if (DIR == DIR_FROM_BOTTOM) {
                return DIR_FROM_TOP;
            } else if (DIR == DIR_FROM_LEFT) {
                return DIR_FROM_RIGHT;
            } else {
                throw new System.Exception("Unknown DIR: " + DIR);
            }
        }

        static void Dir_Prefer(int DIR, out int prefer1, out int prefer2) {
            if (DIR == DIR_FROM_TOP) {
                prefer1 = DIR_FROM_LEFT;
                prefer2 = DIR_FROM_RIGHT;
            } else if (DIR == DIR_FROM_RIGHT) {
                prefer1 = DIR_FROM_TOP;
                prefer2 = DIR_FROM_BOTTOM;
            } else if (DIR == DIR_FROM_BOTTOM) {
                prefer1 = DIR_FROM_RIGHT;
                prefer2 = DIR_FROM_LEFT;
            } else if (DIR == DIR_FROM_LEFT) {
                prefer1 = DIR_FROM_BOTTOM;
                prefer2 = DIR_FROM_TOP;
            } else {
                throw new System.Exception("Unknown DIR: " + DIR);
            }
        }

        static void Sea_Rewrite(int[] cells, RD random, int seaValue, int width, int seaCount, int DIR) {

            if (seaCount >= cells.Length) {
                throw new System.Exception("seaCount >= cells.Length");
            }

            int height = cells.Length / width;

            int r = Dir_Reverse(DIR);
            int d1, d2;
            Dir_Prefer(DIR, out d1, out d2);

            // DIR: 10% d1: 45%, d2: 45%, r: 0%
            Span<int> prefer = stackalloc int[100];
            for (int i = 0; i < 100; i += 1) {
                if (i < 10) {
                    prefer[i] = DIR;
                } else if (i < 55) {
                    prefer[i] = d1;
                } else if (i < 100) {
                    prefer[i] = d2;
                }
            }

            // Shuffle
            for (int i = 0; i < 100; i += 1) {
                int j = random.Next(100);
                int t = prefer[i];
                prefer[i] = prefer[j];
                prefer[j] = t;
            }

            while (seaCount > 0) {
                bool hasSea = false;
                for (int i = 0; i < cells.Length; i++) {
                    int x = i % width;
                    int y = i / width;
                    if (cells[i] == seaValue) {
                        if (DIR == DIR_FROM_TOP) {
                            // 0  =  1
                            // 1     1
                            if (y + 1 < height) {
                                cells[GetIndex(x, y + 1, width)] = seaValue;
                            }
                        } else if (DIR == DIR_FROM_RIGHT) {
                            // 1 0 = 1 1
                            if (x + 1 < width) {
                                cells[GetIndex(x + 1, y, width)] = seaValue;
                            }
                        } else if (DIR == DIR_FROM_BOTTOM) {
                            // 1     1
                            // 0  =  1
                            if (y - 1 >= 0) {
                                cells[GetIndex(x, y - 1, width)] = seaValue;
                            }
                        } else if (DIR == DIR_FROM_LEFT) {
                            // 0 1 = 1 1
                            if (x - 1 >= 0) {
                                cells[GetIndex(x - 1, y, width)] = seaValue;
                            }
                        } else {
                            throw new System.Exception("Unknown DIR: " + DIR);
                        }

                        int nextDir = prefer[random.Next(100)];
                        int nx = x;
                        int ny = y;
                        if (nextDir == DIR_FROM_TOP) {
                            ny -= 1;
                        } else if (nextDir == DIR_FROM_RIGHT) {
                            nx -= 1;
                        } else if (nextDir == DIR_FROM_BOTTOM) {
                            ny += 1;
                        } else if (nextDir == DIR_FROM_LEFT) {
                            nx += 1;
                        } else {
                            throw new System.Exception("Unknown DIR: " + nextDir);
                        }
                        if (nx < 0 || nx >= width || ny < 0 || ny >= height) {
                            continue;
                        }
                        int nextIndex = GetIndex(nx, ny, width);
                        if (cells[nextIndex] != seaValue) {
                            cells[nextIndex] = seaValue;
                            seaCount--;
                        }
                        hasSea = true;
                    }
                }
                if (!hasSea) {
                    throw new System.Exception("No sea");
                }
            }

        }

        static int GetIndex(int x, int y, int width) {
            return y * width + x;
        }

    }

}