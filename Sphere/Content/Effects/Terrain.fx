//
// Terrain.fx
// 
// This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
// for license details.
//

//------- Constants --------
float4x4 View				: VIEW;
float4x4 Projection			: PROJECTION;
float4x4 World				: WORLD;
float4x4 WorldViewProj		: WORLDVIEWPROJECTION;
float4 LightDirection		: LIGHT_DIRECTION;
float4 AmbientColor			: AMBIENT_COLOR;
float AmbientPower			: AMBIENT_POWER;
float4 SpecularColor		: SPECULAR_COLOR;
float SpecularPower			: SPECULAR_POWER;
float4 DiffuseColor			: DIFFUSE_COLOR;
float4 CameraForward		: VIEW_FORWARD;

float TerrainScale			: SCALE_FACTOR;
float TerrainWidth			: TERRAIN_WIDTH;

//------- Texture Samplers --------
Texture TextureMap			: TEXTURE_MAP;
sampler TextureMapSampler = sampler_state { texture = <TextureMap> ; magfilter = LINEAR; minfilter = LINEAR; 
                                                                         mipfilter = LINEAR; AddressU  = Wrap;
                                                                         AddressV  = Wrap; AddressW  = Wrap;};

Texture GrassTexture		: GRASS_TEXTURE;
sampler GrassTextureSampler = sampler_state { texture = <GrassTexture> ; magfilter = LINEAR; minfilter = LINEAR; 
                                                                         mipfilter=LINEAR; AddressU  = Wrap;
                                                                         AddressV  = Wrap; AddressW  = Wrap;};

Texture SandTexture			: SAND_TEXTURE;
sampler SandTextureSampler = sampler_state { texture = <SandTexture> ; magfilter = LINEAR; minfilter = LINEAR; 
                                                                       mipfilter =LINEAR; AddressU  = Wrap;
                                                                       AddressV  = Wrap; AddressW  = Wrap;};

Texture RockTexture			: ROCK_TEXTURE;
sampler RockTextureSampler = sampler_state { texture = <RockTexture> ; magfilter = LINEAR; minfilter = LINEAR; 
                                                                       mipfilter = LINEAR; AddressU  = Wrap;
                                                                       AddressV  = Wrap; AddressW  = Wrap;};

