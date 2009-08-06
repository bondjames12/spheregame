#include <include9.hlsl> 

string ParamID = "0x0001"; // Load DXSAS parser 

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
#include <lightlist9.hlsl>



//**********************************************************************
// Vertex Shaders
//**********************************************************************

#include <defaultvs.hlsl>

//**********************************************************************
// Pixel Shaders
//**********************************************************************

float4 PS(VertexToPixel IN) : COLOR
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