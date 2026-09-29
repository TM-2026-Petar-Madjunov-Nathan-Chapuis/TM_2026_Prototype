//Ce shader a été majoritairement été généré par une IA générative, GPT-5.6 Terra, avec l'accord du tuteur. Toutefois nous comprenons parfaitement son fonctionnement, seul les calculs sont obscures et n'ont pas la place dans ce tm.

Shader "CustomEffects/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VerticalBlur ("Vertical Blur", Float) = 1
        _HorizontalBlur ("Horizontal Blur", Float) = 1
    }

    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);

    float _VerticalBlur;
    float _HorizontalBlur;

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

    Varyings Vert(Attributes v)
    {
        Varyings o;
        o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
        o.uv = v.uv;
        return o;
    }

    float4 BlurVertical(Varyings i) : SV_Target
    {
        const int SAMPLES = 64;
        float2 texel = float2(0, _VerticalBlur / _ScreenParams.y);

        float3 col = 0;

        for (int s = -SAMPLES; s <= SAMPLES; s++)
        {
            float2 offset = float2(0, texel.y * s);
            col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + offset).rgb;
        }

        col /= (SAMPLES * 2 + 1);

        return float4(col, 1);
    }

    float4 BlurHorizontal(Varyings i) : SV_Target
    {
        const int SAMPLES = 64;
        float2 texel = float2(_HorizontalBlur / _ScreenParams.x, 0);

        float3 col = 0;

        for (int s = -SAMPLES; s <= SAMPLES; s++)
        {
            float2 offset = float2(texel.x * s, 0);
            col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + offset).rgb;
        }

        col /= (SAMPLES * 2 + 1);

        return float4(col, 1);
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        ZWrite Off
        Cull Off

        Pass
        {
            Name "Vertical"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment BlurVertical
            ENDHLSL
        }

        Pass
        {
            Name "Horizontal"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment BlurHorizontal
            ENDHLSL
        }
    }
}