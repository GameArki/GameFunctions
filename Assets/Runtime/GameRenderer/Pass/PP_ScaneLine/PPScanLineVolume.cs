using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    public class PPScanLineVolume : VolumeComponent {

        public BoolParameter isEnable = new BoolParameter(false);

    }
}