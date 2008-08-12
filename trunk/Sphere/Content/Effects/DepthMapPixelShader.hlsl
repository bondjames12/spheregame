//This pixel shader requires XSI_VertexToPixel in xsi_include9.hlsl
//takes the screen space pixel position and divides Z by W 

float4 DepthMapPixelShader(XSI_VertexToPixel IN) : COLOR0
{
	//In our shaders texcoord4 contains the global space position of the pixel
    return IN.texcoord4.z/IN.texcoord4.w;
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

technique DepthMapStatic
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VSStatic();
        PixelShader = compile ps_2_0 DepthMapPixelShader();
    }
}

technique DepthMapSkinned
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VSSkinned();
        PixelShader = compile ps_2_0 DepthMapPixelShader();
    }
}