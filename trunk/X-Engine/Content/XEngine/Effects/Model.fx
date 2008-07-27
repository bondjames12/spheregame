float4x4 World; 
float4x4 View; 
float4x4 Projection; 

float4 vecLightDir[3];
float4 LightColor[3];
float4 vecEye;

float NumLights;

texture Texture;
float4 Diffuse;
texture NormalMap;

bool TexturingEnabled = true;
bool NormalMapEnabled = false;

sampler Sampler = sampler_state
{
    Texture = (Texture);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
};

sampler Normal = sampler_state
{
    Texture = (NormalMap);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
};

float3 CameraPosition;

// Vertex shader input structure.
struct VS_INPUT
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};


// Vertex shader output structure.
struct VS_OUTPUT
{
	float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 View : TEXCOORD2;
    float4 WorldPosition : TEXCOORD3;
};


// Vertex shader program.
VS_OUTPUT VertexShader(VS_INPUT input)
{
    VS_OUTPUT output;
    
    // Skin the vertex position.
    float4 position = mul(input.Position, World);
    
    output.Position = mul(mul(position, View), Projection);

    // Skin the vertex normal, then compute lighting.
    float3 normal = normalize(mul(input.Normal, World));
    output.Normal = normal;

    output.TexCoord = input.TexCoord;
    
    output.View = vecEye - position;
    
    output.WorldPosition = mul(input.Position, World);
    output.WorldPosition /= output.WorldPosition.w;
    
    return output;
}


// Pixel shader program.
float4 PixelShader(VS_OUTPUT input) : COLOR0
{
	float4 Color = float4 (0, 0, 0, 1);
	
	for (float i = 0; i < NumLights; i++)
	{
		float4 diffuse = Diffuse;
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

	float4 tex = tex2D(Sampler, input.TexCoord);
	
	if (TexturingEnabled)
		Color *= tex;
	
	Color.a = tex.a;
	
	return Color;
}


technique Model
{
    pass Pass1
    {	
        VertexShader = compile vs_1_1 VertexShader();
        PixelShader = compile ps_2_b PixelShader();
    }
}