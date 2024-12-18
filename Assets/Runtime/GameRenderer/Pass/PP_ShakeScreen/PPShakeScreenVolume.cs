using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    public class PPShakeScreenVolume : VolumeComponent {

        public BoolParameter isEnable = new BoolParameter(false);

        public Vector2Parameter amplitude = new Vector2Parameter(Vector2.zero);
        public FloatParameter frequency = new FloatParameter(0f);
        public FloatParameter duration = new FloatParameter(0f);
        public FloatParameter timer = new FloatParameter(0f);

    }

}