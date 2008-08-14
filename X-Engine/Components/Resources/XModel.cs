using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XSIXNARuntime;

namespace XEngine
{
    public class XModel : XComponent
    {
        Model model;

        public int Number;
        public static int Count;

        public Model Model { get { return model; } }
        public string Filename;
        public XActor ParentActor;

        public XSISASContainer SASData = new XSISASContainer();

        public XModel(XMain X, string Filename) : base(X)
        {
            this.Filename = Filename;

            Count++;
            Number = Count;
        }

        public override void Load(ContentManager Content)
        {
            base.Load(Content);
            this.model = Content.Load<Model>(Filename);
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
            if (Loaded)
            {
                foreach (ModelMesh mesh in model.Meshes)
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
