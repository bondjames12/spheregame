using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace XEngine
{
    public class XModel : XComponent, XLoadable
    {
        Model model;

        public int Number;
        public static int Count;

        public Model Model { get { return model; } }
        public string Filename { get; set; }

        public XModel(XMain X, string Filename) : base(X)
        {
            this.Filename = Filename;

            Count++;
            Number = Count;
        }

        public override void Load(ContentManager Content)
        {
            this.model = Content.Load<Model>(Filename);
            base.Load(Content);
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
