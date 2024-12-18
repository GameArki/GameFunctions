using System;
using UnityEngine;

namespace GameClasses.Camera2DLib.Internal {

    public class Camera2DEffectShakeModel {

        public bool isEnable;

        public float duration;
        public float timer;

        public float frequency;
        public Vector2 amplitude;

        public Camera2DEffectShakeModel() {
            isEnable = false;
            duration = 0;
            frequency = 0;
            amplitude = Vector2.zero;
            timer = 0;
        }

    }

}