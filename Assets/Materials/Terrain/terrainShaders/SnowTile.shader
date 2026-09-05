Shader "Custom/DeformationTesselation"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [Header(Snow)]
        [MainTexture] _BaseMap("Snow HeightMap", 2D) = "white" {}
        _Albedo("Snow Texture Albedo", 2D) = "white" {}
        _Normal("Snow Texture Normal", 2D) = "bump" {}
        _SnowTextureTileSize("Snow Texture Tile Size", float) = 5
        _NormalStrength("Snow normals strength", float) = 1
        _SnowHeight ("Snow height", Float) = 0
        _SnowRedForce ("Snow red force", Float) = 0
        _SnowGreenForce ("Snow green force", Float) = 0
        [Header(Terrain)]
        _TerrainTexture ("Terrain Texture", 2D) = "white" {}
        _TerrainPos ("Terrain Pos", Vector) = (0,0,0,0)
        _TerrainSize ("Terrain Size", Vector) = (0,0,0,0)
        _HeightMapMaxHeight ("Terrain Heightmap max height", Float) = 0
        [Header(Camera)]
        _OrthographicCameraPos ("Orthographic Camera pos", Vector) = (0,0,0,0)
        _OrthographicCameraSize ("Orthographic Camera size", Vector) = (0,0,0,0)
        [Header(TessFactor)]
        [Range(1, 64)] _Tessellation ("Tesselation", Float) = 1
        _MaxTessellationDistance ("Tessellation Distance", Float) = 20
        _HeightMapRotationIteration ("Height map Rotation iterations", Integer) = 2
        _LayerTextureRotationIteration ("Layer Texture Rotation iterations", Integer) = 2
        _MinTess ("Min Tessellation", Float) = 0.01
        _HeightMapCheckDistance ("Height Map Check Distance", Float) = 0.1
        _LayerTextureCheckDistance ("Layer Texture Check Distance", Float) = 0.1
        _ProximityTessellation ("Proximity Tessellation", Float) = 5
        _LayerTextureFactor("Layer Texture Factor", Float) = 5
        [Header(Normals)]
        _NormalCheckDistance ("Normal Check Distance", Float) = 0.1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }


        Pass
        {
            HLSLPROGRAM

            #pragma vertex dummyVert
            #pragma fragment frag
            #pragma hull hull
            #pragma domain domain

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
            };

            struct TessellationFactors
            {
                float edge[3] : SV_TESSFACTOR;
                float inside : SV_INSIDETESSFACTOR;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_TerrainTexture);
            SAMPLER(sampler_TerrainTexture);

            TEXTURE2D(_Albedo);
            SAMPLER(sampler_Albedo);

            TEXTURE2D(_Normal);
            SAMPLER(sampler_Normal);

            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            float _NormalStrength;
            float _SnowTextureTileSize;
            float4 _BaseMap_ST;
            float4 _TerrainTexture_ST;
            float4 _Albedo_ST;
            float4 _Normal_ST;
            float3 _TerrainPos;
            float2 _TerrainSize;
            float3 _OrthographicCameraPos;
            float2 _OrthographicCameraSize;
            float _SnowHeight;
            float _SnowRedForce;
            float _SnowGreenForce;
            float _HeightMapMaxHeight;
            float _Tessellation;
            float _MaxTessellationDistance;
            int _HeightMapRotationIteration;
            int _LayerTextureRotationIteration;
            float _MinTess;
            float _HeightMapCheckDistance;
            float _LayerTextureCheckDistance;
            float _ProximityTessellation;
            float _NormalCheckDistance;
            float _LayerTextureFactor;
            CBUFFER_END

            [patchconstantfunc("patchConstantFunction")] //this basically takes in the patch and calculates a tesselation factor for the triangle, telling the gpu how much to tesselate it.
            [domain("tri")]//tesselating triangles
            [outputcontrolpoints(3)]//patch of 3
            [outputtopology("triangle_cw")] // triangle clockwise, meaning the vertex order of storage
            [partitioning("integer")] // wether the factor can be int, fractional, ... see docs i guess.
            Attributes hull(InputPatch<Attributes, 3> patch, uint id : SV_OUTPUTCONTROLPOINTID) //takes in 3 vertex which is a triangle, and an id
            {
                return patch[id];
            }

            //this takes the factors for each vertex, and based on a linear average figures out the factor for the whole triangle (center and three edges)
            TessellationFactors CalcTriEdgeTessFactors(float3 vertexTessFactors) {
                TessellationFactors f;
                f.edge[0] = 0.5 * (vertexTessFactors.y + vertexTessFactors.z);
                f.edge[1] = 0.5 * (vertexTessFactors.x + vertexTessFactors.z);
                f.edge[2] = 0.5 * (vertexTessFactors.x + vertexTessFactors.y);
                f.inside = (vertexTessFactors.x + vertexTessFactors.y + vertexTessFactors.z) / 3;

                return f;
            }

            float CalcDistanceTessFactor(float3 worldPosition) //this figures out a factor based on the camera position to get more performance.
            {
                const float minDist = 2;
                float dist = distance(worldPosition, _WorldSpaceCameraPos);
                float factor = clamp(1 - (dist - minDist) / (_MaxTessellationDistance - minDist), 0.01, 1);

                return clamp(factor * _Tessellation, 0, _Tessellation);
            }

            float2 CalcCameraUVFromWorld(float3 worldPosition)
            {
                float3 cameraLocal = worldPosition - _OrthographicCameraPos;

                float2 uv;
                uv.x = cameraLocal.x / _OrthographicCameraSize.x * 0.5 + 0.5;
                uv.y = cameraLocal.z / _OrthographicCameraSize.y * 0.5 + 0.5;

                return uv;
            }

            float2 CalcTerrainUVFromWorld(float3 worldPosition)
            {
                float3 terrainLocal = worldPosition - _TerrainPos;

                float2 uv;
                uv.x = terrainLocal.x / _TerrainSize.x * 0.5 + 0.5;
                uv.y = terrainLocal.z / _TerrainSize.y * 0.5 + 0.5;

                return uv;
            }

            float invLerp(float from, float to, float value)
            {
                return (value - from) / (to - from);
            }

            float CalculateTextureChangeTessFactor(float2 uv) //this calculates a factor based on how much the texture changes around the vertex, the more the more tesselation.
            {
                const float pi = 3.14159;

                float4 sampleCenter = SAMPLE_DEPTH_TEXTURE_LOD(_BaseMap, sampler_BaseMap, uv, 0);
                float maxFactor = _MinTess;

                int interations = _HeightMapRotationIteration;
                interations = max(0, interations);

                float degreeIncrement = 2 * pi / interations;
                for (int i = 0; i < interations; ++i)
                {
                    float radians = degreeIncrement * i;
                    half2 offset = half2(cos(radians), sin(radians)) * _HeightMapCheckDistance;

                    float4 sample = SAMPLE_DEPTH_TEXTURE_LOD(_BaseMap, sampler_BaseMap, uv + offset, 0);
                    float delta = length(sampleCenter - sample);
                    float tessFactor = delta * 0.5 * _ProximityTessellation;

                    maxFactor = max(maxFactor, tessFactor);
                };
                return maxFactor;
            };
            float CalculateTerrainLayerTessFactor(float2 uv)
            {
                const float pi = 3.14159;

                float center = SAMPLE_TEXTURE2D_LOD(_TerrainTexture, sampler_TerrainTexture, uv, 0).g;

                float maxChange = 0;
                int iterations = max(1, _LayerTextureRotationIteration);
                float degreeIncrement = 2 * pi / iterations;

                for (int i = 0; i < iterations; ++i)
                {
                    float radians = degreeIncrement * i;
                    half2 offset = half2(cos(radians), sin(radians)) * _LayerTextureCheckDistance;

                    float sample = SAMPLE_TEXTURE2D_LOD( _TerrainTexture, sampler_TerrainTexture, uv + offset, 0).g;
                    float delta = length(center - sample);
                    maxChange = max(maxChange, delta);
                }
                return maxChange * _LayerTextureFactor;
            }

            TessellationFactors DistanceBasedTess(Attributes vertex0, Attributes vertex1, Attributes vertex2)
            {
                float3 vertexTessFactors;
                float3 positionWorldSpace0 = mul(unity_ObjectToWorld, vertex0.positionOS);
                float3 positionWorldSpace1 = mul(unity_ObjectToWorld, vertex1.positionOS);
                float3 positionWorldSpace2 = mul(unity_ObjectToWorld, vertex2.positionOS);

                float2 uv0 = CalcCameraUVFromWorld(positionWorldSpace0);
                float2 uv1 = CalcCameraUVFromWorld(positionWorldSpace1);
                float2 uv2 = CalcCameraUVFromWorld(positionWorldSpace2);

                float2 terrainUv0 = CalcTerrainUVFromWorld(positionWorldSpace0); //takes the center position of the triangle to optimize rendering
                float2 terrainUv1 = CalcTerrainUVFromWorld(positionWorldSpace1);
                float2 terrainUv2 = CalcTerrainUVFromWorld(positionWorldSpace2);

                float terrainHeight0 = SAMPLE_TEXTURE2D_LOD(_TerrainTexture, sampler_TerrainTexture, terrainUv0, 0).r;
                float terrainHeight1 = SAMPLE_TEXTURE2D_LOD(_TerrainTexture, sampler_TerrainTexture, terrainUv1, 0).r;
                float terrainHeight2 = SAMPLE_TEXTURE2D_LOD(_TerrainTexture, sampler_TerrainTexture, terrainUv2, 0).r;

                float3 displacement0 = float3(0, 1, 0) * terrainHeight0 * _HeightMapMaxHeight * 2;
                float3 displacement1 = float3(0, 1, 0) * terrainHeight1 * _HeightMapMaxHeight * 2;
                float3 displacement2 = float3(0, 1, 0) * terrainHeight2 * _HeightMapMaxHeight * 2;

                vertexTessFactors.x = (CalcDistanceTessFactor(positionWorldSpace0 + displacement0) * max(CalculateTerrainLayerTessFactor(terrainUv0), CalculateTextureChangeTessFactor(uv0)));
                vertexTessFactors.y = (CalcDistanceTessFactor(positionWorldSpace1 + displacement1) * max(CalculateTerrainLayerTessFactor(terrainUv1), CalculateTextureChangeTessFactor(uv1)));
                vertexTessFactors.z = (CalcDistanceTessFactor(positionWorldSpace2 + displacement2) * max(CalculateTerrainLayerTessFactor(terrainUv2), CalculateTextureChangeTessFactor(uv2)));

                return CalcTriEdgeTessFactors(vertexTessFactors);
            }

            TessellationFactors patchConstantFunction(InputPatch<Attributes, 3> patch)
            {
                return DistanceBasedTess(patch[0], patch[1], patch[2]);
            }

            Attributes dummyVert(Attributes IN)
            {
                return IN;
            }

            float3 CalcDisplacedPositionWS(float3 positionWS)
            {
                float3 normalWS = float3(0, 1.0, 0); //shure not real but this shader's only meant for flat, upward pointing uvs.
                float2 terrainUv = CalcTerrainUVFromWorld(positionWS);
                float4 terrainTexture = SAMPLE_TEXTURE2D_LOD(_TerrainTexture, sampler_TerrainTexture, terrainUv, 0);

                positionWS += normalWS * terrainTexture.r * _HeightMapMaxHeight * 2; //somehow * 2 is needed here.
                if (terrainTexture.g > 0.001) {
                    positionWS += normalWS * terrainTexture.g * _SnowHeight;
                }

                if (length(positionWS - _OrthographicCameraPos) <= _OrthographicCameraSize.x) { //assumes square orthographic camera
                float2 camUv = CalcCameraUVFromWorld(positionWS);
                float4 snowTexture = SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, camUv, 0);


                positionWS += (normalWS * -_SnowRedForce * snowTexture.r) * terrainTexture.g;
                positionWS += (normalWS * _SnowGreenForce * snowTexture.g) * terrainTexture.g; //avoid snow mouting up where there is none
                };
                return positionWS;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = mul(unity_ObjectToWorld, float4(IN.positionOS.xyz, 1.0));
                float3 offsetRight = float3(_NormalCheckDistance, 0, 0);
                float3 offsetLeft = float3(-_NormalCheckDistance, 0, 0);
                float3 offsetUp = float3(0, 0, _NormalCheckDistance);
                float3 offsetDown = float3(0, 0, -_NormalCheckDistance);

                float3 displacedPosWS = CalcDisplacedPositionWS(positionWS);
                float3 displacedPosWSRight = CalcDisplacedPositionWS(positionWS + offsetRight);
                float3 displacedPosWSLeft = CalcDisplacedPositionWS(positionWS + offsetLeft);
                float3 displacedPosWSUp = CalcDisplacedPositionWS(positionWS + offsetUp);
                float3 displacedPosWSDown = CalcDisplacedPositionWS(positionWS + offsetDown);

                float3 tangentX = displacedPosWSRight - displacedPosWSLeft;
                float3 tangentZ = displacedPosWSUp - displacedPosWSDown;

                OUT.normalWS = normalize(cross(tangentZ, tangentX));
                OUT.tangentWS = normalize(tangentX);
                OUT.bitangentWS = normalize(cross(OUT.tangentWS, OUT.normalWS));

                OUT.positionHCS = TransformWorldToHClip(displacedPosWS);
                OUT.uv = IN.uv;

                OUT.positionWS = displacedPosWS;
                return OUT;
            }

            #define INTERPOLATE(fieldName) \
            data.fieldName = \
            patch[0].fieldName * barycentricCoordinates.x + \
            patch[1].fieldName * barycentricCoordinates.y + \
            patch[2].fieldName * barycentricCoordinates.z;

            [domain("tri")]
            Varyings domain(TessellationFactors factors, OutputPatch<Attributes, 3> patch, float3 barycentricCoordinates : SV_DOMAINLOCATION)
            {
                Attributes data;
                INTERPOLATE(positionOS)
                INTERPOLATE(normal)
                INTERPOLATE(uv)

                return vert(data);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //this removes the vertices from showing on the screen when the mask is below the threshold of 0.0001
                float2 terrainUV = CalcTerrainUVFromWorld(IN.positionWS);
                float snowMask = SAMPLE_TEXTURE2D_LOD(_TerrainTexture, sampler_TerrainTexture, terrainUV, 0).g;
                //stop the vertex from rendering at all
                clip(snowMask - 0.001);

                float snowTileSize = max(_SnowTextureTileSize, 0.001);
                float2 snowUV = (IN.positionWS.xz - _TerrainPos.xz) / snowTileSize; //makes the uvs tiling, if not the whole texture would span the whole terrain
                float2 normalUV = frac(TRANSFORM_TEX(snowUV, _Normal));
                float2 albedoUV = frac(TRANSFORM_TEX(snowUV, _Albedo));

                float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, normalUV), _NormalStrength);
                float4 albedo = SAMPLE_TEXTURE2D(_Albedo, sampler_Albedo, albedoUV);

                float3 normalWS = normalize(
                    IN.tangentWS   * normalTS.x +
                    IN.bitangentWS * normalTS.y +
                    IN.normalWS    * normalTS.z
                );

                Light mainLight = GetMainLight();

                float3 lightDir = normalize(mainLight.direction); //direction of the light
                float lightIntensity = Luminance(mainLight.color.rgb); //calculates a light intensity based on the color of the light

                float NdotL = saturate(dot(normalWS, lightDir));  //produit scalaire pour calculer la lumière 1 si même direction, 0 si opposé

                float3 color = albedo.rgb * lightIntensity * saturate(NdotL + 0.15);

                return float4(color, 1.0);
                return float4(normalWS * 0.5 + 0.5, 1.0);
            }
            ENDHLSL
        }
}
}