struct TessellationFactors {
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};

[patchconstantfunc("patchConstantFunction")]
[domain("tri")]//tesselating triangles
[outputcontrolpoints(3)]//patch of 3
[outputtopology("triangle_cw")] // triangle clockwise, meaning the vertex order of storage
[partitioning("fractional_odd")] // wether the factor can be int, fractional, ... see docs i guess.
PackedVaryings hull(InputPatch<PackedVaryings, 3> patch, uint id : SV_OutputControlPointID)
{
    return patch[id];
}

float CalcDistanceTessFactor(float3 worldPosition) //this figures out a factor based on the camera position to get more performance.
{
    const float minDist = 2;
    float dist = distance(worldPosition, _WorldSpaceCameraPos);
    float factor = clamp(1 - (dist - minDist) / (_MaxTessellationDist - minDist), 0.01, 1);
    
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

float invLerp(float from, float to, float value)
{
   return (value - from) / (to - from);
}

float CalculateTextureChangeTessFactor(float2 uv) //this calculates a factor based on how much the texture changes around the vertex, the more the more tesselation.
{
    const float pi = 3.14159;
    
    float4 sampleCenter = SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, uv, 0);
    float maxFactor = _MinTess;

    float degreeIncrement = 2 * pi / _RotationIterations;
    for (int i = 0; i < _RotationIterations; ++i)
    {
        float radians = degreeIncrement * i;
        half2 offset = half2(cos(radians), sin(radians)) * _CheckDistance;
        
        float4 sample = SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, uv + offset, 0);
        float centerSampleDisplacement = sampleCenter.g - sampleCenter.r;
        float sampleDisplacement = sample.g - sample.r;
        float delta = abs(sampleDisplacement - centerSampleDisplacement);
        float tessellationFactor = invLerp(0, 1, delta) * _ProximityTessellation;
        maxFactor = max(tessellationFactor, maxFactor);
    }
    
    return maxFactor;
}
//this takes the factors for each vertex, and based on a linear average figures out the factor for the whole triangle (center and three edges)
TessellationFactors CalcTriEdgeTessFactors(float3 vertexTessFactors)
{
    TessellationFactors tess;
    
    tess.edge[0] = 0.5 * (vertexTessFactors.y + vertexTessFactors.z);
    tess.edge[1] = 0.5 * (vertexTessFactors.x + vertexTessFactors.z);
    tess.edge[2] = 0.5 * (vertexTessFactors.x + vertexTessFactors.y);
    tess.inside = (vertexTessFactors.x + vertexTessFactors.y + vertexTessFactors.z) / 3.0f;
    
    return tess;
}

TessellationFactors DistanceBasedTess(PackedVaryings vertex0, PackedVaryings vertex1, PackedVaryings vertex2)
{
    float3 vertexTessFactors;
    //linear between distance and texture change.
    vertexTessFactors.x = CalcDistanceTessFactor(vertex0.positionWS) * CalculateTextureChangeTessFactor(CalcCameraUVFromWorld(vertex0.positionWS)); 
    vertexTessFactors.y = CalcDistanceTessFactor(vertex1.positionWS) * CalculateTextureChangeTessFactor(CalcCameraUVFromWorld(vertex1.positionWS));
    vertexTessFactors.z = CalcDistanceTessFactor(vertex2.positionWS) * CalculateTextureChangeTessFactor(CalcCameraUVFromWorld(vertex2.positionWS));
    
    return CalcTriEdgeTessFactors(vertexTessFactors);
}

TessellationFactors patchConstantFunction(InputPatch<PackedVaryings, 3> patch)
{
	return DistanceBasedTess(patch[0], patch[1], patch[2]);
}

void vertTess(inout PackedVaryings IN)
{    
    float2 uv = CalcCameraUVFromWorld(IN.positionWS);
    float4 textureValue = SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, uv, 0);
    IN.positionWS += IN.normalWS * -_SnowRedForce * textureValue.r;
    IN.positionWS += IN.normalWS * _SnowGreenForce * textureValue.g;
    
    float3 objectPos = TransformWorldToObject(IN.positionWS);
    // object to clip space
    IN.positionCS = TransformObjectToHClip(objectPos);
}

#define INTERPOLATE(fieldName) data.fieldName = \
	patch[0].fieldName * barycentricCoordinates.x + \
	patch[1].fieldName * barycentricCoordinates.y + \
	patch[2].fieldName * barycentricCoordinates.z;

[domain("tri")]
//This domain shader gets called on each vertex being created by the gpu after it has got the tessellation factors.
//Each vertex gets interpolated variables based on its barycentricCoordinates (lookup on wikipedia), and the position gets an offset in the vertexShader.
PackedVaryings domain(TessellationFactors factors, OutputPatch<PackedVaryings, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)  
{
    PackedVaryings data;
    ZERO_INITIALIZE(PackedVaryings, data); //sets each value in memory to null.
    
    INTERPOLATE(positionCS) //position will get height reajusted in the vertex shader
    INTERPOLATE(normalWS) //not really necessary the vertex will be run each into the vertex shader which will offset them, rendering this orginial value useless.
    INTERPOLATE(tangentWS) //not really necessary the vertex will be run each into the vertex shader which will offset them, rendering this orginial value useless.
    INTERPOLATE(texCoord0) //UVs
    INTERPOLATE(positionWS) //position will get height reajusted in the vertex shader
    
    vertTess(data);
    
    return data;
}