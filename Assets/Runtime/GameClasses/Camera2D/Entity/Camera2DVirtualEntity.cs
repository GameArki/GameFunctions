using System;
using UnityEngine;

namespace GameClasses.Camera2DLib.Internal {

    public class Camera2DVirtualEntity {

        // Base
        public int id;
        public Vector2 pos_true;
        public Vector2 pos_final;
        public float orthographicSize_true;
        public float orthographicSize_final;
        public float aspect_true; // width / height
        public float aspect_final; // width / height

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