using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#pragma warning disable CS0660
#pragma warning disable CS0661

[StructLayout(LayoutKind.Explicit)]
public struct Bit128 : IEquatable<Bit128> {

    [FieldOffset(0)]
    public decimal decimalValue;

    [FieldOffset(0)]
    public byte b0;

    [FieldOffset(1)]
    public byte b1;

    [FieldOffset(2)]
    public byte b2;

    [FieldOffset(3)]
    public byte b3;

    [FieldOffset(4)]
    public byte b4;

    [FieldOffset(5)]
    public byte b5;

    [FieldOffset(6)]
    public byte b6;

    [FieldOffset(7)]
    public byte b7;

    [FieldOffset(8)]
    public byte b8;

    [FieldOffset(9)]
    public byte b9;

    [FieldOffset(10)]
    public byte b10;

    [FieldOffset(11)]
    public byte b11;

    [FieldOffset(12)]
    public byte b12;

    [FieldOffset(13)]
    public byte b13;

    [FieldOffset(14)]
    public byte b14;

    [FieldOffset(15)]
    public byte b15;

    [FieldOffset(0)]
    public int i0;

    [FieldOffset(4)]
    public int i1;

    [FieldOffset(8)]
    public int i2;

    [FieldOffset(12)]
    public int i3;

    [FieldOffset(0)]
    public long l0;

    [FieldOffset(8)]
    public long l1;

    [FieldOffset(0)]
    public float f0;

    [FieldOffset(4)]
    public float f1;

    [FieldOffset(8)]
    public float f2;

    [FieldOffset(12)]
    public float f3;

    [FieldOffset(0)]
    public double d0;

    [FieldOffset(8)]
    public double d1;

    public Bit128(decimal value) {
        this = default;
        this.decimalValue = value;
    }

    public static bool operator ==(Bit128 a, Bit128 b) {
        return a.decimalValue == b.decimalValue;
    }

    public static bool operator !=(Bit128 a, Bit128 b) {
        return a.decimalValue != b.decimalValue;
    }

    bool IEquatable<Bit128>.Equals(Bit128 other) {
        return this.decimalValue == other.decimalValue;
    }

    public static bool operator <(Bit128 a, Bit128 b) {
        return a.decimalValue < b.decimalValue;
    }

    public static bool operator >(Bit128 a, Bit128 b) {
        return a.decimalValue > b.decimalValue;
    }

}