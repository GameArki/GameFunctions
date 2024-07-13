using System.Runtime.InteropServices;
using UnityEngine;

namespace GameFunctions.PathfindingInternal {

    [StructLayout(LayoutKind.Explicit)]
    public struct Bit96 {

        [FieldOffset(0)] public float f32_0;
        [FieldOffset(0)] public int i32_0;

        [FieldOffset(4)] public float f32_1;
        [FieldOffset(4)] public int i32_1;

        [FieldOffset(8)] public float f32_2;
        [FieldOffset(8)] public int i32_2;

        [FieldOffset(0)] public ulong u64_0;
        [FieldOffset(8)] public uint u32_0;

        public Bit96(float f32_0, int i32_1, int i32_2) {
            this.u64_0 = 0;
            this.u32_0 = 0;
            this.i32_0 = 0;
            this.f32_1 = 0;
            this.f32_2 = 0;
            this.f32_0 = f32_0;
            this.i32_1 = i32_1;
            this.i32_2 = i32_2;
        }

        public override bool Equals(object obj) {
            if (obj is Bit96) {
                Bit96 other = (Bit96)obj;
                return this.u64_0 == other.u64_0 && this.u32_0 == other.u32_0;
            }
            return false;
        }

        public override int GetHashCode() {
            return this.u64_0.GetHashCode() ^ this.u32_0.GetHashCode();
        }
        public static bool operator ==(Bit96 a, Bit96 b) => a.u64_0 == b.u64_0 && a.u32_0 == b.u32_0;
        public static bool operator !=(Bit96 a, Bit96 b) => a.u64_0 != b.u64_0 || a.u32_0 != b.u32_0;
        public static bool operator <(Bit96 a, Bit96 b) => a.u64_0 < b.u64_0 || (a.u64_0 == b.u64_0 && a.u32_0 < b.u32_0);
        public static bool operator >(Bit96 a, Bit96 b) => a.u64_0 > b.u64_0 || (a.u64_0 == b.u64_0 && a.u32_0 > b.u32_0);
        public static bool operator <=(Bit96 a, Bit96 b) => a.u64_0 < b.u64_0 || (a.u64_0 == b.u64_0 && a.u32_0 <= b.u32_0);
        public static bool operator >=(Bit96 a, Bit96 b) => a.u64_0 > b.u64_0 || (a.u64_0 == b.u64_0 && a.u32_0 >= b.u32_0);

    }

}