using System;
using UnityEngine;
using RD = System.Random;

namespace GameFunctions {

    public static class GFCellGenerator {

        public static int[] NewCells(int width, int height) {
            return new int[width * height];
        }

        public static bool Gen_Sea(int[] cells, RD random, int width, int count, int paddingLeft, int paddingRight, int paddingUp, int paddingDown, int type) {
            // cells is a 2D array, but we use 1D array to store it
            int height = cells.Length / width;
            return true;
        }

        static int GetIndex(int x, int y, int width) {
            return y * width + x;
        }

    }

}