using System;
using UnityEngine;

namespace GameClasses.Camera2DLib.Internal {

    public class Camera2DVirtualEntity {

        // Base
        public int id;
        public Vector2 pos;
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

        public Camera2DVirtualEntity() { }

    }

}