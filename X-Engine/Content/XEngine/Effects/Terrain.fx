//////////////////////////////////////////////////////////////////////////////
//
// 		Simple Atmospheric Scattering
// 		Matthieu Laban - 2007
//
//		Slightly modified (non-skydome lighting) by Alex Urbano - 2008  
//
//////////////////////////////////////////////////////////////////////////////

float4x4 WorldIT;
float4x4 WorldViewProj;
float4x4 World;
float4x4 ViewInv;

struct appdata {
	float3 Position				: POSITION;
	float4 Normal				: NORMAL;
	float2 UV0					: TEXCOORD0;
  	half4 Tangent				: TANGENT0;
  	half4 Binormal				: BINORMAL0;
};

struct vertexOutput {
	float4 HPosition	 		: POSITION;
	float3 WorldLightVec		: TEXCOORD0;
	float3 WorldNormal	    	: TEXCOORD1;
	float3 WorldEyeDirection	: TEXCOORD2;
	half3  WorldTangent			: TEXCOORD3;
	float4 Position				: TEXCOORD4;
  	float2 UV					: TEXCOORD5;
  	half Fog 					: TEXCOORD6;
  	half2 Altitudes 			: TEXCOORD7;
	float4 Pos2DAsSeenByLight   : TEXCOORD8;
};

float4 LightDirection = {100.0f, 100.0f, 100.0f, 1.0f};
float4 LightColor = {1.0f, 1.0f, 1.0f, 1.0f};
float4 LightColorAmbient = {0.0f, 0.0f, 0.0f, 1.0f};

float4 FogColor = {1.0f, 1.0f, 1.0f, 1.0f};

float fDensity ;

bool isSkydome;

float SunLightness = 0.2; 

float sunRadiusAttenuation = 256;

float largeSunLightness = 0.2;
float largeSunRadiusAttenuation = 3;
float dayToSunsetSharpness = 1.5;
float hazeTopAltitude = 20; 

texture TextureR;
sampler TextureRSampler = sampler_state { 
	Texture = <TextureR>; 	
	MinFilter = Linear;
	MipFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;  };
	
texture TextureG;
sampler TextureGSampler = sampler_state { 
	Texture = <TextureG>; 	
	MinFilter = Linear;
	MipFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;  };
	
texture TextureB;
sampler TextureBSampler = sampler_state { 
	Texture = <TextureB>; 	
	MinFilter = Linear;
	MipFilter = Linear;
	MagFilter = Linear; 
	AddressU = Wrap;
	AddressV = Wrap;  };
	
texture TextureMap;

sampler TextureMapSampler = sampler_state { 
	Texture = <TextureMap>; 	
	MinFilter = Linear;
	MipFilter = Linear;
	MagFilter = Linear; 
	AddressU = Clamp;
	AddressV = Clamp;  };
	
texture SkyTextureNight;

sampler SurfSamplerSkyTextureNight = sampler_state
{
	Texture = <SkyTextureNight>;
	MinFilter = Linear;
	MipFilter = Linear;
	MagFilter = Linear;
	AddressU = mirror; 
	AddressV = mirror;
};

texture SkyTextureSunset;

sampler SurfSamplerSkyTextureSunset = sampler_state
{
	Texture = <SkyTextureSunset>;
	MinFilter = Linear;
	MipFilter = Linear;
	MagFilter = Linear;
	AddressU = mirror; 
	AddressV = mirror;
};

texture SkyTextureDay;

sampler SurfSamplerSkyTextureDay = sampler_state
{
	Texture = <SkyTextureDay>;
	MinFilter = Linear;
	MipFilter = Linear;
	MagFilter = Linear;
	AddressU = mirror; 
	AddressV = mirror;
};


Texture xShadowMap;

sampler ShadowMapSampler = sampler_state
{ 
	texture = <xShadowMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = clamp;
	AddressV = clamp;
};

float4x4 LightWorldViewProj;

vertexOutput mainVS (appdata IN)
{
	vertexOutput OUT;
	float4 Po = float4(IN.Position.xyz,1);
	OUT.HPosition = mul( Po, WorldViewProj);
	
	OUT.WorldNormal = mul( IN.Normal, WorldIT).xyz;
	OUT.WorldTangent = mul(IN.Tangent, WorldIT).xyz;

	OUT.WorldLightVec = -LightDirection;
	
	float3 Pw = mul( Po, World).xyz;
	OUT.WorldEyeDirection = ViewInv[3].xyz - Pw;
	
	OUT.Altitudes.x = ViewInv[3].y;	
	
  	float4 pos = mul( IN.Position, World);
  
 	float dist = length(OUT.WorldEyeDirection);
	OUT.Fog = (1.f/exp(pow(dist * fDensity, 2)));

	OUT.Altitudes.y = Pw.y;

	OUT.UV = IN.UV0;
	OUT.Position = OUT.HPosition; //save the screen space position i texcoord4
	//calc this but its not used
	OUT.Pos2DAsSeenByLight = mul(IN.Position, LightWorldViewProj);
	return OUT;
}

