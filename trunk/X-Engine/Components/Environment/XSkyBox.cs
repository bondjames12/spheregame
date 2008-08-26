using System;
using System.Collections.Generic;
using System.Text;
using XEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XSkyBox : XComponent, XDrawable
    {
        string[] Filenames;
        
        Effect effect;
        Model model;

        public XSkyBox(ref XMain X)
            : base(ref X)
        {
            DrawOrder = 19;
        }

        public XSkyBox(ref XMain X, string Front, string Back, string Left, string Right, string Top, string Bottom) : base(ref X)
        {
            Filenames = new string[] { Bottom, Front, Back, Top, Left, Right };
        }

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            effect = Content.Load<Effect>(@"Content\XEngine\Effects\Skybox");
            model = Content.Load<Model>(@"Content\XEngine\Models\Skybox");

            Texture2D[] Sides = new Texture2D[6];

            for (int i = 0; i < 6; i++)
            {
                if (Filenames != null && !string.IsNullOrEmpty(Filenames[i]))
                    Sides[i] = Content.Load<Texture2D>(Filenames[i]);
            }

                int x = 0;
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        Texture2D tex = ((BasicEffect)part.Effect).Texture;

                        part.Effect = effect.Clone(X.GraphicsDevice);

                        if (Filenames != null && !string.IsNullOrEmpty(Filenames[x]))
                            tex = Sides[x];
                            
                        part.Effect.Parameters["tex"].SetValue(tex);

                        x++;
                    }
                }
            
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            //X.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
            //X.GraphicsDevice.RenderState.DepthBufferEnable = false;

            X.GraphicsDevice.RenderState.DepthBufferEnable = true;
            X.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * Matrix.CreateScale(100) * Matrix.CreateTranslation(Camera.Position));
                    effect.Parameters["Projection"].SetValue(Camera.Projection);
                    effect.Parameters["View"].SetValue(Camera.View);
                }
                mesh.Draw();
                i++;
            }
        }
    }
}
