using System.Runtime.InteropServices;
using UnityEngine;

namespace GameFunctions.PathfindingInternal {

    [StructLayout(LayoutKind.Explicit)]
    public struct I32I32_U64 {

        [FieldOffset(0)] public int i32_0;
        [FieldOffset(4)] public int i32_1;
        [FieldOffset(0)] public ulong u64;

        public I32I32_U64(Vector2Int v) {
            this.u64 = 0;
            this.i32_0 = v.x;
            this.i32_1 = v.y;
        }

        public I32I32_U64(int i32_0, int i32_1) {
            this.u64 = 0;
            this.i32_0 = i32_0;
            this.i32_1 = i32_1;
        }

        public override bool Equals(object obj) {
            if (obj is I32I32_U64) {
                I32I32_U64 other = (I32I32_U64)obj;
                return this.u64 == other.u64;
            }
            return false;
        }

        public override int GetHashCode() => this.u64.GetHashCode();
        public static bool operator ==(I32I32_U64 a, I32I32_U64 b) => a.u64 == b.u64;
        public static bool operator !=(I32I32_U64 a, I32I32_U64 b) => a.u64 != b.u64;

    }

}