Shader "Custom/ChannelPacker"
{
    Properties
    {
        _HeightMap("Height Map", 2D) = "white" {}
        _LayerMask("LayerMask", 2D) = "white" {}
        [MainTexture] _CameraTexture("Camera Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_HeightMap);
            SAMPLER(sampler_HeightMap);
            TEXTURE2D(_LayerMask);
            SAMPLER(sampler_LayerMask);
            TEXTURE2D(_CameraTexture);
            SAMPLER(sampler_CameraTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 _HeightMap;
                float4 _LayerMask;
                float4 _CameraTexture;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_CameraTexture, sampler_CameraTexture, IN.uv);
                half4 added = half4(color.r, color.g, SAMPLE_TEXTURE2D(_HeightMap, sampler_HeightMap, IN.uv).b, SAMPLE_TEXTURE2D(_LayerMask, sampler_LayerMask, IN.uv).r)
                return added;
            }
            ENDHLSL
        }
    }
}
