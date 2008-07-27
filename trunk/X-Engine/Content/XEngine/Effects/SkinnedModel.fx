//-----------------------------------------------------------------------------
// SkinnedModel.fx
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Maximum number of bone matrices we can render using shader 2.0 in a single pass.
// If you change this, update SkinnedModelProcessor.cs to match.
#define MaxBones 59

float4x4 World; 
float4x4 View; 
float4x4 Projection; 

float4 vecLightDir[3];
float4 LightColor[3];
float4 vecEye;

float4x4 Bones[MaxBones];

float NumLights;

texture Texture;

sampler Sampler = sampler_state
{
    Texture = (Texture);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
};


// Vertex shader input structure.
struct VS_INPUT
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float4 BoneIndices : BLENDINDICES0;
    float4 BoneWeights : BLENDWEIGHT0;
};


// Vertex shader output structure.
struct VS_OUTPUT
{
	float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 View : TEXCOORD2;
};


// Vertex shader program.
VS_OUTPUT VertexShader(VS_INPUT input)
{
    VS_OUTPUT output;
    
    // Blend between the weighted bone matrices.
    float4x4 skinTransform = 0;
    
    skinTransform += Bones[input.BoneIndices.x] * input.BoneWeights.x;
    skinTransform += Bones[input.BoneIndices.y] * input.BoneWeights.y;
    skinTransform += Bones[input.BoneIndices.z] * input.BoneWeights.z;
    skinTransform += Bones[input.BoneIndices.w] * input.BoneWeights.w;
    
    // Skin the vertex position.
    float4 position = mul(input.Position, skinTransform);
    
    output.Position = mul(mul(position, View), Projection);

    // Skin the vertex normal, then compute lighting.
    float3 normal = normalize(mul(input.Normal, skinTransform));
    output.Normal = normal;

    output.TexCoord = input.TexCoord;
    
    output.View = vecEye - position;
    
    return output;
}


// Pixel shader program.
float4 PixelShader(VS_OUTPUT input) : COLOR0
{
	float4 Color = float4 (0, 0, 0, 1);
	
	for (float i = 0; i < NumLights; i++)
	{
		float4 diffuse = {LightColor[i].rgb, 1.0f};
		float4 ambient = { 0.1f, 0.1f, 0.1f, 1.0f};

		float3 Normal = normalize(input.Normal);
		float3 LightDir = normalize(vecLightDir[i]);
		float3 ViewDir = normalize(input.View); 
    
		float4 diff = saturate(dot(Normal, LightDir)); // diffuse component

		// R = 2 * (N.L) * N - L
		float3 Reflect = normalize(2 * diff * Normal - LightDir); 
		float4 specular = pow(saturate(dot(Reflect, ViewDir)), LightColor[i].a); // R.V^n

		// I = Acolor + Dcolor * N.L + (R.V)n
		Color += ambient + diffuse  * diff + specular; 
	}
	
	float4 Final = Color * tex2D(Sampler, input.TexCoord);
	
	return Final;
}


technique SkinnedModelTechnique
{
    pass SkinnedModelPass
    {	
        VertexShader = compile vs_1_1 VertexShader();
        PixelShader = compile ps_2_b PixelShader();
    }
}
