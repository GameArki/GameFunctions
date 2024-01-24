using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameFunctions.PathfindingInternal {

    [StructLayout(LayoutKind.Explicit)]
    public struct GFCell : IEquatable<GFCell>, IComparable<GFCell> {

        [FieldOffset(0)]
        public Vector2Int pos;
        [FieldOffset(0)]
        public I32I32_U64 key;
        [FieldOffset(8)]
        public float fCost;
        [FieldOffset(12)]
        public float gCost;
        [FieldOffset(16)]
        public I32I32_U64 parent;

        public GFCell(Vector2Int pos, float fCost, float gCost, I32I32_U64 parent) {
            this.key = new I32I32_U64();
            this.pos = pos;
            this.fCost = fCost;
            this.gCost = gCost;
            this.parent = parent;
        }

        public bool Equals(GFCell other) {
            return pos.Equals(other.pos);
        }

        public int CompareTo(GFCell other) {
            if (fCost < other.fCost) {
                return -1;
            } else if (fCost > other.fCost) {
                return 1;
            } else {
                return 0;
            }
        }

        public override bool Equals(object obj) {
            return obj is GFCell other && Equals(other);
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public static bool operator ==(GFCell left, GFCell right) {
            return left.key == right.key;
        }

        public static bool operator !=(GFCell left, GFCell right) {
            return !(left.key == right.key);
        }

    }

}