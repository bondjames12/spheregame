#include <xsi_include9.hlsl> 

//**********************************************************************
// Tweakables
//**********************************************************************

float3 AmbientColor
<
	string SasUiControl = "ColorPicker";
	string SasUiLabel =  "Ambient";	
> = {0.3f, 0.3f, 0.3f};

float3 DiffuseColor
<
	string SasUiControl = "ColorPicker";
	string SasUiLabel =  "Diffuse";	
> = {0.7f, 0.7f, 0.7f};

float3 SpecularColor
<
	string SasUiControl = "ColorPicker";
	string SasUiLabel =  "Specular";	
> = {1.0f, 1.0f, 1.0f};

float SpecularPower
<
	string SasUiControl = "Slider";
	string SasUiLabel = "Specular Power";
	float SasUiMin = 1;
	float SasUiMax = 200;	
> = 20.0f;

texture2D AmbientMap
<
	string ResourceName = "default_ambocc_map.png";
	string ResourceType = "2D";
>;

texture2D AlbedoMap
<
	string ResourceName = "default_surface_map.png";
	string ResourceType = "2D";
>;

texture2D NormalMap
<
	string ResourceName = "default_normal_map.png";
	string ResourceType = "2D";
>;

sampler2D AmbientSampler = sampler_state
{
	Texture = <AmbientMap>;
    MipFilter = LINEAR; 
    MinFilter = LINEAR;
    MagFilter = LINEAR;	
};

sampler2D AlbedoSampler = sampler_state
{
	Texture = <AlbedoMap>;
    MipFilter = LINEAR; 
    MinFilter = LINEAR;
    MagFilter = LINEAR;	
};

sampler2D NormalSampler = sampler_state
{
	Texture = <NormalMap>;
    MipFilter = LINEAR; 
    MinFilter = LINEAR;
    MagFilter = LINEAR;	
};

//**********************************************************************
// Runtime bound
//**********************************************************************


float4 sieye
<
	string SasBindAddress = "Sas.Camera.Position"; 
>;


// lights
#include <xsi_lightlist9.hlsl> 


//**********************************************************************
// Vertex Shaders
//**********************************************************************

#include <xsi_defaultvs.hlsl>


//**********************************************************************
// Pixel Shaders
//**********************************************************************

float4 Phong_2
(
	XSI_VertexToPixel IN
) : COLOR
{
	#include<xsi_lightdef9.hlsl>

	float3 globalpos = IN.texcoord4;	
	float3x3 TangentToWorldSpace;
	
	float3 lightcolor = 0.0f;
	
	//*************************************************
	// sample our textures
	//*************************************************
	float3 normaltex = tex2D(NormalSampler, IN.texcoord0) * 2 - 1;
	float3 ambienttex = tex2D(AmbientSampler, IN.texcoord0);
	float3 albedotex = tex2D(AlbedoSampler, IN.texcoord0);
	
	AmbientColor = AmbientColor * ambienttex;
	DiffuseColor = DiffuseColor * albedotex;
	
	TangentToWorldSpace[0] = IN.texcoord5.xyz;
	TangentToWorldSpace[1] = IN.texcoord6.xyz;	
	TangentToWorldSpace[2] = IN.texcoord7.xyz;
	
	//*************************************************
	// convert normal in world space
	//*************************************************
	float3 normal = normalize(mul(normaltex,TangentToWorldSpace));
	
	//*************************************************
	// eye to vertex	
	//*************************************************
	float3 eyetovert = normalize(sieye.xyz - globalpos);
	
	//*************************************************
	// light up
	//*************************************************
	
	float3 ts_lightdir;
	float3 ts_hvec;
		
	int loop;
	for(loop = 0; loop < NUM_LIGHTS; loop++)
	{
	
		// light direction 
		ts_lightdir = LightPos[loop].xyz - globalpos;
		
		float lightdistance = length(ts_lightdir);
		ts_lightdir = normalize(ts_lightdir);
	
		// half vector
		ts_hvec = normalize(eyetovert + ts_lightdir);
	
		float3 currentcolor =  LightCol[loop] * (
			(DiffuseColor * clamp(dot(normal, ts_lightdir), 0, 1)) +    // diffuse
			(SpecularColor * LightCol[loop] * pow(clamp(dot(normal, ts_hvec),0,1), SpecularPower)));	// specular
			
		lightcolor += currentcolor;
	}	
	
	
	float4 result;
	result.xyz = AmbientColor + lightcolor ;	
	result.w = 1;


	return result;
}

//**********************************************************************
// Techniques
//**********************************************************************


technique Static
{
	pass p0
	{		
		VertexShader = compile vs_2_0 VSStatic();
		PixelShader = compile ps_2_0 Phong_2();
	}
}

technique Skinned
{
	pass p0
	{		
		VertexShader = compile vs_2_0 VSSkinned();
		PixelShader = compile ps_2_0 Phong_2();
	}
}

