float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position		: POSITION0;
	float4 Color		: COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color		: COLOR0;
};

//------------------------------------
// Vertex Shader
//------------------------------------

VertexShaderOutput LineRenderingVS(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.Color = input.Color;

    return output;
}

//------------------------------------
// Pixel Shader
//------------------------------------

float4 LineRenderingPS(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}

//------------------------------------
// Technique
//------------------------------------

technique LineRendering3D
{
	pass PassFor3D
	{
		VertexShader = compile vs_1_1 LineRenderingVS();
		PixelShader = compile ps_2_b LineRenderingPS();
	}
}

