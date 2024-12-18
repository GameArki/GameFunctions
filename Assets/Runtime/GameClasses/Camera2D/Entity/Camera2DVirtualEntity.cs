using System;
using UnityEngine;

namespace GameClasses.Camera2DLib.Internal {

    public class Camera2DVirtualEntity {

        // Base
        public int id;

        public Vector2 pos;
        public float orthographicSize;
        public float aspect; // width ÷ height

        // Follow
        public Camera2DFollowModel followModel;

        // Confine
        public Camera2DConfineModel confineModel;

        // Effect: Shake
        public Camera2DEffectShakeModel shakeModel;

        // Effect: ZoomIn
        public Camera2DEffectZoomInModel zoomInModel;

        public Camera2DVirtualEntity() {
            followModel = new Camera2DFollowModel();
            confineModel = new Camera2DConfineModel();
            shakeModel = new Camera2DEffectShakeModel();
            zoomInModel = new Camera2DEffectZoomInModel();
        }

    }

}