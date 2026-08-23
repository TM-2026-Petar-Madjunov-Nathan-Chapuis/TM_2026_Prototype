Shader "Custom/TerrainTexturePacker"
{
    Properties
    {
        _MainTex ("Heightmap", 2D) = "black" {}
        _LayerMask ("Layer Mask", 2D) = "white" {}
    }

    SubShader
    {
        ZTest Always
        Cull Off
        ZWrite Off

        HLSLINCLUDE

        #include "UnityCG.cginc"
        #include "Packages/com.unity.terrain-tools/Shaders/TerrainTools.hlsl"

        sampler2D _MainTex;
        sampler2D _LayerMask;

        struct appdata_t
        {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        v2f vert(appdata_t v)
        {
            v2f o;

            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;

            return o;
        }

        ENDHLSL

        Pass
        {
            Name "Pack"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4 frag(v2f i) : SV_Target
            {
                // IMPORTANT:
                // Unity Terrain heightmaps must be decoded with
                // UnpackHeightmap(), not sampled directly as .r.
                float height = UnpackHeightmap(
                    tex2D(_MainTex, i.uv)
                );

                float layer = tex2D(
                    _LayerMask,
                    i.uv
                ).r;

                // Store the DECODED height in R
                // and the layer mask in G.
                return float4(
                    height,
                    layer,
                    0.0,
                    1.0
                );
            }

            ENDHLSL
        }
    }
}