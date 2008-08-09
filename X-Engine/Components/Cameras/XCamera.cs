using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public float NearPlane = 0.1f;
        public float FarPlane = 1000;

        public BoundingFrustum Frustrum;

        public XCamera(XMain X) : base(X)
        {
            Projection = GenerateProjection(X, MathHelper.PiOver4, ProjectionType.Perspective);
        }

        public enum ProjectionType { Perspective, Orthographic }

        public Matrix GenerateProjection(XMain X, float FoV, ProjectionType type)
        {
            AspectRatio = (float)X.GraphicsDevice.Viewport.Width / (float)X.GraphicsDevice.Viewport.Height;
            return GenerateProjection(X, FoV, AspectRatio, type);
        }

        public Matrix GenerateProjection(XMain X, float FoV, float AspectRatio, ProjectionType type)
        {
            if (type == ProjectionType.Perspective)
            {
                NearPlane = 0.1f;
                FarPlane = 1000;
                return Matrix.CreatePerspectiveFieldOfView(FoV, AspectRatio, NearPlane, FarPlane);
            }
            else if (type == ProjectionType.Orthographic)
            {
                NearPlane = -170;
                FarPlane = 170;
                return Matrix.CreateOrthographic(100, 100, NearPlane, FarPlane);
            }

            return Matrix.Identity;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            View = Matrix.CreateLookAt(Position, Target, Up);
            Frustrum = new BoundingFrustum(View * Projection);
        }
    }
}
