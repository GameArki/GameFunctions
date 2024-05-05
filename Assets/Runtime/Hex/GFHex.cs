using System;
using UnityEngine;

namespace GameFunctions {

    /// <summary> 尖顶六边形 / Hexagon with Pointy Top </summary>
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

        public static Vector2Int RenderPosToLogicPos(Vector2 input, float outterRadius, float gap) {
            float innerRadius = GetInnerRadius(outterRadius);
            Vector2Int guessCenter = new Vector2Int(
                                Mathf.RoundToInt(input.x / (innerRadius * 2f + gap)),
                                Mathf.RoundToInt(input.y / (outterRadius * 1.5f + gap)));
            Vector2Int min = new Vector2Int(guessCenter.x - 2, guessCenter.y - 2);
            Vector2Int max = new Vector2Int(guessCenter.x + 2, guessCenter.y + 2);
            float minDistanceSqr = float.MaxValue;
            Vector2Int result = guessCenter;
            for (int y = min.y; y <= max.y; y++) {
                for (int x = min.x; x <= max.x; x++) {
                    Vector2Int centerLogic = new Vector2Int(x, y);
                    Vector2 center = Render_GetCenterPos(centerLogic, outterRadius, gap);
                    float distanceSqr = Vector2.SqrMagnitude(input - center);
                    if (distanceSqr < minDistanceSqr) {
                        minDistanceSqr = distanceSqr;
                        result = centerLogic;
                    }
                }
            }
            return result;
        }

        public static float GetInnerRadius(float outterRadius) {
            return outterRadius * Mathf.Sqrt(3f) / 2f;
        }

        public static Vector2 Render_GetCenterPos(Vector2Int input, float outterRadius, float gap) {
            float innerRadius = GetInnerRadius(outterRadius);
            if ((input.y & 1) == 1) {
                return new Vector2(input.x * (innerRadius * 2f + gap), input.y * (outterRadius * 1.5f + gap));
            } else {
                return new Vector2(input.x * (innerRadius * 2f + gap) + innerRadius + gap, input.y * (outterRadius * 1.5f + gap));
            }
        }

        public static int Render_GetHexCorners(Vector2 input, float outterRadius, Span<Vector2> output) {
            float innerRadius = GetInnerRadius(outterRadius);
            output[0] = new Vector2(input.x, input.y + outterRadius); // up point
            output[1] = new Vector2(input.x + innerRadius, input.y + outterRadius * 0.5f); // right-up point
            output[2] = new Vector2(input.x + innerRadius, input.y - outterRadius * 0.5f); // right-down point
            output[3] = new Vector2(input.x, input.y - outterRadius); // down point
            output[4] = new Vector2(input.x - innerRadius, input.y - outterRadius * 0.5f); // left-down point
            output[5] = new Vector2(input.x - innerRadius, input.y + outterRadius * 0.5f); // left-up point
            return Neighbor_Count;
        }

#if UNITY_EDITOR
        public static void DrawGizmos(Vector2Int input, float outterRadius, float gap) {
            // six lines
            Vector2 center = Render_GetCenterPos(input, outterRadius, gap);
            Span<Vector2> corners = stackalloc Vector2[Neighbor_Count];
            Render_GetHexCorners(center, outterRadius, corners);
            for (int i = 0; i < Neighbor_Count; i++) {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % Neighbor_Count]);
            }
        }
#endif

        #region Neighbor
        public const int Neighbor_Count = 6;
        /// <summary> 6 Neighbors, Clockwise from left-up </summary>
        public static int Neighbors_Logic(Vector2Int input, ref Span<Vector2Int> output) {
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