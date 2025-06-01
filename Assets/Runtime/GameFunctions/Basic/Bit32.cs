using System;
using System.Runtime.InteropServices;

#pragma warning disable CS0660
#pragma warning disable CS0661

[StructLayout(LayoutKind.Explicit)]
public struct Bit32 : IEquatable<Bit32> {

    [FieldOffset(0)]
    public int intValue;

    [FieldOffset(0)]
    public uint uintValue;

    [FieldOffset(0)]
    public float floatValue;

    [FieldOffset(0)]
    public byte b0;

    [FieldOffset(1)]
    public byte b1;

    [FieldOffset(2)]
    public byte b2;

    [FieldOffset(3)]
    public byte b3;

    [FieldOffset(0)]
    public short short1;

    [FieldOffset(2)]
    public short short2;

    bool IEquatable<Bit32>.Equals(Bit32 other) {
        return this.intValue == other.intValue;
    }

    public static bool operator ==(Bit32 a, Bit32 b) {
        return a.intValue == b.intValue;
    }

    public static bool operator !=(Bit32 a, Bit32 b) {
        return a.intValue != b.intValue;
    }

}