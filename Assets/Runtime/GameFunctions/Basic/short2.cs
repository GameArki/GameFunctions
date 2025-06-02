using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;

#pragma warning disable CS0660
#pragma warning disable CS0661

[StructLayout(LayoutKind.Explicit)]
public struct short2 : IEquatable<short2> {

    [FieldOffset(0)]
    uint value;

    [FieldOffset(0)]
    public short x;

    [FieldOffset(2)]
    public short y;

    public short2(short x, short y) {
        value = 0;
        this.x = x;
        this.y = y;
    }

    public bool Equals(short2 other) {
        return value == other.value;
    }

    public static bool operator ==(short2 a, short2 b) {
        return a.Equals(b);
    }

    public static bool operator !=(short2 a, short2 b) {
        return !a.Equals(b);
    }

    public static short2 operator +(short2 a, short2 b) {
        return new short2((short)(a.x + b.x), (short)(a.y + b.y));
    }

    public static short2 operator -(short2 a, short2 b) {
        return new short2((short)(a.x - b.x), (short)(a.y - b.y));
    }

}