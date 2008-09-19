//include XSI data structures
#include <xsi_include9.hlsl> 

/************* UN-TWEAKABLES **************/
float4x4 Wvp : WorldViewProjection < string UIWidget="None"; >;
float4x4 World : World < string UIWidget="None"; >;

/******** TWEAKABLES ****************************************/
float4 GlassColor <
    string UIName = "Glass Color";
    string UIWidget = "Color";
> = {0.0f, 0.0f, 0.0f, 0.0f};

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
float4 strokeTexPS() : COLOR0 {
	return GlassColor;
}

///////////////////////////////////////
/// TECHNIQUES ////////////////////////
///////////////////////////////////////
 
technique Main 
{
    pass p0   
	{        
        VertexShader = compile vs_3_0 VSStatic();
	    //ZEnable = true;
	    //ZWriteEnable = true;
	    AlphaBlendEnable = true;
	    SrcBlend = One;
	    DestBlend = InvSrcColor;
	    //CullMode = CW;
        PixelShader = compile ps_3_0 strokeTexPS();
    }
}
