using System;
using System.Runtime.InteropServices;

#pragma warning disable 0660
#pragma warning disable 0661

namespace GameClasses.RuleTileDrawer.Internal {

    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Vector2HalfSByte : IEquatable<Vector2HalfSByte> {

        // | 0000 | 0000 |
        // |  y   |  x   |
        [FieldOffset(0)]
        public ushort val;

        // -128 ~ 127
        [FieldOffset(0)]
        public sbyte x;

        // -128 ~ 127
        [FieldOffset(1)]
        public sbyte y;

        public Vector2HalfSByte(sbyte x, sbyte y) {
            val = 0;
            this.x = x;
            this.y = y;
        }

        public void SetX(int x) {
            this.x = (sbyte)x;
        }

        public void SetY(int y) {
            this.y = (sbyte)y;
        }

        public int ToPos() {

            // 最大支持7*7的Tile

            // -3,-3 -> 0, 左下角往上0行
            // -2,-3 -> 1
            // -1,-3 -> 2
            //  0,-3 -> 3
            //  1,-3 -> 4
            //  2,-3 -> 5
            //  3,-3 -> 6, 右下角往上0行

            //  -3,-2 -> 7, 左下角往上1行
            //  -2,-2 -> 8
            //  -1,-2 -> 9
            //   0,-2 -> 10
            //   1,-2 -> 11
            //   2,-2 -> 12
            //   3,-2 -> 13, 右下角往上1行

            //  -3,-1 -> 14, 左下角往上2行
            //  -2,-1 -> 15
            //  -1,-1 -> 16
            //   0,-1 -> 17
            //   1,-1 -> 18
            //   2,-1 -> 19
            //   3,-1 -> 20, 右下角往上2行

            //  -3, 0 -> 21, 左下角往上3行
            //  -2, 0 -> 22
            //  -1, 0 -> 23
            //   0, 0 -> 24
            //   1, 0 -> 25
            //   2, 0 -> 26
            //   3, 0 -> 27, 右下角往上3行

            //  -3, 1 -> 28, 左下角往上4行
            //  -2, 1 -> 29
            //  -1, 1 -> 30
            //   0, 1 -> 31
            //   1, 1 -> 32
            //   2, 1 -> 33
            //   3, 1 -> 34, 右下角往上4行

            //  -3, 2 -> 35, 左下角往上5行
            //  -2, 2 -> 36
            //  -1, 2 -> 37
            //   0, 2 -> 38
            //   1, 2 -> 39
            //   2, 2 -> 40
            //   3, 2 -> 41, 右下角往上5行

            //  -3, 3 -> 42, 左下角往上6行
            //  -2, 3 -> 43
            //  -1, 3 -> 44
            //   0, 3 -> 45
            //   1, 3 -> 46
            //   2, 3 -> 47
            //   3, 3 -> 48, 右下角往上6行

            return ((y + 3) * 7 + (x + 3)) * 2;

        }

        bool IEquatable<Vector2HalfSByte>.Equals(Vector2HalfSByte other) {
            return val == other.val;
        }

        public static bool operator ==(Vector2HalfSByte a, Vector2HalfSByte b) {
            return a.val == b.val;
        }

        public static bool operator !=(Vector2HalfSByte a, Vector2HalfSByte b) {
            return a.val != b.val;
        }

    }

}