using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace XEngine
{
    public class XRenderer : XComponent, XDrawable
    {
        public XRenderer(XMain X) : base(X) 
        {
        }

        public override void Draw(GameTime gameTime, XCamera Camera)
        {
            X.GraphicsDevice.Clear(Color.CornflowerBlue);

            X.GraphicsDevice.RenderState.DepthBufferEnable = true;
            X.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            List<List<XComponent>> draw = new List<List<XComponent>>();

            foreach (XComponent component in X.Components)
            {
                if (component.AutoDraw && component is XDrawable && !(component is XRenderer) && !(component is XConsole))
                {
                    if (component.DrawOrder > draw.Count)
                    {
                        List<XComponent> level = new List<XComponent>();
                        level.Add(component);
                        draw.Add(level);
                    }
                    else
                    {
                        draw[component.DrawOrder - 1].Add(component);
                    }
                }
            }

            foreach (List<XComponent> level in draw)
                foreach (XComponent compoenent in level)
                    compoenent.Draw(gameTime, Camera);

            X.spriteBatch.Begin();

            if (X.Console.Visible)
                X.Console.Draw(gameTime, Camera);
            else
                X.Debug.Draw(gameTime, Camera);

            X.spriteBatch.End();
        }

        public virtual void DrawModel(XModel Model, XCamera Camera, Matrix World)
        {
            foreach (ModelMesh mesh in Model.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = World;
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;

                    effect.PreferPerPixelLighting = true;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
