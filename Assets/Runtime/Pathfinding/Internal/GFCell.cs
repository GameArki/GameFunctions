using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameFunctions.PathfindingInternal {

    [StructLayout(LayoutKind.Explicit)]
    public struct GFCell : IEquatable<GFCell>, IComparable<GFCell>, IComparer<GFCell> {

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

        bool IEquatable<GFCell>.Equals(GFCell other) {
            return key == other.key;
        }

        int IComparable<GFCell>.CompareTo(GFCell other) {
            if (fCost < other.fCost) {
                return -1;
            } else if (fCost > other.fCost) {
                return 1;
            } else {
                if (gCost < other.gCost) {
                    return -1;
                } else if (gCost > other.gCost) {
                    return 1;
                } else {
                    return 0;
                }
            }
        }

        public override bool Equals(object obj) {
            return obj is GFCell other && (other.key == key);
        }

        public override int GetHashCode() {
            return key.GetHashCode();
        }

        int IComparer<GFCell>.Compare(GFCell x, GFCell y) {
            if (x.fCost < y.fCost) {
                return -1;
            } else if (x.fCost > y.fCost) {
                return 1;
            } else {
                return 0;
            }
        }

        public static bool operator ==(GFCell left, GFCell right) {
            return left.key == right.key;
        }

        public static bool operator !=(GFCell left, GFCell right) {
            return !(left.key == right.key);
        }

    }

}