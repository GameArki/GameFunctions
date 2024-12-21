using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    [Serializable]
    public class PPShakeScreenPass : ScriptableRenderPass {

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
                shader = Shader.Find("NJM/Shader_PP_ShakeScreen");
            }
#endif
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            if (material == null) {
                material = CoreUtils.CreateEngineMaterial(shader);
                renderTag = "PP_ShakeScreen";
                mainTexID = Shader.PropertyToID("_MainTex");
                tempTexID = Shader.PropertyToID("_TempText");
            }
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
            var volume = stack.GetComponent<PPShakeScreenVolume>();
            if (volume == null || !volume.isEnable.value) {
                return;
            }

            var cameraData = renderingData.cameraData;
            var camera = cameraData.camera;
            var cmd = CommandBufferPool.Get(renderTag);
            var src = currentTarget;
            var dst = tempTexID;

            material.SetFloat("_AmplitudeX", volume.amplitude.value.x);
            material.SetFloat("_AmplitudeY", volume.amplitude.value.y);
            material.SetFloat("_Frequency", volume.frequency.value);
            material.SetFloat("_Duration", volume.duration.value);
            material.SetFloat("_Timer", volume.timer.value);

            cmd.SetGlobalTexture(mainTexID, src);
            cmd.GetTemporaryRT(dst, cameraTextureDescriptor);
            cmd.Blit(src, dst);
            cmd.Blit(dst, src, material, 0);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }

    }

}