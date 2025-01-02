using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    public class PPFilmBorderVolume : VolumeComponent {

        public BoolParameter isEnable = new BoolParameter(false);

        public FloatRangeParameter borderOffset = new FloatRangeParameter(new Vector2(0, 1), 0, 1);

    }

}