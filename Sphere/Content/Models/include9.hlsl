//************************************************************
// standard include file for XSI DirectX 9 HLSL shaders
//************************************************************

//************************************************************
// Application to vertex data
//************************************************************
struct AppToVertex
{
	float4 position : POSITION;
	float4 normal : NORMAL;
	float4 color0 : COLOR0;
	float4 color1 : COLOR1;
	float4 texcoord0 : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
	float4 texcoord3 : TEXCOORD3;
	float4 texcoord4 : TEXCOORD4;
	float4 texcoord5 : TEXCOORD5;
	float4 texcoord6 : TEXCOORD6;
	float4 texcoord7 : TEXCOORD7;
    float4 BoneIndices : BLENDINDICES0;
    float4 BoneWeights : BLENDWEIGHT0;	
};

//************************************************************
// Vertex to pixel data
//************************************************************
struct VertexToPixel
{
	float4 position : POSITION;
	float4 color0 : COLOR0;
	float4 color1 : COLOR1;
	float4 texcoord0 : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
	float4 texcoord3 : TEXCOORD3;
	float4 texcoord4 : TEXCOORD4;
	float4 texcoord5 : TEXCOORD5;
	float4 texcoord6 : TEXCOORD6;
	float4 texcoord7 : TEXCOORD7;
};
