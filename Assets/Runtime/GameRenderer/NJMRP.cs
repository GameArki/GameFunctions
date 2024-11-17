using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    public class NJMRP : ScriptableRendererFeature {

        public NJMScanLinePass scanLinePass;

        public override void Create() {
            if (scanLinePass == null) {
                scanLinePass = new NJMScanLinePass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            renderer.EnqueuePass(scanLinePass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) {
            base.SetupRenderPasses(renderer, renderingData);
            scanLinePass.Setup(renderer.cameraColorTargetHandle);
        }

    }
}
