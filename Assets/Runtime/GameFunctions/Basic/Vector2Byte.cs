using System;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS0660
#pragma warning disable CS0661

[StructLayout(LayoutKind.Explicit)]
public struct Vector2Byte : IEquatable<Vector2Byte> {
    [FieldOffset(0)]
    ushort value;

    [FieldOffset(0)]
    public byte x;

    [FieldOffset(1)]
    public byte y;

    public Vector2Byte(byte x, byte y) {
        this.value = 0;
        this.x = x;
        this.y = y;
    }

    public Vector2Byte(Vector2Int v) {
        this.value = 0;
        this.x = (byte)v.x;
        this.y = (byte)v.y;
    }

    // +
    public static Vector2Int operator +(Vector2Byte a, Vector2Byte b) {
        return new Vector2Int(a.x + b.x, a.y + b.y);
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Byte b) {
        return new Vector2Int(a.x + b.x, a.y + b.y);
    }

    public static Vector2Int operator +(Vector2Byte a, Vector2Int b) {
        return new Vector2Int(a.x + b.x, a.y + b.y);
    }

    // -
    public static Vector2Int operator -(Vector2Byte a, Vector2Byte b) {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }

    public static Vector2Int operator -(Vector2Int a, Vector2Byte b) {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }

    public static Vector2Int operator -(Vector2Byte a, Vector2Int b) {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }

    // ==
    bool IEquatable<Vector2Byte>.Equals(Vector2Byte other) {
        return this.value == other.value;
    }

    public static bool operator ==(Vector2Byte a, Vector2Byte b) {
        return a.value == b.value;
    }

    public static bool operator !=(Vector2Byte a, Vector2Byte b) {
        return a.value != b.value;
    }

    // implicit
    public static implicit operator Vector2Int(Vector2Byte v) {
        return new Vector2Int(v.x, v.y);
    }

    public static implicit operator Vector2Byte(Vector2Int v) {
        return new Vector2Byte((byte)v.x, (byte)v.y);
    }

}