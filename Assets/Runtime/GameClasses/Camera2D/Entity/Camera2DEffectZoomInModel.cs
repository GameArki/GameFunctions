using System;
using UnityEngine;
using GameFunctions;

namespace GameClasses.Camera2DLib.Internal {

    public class Camera2DEffectZoomInModel {

        public bool isEnable;

        public GFEasingEnum easingType;
        public float duration;
        public float timer;

        public float targetMultiply;

        public bool isAutoRestore;
        public float restoreDelaySec;
        public float restoreDelayTimer;
        public float restoreDuration;
        public GFEasingEnum restoreEasingType;

    }

}