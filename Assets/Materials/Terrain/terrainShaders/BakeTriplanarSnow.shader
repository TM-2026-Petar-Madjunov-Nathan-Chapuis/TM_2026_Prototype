 Shader "Custom/BakeTriplanarSnow"
{
    Properties
    {
        [MainTexture] _MainTex("Source Texture", 2D) = "white" {}
        _Normal_Displacement("Normal UV displacement", Float) = 0.01
        _TerrainSize("Terrain size (width, height, length)", Vector) = (1, 1, 1, 0)
        [Header(Snow)]
        _Snow_Surface("Snow Surface (0=None 1=full)", Float) = 0.4
        _Snow_Contrast("Snow Contrast", Float) = 4
        _Snow_Direction("Snow Direction", Vector) = (0,1,0,0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        ZTest Always
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Packages/com.unity.terrain-tools/Shaders/TerrainTools.hlsl"

            sampler2D _MainTex;

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float _Snow_Surface;
                float _Snow_Contrast;
                float _Normal_Displacement;
                float4 _TerrainSize;
                float3 _Snow_Direction;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                // Normal calculation
                float2 leftUV  = saturate(IN.uv - float2(_Normal_Displacement, 0));
                float2 rightUV = saturate(IN.uv + float2(_Normal_Displacement, 0));
                float2 upUV    = saturate(IN.uv - float2(0, _Normal_Displacement));
                float2 downUV  = saturate(IN.uv + float2(0, _Normal_Displacement));

                float left  = UnpackHeightmap(tex2D(_MainTex, leftUV));
                float right = UnpackHeightmap(tex2D(_MainTex, rightUV));
                float up    = UnpackHeightmap(tex2D(_MainTex, upUV));
                float down  = UnpackHeightmap(tex2D(_MainTex, downUV));

                float sampleDistance = max(abs(_Normal_Displacement), 0.000001);
                float slopeX = (right - left) * _TerrainSize.y / (2.0 * sampleDistance * _TerrainSize.x);
                float slopeY = (down - up) * _TerrainSize.y / (2.0 * sampleDistance * _TerrainSize.z);

                float3 normal = normalize(float3(-slopeX, 1.0, -slopeY));

                float3 snowDirection = normalize(_Snow_Direction);
                //triplanar, a bit obscure
                float upwardness = saturate(dot(normal, snowDirection));
                float threshold = 1.0 - saturate(_Snow_Surface);
                float snowMask = smoothstep(0.49, 0.51, (upwardness - threshold) * _Snow_Contrast);

                return normalize(float4(snowMask, 1.0 - snowMask, 0.0, 1.0));
            }
            ENDHLSL
        }
}
}