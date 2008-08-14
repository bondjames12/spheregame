using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public enum RenderTypes
    {
        Normal, Depth 
    }


    public class XCamera : XComponent, XUpdateable
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Target = new Vector3(0, 0, -10);
        public Vector3 Up = new Vector3(0, 1, 0);
        public RenderTypes RenderType;

        public Matrix View;
        public Matrix Projection;
        public Matrix RotationMatrix;

        public Vector3 Direction { get { return Target - Position; } }

        public float AspectRatio;
        public float NearPlane = 1f;
        public float FarPlane = 100;

        public BoundingFrustum Frustrum;

        public XCamera(XMain X, float NearPlane, float FarPlane)
            : base(X)
        {
            Projection = GenerateProjection(X, MathHelper.PiOver4, ProjectionType.Perspective,NearPlane,  FarPlane);
            RenderType = RenderTypes.Normal;
        }

        public enum ProjectionType { Perspective, Orthographic }

        public Matrix GenerateProjection(XMain X, float FoV, ProjectionType type, float NearPlane, float FarPlane)
        {
            AspectRatio = (float)X.GraphicsDevice.Viewport.Width / (float)X.GraphicsDevice.Viewport.Height;
            return GenerateProjection(X, FoV, AspectRatio, type,NearPlane,FarPlane);
        }

        public Matrix GenerateProjection(XMain X, float FoV, float AspectRatio, ProjectionType type, float NearPlane, float FarPlane)
        {
            if (type == ProjectionType.Perspective)
            {
                return Matrix.CreatePerspectiveFieldOfView(FoV, AspectRatio, NearPlane, FarPlane);
            }
            else if (type == ProjectionType.Orthographic)
            {
                //NearPlane = -170;
                //FarPlane = 170;
                return Matrix.CreateOrthographic(100, 100, NearPlane, FarPlane);
            }

            return Matrix.Identity;
        }

        public override void Update(ref GameTime gameTime)
        {
            View = Matrix.CreateLookAt(Position, Target, Up);
            Frustrum = new BoundingFrustum(View * Projection);
        }
    }
}
