//-----------------------------------------------------------------------------
// ParticleEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Camera parameters.
float4x4 View : View;
float4x4 Projection : Projection;
float ViewportHeight;

//Added Aug 10:Colin Using a Texture Projection matrix in soft particle calcs
float4x4 matTexProj <
	string UIName = "Texture Projection Matrix"; //matric used to transform UV
>;

// The current time, in seconds.
float CurrentTime;


// Parameters describing how the particles animate.
float Duration;
float DurationRandomness;
float3 Gravity;
float EndVelocity;
float4 MinColor;
float4 MaxColor;


// These float2 parameters describe the min and max of a range.
// The actual value is chosen differently for each particle,
// interpolating between x and y by some random amount.
float2 RotateSpeed;
float2 StartSize;
float2 EndSize;


// Particle texture and sampler.
texture Texture, DepthMap;

sampler TextureSampler = sampler_state
{
    Texture = (Texture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler DepthSampler = sampler_state 
{
    Texture		= (DepthMap);
   	MipFilter	= LINEAR;
    MinFilter	= LINEAR;
    MagFilter	= LINEAR;
};

// Vertex shader input structure describes the start position and
// velocity of the particle, and the time at which it was created,
// along with some random values that affect its size and rotation.
struct VertexShaderInput
{
    float3 Position : POSITION0;
    float3 Velocity : NORMAL0;
    float4 Random : COLOR0;
    float Time : TEXCOORD0;
};


// Vertex shader output structure specifies the position, size, and
// color of the particle, plus a 2x2 rotation matrix (packed into
// a float4 value because we don't have enough color interpolators
// to send this directly as a float2x2).
//Aug 8: Colin: Added depth sent through TEXCOORD1. Used to calc soft edges(alpha) to avoid seeing clipping
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float  Size : PSIZE0;
    float4 Color : COLOR0;
    float4 Rotation : COLOR1;
	float  Depth   : TEXCOORD1;
	float4 TexDepth	: TEXCOORD2;
};


// Vertex shader helper for computing the position of a particle.
float4 ComputeParticlePosition(float3 position, float3 velocity,
                               float age, float normalizedAge)
{
    float startVelocity = length(velocity);

    // Work out how fast the particle should be moving at the end of its life,
    // by applying a constant scaling factor to its starting velocity.
    float endVelocity = startVelocity * EndVelocity;
    
    // Our particles have constant acceleration, so given a starting velocity
    // S and ending velocity E, at time T their velocity should be S + (E-S)*T.
    // The particle position is the sum of this velocity over the range 0 to T.
    // To compute the position directly, we must integrate the velocity
    // equation. Integrating S + (E-S)*T for T produces S*T + (E-S)*T*T/2.

    float velocityIntegral = startVelocity * normalizedAge +
                             (endVelocity - startVelocity) * normalizedAge *
                                                             normalizedAge / 2;
     
    position += normalize(velocity) * velocityIntegral * Duration;
    
    // Apply the gravitational force.
    position += Gravity * age * normalizedAge;
    
	return float4(position, 1);
}


// Vertex shader helper for computing the size of a particle.
float ComputeParticleSize(float4 projectedPosition,
                          float randomValue, float normalizedAge)
{
    // Apply a random factor to make each particle a slightly different size.
    float startSize = lerp(StartSize.x, StartSize.y, randomValue);
    float endSize = lerp(EndSize.x, EndSize.y, randomValue);
    
    // Compute the actual size based on the age of the particle.
    float size = lerp(startSize, endSize, normalizedAge);
    
    // Project the size into screen coordinates.
    return size * Projection._m11 / projectedPosition.w * ViewportHeight / 2;
}


// Vertex shader helper for computing the color of a particle.
float4 ComputeParticleColor(float4 projectedPosition,
                            float randomValue, float normalizedAge)
{
    // Apply a random factor to make each particle a slightly different color.
    float4 color = lerp(MinColor, MaxColor, randomValue);
    
    // Fade the alpha based on the age of the particle. This curve is hard coded
    // to make the particle fade in fairly quickly, then fade out more slowly:
    // plot x*(1-x)*(1-x) for x=0:1 in a graphing program if you want to see what
    // this looks like. The 6.7 scaling factor normalizes the curve so the alpha
    // will reach all the way up to fully solid.
    
    color.a *= normalizedAge * (1-normalizedAge) * (1-normalizedAge) * 6.7;
    
    // On Xbox, point sprites are clipped away entirely as soon as their center
    // point moves off the screen, even when rest of the sprite should still be
    // visible. This causes an irritating flicker when large particles reach the
    // edge of the screen. We can hide this problem by fading the sprite out
    // slightly before it is about to get clipped.
#ifdef XBOX
    float2 screenPosition = abs(projectedPosition.xy / projectedPosition.w);
    
    float distanceFromBorder = 1 - max(screenPosition.x, screenPosition.y);
    
    // The value 16 is chosen arbitrarily. Make this smaller to fade particles
    // out sooner, or larger to fade them later (which can cause visible popping).
    color.a *= saturate(distanceFromBorder * 16);
#endif
    
    return color;
}


// Vertex shader helper for computing the rotation of a particle.
float4 ComputeParticleRotation(float randomValue, float age)
{    
    // Apply a random factor to make each particle rotate at a different speed.
    float rotateSpeed = lerp(RotateSpeed.x, RotateSpeed.y, randomValue);
    
    float rotation = rotateSpeed * age;

    // Compute a 2x2 rotation matrix.
    float c = cos(rotation);
    float s = sin(rotation);
    
    float4 rotationMatrix = float4(c, -s, s, c);
    
    // Normally we would output this matrix using a texture coordinate interpolator,
    // but texture coordinates are generated directly by the hardware when drawing
    // point sprites. So we have to use a color interpolator instead. Only trouble
    // is, color interpolators are clamped to the range 0 to 1. Our rotation values
    // range from -1 to 1, so we have to scale them to avoid unwanted clamping.
    
    rotationMatrix *= 0.5;
    rotationMatrix += 0.5;
    
    return rotationMatrix;
}


// Custom vertex shader animates particles entirely on the GPU.
VertexShaderOutput VS(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    // Compute the age of the particle.
    float age = CurrentTime - input.Time;
    
    // Apply a random factor to make different particles age at different rates.
    age *= 1 + input.Random.x * DurationRandomness;
    
    // Normalize the age into the range zero to one.
    float normalizedAge = saturate(age / Duration);

    // Compute the particle position, size, color, and rotation.
    float4 position = ComputeParticlePosition(input.Position, input.Velocity,
                                              age, normalizedAge);
											  
    //Get the depth map texture coordinaes
	output.TexDepth = mul(position, matTexProj);
	
	//output.Pos= mul(output.Pos, matViewProj);
	// Apply the camera view and projection transforms to get a screen space position
    output.Position = mul(mul(position, View), Projection);
	
    output.Size = ComputeParticleSize(output.Position, input.Random.y, normalizedAge);
    output.Color = ComputeParticleColor(output.Position, input.Random.z, normalizedAge);
    output.Rotation = ComputeParticleRotation(input.Random.w, age);
	
	//Get depth after view,projection transforms
	output.Depth = output.Position.z;  
	
    return output;
}


// Pixel shader input structure for particles that do not rotate.
struct NonRotatingPixelShaderInput
{
    float4 Color : COLOR0;
    float  Depth   : TEXCOORD1; //used for soft particle alpha change
	float4 TexDepth	: TEXCOORD2;
#ifdef XBOX
    float2 TextureCoordinate : SPRITETEXCOORD;
#else
    float2 TextureCoordinate : TEXCOORD0;
#endif
};


// Pixel shader for drawing particles that do not rotate.
float4 NonRotatingPixelShader(NonRotatingPixelShaderInput input) : COLOR0
{
	//Sample the depth from the depth map:
	float SceneDepth = tex2Dproj(DepthSampler, input.TexDepth);
	
	//Depth of the particle:
	//float CurDepth = input.Depth;
	
	//Get depth difference between the particle and the scene:
	float DepthDiff= SceneDepth-input.Depth;
	
	DepthDiff = saturate(DepthDiff);
	
	//Lerp between red and yellow according to alpha
	//float4 red= float4(1.0f,0.0,0,0);
	//float4 yellow= float4(1.0f,1.0f,0,0);
	//float4 OUT= lerp(red, yellow, pow(IN.A, 2));
	float4 final = input.Color;
	//OUT.a= IN.A;
	
	//Adjust transparency according to the depth difference:
	final.a *= DepthDiff;
	float4 tex = tex2D(TextureSampler, input.TextureCoordinate);
	return final * tex;
}


// Pixel shader input structure for particles that can rotate.
struct RotatingPixelShaderInput
{
    float4 Color : COLOR0;
    float4 Rotation : COLOR1;
	float  Depth   : TEXCOORD1; //used for soft particle alpha change
    float4 TexDepth	: TEXCOORD2;
#ifdef XBOX
    float2 TextureCoordinate : SPRITETEXCOORD;
#else
    float2 TextureCoordinate : TEXCOORD0;
#endif
};


// Pixel shader for drawing particles that can rotate. It is not actually
// possible to rotate a point sprite, so instead we rotate our texture
// coordinates. Leaving the sprite the regular way up but rotating the
// texture has the exact same effect as if we were able to rotate the
// point sprite itself.
float4 RotatingPixelShader(RotatingPixelShaderInput input) : COLOR0
{
    float2 textureCoordinate = input.TextureCoordinate;

    // We want to rotate around the middle of the particle, not the origin,
    // so we offset the texture coordinate accordingly.
    textureCoordinate -= 0.5;
    
    // Apply the rotation matrix, after rescaling it back from the packed
    // color interpolator format into a full -1 to 1 range.
    float4 rotation = input.Rotation * 2 - 1;
    
    textureCoordinate = mul(textureCoordinate, float2x2(rotation));
    
    // Point sprites are squares. So are textures. When we rotate one square
    // inside another square, the corners of the texture will go past the
    // edge of the point sprite and get clipped. To avoid this, we scale
    // our texture coordinates to make sure the entire square can be rotated
    // inside the point sprite without any clipping.
    textureCoordinate *= sqrt(2);
    
    // Undo the offset used to control the rotation origin.
    textureCoordinate += 0.5;

	//Sample the depth from the depth map:
	float SceneDepth = tex2Dproj(DepthSampler, input.TexDepth);
	
	//Get depth difference between the particle and the scene:
	float DepthDiff= SceneDepth-input.Depth;
	
	DepthDiff = saturate(DepthDiff);

	float4 final = input.Color;

	//Adjust transparency according to the depth difference:
	final.a *= DepthDiff;
	float4 tex = tex2D(TextureSampler, textureCoordinate);
	return final * tex;
}


// Effect technique for drawing particles that do not rotate. Works with shader 1.1.
technique NonRotatingParticles
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 NonRotatingPixelShader();
    }
}


// Effect technique for drawing particles that can rotate. Requires shader 2.0.
technique RotatingParticles
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 RotatingPixelShader();
    }
}
