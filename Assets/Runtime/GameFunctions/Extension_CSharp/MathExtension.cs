using System;
using UnityEngine;

public static class MathExtension {

    public static int ToOne(this float value) {
        if (value > 0) {
            return 1;
        } else if (value < 0) {
            return -1;
        } else {
            return 0;
        }
    }

}