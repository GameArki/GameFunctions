using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.Burst;

#pragma warning disable CS0660
#pragma warning disable CS0661

[BurstCompile]
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

    [BurstCompile]
    public static bool operator ==(in short2 a, in short2 b) {
        return a.value == b.value;
    }

    [BurstCompile]
    public static bool operator !=(in short2 a, in short2 b) {
        return a.value != b.value;
    }

    [BurstCompile]
    public static void Add(in short2 a, in short2 b, out short2 result) {
        result = new short2((short)(a.x + b.x), (short)(a.y + b.y));
    }

    [BurstCompile]
    public static void Minus(in short2 a, in short2 b, out short2 result) {
        result = new short2((short)(a.x - b.x), (short)(a.y - b.y));
    }

}