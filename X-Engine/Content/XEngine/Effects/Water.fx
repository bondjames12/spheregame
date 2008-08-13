float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float4x4 xReflectionView;
float4x4 xWindDirection;
float3 xCamPos;
float3 xLightDirection;
float xWaveLength;
float xWaveHeight;
float xTime;
float xWindForce;

bool reflect;
bool refract;

Texture xReflectionMap;
sampler ReflectionSampler = sampler_state { texture = <xReflectionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xRefractionMap;
sampler RefractionSampler = sampler_state { texture = <xRefractionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xWaterBumpMap;
sampler WaterBumpMapSampler = sampler_state { texture = <xWaterBumpMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = wrap; AddressV = wrap;};

struct WaterVertexToPixel
{
    float4 Position                 : POSITION;    
    float4 ReflectionMapSamplingPos    : TEXCOORD1;
    float2 BumpMapSamplingPos        : TEXCOORD2;
    float4 RefractionMapSamplingPos : TEXCOORD3;
    float4 Position3D                : TEXCOORD4;
	float4 Position2D                : TEXCOORD5;
};

struct WaterPixelToFrame
{
    float4 Color : COLOR0;
};

WaterVertexToPixel WaterVS(float4 inPos : POSITION, float2 inTex: TEXCOORD)
{
    WaterVertexToPixel Output = (WaterVertexToPixel)0;
    float4x4 preViewProjection = mul (xView, xProjection);
    float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    float4x4 preReflectionViewProjection = mul (xReflectionView, xProjection);
    float4x4 preWorldReflectionViewProjection = mul (xWorld, preReflectionViewProjection);
    
    Output.Position = mul(inPos, preWorldViewProjection);
    Output.ReflectionMapSamplingPos = mul(inPos, preWorldReflectionViewProjection);
    Output.RefractionMapSamplingPos = mul(inPos, preWorldViewProjection);
    Output.Position2D = Output.Position;
	Output.Position3D = inPos;



     float4 absoluteTexCoords = float4(inTex, 0, 1);
     float4 rotatedTexCoords = mul(absoluteTexCoords, xWindDirection);
     float2 moveVector = float2(0, 1);
     Output.BumpMapSamplingPos = rotatedTexCoords.xy/xWaveLength + xTime*xWindForce*moveVector.xy;

     return Output;    
}

WaterPixelToFrame WaterPS(WaterVertexToPixel PSIn)
{
    WaterPixelToFrame Output = (WaterPixelToFrame)0;
    
    float2 ProjectedTexCoords;
    ProjectedTexCoords.x = PSIn.ReflectionMapSamplingPos.x/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;
    ProjectedTexCoords.y = -PSIn.ReflectionMapSamplingPos.y/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;

    float4 bumpColor = tex2D(WaterBumpMapSampler, PSIn.BumpMapSamplingPos);
    float2 perturbation = xWaveHeight*(bumpColor.rg - 0.5f);
    float2 perturbatedTexCoords = ProjectedTexCoords + perturbation;
    float4 reflectiveColor = tex2D(ReflectionSampler, perturbatedTexCoords);

    float2 ProjectedRefrTexCoords;
    ProjectedRefrTexCoords.x = PSIn.RefractionMapSamplingPos.x/PSIn.RefractionMapSamplingPos.w/2.0f + 0.5f;
    ProjectedRefrTexCoords.y = -PSIn.RefractionMapSamplingPos.y/PSIn.RefractionMapSamplingPos.w/2.0f + 0.5f;
    float2 perturbatedRefrTexCoords = ProjectedRefrTexCoords + perturbation;
    float4 refractiveColor = tex2D(RefractionSampler, perturbatedRefrTexCoords);

    float3 eyeVector = normalize(xCamPos - PSIn.Position3D);
    float3 normalVector = float3(0,1,0);
    float fresnelTerm = dot(eyeVector, normalVector);
    
    float4 combinedColor = float4(.2, .4, .8, 1);
    if (reflect && refract)
		combinedColor = refractiveColor*fresnelTerm + reflectiveColor *(1-fresnelTerm);
    else
    {
		if (reflect)
			combinedColor = reflectiveColor;
		if (refract)
			combinedColor = refractiveColor;
    }

    float4 dullColor = float4(0.3f, 0.3f, 0.5f, 1.0f);
    float dullBlendFactor = 0.2f;

    Output.Color = dullBlendFactor*dullColor + (1-dullBlendFactor)*combinedColor;

    return Output;
}

//takes the screen space pixel position and divides Z by W 

float4 DepthMapPixelShader(WaterVertexToPixel IN) : COLOR0
{
	//In our shaders texcoord4 contains the global space position of the pixel
    return IN.Position2D.z/IN.Position2D.w;
}

//The X and Y coordinates of the PSIn.Position contain the X and Y values of the screen coordinate
// of the current pixel, but the Z coordinate is also very useful as it contains the distance between
// the camera and the pixel.
//However, this vector is the result of a multiplication of a vector and a 4x4 matrix, which happened 
//in the vertex shader. the result of such a multiplication has 4 components: X,Y,Z and W. We cannot
// use any of the X,Y or Z components immediately, we first need to divide them by the W component. 
//The W component is called the ?homogeneous? component, you can find more explanation on this 
//in the ?Extra Reading? section of my site.
//After dividing the Z compontent by the homogeneous component, the result will be between 0 and 1,
// where 0 corresponds to pixels at the near clipping plane and 1 to pixels at the far clipping plane, 
//as defined in the creation of the Projection matrix.
//http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series3/Shadow_map.php

technique Water
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 WaterVS();
        PixelShader = compile ps_2_0 WaterPS();
    }
}

technique DepthMapStatic
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 WaterVS();
        PixelShader = compile ps_2_0 DepthMapPixelShader();
    }
}

