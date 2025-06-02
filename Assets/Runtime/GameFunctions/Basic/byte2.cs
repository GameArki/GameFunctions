using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;

#pragma warning disable CS0660
#pragma warning disable CS0661

[StructLayout(LayoutKind.Explicit)]
public struct byte2 : IEquatable<byte2> {

    [FieldOffset(0)]
    ushort value;

    [FieldOffset(0)]
    public byte x;

    [FieldOffset(1)]
    public byte y;

    public byte2(byte x, byte y) {
        value = 0;
        this.x = x;
        this.y = y;
    }

    public bool Equals(byte2 other) {
        return value == other.value;
    }

    public static bool operator ==(byte2 a, byte2 b) {
        return a.Equals(b);
    }

    public static bool operator !=(byte2 a, byte2 b) {
        return !a.Equals(b);
    }
}