using System;
using UnityEngine;

namespace GameClasses.Camera2DLib.Internal {

    public class Camera2DFollowModel {

        public bool isEnable;

        public float followDampingX;
        public float followDampingXOrigin;
        public float lastDampingDirX;

        public float followDampingY;
        public float followDampingYOrigin;
        public float lastDampingDirY;

        public Vector2 followTargetPos;
        public Vector2 followOffset;

        public Vector2 deadZoneSize;

    }

}