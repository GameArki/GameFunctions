using System;

namespace GameClasses.CellBakerLib.Internal {

    public static class PositionFunctions {

        public static int GetIndex(int x, int y, int width) {
            return y * width + x;
        }

        public static (int x, int y) GetXY(int index, int width) {
            int y = index / width;
            int x = index % width;
            return (x, y);
        }

        public static int RandomDirection(Random rd, int width) {
            int up = -width;
            int down = width;
            int left = -1;
            int right = 1;
            Span<int> directions = stackalloc int[4] { up, down, left, right };
            int randomIndex = rd.Next(0, 4);
            return directions[randomIndex];
        }

        public static int GetFallbackIndexByDirection(int curCellIndex, int dir, int width, int height) {
            int curX, curY;
            (curX, curY) = GetXY(curCellIndex, width);
            if (dir == -width) {
                curY--;
            } else if (dir == width) {
                curY++;
            } else if (dir == -1) {
                curX--;
            } else if (dir == 1) {
                curX++;
            }
            curX = Math.Clamp(curX, 0, width - 1);
            curY = Math.Clamp(curY, 0, height - 1);
            return GetIndex(curX, curY, width);
        }

    }

}