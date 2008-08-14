#include <xsi_include9.hlsl> 

//**********************************************************************
// Tweakables
//**********************************************************************

float4 ColorParameter
<
	string SasUiControl = "ColorPicker";
	string SasUiLabel =  "Color";	
> = {1.0f, 1.0f, 1.0f, 1.0f};

texture2D AlbedoMap
<
	string ResourceName = "default_surface_map.png";
	string ResourceType = "2D";
>;

sampler2D AlbedoSampler = sampler_state
{
	Texture = <AlbedoMap>;
	MipFilter = LINEAR; 
	MinFilter = LINEAR;
	MagFilter = LINEAR;	
};

//**********************************************************************
// Runtime bound
//**********************************************************************
// lights
#include <xsi_lightlist9.hlsl>



//**********************************************************************
// Vertex Shaders
//**********************************************************************

#include <xsi_defaultvs.hlsl>

//**********************************************************************
// Pixel Shaders
//**********************************************************************

float4 PS(XSI_VertexToPixel IN) : COLOR0
{
	float4 albedotex = tex2D(AlbedoSampler, IN.texcoord0);
	albedotex.w = 1;
	return ColorParameter * albedotex;
}

//**********************************************************************
// Techniques
//**********************************************************************


technique Static
{
	pass p0
	{		
		VertexShader = compile vs_2_0 VSStatic();
		PixelShader = compile ps_2_0 PS();
	}
}

technique Skinned
{
	pass p0
	{		
		VertexShader = compile vs_2_0 VSSkinned();
		PixelShader = compile ps_2_0 PS();
	}
}

