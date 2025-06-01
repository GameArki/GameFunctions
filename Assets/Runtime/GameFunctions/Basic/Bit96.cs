using System;
using System.Runtime.InteropServices;

#pragma warning disable CS0660
#pragma warning disable CS0661

[StructLayout(LayoutKind.Explicit)]
public struct Bit96 : IEquatable<Bit96> {

    [FieldOffset(0)] public float f0;
    [FieldOffset(0)] public int i0;

    [FieldOffset(4)] public float f1;
    [FieldOffset(4)] public int i1;

    [FieldOffset(8)] public float f2;
    [FieldOffset(8)] public int i2;

    [FieldOffset(0)] public ulong v0;
    [FieldOffset(8)] public uint v1;

    public Bit96(float f32_0, int i32_1, int i32_2) {
        this = default;
        this.f0 = f32_0;
        this.i1 = i32_1;
        this.i2 = i32_2;
    }

    bool IEquatable<Bit96>.Equals(Bit96 other) {
        return this.v0 == other.v0 && this.v1 == other.v1;
    }

    public static bool operator ==(Bit96 a, Bit96 b) => a.v0 == b.v0 && a.v1 == b.v1;
    public static bool operator !=(Bit96 a, Bit96 b) => a.v0 != b.v0 || a.v1 != b.v1;
    public static bool operator <(Bit96 a, Bit96 b) => a.v0 < b.v0 || (a.v0 == b.v0 && a.v1 < b.v1);
    public static bool operator >(Bit96 a, Bit96 b) => a.v0 > b.v0 || (a.v0 == b.v0 && a.v1 > b.v1);
    public static bool operator <=(Bit96 a, Bit96 b) => a.v0 < b.v0 || (a.v0 == b.v0 && a.v1 <= b.v1);
    public static bool operator >=(Bit96 a, Bit96 b) => a.v0 > b.v0 || (a.v0 == b.v0 && a.v1 >= b.v1);

}