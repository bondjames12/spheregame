#define Iterations 128

float2 Pan;
float Zoom;
float Aspect;

float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float2 c = (texCoord - 0.5) * Zoom * float2(1, Aspect) - Pan;
    float2 v = 0;
    
    for (int n = 0; n < Iterations; n++)
    {
        v = float2(v.x * v.x - v.y * v.y, v.x * v.y * 2) + c;
    }
    
    return (dot(v, v) > 1) ? 1 : 0;
}

technique
{
    pass
    {
        PixelShader = compile ps_3_0 PixelShader();
    }
}