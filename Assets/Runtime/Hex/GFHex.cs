using System;
using UnityEngine;

namespace GameFunctions {

    public static class GFHex {

        // x
        // y
        // z = -x - y
        public static int Distance(Vector2Int a, Vector2Int b) {
            int az = -a.x - a.y;
            int bz = -b.x - b.y;
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            int dz = Mathf.Abs(az - bz);
            return Mathf.Max(dx, dy, dz);
        }

        #region Neighbor
        public const int Neighbor_Count = 6;
        /// <summary> 6 Neighbors, Clockwise from left-up </summary>
        public static int Neighbors(Vector2Int input, Span<Vector2Int> output) {
            output[0] = Neighbor_LeftUp(input);
            output[1] = Neighbor_RightUp(input);
            output[2] = Neighbor_Right(input);
            output[3] = Neighbor_RightDown(input);
            output[4] = Neighbor_LeftDown(input);
            output[5] = Neighbor_Left(input);
            return Neighbor_Count;
        }

        public static Vector2Int Neighbor_LeftUp(Vector2Int input) {
            return new Vector2Int(input.x - 1, input.y + 1);
        }

        public static Vector2Int Neighbor_RightUp(Vector2Int input) {
            return new Vector2Int(input.x, input.y + 1);
        }

        public static Vector2Int Neighbor_Left(Vector2Int input) {
            return new Vector2Int(input.x - 1, input.y);
        }

        public static Vector2Int Neighbor_Right(Vector2Int input) {
            return new Vector2Int(input.x + 1, input.y);
        }

        public static Vector2Int Neighbor_LeftDown(Vector2Int input) {
            return new Vector2Int(input.x, input.y - 1);
        }

        public static Vector2Int Neighbor_RightDown(Vector2Int input) {
            return new Vector2Int(input.x + 1, input.y - 1);
        }
        #endregion Neighbor

    }

}