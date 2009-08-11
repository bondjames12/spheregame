float4x4 World;
float4x4 View;
float4x4 Projection;

Texture tex;
sampler texsamp = sampler_state { texture = <tex>; };

float3 EyePosition : CameraPosition;

Texture skyCubeTexture;
samplerCUBE skyCubeSampler = sampler_state 
{ 
	texture = <skyCubeTexture> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
};

VertexShaderOutput VSSky(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.Texcoord = input.Texcoord;

    return output;
}

float4 PSSky(VertexShaderOutput input) : COLOR0
{
	float4 Tex = tex2D(texsamp, input.Texcoord);
	
    return Tex * 1.05;
}

//---------------------------------------------------------------------------------------
//Sky Cube Map Technique-----------------------------------------------------------------
//if we are using CubeMappedSky we need EyePosition and skyCubeTexture

struct VSSkyCubeInput
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
};

struct VSSkyCubeOutput
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 ViewDirection : TEXCOORD1;
};
float4 CubeMapLookup(float3 CubeTexcoord)
{    
    return texCUBE(skyCubeSampler, CubeTexcoord);
}
VSSkyCubeOutput VSSkyCube(VSSkyCubeInput input)
{
    VSSkyCubeOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.Texcoord = input.Texcoord;
    output.ViewDirection = EyePosition - worldPosition;
    return output;
}
float4 PSSkyCube(VSSkyCubeOutput input) : COLOR0
{
	float3 ViewDirection = normalize(input.ViewDirection);	
   	return CubeMapLookup(-ViewDirection);
}
//---------------------------------------------------------------------------------------------

technique Sky
{
    pass Pass1
    {
        CullMode = None;
        VertexShader = compile vs_3_0 VSSky();
        PixelShader = compile ps_3_0 PSSky();
    }
}

technique SkyCubeMap
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VSSkyCube();
        PixelShader = compile ps_3_0 PSSkyCube();
    }
}
