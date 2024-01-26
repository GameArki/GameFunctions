using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameFunctions {

    public static class GFPhysicsIntersection2D {

        // ==== POINT X LINE ====
        public static bool IsPointXLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd, out Vector2 closestPoint) {
            Vector2 lineDir = lineEnd - lineStart;
            float lineLength = lineDir.magnitude;
            if (lineLength == 0) {
                closestPoint = lineStart;
                return false;
            }
            lineDir /= lineLength;
            float dot = Vector2.Dot(point - lineStart, lineDir);
            if (dot < 0) {
                closestPoint = lineStart;
                return false;
            }
            if (dot > lineLength) {
                closestPoint = lineEnd;
                return false;
            }
            closestPoint = lineStart + lineDir * dot;
            return true;
        }

        // ==== LINE X LINE ====
        public static bool IsLineXLine(Vector2 aStart, Vector2 aEnd, Vector2 bStart, Vector2 bEnd, out Vector2 intersection) {
            Vector2 aSeg = aEnd - aStart;
            Vector2 bSeg = bEnd - bStart;
            float cross = Cross(aSeg, bSeg);
            if (cross == 0) {
                intersection = Vector2.zero;
                return false;
            }
            Vector2 seg = bStart - aStart;
            float t = Cross(seg, bSeg) / cross;
            float u = Cross(seg, aSeg) / cross;
            if (t >= 0 && t <= 1 && u >= 0 && u <= 1) {
                intersection = aStart + aSeg * t;
                return true;
            }
            intersection = Vector2.zero;
            return false;
        }

        // ==== POINT X RECT ====
        public static bool IsPointXRect(Vector2 point, Vector2 rectCenter, Vector2 rectSize) {
            (Vector2 min, Vector2 max) = GetRectMinMax(rectCenter, rectSize);
            return IsPointXRectAABB(point, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointXRectAABB(Vector2 point, Vector2 rectMin, Vector2 rectMax) {
            return point.x >= rectMin.x && point.x <= rectMax.x && point.y >= rectMin.y && point.y <= rectMax.y;
        }

        // ==== RECT X RECT ====
        public static bool IsRectXRect(Vector2 aCenter, Vector2 aSize, Vector2 bCenter, Vector2 bSize) {
            (Vector2 amin, Vector2 amax) = GetRectMinMax(aCenter, aSize);
            (Vector2 bmin, Vector2 bmax) = GetRectMinMax(bCenter, bSize);
            return IsRectXRectAABB(amin, amax, bmin, bmax);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static (Vector2 min, Vector2 max) GetRectMinMax(Vector2 center, Vector2 size) {
            Vector2 halfSize = size / 2;
            return (center - halfSize, center + halfSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRectXRectAABB(Vector2 aMin, Vector2 aMax, Vector2 bMin, Vector2 bMax) {
            return aMin.x <= bMax.x && aMax.x >= bMin.x && aMin.y <= bMax.y && aMax.y >= bMin.y;
        }

        // ==== CIRCLE X CIRCLE ====
        public static bool IsCircleXCircle(Vector2 aCenter, float aRadius, Vector2 bCenter, float bRadius) {
            float radiusSum = aRadius + bRadius;
            return Vector2.SqrMagnitude(aCenter - bCenter) <= (radiusSum * radiusSum);
        }

        // ==== CIRCLE X RECT ====
        public static bool IsCircleXRect(Vector2 circleCenter, float circleRadius, Vector2 rectCenter, Vector2 rectSize) {
            (Vector2 rectMin, Vector2 rectMax) = GetRectMinMax(rectCenter, rectSize);
            Vector2 closestPoint = new Vector2(
                Mathf.Clamp(circleCenter.x, rectMin.x, rectMax.x),
                Mathf.Clamp(circleCenter.y, rectMin.y, rectMax.y)
            );
            return Vector2.SqrMagnitude(circleCenter - closestPoint) <= (circleRadius * circleRadius);
        }

        public static bool IsCircleXRectAABB(Vector2 circleCenter, float circleRadius, Vector2 rectMin, Vector2 rectMax) {
            Vector2 closestPoint = new Vector2(
                Mathf.Clamp(circleCenter.x, rectMin.x, rectMax.x),
                Mathf.Clamp(circleCenter.y, rectMin.y, rectMax.y)
            );
            return Vector2.SqrMagnitude(circleCenter - closestPoint) <= (circleRadius * circleRadius);
        }

        // ==== Generic ====
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float Cross(Vector2 a, Vector2 b) {
            return a.x * b.y - a.y * b.x;
        }

    }

}