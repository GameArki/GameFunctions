using System.Runtime.CompilerServices;
using UnityEngine;

public static class Vector2Extension {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int RoundToVector2Int(this Vector2 v) {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int FloorToVector2Int(this Vector2 v) {
        return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int CeilToVector2Int(this Vector2 v) {
        return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
    }

}