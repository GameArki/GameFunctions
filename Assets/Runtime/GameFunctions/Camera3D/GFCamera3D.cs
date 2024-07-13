using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions {

    public static class GFCamera3D {

        public static Vector3 GetFollowPos(Vector3 cameraForward, Vector3 targetPos, Vector3 followOffset) {
            Quaternion rotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            Vector3 offset = rotation * followOffset;
            Vector3 target = targetPos + offset;
            return target;
        }

        public static Vector3 GetRotateForward(Vector3 cameraForward, Vector2 rotateOffset) {
            Quaternion rotation = Quaternion.Euler(rotateOffset.y, rotateOffset.x, 0);
            Vector3 forward = rotation * cameraForward;
            return forward;
        }

    }

}
