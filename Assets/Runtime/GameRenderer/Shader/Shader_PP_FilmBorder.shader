Shader "NJM/Shader_PP_FilmBorder" {

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _BorderSize("BorderSize", Range(0, 1)) = 0.1
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
                float _BorderSize;
            CBUFFER_END

            TEXTURE2D (_MainTex);
            SAMPLER(sampler_MainTex);

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
                half4 col = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv);
                if (i.uv.y < _BorderSize || i.uv.y > 1 - _BorderSize)
                {
                    col = half4(0, 0, 0, 1);
                }

                return col;
            }
            ENDHLSL
        }
    }
    
    Fallback "Diffuse"
}