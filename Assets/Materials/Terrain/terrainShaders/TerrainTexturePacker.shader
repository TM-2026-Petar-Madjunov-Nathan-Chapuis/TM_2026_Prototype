Shader "Custom/TerrainTexturePacker"
{
    Properties
    {
        _MainTex ("Heightmap", 2D) = "black" {}
        _LayerMask ("Layer Mask", 2D) = "white" {}
        _TerrainLayerMask ("Terrain Layer Mask", 2D) = "white" {}
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
        sampler2D _TerrainLayerMask;

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
                // Unity Terrain heightmaps must be decoded with
                // UnpackHeightmap(), not sampled directly as .r.
                // coming from : https://docs.unity3d.com/Packages/com.unity.terrain-tools@4.0/manual/create-filterstacks-and-filters.html
                float height = UnpackHeightmap(tex2D(_MainTex, i.uv));
                float layer = tex2D(_LayerMask, i.uv).r;
                float terrainMask = tex2D(_TerrainLayerMask, i.uv).r;

                return float4(height, layer * terrainMask, 0.0, 1.0);
            }

            ENDHLSL
        }
    }
}