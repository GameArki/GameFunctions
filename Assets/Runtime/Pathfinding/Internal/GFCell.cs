using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameFunctions.PathfindingInternal {

    public class GFCell : IEquatable<GFCell>, IComparable<GFCell> {

        public Vector2Int pos;
        public I32I32_U64 key;
        public float fCost;
        public float gCost;
        public float hCost;
        public GFCell parent;

        public GFCell() {}

        public void Init(Vector2Int pos, float fCost, float gCost, float hCost, GFCell parent) {
            this.pos = pos;
            this.key = new I32I32_U64(pos);
            this.fCost = fCost;
            this.gCost = gCost;
            this.hCost = hCost;
            this.parent = parent;
        }

        bool IEquatable<GFCell>.Equals(GFCell other) {
            return key == other.key;
        }

        public override bool Equals(object obj) {
            GFCell other = obj as GFCell;
            Debug.Log("Equals: " + key + " " + other.key);
            if (other != null) {
                return key == other.key;
            }
            return false;
        }

        public override int GetHashCode() {
            Debug.Log(" GetHashCode: " + key.GetHashCode());
            return key.GetHashCode();
        }

        int IComparable<GFCell>.CompareTo(GFCell other) {

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