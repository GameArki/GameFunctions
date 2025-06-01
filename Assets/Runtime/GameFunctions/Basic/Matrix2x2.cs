using System;
using UnityEngine;

public struct Matrix2x2 {

    // [m00, m01
    //  m10, m11]
    public float m00;
    public float m01;
    public float m10;
    public float m11;

    public Matrix2x2(float m00, float m01, float m10, float m11) {
        this.m00 = m00;
        this.m01 = m01;
        this.m10 = m10;
        this.m11 = m11;
    }

    public static Matrix2x2 operator *(Matrix2x2 a, Matrix2x2 b) {
        return new Matrix2x2(
            a.m00 * b.m00 + a.m01 * b.m10,
            a.m00 * b.m01 + a.m01 * b.m11,
            a.m10 * b.m00 + a.m11 * b.m10,
            a.m10 * b.m01 + a.m11 * b.m11
        );
    }

    public static Matrix2x2 operator *(Matrix2x2 a, float b) {
        return new Matrix2x2(
            a.m00 * b,
            a.m01 * b,
            a.m10 * b,
            a.m11 * b
        );
    }

    public static Matrix2x2 operator *(float a, Matrix2x2 b) {
        return new Matrix2x2(
            a * b.m00,
            a * b.m01,
            a * b.m10,
            a * b.m11
        );
    }

    public static Vector2 operator *(Matrix2x2 a, Vector2 b) {
        return new Vector2(
            a.m00 * b.x + a.m01 * b.y,
            a.m10 * b.x + a.m11 * b.y
        );
    }

    public static Matrix2x2 operator +(Matrix2x2 a, Matrix2x2 b) {
        return new Matrix2x2(
            a.m00 + b.m00,
            a.m01 + b.m01,
            a.m10 + b.m10,
            a.m11 + b.m11
        );
    }

    public static Matrix2x2 operator -(Matrix2x2 a, Matrix2x2 b) {
        return new Matrix2x2(
            a.m00 - b.m00,
            a.m01 - b.m01,
            a.m10 - b.m10,
            a.m11 - b.m11
        );
    }

    public static Matrix2x2 operator -(Matrix2x2 a) {
        return new Matrix2x2(
            -a.m00,
            -a.m01,
            -a.m10,
            -a.m11
        );
    }

    public static Matrix2x2 operator /(Matrix2x2 a, float b) {
        return new Matrix2x2(
            a.m00 / b,
            a.m01 / b,
            a.m10 / b,
            a.m11 / b
        );
    }

    public static Matrix2x2 operator /(float a, Matrix2x2 b) {
        return new Matrix2x2(
            a / b.m00,
            a / b.m01,
            a / b.m10,
            a / b.m11
        );
    }

    public static Matrix2x2 operator /(Matrix2x2 a, Matrix2x2 b) {
        return new Matrix2x2(
            a.m00 / b.m00,
            a.m01 / b.m01,
            a.m10 / b.m10,
            a.m11 / b.m11
        );
    }

    public static bool operator ==(Matrix2x2 a, Matrix2x2 b) {
        return a.m00 == b.m00 && a.m01 == b.m01 && a.m10 == b.m10 && a.m11 == b.m11;
    }

    public static bool operator !=(Matrix2x2 a, Matrix2x2 b) {
        return a.m00 != b.m00 || a.m01 != b.m01 || a.m10 != b.m10 || a.m11 != b.m11;
    }

    public override bool Equals(object obj) {
        if (obj is Matrix2x2) {
            Matrix2x2 b = (Matrix2x2)obj;
            return this == b;
        }
        return false;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override string ToString() {
        return string.Format("[{0}, {1}\r\n {2}, {3}]", m00, m01, m10, m11);
    }

    public static Matrix2x2 Identity() {
        return new Matrix2x2(1, 0, 0, 1);
    }

    public static Matrix2x2 Zero() {
        return new Matrix2x2(0, 0, 0, 0);
    }

    public static Matrix2x2 Rotate(float angle) {
        float cos = MathF.Cos(angle);
        float sin = MathF.Sin(angle);
        return new Matrix2x2(cos, -sin, sin, cos);
    }

    public static Matrix2x2 Scale(float x, float y) {
        return new Matrix2x2(x, 0, 0, y);
    }

    public static Matrix2x2 Scale(float s) {
        return new Matrix2x2(s, 0, 0, s);
    }

    public static Matrix2x2 Scale(Vector2 v) {
        return new Matrix2x2(v.x, 0, 0, v.y);
    }

    public static Matrix2x2 Translate(float x, float y) {
        return new Matrix2x2(1, x, 0, y);
    }

    public static Matrix2x2 Translate(Vector2 v) {
        return new Matrix2x2(1, v.x, 0, v.y);
    }

    public static Matrix2x2 Shear(float x, float y) {
        return new Matrix2x2(1, x, y, 1);
    }

    public static Matrix2x2 Shear(Vector2 v) {
        return new Matrix2x2(1, v.x, v.y, 1);
    }

    public static Matrix2x2 Inverse(Matrix2x2 m) {
        float det = m.m00 * m.m11 - m.m01 * m.m10;
        if (det == 0) {
            throw new System.Exception("Can't inverse a matrix with det = 0");
        }
        return new Matrix2x2(
            m.m11 / det,
            -m.m01 / det,
            -m.m10 / det,
            m.m00 / det
        );
    }

}