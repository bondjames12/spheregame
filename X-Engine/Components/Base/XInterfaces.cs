using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public interface XUpdateable
    {
        void Update(GameTime gameTime);
    }

    public interface XDrawable
    {
        void Draw(GameTime gameTime, XCamera Camera);
        void SetProjection(Matrix Projection);
    }
}