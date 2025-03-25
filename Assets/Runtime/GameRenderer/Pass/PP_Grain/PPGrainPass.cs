using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameRenderer {

    [Serializable]
    public class PPGrainPass : ScriptableRenderPass {

        [SerializeField] Shader shader;
        Material material;
        string renderTag;
        int mainTexID;
        int tempTexID;
        RenderTargetIdentifier currentTarget;
        RenderTextureDescriptor cameraTextureDescriptor;

        // Temp
        float time;

        public void Setup(RenderTargetIdentifier rt) {
#if UNITY_EDITOR
            if (shader == null) {
                shader = Shader.Find("NJM/Shader_PP_Grain");
            }
#endif
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            if (material == null) {
                material = CoreUtils.CreateEngineMaterial(shader);
                renderTag = "PP_Grain";
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
            var volume = stack.GetComponent<PPGrainVolume>();
            if (volume == null || !volume.isEnable.value) {
                return;
            }

            var cameraData = renderingData.cameraData;
            var camera = cameraData.camera;
            var cmd = CommandBufferPool.Get(renderTag);
            var src = currentTarget;
            var dst = tempTexID;

            time += Time.unscaledDeltaTime;
            float interval = volume.interval.value;
            if (time >= interval) {
                material.SetFloat("_Interval", interval * Time.time);
                material.SetFloat("_GrainIntensity", volume.intensity.value);
                material.SetFloat("_GrainSize", volume.size.value);
                time -= interval;
            }

            cmd.SetGlobalTexture(mainTexID, src);
            cmd.GetTemporaryRT(dst, cameraTextureDescriptor);
            cmd.Blit(src, dst);
            cmd.Blit(dst, src, material, 0);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }

    }
}