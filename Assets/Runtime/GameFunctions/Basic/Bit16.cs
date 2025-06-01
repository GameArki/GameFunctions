using System;
using System.Runtime.InteropServices;

#pragma warning disable CS0660
#pragma warning disable CS0661

[StructLayout(LayoutKind.Explicit)]
public struct Bit16 : IEquatable<Bit16>{

    [FieldOffset(0)]
    public short shortValue;

    [FieldOffset(0)]
    public ushort ushortValue;

    [FieldOffset(0)]
    public char charValue;

    [FieldOffset(0)]
    public byte b0;

    [FieldOffset(1)]
    public byte b1;

    bool IEquatable<Bit16>.Equals(Bit16 other) {
        return this.shortValue == other.shortValue;
    }

    public static bool operator ==(Bit16 a, Bit16 b) {
        return a.shortValue == b.shortValue;
    }

    public static bool operator !=(Bit16 a, Bit16 b) {
        return a.shortValue != b.shortValue;
    }

}
