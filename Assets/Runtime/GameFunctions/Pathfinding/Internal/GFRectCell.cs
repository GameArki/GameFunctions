using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameFunctions.PathfindingInternal {

    public struct GFRectCell : IEquatable<GFRectCell>, IComparable<GFRectCell> {

        public Vector2Int pos;
        public float fCost;
        public float gCost;
        public float hCost;

        public void Init(Vector2Int pos, float fCost, float gCost, float hCost) {
            this.pos = pos;
            this.fCost = fCost;
            this.gCost = gCost;
            this.hCost = hCost;
        }

        bool IEquatable<GFRectCell>.Equals(GFRectCell other) {
            return pos == other.pos;
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        int IComparable<GFRectCell>.CompareTo(GFRectCell other) {

            Bit128 fKey = new Bit128();
            fKey.i32_0 = pos.y;
            fKey.i32_1 = pos.x;
            fKey.f32_2 = hCost;
            fKey.f32_3 = fCost;

            Bit128 otherFKey = new Bit128();
            otherFKey.i32_0 = other.pos.y;
            otherFKey.i32_1 = other.pos.x;
            otherFKey.f32_2 = other.hCost;
            otherFKey.f32_3 = other.fCost;

            if (pos.x == other.pos.x && pos.y == other.pos.y) {
                return 0;
            }

            if (fKey < otherFKey) {
                return -1;
            } else if (fKey > otherFKey) {
                return 1;
            } else {
                return 0;
            }
        }

    }

}