using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public interface XUpdateable
    {
        void Update(ref GameTime gameTime);
    }

    public interface XDrawable
    {
        void Draw(ref GameTime gameTime,ref XCamera Camera);
        void SetProjection(Matrix Projection);
    }
}