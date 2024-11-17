using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    [Serializable]
    public class NJMScanLinePass : ScriptableRenderPass {

        Shader shader;
        Material material;
        string renderTag;
        int mainTexID;
        int tempTexID;
        RenderTargetIdentifier currentTarget;

        public void Setup(RenderTargetIdentifier rt) {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            shader = Shader.Find("NJM/Shader_PP_ScanLine");
            material = CoreUtils.CreateEngineMaterial(shader);
            renderTag = "NJM ScanLine";
            mainTexID = Shader.PropertyToID("_MainTex");
            tempTexID = Shader.PropertyToID("_TempText");
            currentTarget = rt;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

            bool isEnablePP = renderingData.cameraData.postProcessEnabled;
            if (!isEnablePP) {
                return;
            }

            if (material == null) {
                return;
            }

            var stack = VolumeManager.instance.stack;
            var volume = stack.GetComponent<NJMScanLineVomume>();
            if (!volume.isEnable.value) {
                return;
            }

            var cameraData = renderingData.cameraData;
            var camera = cameraData.camera;
            var cmd = CommandBufferPool.Get(renderTag);
            var src = currentTarget;
            var dst = tempTexID;

            cmd.SetGlobalTexture(mainTexID, src);
            cmd.GetTemporaryRT(dst, camera.scaledPixelWidth, camera.scaledPixelHeight,0,FilterMode.Point, RenderTextureFormat.Default);
            // cmd.GetTemporaryRT(dst, cameraData.cameraTargetDescriptor);
            cmd.Blit(src, dst);
            cmd.Blit(dst, src, material, 0);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }

    }
}