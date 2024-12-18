Shader "NJM/Shader_PP_ShakeScreen" {

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _AmplitudeX("_AmplitudeX", Range(0, 1)) = 0.01
        _AmplitudeY("_AmplitudeY", Range(0, 1)) = 0.01
        _Frequency("Frequency", Range(0, 100)) = 1
        _Timer("Timer", Float) = 0
        _Duration("Duration", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Cull Off
        ZWrite Off
        ZTest Always
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
            CBUFFER_END

            TEXTURE2D (_MainTex);
            SAMPLER(sampler_MainTex);
            float _AmplitudeX;
            float _AmplitudeY;
            float _Frequency;
            float _Timer;
            float _Duration;

        ENDHLSL

        Pass
        {
            Tags{ "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.texcoord;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {

                float percent = (_Duration - _Timer) / _Duration;
                _AmplitudeX = percent * _AmplitudeX;
                _AmplitudeY = percent * _AmplitudeY;

                i.uv += float2(sin(_Time.y * _Frequency) * _AmplitudeX, sin(_Time.y * _Frequency) * _AmplitudeY);

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                return col;
            }
            ENDHLSL
        }
    }
    
    Fallback "Diffuse"
}