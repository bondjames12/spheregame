float4x4 worldViewProjection;


struct VertexShaderInput
{
    float4 position		: POSITION0;
	float2 texCoord		: TEXCOORD0;
	float3 normal		: NORMAL0;
	float3 binormal		: BINORMAL0;
	float3 tangent		: TANGENT0;
};

struct VertexShaderOutput
{
    float4 position : POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

	output.position = mul(input.position, worldViewProjection);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return float4(1, 0, 0, 1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
}
