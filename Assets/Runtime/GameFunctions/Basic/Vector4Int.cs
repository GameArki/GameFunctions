using System;
using System.Runtime.InteropServices;

#pragma warning disable CS0660
#pragma warning disable CS0661

[StructLayout(LayoutKind.Explicit)]
public struct Vector4Int : IEquatable<Vector4Int> {

    [FieldOffset(0)]
    decimal decimalValue;

    [FieldOffset(0)]
    public int x;

    [FieldOffset(4)]
    public int y;

    [FieldOffset(8)]
    public int z;

    [FieldOffset(12)]
    public int w;

    public Vector4Int(int x, int y, int z, int w) {
        this.decimalValue = 0;
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public int Min() {
        return Math.Min(Math.Min(x, y), Math.Min(z, w));
    }

    public static Vector4Int operator +(Vector4Int a, Vector4Int b) {
        return new Vector4Int(
            a.x + b.x,
            a.y + b.y,
            a.z + b.z,
            a.w + b.w
        );
    }

    public static Vector4Int operator -(Vector4Int a, Vector4Int b) {
        return new Vector4Int(
            a.x - b.x,
            a.y - b.y,
            a.z - b.z,
            a.w - b.w
        );
    }

    public static Vector4Int operator *(Vector4Int a, int b) {
        return new Vector4Int(
            a.x * b,
            a.y * b,
            a.z * b,
            a.w * b
        );
    }

    public static Vector4Int operator *(int a, Vector4Int b) {
        return new Vector4Int(
            a * b.x,
            a * b.y,
            a * b.z,
            a * b.w
        );
    }

    public static Vector4Int operator /(Vector4Int a, int b) {
        return new Vector4Int(
            a.x / b,
            a.y / b,
            a.z / b,
            a.w / b
        );
    }

    public static Vector4Int operator /(int a, Vector4Int b) {
        return new Vector4Int(
            a / b.x,
            a / b.y,
            a / b.z,
            a / b.w
        );
    }

    bool IEquatable<Vector4Int>.Equals(Vector4Int other) {
        return this.decimalValue == other.decimalValue;
    }

    public static bool operator ==(Vector4Int a, Vector4Int b) {
        return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
    }

    public static bool operator !=(Vector4Int a, Vector4Int b) {
        return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
    }

    public override string ToString() {
        return string.Format("({0}, {1}, {2}, {3})", x, y, z, w);
    }

}