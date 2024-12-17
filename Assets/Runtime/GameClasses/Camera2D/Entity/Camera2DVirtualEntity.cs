using System;
using UnityEngine;

namespace GameClasses.Camera2DLib.Internal {

    public class Camera2DVirtualEntity {

        // Base
        public int id;
        public Vector2 truePos;
        public Vector2 finalPos;
        public float orthographicSize;
        public float aspect; // width / height

        // Follow
        public bool isFollow;
        public Vector2 followTargetPos;
        public Vector2 followOffset;
        public float followDampingX;
        public float followDampingXOrigin;
        public float followDampingY;
        public float followDampingYOrigin;

        // Confine
        public bool isConfine;
        public Vector4 minMaxBounds;

        // Shake
        public bool isShake;
        public float shakeDuration;
        public float shakeFrequency;
        public Vector2 shakeAmplitude;
        public float shakeTimer;

        public Camera2DVirtualEntity() { }

    }

}