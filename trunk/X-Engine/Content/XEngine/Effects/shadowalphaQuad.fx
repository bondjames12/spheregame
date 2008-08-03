
//Input variables

texture baseTexture;

sampler baseSampler = 
sampler_state
{
    Texture = < baseTexture >;
    MipFilter = POINT;
    MinFilter = POINT;
    MagFilter = POINT;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
};


struct VS_INPUT
{
    float4 ObjectPos: POSITION;
    float2 TextureCoords: TEXCOORD0;
};

struct VS_OUTPUT 
{
   float4 ScreenPos:   POSITION;
   float2 TextureCoords: TEXCOORD0;
};

struct PS_OUTPUT 
{
   float4 Color:   COLOR;
};



VS_OUTPUT SimpleVS(VS_INPUT In)
{
   VS_OUTPUT Out;

    //Move to screen space
    Out.ScreenPos = In.ObjectPos;
    Out.TextureCoords = In.TextureCoords;
    
    return Out;
}

PS_OUTPUT SimplePS(VS_OUTPUT In)
{
    PS_OUTPUT Out;
    
    Out.Color = tex2D(baseSampler, In.TextureCoords);
    
    Out.Color.a = 0.3f;
    
    return Out;
}

//--------------------------------------------------------------//
// Technique Section for Simple screen transform
//--------------------------------------------------------------//
technique Simple
{
   pass Single_Pass
   {
        SrcBlend = SrcAlpha; 
        DestBlend = InvSrcAlpha; 

        VertexShader = compile vs_2_0 SimpleVS();
        PixelShader = compile ps_2_0 SimplePS();
   }
}