using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    [Serializable]
    public class FWDGrassWavePass : ScriptableRenderPass {

        public void Setup(RenderTargetIdentifier rt) {

        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            // 平行风: 风频, 风向, 风速

            // 踩踏: 位置, 速度
        }

    }
}