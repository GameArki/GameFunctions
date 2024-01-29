using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameFunctions {

    public static class Vector2Extension {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToVector2Int(this Vector2 v) {
            return new Vector2Int((int)v.x, (int)v.y);
        }

    }
}