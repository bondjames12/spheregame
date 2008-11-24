using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
//using XSIXNARuntime;
using System.Collections.Generic;

namespace XEngine
{
    public class XModel : XComponent
    {
        public Model Model;
        public BoundingBox Boundingbox;
        public bool HasAnimatedTextures = false;
        public Dictionary<int,XGifTexture> Giftextures;

        public int Number;
        public static int Count;

        public string Filename;
        public object Parent;

        public SASContainer SASData = new SASContainer();

        #region EditorProperties

        public string Filename_editor { get {return Filename;} set { Filename = value;} }

        #endregion

        public XModel(ref XMain X, string Filename) : base(ref X)
        {
            this.Filename = Filename;
            Giftextures = new Dictionary<int,XGifTexture>();
            Count++;
            Number = Count;
        }

        public override void Load(ContentManager Content)
        {
            //Attempt to load the asset from filesystem
            this.Model = Content.Load<Model>(Filename);
            System.Diagnostics.Debug.WriteLine(Filename);
            //if loaded then generate a bounding box
            if (this.Model != null)
                Boundingbox = X.Tools.CreateBoundingBox(this.Model);
            
                //look for any animated textures
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    for(int k=0;k<mesh.Effects.Count;k++)
                    {
                        Effect effect = mesh.Effects[k];

                        foreach (EffectParameter para in effect.Parameters)
                        {
                            if(para.ParameterType == EffectParameterType.String)
                            {
                                if (para.Name.Contains("AnimationFileName"))
                                {
                                    string animfilename = para.GetValueString();
                                    if (!string.IsNullOrEmpty(animfilename))
                                    {
                                        //found an animation texture filename!
                                        //does it have a file extension??
                                        animfilename = System.IO.Path.GetDirectoryName(animfilename) + "\\" +
                                                       System.IO.Path.GetFileNameWithoutExtension(animfilename);
                                        //load gif texture
                                        XGifTexture anim = new XGifTexture(ref X, animfilename);
                                        anim.Load(X.Content);
                                        anim.Start();
                                        Giftextures.Add(k, anim);
                                        HasAnimatedTextures = true;
                                        //System.Diagnostics.Debug.WriteLine(animfilename);
                                    }
                                }
                            }
                        }
                    }
                }

            base.Load(Content);
        }

        public void InitDefaultSASLighting()
        {
            // initialize lights by default
            SASPointLight light1 = new SASPointLight();
            SASPointLight light2 = new SASPointLight();
            SASPointLight light3 = new SASPointLight();

            light1.Color = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            light2.Color = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            light3.Color = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);

            light1.Position = new Vector4(100.0f, 100.0f, 100.0f, 1.0f);
            light2.Position = new Vector4(-100.0f, 100.0f, 100.0f, 1.0f);
            light3.Position = new Vector4(0.0f, 0.0f, -100.0f, 1.0f);

            light1.Range = 10000.0f;
            light2.Range = 10000.0f;
            light3.Range = 10000.0f;

            SASData.PointLights.Add(light1);
            SASData.PointLights.Add(light2);
            SASData.PointLights.Add(light3);
        }

        public void SetShader(Effect setEffectTo)
        {
            if (loaded)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        Effect effect = setEffectTo.Clone(X.GraphicsDevice);

                        part.Effect = effect;
                    }
                }
            }
        }
    }
}