float4 mainPS(vertexOutput IN) : COLOR0
{	
	float4 colorOutput = float4(0,0,0,1);
	float4 Map = tex2D(TextureMapSampler, float2(IN.UV.x, IN.UV.y));
	float4 DiffuseColor = Map.r * tex2D(TextureRSampler, IN.UV * 10) + Map.g * tex2D(TextureGSampler, IN.UV * 10) + Map.b * tex2D(TextureBSampler, IN.UV * 10);
	
	float4 colorAmbient = DiffuseColor;
		
	// Calculate light/eye/normal vectors
	float eyeAlt = IN.Altitudes.x;
	float3 eyeVec = normalize(IN.WorldEyeDirection);
	float3 normal = normalize(IN.WorldNormal);
	float3 lightVec = normalize(IN.WorldLightVec);
	
	// Calculate the amount of direct light	
	float NdotL = max( dot( normal, -lightVec), 0);
	
	float4 colorDiffuse  = DiffuseColor * (NdotL * LightColor) + LightColorAmbient * DiffuseColor;
	colorOutput += colorDiffuse;		
	colorOutput.a = 1.0f;
	
	// Calculate sun highlight...	
	float sunHighlight = pow(max(0, dot(lightVec, -eyeVec)), sunRadiusAttenuation) * SunLightness;	
	// Calculate a wider sun highlight 
	float largeSunHighlight = pow(max(0, dot(lightVec, -eyeVec)), largeSunRadiusAttenuation) * largeSunLightness;
	
	// Calculate 2D angle between pixel to eye and sun to eye
	float3 flatLightVec = normalize(float3(lightVec.x, 0, lightVec.z));
	float3 flatEyeVec = normalize(float3(eyeVec.x, 0, eyeVec.z));
	float diff = dot(flatLightVec, -flatEyeVec);	
	
	// Based on camera altitude, the haze will look different and will be lower on the horizon.
	// This is simulated by raising YAngle to a certain power based on the difference between the
	// haze top and camera altitude. 
	// This modification of the angle will show more blue sky above the haze with a sharper separation.
	// Lerp between 0.25 and 1.25
	float val = lerp(0.25, 1.25, min(1, hazeTopAltitude / max(0.0001, eyeAlt)));
	// Apply the power to sharpen the edge between haze and blue sky
	float YAngle = pow(max(0, -eyeVec.y), val);	
	
	// Fetch the 3 colors we need based on YAngle and angle from eye vector to the sun
	float4 fogColorDay = tex2D( SurfSamplerSkyTextureDay, float2( 1 - (diff + 1) * 0.5, 1-YAngle));
	float4 fogColorSunset = tex2D( SurfSamplerSkyTextureSunset, float2( 1 - (diff + 1) * 0.5, 1-YAngle));
	float4 fogColorNight = tex2D( SurfSamplerSkyTextureNight, float2( 1 - (diff + 1) * 0.5, 1-YAngle));
	
	float4 fogColor;
	
	// If the light is above the horizon, then interpolate between day and sunset
	// Otherwise between sunset and night
	if (lightVec.y > 0)
	{
		// Transition is sharpened with dayToSunsetSharpness to make a more realistic cut 
		// between day and sunset instead of a linear transition
		fogColor = lerp(fogColorDay, fogColorSunset, min(1, pow(1 - lightVec.y, dayToSunsetSharpness)));
	}
	else
	{
		// Slightly different scheme for sunset/night.
		fogColor = lerp(fogColorSunset, fogColorNight, min(1, -lightVec.y * 4));
	}
	
	// Add sun highlights
	fogColor += sunHighlight + largeSunHighlight;
    
	// Apply fog on output color
	colorOutput = lerp(fogColor, colorOutput, IN.Fog);
		
	return colorOutput;
}

vertexOutput ShadowedStaticVS(appdata IN)
{
	vertexOutput OUT;
	float4 Po = float4(IN.Position.xyz,1);
	OUT.HPosition = mul( Po, WorldViewProj);
	
	OUT.WorldNormal = mul( IN.Normal, WorldIT).xyz;
	OUT.WorldTangent = mul(IN.Tangent, WorldIT).xyz;

	OUT.WorldLightVec = -LightDirection;
	
	float3 Pw = mul( Po, World).xyz;
	OUT.WorldEyeDirection = ViewInv[3].xyz - Pw;
	
	OUT.Altitudes.x = ViewInv[3].y;	
	
  	float4 pos = mul( IN.Position, World);
  
 	float dist = length(OUT.WorldEyeDirection);
	OUT.Fog = (1.f/exp(pow(dist * fDensity, 2)));

	OUT.Altitudes.y = Pw.y;

	OUT.UV = IN.UV0;
	OUT.Position = OUT.HPosition; //save the screen space position i texcoord4
	OUT.Pos2DAsSeenByLight = mul(IN.Position, LightWorldViewProj);  
	
	return OUT;
}

