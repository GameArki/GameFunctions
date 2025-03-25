using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    public class PPGrainVolume : VolumeComponent {

        public BoolParameter isEnable = new BoolParameter(false);
        public FloatParameter intensity = new FloatParameter(0.5f);
        public FloatParameter size = new FloatParameter(1.0f);
        public FloatParameter interval = new FloatParameter(0.1f);

    }
}