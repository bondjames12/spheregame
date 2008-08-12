#include <xsi_include9.hlsl> 

//**********************************************************************
// Tweakables
//**********************************************************************

float4 ColorParameter
<
	string SasUiControl = "ColorPicker";
	string SasUiLabel =  "Color";	
> = {1.0f, 1.0f, 1.0f, 1.0f};

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

float4 PS
(
	XSI_VertexToPixel IN
) : COLOR
{
	return ColorParameter;
}

//**********************************************************************
// Techniques
//**********************************************************************
#include <DepthMapPixelShader.hlsl>

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