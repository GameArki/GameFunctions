using System.Runtime.InteropServices;
using UnityEngine;

namespace GameFunctions.PathfindingInternal {

    [StructLayout(LayoutKind.Explicit)]
    public struct Bit128 {

        [FieldOffset(0)] public float f32_0;
        [FieldOffset(0)] public int i32_0;

        [FieldOffset(4)] public float f32_1;
        [FieldOffset(4)] public int i32_1;

        [FieldOffset(8)] public float f32_2;
        [FieldOffset(8)] public int i32_2;

        [FieldOffset(12)] public float f32_3;
        [FieldOffset(12)] public int i32_3;

        [FieldOffset(0)] public ulong u64_0;
        [FieldOffset(8)] public ulong u64_1;

        public override bool Equals(object obj) {
            if (obj is Bit128) {
                Bit128 other = (Bit128)obj;
                return this.u64_0 == other.u64_0 && this.u64_1 == other.u64_1;
            }
            return false;
        }

        public override int GetHashCode() {
            return this.u64_0.GetHashCode() ^ this.u64_1.GetHashCode();
        }

        public static bool operator ==(Bit128 a, Bit128 b) {
            return a.u64_0 == b.u64_0 && a.u64_1 == b.u64_1;
        }

        public static bool operator !=(Bit128 a, Bit128 b) {
            return a.u64_0 != b.u64_0 || a.u64_1 != b.u64_1;
        }

        public static bool operator <(Bit128 a, Bit128 b) {
            return a.u64_1 < b.u64_1 || (a.u64_1 == b.u64_1 && a.u64_0 < b.u64_0);
        }

        public static bool operator >(Bit128 a, Bit128 b) {
            return a.u64_1 > b.u64_1 || (a.u64_1 == b.u64_1 && a.u64_0 > b.u64_0);
        }

    }

}