Texture GrassNormal			:GRASS_NORMAL;
sampler2D GrassNormalSampler : TEXUNIT1 = sampler_state
{ Texture   = (GrassNormal); magfilter = LINEAR; minfilter = LINEAR; 
                             mipfilter = LINEAR; AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

Texture SandNormal			: SAND_NORMAL;
sampler2D SandNormalSampler : TEXUNIT1 = sampler_state
{ Texture   = (SandNormal); magfilter  = LINEAR; minfilter = LINEAR; 
                             mipfilter = LINEAR; AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

Texture RockNormal			: ROCK_NORMAL;
sampler2D RockNormalSampler : TEXUNIT1 = sampler_state
{ Texture   = (RockNormal); magfilter = LINEAR; minfilter = LINEAR; 
                             mipfilter=LINEAR; AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

//------- Technique: MultiTexturedNormaled --------
 
 struct VS_INPUT
 {
     float4 Position            : POSITION0;    
     float3 Normal              : NORMAL0;    
     //float3 Binormal            : BINORMAL0;	// Needed for accurate normal mapping
     //float3 Tangent             : TANGENT0;   // Needed for accurate normal mapping
 };

struct VS_OUTPUT
{
    float4 position            : POSITION0;
    float2 texCoord            : TEXCOORD0;
    float2 texCoordNoWrap	   : TEXCOORD1;
    float3 Binormal			   : TEXCOORD2;
    float3 Tangent			   : TEXCOORD3;
    float3 Normal			   : TEXCOORD4;
};

 VS_OUTPUT MultiTexturedNormaledVS( VS_INPUT input)    
 {
     VS_OUTPUT Output;

     // These estimated values should work on terrain that isn't steep,
     // while steep terrain will not be fully normal mapped.
     float3 Binormal = (0.0f, 0.0f, -0.5f);
     float3 Tangent = (0.0f, -0.5f, 0.0f);
     
     Output.position = mul(input.Position, WorldViewProj);
     Output.Normal = mul(input.Normal, World);

     float4 worldSpacePos = mul(input.Position, World);

     Output.texCoord.x = input.Position.x * 0.05f / TerrainScale;
     Output.texCoord.y = input.Position.z * 0.05f / TerrainScale;
     Output.texCoordNoWrap.x = input.Position.z / (TerrainWidth * TerrainScale);
     Output.texCoordNoWrap.y = input.Position.x / (TerrainWidth * TerrainScale);
     
     Output.Tangent =  mul(Tangent,  World);
	 Output.Binormal = mul(Binormal, World);

     return Output;    
 }
 
 float4 MultiTexturedNormaledPS(VS_OUTPUT input) : COLOR0
 {
	 float3 TerrainColorWeight = tex2D(TextureMapSampler, input.texCoordNoWrap);
	 
	 input.Normal = normalize(input.Normal);
 
     float3 normalFromMap = (2.0f * tex2D(SandNormalSampler, input.texCoord) - 1.0f) * TerrainColorWeight.r;
     normalFromMap += (2.0f * tex2D(GrassNormalSampler, input.texCoord) - 1.0f) * TerrainColorWeight.g;
     normalFromMap += (2.0f * tex2D(RockNormalSampler, input.texCoord) - 1.0f)  * TerrainColorWeight.b;
     normalFromMap = normalize(mul(normalFromMap, float3x3(input.Tangent, input.Binormal, input.Normal))) * 0.5f;

     // Factor in normal mapping and terrain vertex normals as well in lighting of the pixel
     float lightingFactor = saturate(dot(normalFromMap + input.Normal, -LightDirection));

     float4 Color = tex2D(SandTextureSampler, input.texCoord)   * TerrainColorWeight.r;
     Color += tex2D(GrassTextureSampler, input.texCoord) * TerrainColorWeight.g;
     Color += tex2D(RockTextureSampler, input.texCoord)  * TerrainColorWeight.b;

     float3 Reflect = (lightingFactor * input.Normal) + LightDirection;
     float3 specular = pow(saturate(dot(Reflect, -CameraForward)), SpecularPower);
     
	 Color.rgb *= (AmbientColor + (DiffuseColor * lightingFactor) + (SpecularColor * specular * lightingFactor)) * AmbientPower;
	 Color.a = 1.0f;
 
     return Color;
	//return float4(1, 0, 0, 1);
 }
 
 technique MultiTexturedNormaled
 {
     pass Pass0
     {
         VertexShader = compile vs_1_1 MultiTexturedNormaledVS();
         PixelShader = compile ps_2_0 MultiTexturedNormaledPS();
     }
 }
 
 // ================================================
 //------- Technique: MultiTextured --------
 
 struct VSBASIC_INPUT
 {
     float4 Position            : POSITION0;    
     float3 Normal              : NORMAL0;    
 };

struct VSBASIC_OUTPUT
{
    float4 position            : POSITION0;
    float2 texCoord            : TEXCOORD0;
    float2 texCoordNoWrap	   : TEXCOORD1;
    float3 Normal			   : TEXCOORD5;
};

 VSBASIC_OUTPUT MultiTexturedVS( VSBASIC_INPUT input)    
 {
     VSBASIC_OUTPUT Output;
     
     Output.position = mul(input.Position, WorldViewProj);
     Output.Normal = mul(input.Normal, World);

     float4 worldSpacePos = mul(input.Position, World);

     Output.texCoord.x = input.Position.x * .05f / TerrainScale;
     Output.texCoord.y = input.Position.y * .05f / TerrainScale;
     Output.texCoordNoWrap.x = input.Position.y / (TerrainWidth * TerrainScale);
     Output.texCoordNoWrap.y = input.Position.x / (TerrainWidth * TerrainScale);

     return Output;    
 }
 
 float4 MultiTexturedPS(VSBASIC_OUTPUT input) : COLOR0
 {
	 float3 TerrainColorWeight = tex2D(TextureMapSampler, input.texCoordNoWrap);
	 
	 input.Normal = normalize(input.Normal);

     // Factor in normal mapping and terrain vertex normals as well in lighting of the pixel
     float lightingFactor = saturate(dot(input.Normal, -LightDirection));

     // Multi-texture blending occurs in these three lines
     float4 Color = tex2D(SandTextureSampler, input.texCoord)   * TerrainColorWeight.r;
     Color += tex2D(GrassTextureSampler, input.texCoord) * TerrainColorWeight.g;
     Color += tex2D(RockTextureSampler,  input.texCoord) * TerrainColorWeight.b;

     float3 Reflect = (lightingFactor * input.Normal) + LightDirection;
     float3 specular = pow(saturate(dot(Reflect, -CameraForward)), SpecularPower);
     
	 Color.rgb *= (AmbientColor + (DiffuseColor * lightingFactor) + (SpecularColor * specular * lightingFactor)) * AmbientPower;
	 Color.a = 1.0f;
 
     return Color;
 }
 
 technique MultiTextured
 {
     pass Pass0
     {
         VertexShader = compile vs_1_1 MultiTexturedVS();
         PixelShader = compile ps_2_0 MultiTexturedPS();
     }
 }