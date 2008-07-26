using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public interface XLoadable
    {
        bool Loaded { get; }
        void Load(ContentManager Content);
    }

    public interface XUpdateable
    {
        void Update(GameTime gameTime);
    }

    public interface XDrawable
    {
        int DrawOrder { get; set; }
        bool AutoDraw { get; set; }
        void Draw(GameTime gameTime, XCamera Camera);
        void SetProjection(Matrix Projection);
    }
}