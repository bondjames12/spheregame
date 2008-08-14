using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace XEngine
{
    public class XDepthMap
    {
        //Store a reference to the XMain class
        XMain X;
        //Used to render the depth scene to
        public RenderTarget2D renderTarget;
        public DepthStencilBuffer depthbuffer;
        //Used to save the depthmap onto form the depth scene render
        public Texture2D depthMap;

        //Constructors
        public XDepthMap(XMain X)
        {
            this.X = X;
        }

        /// <summary>
        /// Initializes a new Render Target and sets the current Render Target to a target used by XDepthMap to save Depth information
        /// Call this before any Draw calls
        /// </summary>
        public void StartRenderToDepthMap()
        {
            //Initialize the Rendertarget to render the depth scene to using the device Presentation parameters
            PresentationParameters pp = X.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(X.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, 1, SurfaceFormat.Single);//X.GraphicsDevice.DisplayMode.Format);
            //depthbuffer = new DepthStencilBuffer(X.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, DepthFormat.Depth16);
            X.GraphicsDevice.SetRenderTarget(0, renderTarget);
            X.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

        }

        /// <summary>
        /// Resets the Render Target to normal and saves the Render Target to a Texture.
        /// Call this at the end of the Draw calls
        /// </summary>
        public void EndRenderToDepthMap()
        {
            //Resets Render Target to normal backbuffer
            X.GraphicsDevice.SetRenderTarget(0, null);
            //Saves the render target as a texture2d
            depthMap = renderTarget.GetTexture();
            //for testing? saves the texture to a file!
            //depthMap.Save("depthmap.bmp", ImageFileFormat.Bmp);
        }

    }
}
