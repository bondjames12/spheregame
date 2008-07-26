using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XCamera : XComponent, XUpdateable
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Target = new Vector3(0, 0, -10);
        public Vector3 Up = new Vector3(0, 1, 0);

        public Matrix View;
        public Matrix Projection;
        public Matrix RotationMatrix;

        public Vector3 Direction { get { return Target - Position; } }

        public float AspectRatio;

        public XCamera(XMain X) : base(X)
        {
            Projection = GenerateProjection(X, MathHelper.PiOver4);
        }

        public Matrix GenerateProjection(XMain X, float FoV)
        {
            AspectRatio = (float)X.Game.Window.ClientBounds.Width / (float)X.Game.Window.ClientBounds.Height;
            return GenerateProjection(X, FoV, AspectRatio);
        }

        public Matrix GenerateProjection(XMain X, float FoV, float AspectRatio)
        {
            return Matrix.CreatePerspectiveFieldOfView(FoV, AspectRatio, 1, 10000);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            View = Matrix.CreateLookAt(Position, Target, Up);
        }
    }
}
