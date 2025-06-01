using System;

#pragma warning disable 0660, 0661

/// <summary>
/// 用于表示一组开关，只有全部打开时才算是开启; 单个关闭即算关闭
/// </summary>
public struct SeriesBool {

    byte value_current;
    byte value_on;

    public SeriesBool(params byte[] bits) {
        value_current = 0;
        value_on = 0;
        RegisterAll(bits);
    }

    // 哪些开关打开后，才算是开启
    public void Register(byte bitID) {
        value_on |= (byte)(1 << bitID);
    }

    public void RegisterAll(params byte[] bits) {
        foreach (var bit in bits) {
            Register(bit);
        }
    }

    public void SetOn(byte bitID, bool on) {
        if (on) {
            On(bitID);
        } else {
            Off(bitID);
        }
    }

    // 打开某开关
    public void On(byte bitID) {
        value_current |= (byte)(1 << bitID);
    }

    public void OnAll() {
        value_current = value_on;
    }

    // 关闭某开关
    public void Off(byte bitID) {
        value_current &= (byte)~(1 << bitID);
    }

    public void OffAll() {
        value_current = 0;
    }

    public bool IsOn() {
        return value_current == value_on;
    }

    public static explicit operator bool(SeriesBool me) {
        return me.IsOn();
    }

    public static bool operator !(SeriesBool me) {
        return !me.IsOn();
    }

    public static bool operator ==(SeriesBool me, bool other) {
        return me.IsOn() == other;
    }

    public static bool operator !=(SeriesBool me, bool other) {
        return me.IsOn() != other;
    }

}