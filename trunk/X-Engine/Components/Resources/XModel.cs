using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
//using XSIXNARuntime;
using System.Collections.Generic;

namespace XEngine
{
    public class XModel : XComponent
    {
        private XMain X;
        public Model Model;
        public BoundingBox bb;
        public BoundingSphere bs;
        public bool HasAnimatedTextures = false;
        public Dictionary<int,XGifTexture> Giftextures;

        public int Number;
        public static int Count;

        public string Filename;
        public object Parent;

        public SASContainer SASData;

        #region EditorProperties

        public string Filename_editor { get {return Filename;} set { Filename = value;} }

        #endregion

        public XModel(ref XMain X, string Filename) : base(ref X)
        {
            this.X = X;
            SASData = new SASContainer(ref X);
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
                //Load bounding information from Model.Tag (this was added by the BoundingCalculator) processor
                //Boundingbox = X.Tools.CreateBoundingBox(this.Model);
                try
                {
                    if(Model.Tag is Dictionary<string, object>)
                    {
                        if((Model.Tag as Dictionary<string, object>).ContainsKey("BoundingBox"))
                        {
                            bb = (BoundingBox)(Model.Tag as Dictionary<string, object>)["BoundingBox"];
                            bs = (BoundingSphere)(Model.Tag as Dictionary<string, object>)["BoundingSphere"];
                        }//TODO: animation data, have not added bb and bs to that yet
                    }
                }
                catch (System.Exception ex)
                {
                    throw new System.NullReferenceException("Load Bounding information from Model.Tag failed. Is this model using the Animation or Bounding Calculator Processors?", ex);
                }

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
