using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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

        public Texture2D GetMandelBrot(Vector2 pan, float zoom)
        {
                Effect mandelbrot;
                Texture2D texture;

                mandelbrot = X.Content.Load<Effect>("Content/Effects/Mandelbrot");

                int w = X.GraphicsDevice.Viewport.Width;
                int h = X.GraphicsDevice.Viewport.Height;

                texture = new Texture2D(X.GraphicsDevice, w, h, 1,
                                             TextureUsage.None, SurfaceFormat.Color);

                float aspectRatio = (float)h / (float)w;

                mandelbrot.Parameters["Pan"].SetValue(pan);
                mandelbrot.Parameters["Zoom"].SetValue(zoom);
                mandelbrot.Parameters["Aspect"].SetValue(aspectRatio);

                RenderTarget2D rendertarg = new RenderTarget2D(X.GraphicsDevice, 256, 256, 1, SurfaceFormat.Color);
                DepthStencilBuffer stencil = new DepthStencilBuffer(X.GraphicsDevice, 256, 256, DepthFormat.Depth24);
                DepthStencilBuffer old = X.GraphicsDevice.DepthStencilBuffer;
                X.GraphicsDevice.DepthStencilBuffer = stencil;
                X.GraphicsDevice.SetRenderTarget(0, rendertarg);
                X.GraphicsDevice.Clear(Color.White);


                SpriteBatch spriteBatch = new SpriteBatch(X.GraphicsDevice);

                spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                mandelbrot.Begin();
                mandelbrot.CurrentTechnique.Passes[0].Begin();

                spriteBatch.Draw(texture, Vector2.Zero, Color.White);

                spriteBatch.End();
                mandelbrot.CurrentTechnique.Passes[0].End();
                mandelbrot.End();

                X.GraphicsDevice.SetRenderTarget(0, null);
                X.GraphicsDevice.DepthStencilBuffer = old;

                return rendertarg.GetTexture();
        }
    }
}
