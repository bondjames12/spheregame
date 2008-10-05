using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XSIXNARuntime;

namespace XEngine
{
    public class XModel : XComponent
    {
        public Model Model;
        public BoundingBox boundingBox;

        public int Number;
        public static int Count;

        public string Filename;
        public object Parent;

        public XSISASContainer SASData = new XSISASContainer();

        #region EditorProperties

        public string Filename_editor { get {return Filename;} set { Filename = value;} }

        #endregion

        public XModel(ref XMain X, string Filename) : base(ref X)
        {
            this.Filename = Filename;

            Count++;
            Number = Count;
        }

        public override void Load(ContentManager Content)
        {
            //Attempt to load the asset from filesystem
            this.Model = Content.Load<Model>(Filename);
            //if loaded then generate a bounding box
            if (this.Model != null)
                boundingBox = X.Tools.CreateBoundingBox(this.Model);
            //look for any animated textures
            /*foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    foreach(EffectParameter para in part.Effect)
                    {
                        para.
                    }
                }
            }*/

    base.Load(Content);
        }

        public void InitDefaultSASLighting()
        {
            // initialize lights by default
            XSISASPointLight light1 = new XSISASPointLight();
            XSISASPointLight light2 = new XSISASPointLight();
            XSISASPointLight light3 = new XSISASPointLight();

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
