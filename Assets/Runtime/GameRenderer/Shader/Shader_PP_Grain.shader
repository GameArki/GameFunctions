Shader "NJM/Shader_PP_Grain"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _GrainIntensity ("Grain Intensity", Range(0, 1)) = 0.03
        _GrainSize ("Grain Size", Range(0.1, 10)) = 1.0
        _Interval ("Interval", Range(0, 1)) = 0.1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _GrainIntensity;
            float _GrainSize;
            float _Interval;

            // 随机噪点函数（简化版）
            float rand(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 原始颜色
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 生成噪点（基于UV和时间变化）
                float2 grainUV = i.uv * _GrainSize;
                float noise = rand(grainUV + _Interval);
                
                // 混合噪点到颜色
                col.rgb += (noise - 0.5) * _GrainIntensity;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}