float4 ShadowedStaticPS(vertexOutput IN) : COLOR0
{	
	float4 colorOutput = float4(0,0,0,1);
	float4 Map = tex2D(TextureMapSampler, float2(IN.UV.x, IN.UV.y));
	float4 DiffuseColor = Map.r * tex2D(TextureRSampler, IN.UV * 10) + Map.g * tex2D(TextureGSampler, IN.UV * 10) + Map.b * tex2D(TextureBSampler, IN.UV * 10);
	
	float4 colorAmbient = DiffuseColor;
		
	// Calculate light/eye/normal vectors
	float eyeAlt = IN.Altitudes.x;
	float3 eyeVec = normalize(IN.WorldEyeDirection);
	float3 normal = normalize(IN.WorldNormal);
	float3 lightVec = normalize(IN.WorldLightVec);
	
	// Calculate the amount of direct light	
	float NdotL = max( dot( normal, -lightVec), 0);
	
	float4 colorDiffuse  = DiffuseColor * (NdotL * LightColor) + LightColorAmbient * DiffuseColor;
	colorOutput += colorDiffuse;		
	colorOutput.a = 1.0f;
	
	// Calculate sun highlight...	
	float sunHighlight = pow(max(0, dot(lightVec, -eyeVec)), sunRadiusAttenuation) * SunLightness;	
	// Calculate a wider sun highlight 
	float largeSunHighlight = pow(max(0, dot(lightVec, -eyeVec)), largeSunRadiusAttenuation) * largeSunLightness;
	
	// Calculate 2D angle between pixel to eye and sun to eye
	float3 flatLightVec = normalize(float3(lightVec.x, 0, lightVec.z));
	float3 flatEyeVec = normalize(float3(eyeVec.x, 0, eyeVec.z));
	float diff = dot(flatLightVec, -flatEyeVec);	
	
	// Based on camera altitude, the haze will look different and will be lower on the horizon.
	// This is simulated by raising YAngle to a certain power based on the difference between the
	// haze top and camera altitude. 
	// This modification of the angle will show more blue sky above the haze with a sharper separation.
	// Lerp between 0.25 and 1.25
	float val = lerp(0.25, 1.25, min(1, hazeTopAltitude / max(0.0001, eyeAlt)));
	// Apply the power to sharpen the edge between haze and blue sky
	float YAngle = pow(max(0, -eyeVec.y), val);	
	
	// Fetch the 3 colors we need based on YAngle and angle from eye vector to the sun
	float4 fogColorDay = tex2D( SurfSamplerSkyTextureDay, float2( 1 - (diff + 1) * 0.5, 1-YAngle));
	float4 fogColorSunset = tex2D( SurfSamplerSkyTextureSunset, float2( 1 - (diff + 1) * 0.5, 1-YAngle));
	float4 fogColorNight = tex2D( SurfSamplerSkyTextureNight, float2( 1 - (diff + 1) * 0.5, 1-YAngle));
	
	float4 fogColor;
	
	// If the light is above the horizon, then interpolate between day and sunset
	// Otherwise between sunset and night
	if (lightVec.y > 0)
	{
		// Transition is sharpened with dayToSunsetSharpness to make a more realistic cut 
		// between day and sunset instead of a linear transition
		fogColor = lerp(fogColorDay, fogColorSunset, min(1, pow(1 - lightVec.y, dayToSunsetSharpness)));
	}
	else
	{
		// Slightly different scheme for sunset/night.
		fogColor = lerp(fogColorSunset, fogColorNight, min(1, -lightVec.y * 4));
	}
	
	// Add sun highlights
	fogColor += sunHighlight + largeSunHighlight;
    
	// Apply fog on output color
	colorOutput = lerp(fogColor, colorOutput, IN.Fog);
	
	//calc some shadows
	//but first calc a texture projecture for the shadow map in homogenious space
	float2 ProjectedTexCoords;
    ProjectedTexCoords[0] = IN.Pos2DAsSeenByLight.x/IN.Pos2DAsSeenByLight.w/2.0f +0.5f;
    ProjectedTexCoords[1] = -IN.Pos2DAsSeenByLight.y/IN.Pos2DAsSeenByLight.w/2.0f +0.5f;
		
	return colorOutput * tex2D(ShadowMapSampler, ProjectedTexCoords);
	
	
	//return colorOutput;
}

//This pixel shader requires XSI_VertexToPixel in xsi_include9.hlsl
//takes the screen space pixel position and divides Z by W 

float4 DepthMapPixelShader(vertexOutput IN) : COLOR0
{
	//In our shaders texcoord4 contains the global space position of the pixel
    return IN.Position.z/IN.Position.w;
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

technique ShadowedStatic
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 ShadowedStaticVS();
        PixelShader = compile ps_3_0 ShadowedStaticPS();
    }
}

technique SkyDome 
{
	pass p0
	{
		VertexShader = compile vs_3_0 mainVS();
		PixelShader = compile ps_3_0 mainPS();
	}
}

technique DepthMapStatic
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 mainVS();
        PixelShader = compile ps_3_0 DepthMapPixelShader();
    }
}


