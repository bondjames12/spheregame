using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public class XTextureGenerator : XComponent
    {
        public XTextureGenerator(XMain X) : base(X) { }

        public Texture2D GetFlatColor(Color color)
        {
            RenderTarget2D rendertarg = new RenderTarget2D(X.GraphicsDevice, 1, 1, 1, SurfaceFormat.Color);
            DepthStencilBuffer stencil = new DepthStencilBuffer(X.GraphicsDevice, 1, 1, DepthFormat.Depth24);
            DepthStencilBuffer old = X.GraphicsDevice.DepthStencilBuffer;
            X.GraphicsDevice.DepthStencilBuffer = stencil;
            X.GraphicsDevice.SetRenderTarget(0, rendertarg);
            X.GraphicsDevice.Clear(color);
            X.GraphicsDevice.SetRenderTarget(0, null);
            X.GraphicsDevice.DepthStencilBuffer = old;
            return rendertarg.GetTexture();
        }
    }
}
