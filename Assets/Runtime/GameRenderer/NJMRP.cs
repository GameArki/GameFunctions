using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    public class NJMRP : ScriptableRendererFeature {

        public PPShakeScreenPass pp_shakeScreen;
        public PPScanLinePass pp_scanLine;

        public override void Create() {
            if (pp_shakeScreen == null) {
                pp_shakeScreen = new PPShakeScreenPass();
            }
            if (pp_scanLine == null) {
                pp_scanLine = new PPScanLinePass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            if (pp_shakeScreen != null) {
                renderer.EnqueuePass(pp_shakeScreen);
            }
            if (pp_scanLine != null) {
                renderer.EnqueuePass(pp_scanLine);
            }
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) {
            base.SetupRenderPasses(renderer, renderingData);
            if (pp_shakeScreen != null) {
                pp_shakeScreen.Setup(renderer.cameraColorTargetHandle);
            }
            if (pp_scanLine != null) {
                pp_scanLine.Setup(renderer.cameraColorTargetHandle);
            }
        }

    }
}
