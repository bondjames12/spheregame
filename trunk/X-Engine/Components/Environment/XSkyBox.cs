using System;
using System.Collections.Generic;
using System.Text;
using XEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XSkyBox : XComponent, XIDrawable
    {
        List<string> Filenames;
        TextureCube cube;
        
        Effect effect;
        Model model;

        #region Editor Properties
        public string SkyCubeMap
        {
            get 
            { 
                if (Filenames.Count == 1) 
                    return Filenames[0]; 
                else
                    return ""; 
            }
            set
            {
                Filenames = new List<string>();
                Filenames.Add(value);
            }
        }

        #endregion

        public XSkyBox(ref XMain X)
            : base(ref X)
        {
            DrawOrder = 19;
        }

        public XSkyBox(ref XMain X, string Front, string Back, string Left, string Right, string Top, string Bottom) : base(ref X)
        {
            DrawOrder = 19;
            Filenames = new List<string>(new string[] { Bottom, Front, Back, Top, Left, Right });
        }

        public XSkyBox(ref XMain X, string SkyCubeMap)
            : base(ref X)
        {
            DrawOrder = 19;
            Filenames = new List<string>();
            if(!string.IsNullOrEmpty(SkyCubeMap)) Filenames.Add(SkyCubeMap);
        }

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            effect = Content.Load<Effect>(@"Content\XEngine\Effects\Skybox");
            model = Content.Load<Model>(@"Content\XEngine\Models\Skybox");

            if (Filenames.Count == 1) //theres only 1 texture in this list it must be a cube map
            {
                effect.CurrentTechnique = effect.Techniques["SkyCubeMap"];
                cube = Content.Load<TextureCube>(Filenames[0]);
            }
            else
            {
                effect.CurrentTechnique = effect.Techniques["Sky"];

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
            base.Load(X.Content);
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            if (!loaded) return;
            //X.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
            //X.GraphicsDevice.RenderState.DepthBufferEnable = false;

            X.GraphicsDevice.RenderState.DepthBufferEnable = true;
            X.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);


            if (Filenames.Count == 1)
            {
                effect.Parameters["Projection"].SetValue(Camera.Projection);
                effect.Parameters["View"].SetValue(Camera.View);

                if (Filenames.Count == 1)
                {
                    effect.Parameters["EyePosition"].SetValue(Camera.Position);
                    effect.Parameters["skyCubeTexture"].SetValue(cube);
                }

                for (int pass = 0; pass < effect.CurrentTechnique.Passes.Count; pass++)
                {
                    for (int msh = 0; msh < model.Meshes.Count; msh++)
                    {
                        ModelMesh mesh = model.Meshes[msh];

                        effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * Matrix.CreateScale(100) * Matrix.CreateTranslation(Camera.Position));

                        for (int prt = 0; prt < mesh.MeshParts.Count; prt++)
                            mesh.MeshParts[prt].Effect = effect;
                        mesh.Draw();
                    }
                }
            }
            else
            {//render our other method
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * Matrix.CreateScale(100) * Matrix.CreateTranslation(Camera.Position));
                        effect.Parameters["Projection"].SetValue(Camera.Projection);
                        effect.Parameters["View"].SetValue(Camera.View);
                    }
                    mesh.Draw();
                }
            }

        }
    }
}
