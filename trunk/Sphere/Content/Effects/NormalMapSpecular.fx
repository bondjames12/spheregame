
float4x4 worldViewProjection	: WORLDVIEWPROJECTION;
float4x4 world					: WORLD;
float4x4 view					: VIEW;

float specularPower				: SPECULAR_POWER;
float4 specularColor			: SPECULAR_COLOR;

float ambientCoeff				: AMBIENT_COEFF;
float diffuseCoeff				: DIFFUSE_COEFF;
float specularCoeff				: SPECULAR_COEFF;

texture diffuseMap				: DIFFUSE_MAP;
texture normalMap				: NORMAL_MAP;

sampler diffuseSampler = sampler_state
{
	Texture = (diffuseMap);
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

sampler normalSampler = sampler_state
{
	Texture = (normalMap);
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};


struct VertexInput
{
	float4 	position	: POSITION;
	float2 	texCoords	: TEXCOORD0;
	float3	normal	: NORMAL0;
	float3	binormal	: BINORMAL0;
	float3	tangent	: TANGENT0;
};

struct VertexToPixel
{
	float4		position		: POSITION;
	float2		texCoords		: TEXCOORD0;
	float3		lightDir		: TEXCOORD1;
	float3		viewDir			: TEXCOORD2;
};

VertexToPixel GenericTransform_VS(VertexInput input)
{
	VertexToPixel output;
	
	// Transform vertex by world-view-projection matrix
	output.position = mul(input.position, worldViewProjection);
	float4 worldPos = mul(input.position, world);
	
	float3 lightPos = float3(10.0f, 0.0f, 5.0f);
	float3 viewPos = mul(-view._m30_m31_m32, transpose(view));

	float3 lightDir = lightPos - worldPos.xyz;
	float3 viewDir = viewPos - worldPos.xyz;
	
	float3 normNormal = normalize(input.normal);
	float3 normBinormal = normalize(input.binormal);
	float3 normTangent = normalize(input.tangent);
	
	output.viewDir.x = dot(normTangent, viewDir);
	output.viewDir.y = dot(normBinormal, viewDir);
	output.viewDir.z = dot(normNormal, viewDir);
	
	output.lightDir.x = dot(normTangent, lightDir);
	output.lightDir.y = dot(normBinormal, lightDir);
	output.lightDir.z = dot(normNormal, lightDir);
	
	output.texCoords = input.texCoords;
	
	return output;
}

float4 GenericTransform_PS(VertexToPixel input) : COLOR0
{
	float4 diffuseColor = tex2D(diffuseSampler, input.texCoords);
	float3 normal = normalize((tex2D(normalSampler, input.texCoords).xyz * 2.0f) - 1.0f);
	
	float3 normLightDir = normalize(input.lightDir);
	float3 normViewDir = normalize(input.viewDir);
	
	float nDotL = dot(normal, normLightDir);
	
	float3 refl = normalize(((2.0f * normal) * nDotL) - normLightDir);
	float rDotV = max(dot(refl, normViewDir), 0.0f);
	
	float4 retColor = (ambientCoeff + diffuseCoeff) * diffuseColor * nDotL + specularCoeff * pow(rDotV, specularPower) * specularColor;
	
	return float4(retColor.xyz, 1.0f);
}

technique GenericTransform
{
   pass Single_Pass
   {
		VertexShader = compile vs_2_0 GenericTransform_VS();
		PixelShader = compile ps_2_0 GenericTransform_PS();
   }

}
