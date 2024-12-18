using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    [Serializable]
    public class PPScanLinePass : ScriptableRenderPass {

        [SerializeField] Shader shader;
        Material material;
        string renderTag;
        int mainTexID;
        int tempTexID;
        RenderTargetIdentifier currentTarget;
        RenderTextureDescriptor cameraTextureDescriptor;

        public void Setup(RenderTargetIdentifier rt) {
#if UNITY_EDITOR
            if (shader == null) {
                shader = Shader.Find("NJM/Shader_PP_ScanLine");
            }
#endif
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            material = CoreUtils.CreateEngineMaterial(shader);
            renderTag = "NJM ScanLine";
            mainTexID = Shader.PropertyToID("_MainTex");
            tempTexID = Shader.PropertyToID("_TempText");
            currentTarget = rt;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            this.cameraTextureDescriptor = cameraTextureDescriptor;
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
            var volume = stack.GetComponent<PPScanLineVomume>();
            if (!volume.isEnable.value) {
                return;
            }

            var cameraData = renderingData.cameraData;
            var camera = cameraData.camera;
            var cmd = CommandBufferPool.Get(renderTag);
            var src = currentTarget;
            var dst = tempTexID;

            cmd.SetGlobalTexture(mainTexID, src);
            // cmd.GetTemporaryRT(dst, camera.scaledPixelWidth, camera.scaledPixelHeight, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.GetTemporaryRT(dst, cameraTextureDescriptor);
            cmd.Blit(src, dst);
            cmd.Blit(dst, src, material, 0);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }

    }
}