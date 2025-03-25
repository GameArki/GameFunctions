using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    public class PPGrainVolume : VolumeComponent {

        public BoolParameter isEnable = new BoolParameter(false);
        public FloatParameter interval = new FloatParameter(0.1f);

    }
}