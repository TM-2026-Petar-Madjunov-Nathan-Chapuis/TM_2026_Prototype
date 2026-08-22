Shader "Custom/ChannelPacking"
{
    Properties {
        _HeightMap ("Height Map", 2D) = "white" {}
        _LayerMask ("Layer Mask", 2D) = "white" {}
    }
    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        ENDHLSL

        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "ChannelPacking"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            TEXTURE2D(_HeightMap);
            SAMPLER(sampler_LinearClamp);

            TEXTURE2D(_LayerMask);

            float4 Frag (Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgba;
                float4 added = float4(color.r, color.g, SAMPLE_TEXTURE2D(_HeightMap, sampler_LinearClamp, input.texcoord).b, SAMPLE_TEXTURE2D(_LayerMask, sampler_LinearClamp, input.texcoord).r);
                return float4(1,1,1,1);
            }

            ENDHLSL
        }
    }
}
