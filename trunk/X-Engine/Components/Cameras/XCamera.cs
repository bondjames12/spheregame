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
        public XCamera Base;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Target = new Vector3(0, 0, -10);
        public Vector3 Up = new Vector3(0, 1, 0);
        public RenderTypes RenderType;

        public Matrix View;
        public Matrix Projection;
        public Matrix RotationMatrix;

        public Matrix ShadowView;
        public Matrix ShadowProjection;

        public Vector3 Direction { get { return Target - Position; } }

        public float AspectRatio;
        public float NearPlane;
        public float FarPlane;

        public BoundingFrustum Frustrum;

        public XCamera(ref XMain X, float nearplane, float farplane)
            : base(ref X)
        {
            this.NearPlane = nearplane;
            this.FarPlane = farplane;
            Projection = GenerateProjection(ref X, MathHelper.PiOver4, ProjectionType.Perspective,NearPlane,  FarPlane);
            RenderType = RenderTypes.Normal;
            Base = this;
            DrawOrder = 50000;
        }

        public enum ProjectionType { Perspective, Orthographic }

        public Matrix GenerateProjection(ref XMain X, float FoV, ProjectionType type, float nearplane, float farplane)
        {
            AspectRatio = (float)X.GraphicsDevice.Viewport.Width / (float)X.GraphicsDevice.Viewport.Height;
            return GenerateProjection(ref X, FoV, AspectRatio, type,nearplane,farplane);
        }

        public Matrix GenerateProjection(ref XMain X, float FoV, float AspectRatio, ProjectionType type, float nearplane, float farplane)
        {
            if (type == ProjectionType.Perspective)
            {
                return Matrix.CreatePerspectiveFieldOfView(FoV, AspectRatio, nearplane, farplane);
            }
            else if (type == ProjectionType.Orthographic)
            {
                return Matrix.CreateOrthographic(100, 100, nearplane, farplane);
            }

            return Matrix.Identity;
        }

        public void SetOrthographic(float width, float height, float near, float far)
        {
            this.NearPlane = near;
            this.FarPlane = far;
            Projection = Matrix.CreateOrthographic(width, height, near, far);
        }

        public void SetPerspective(float FOV, float AspectRatio, float near, float far)
        {
            this.AspectRatio = AspectRatio;
            this.NearPlane = near;
            this.FarPlane = far;
            Projection = Matrix.CreatePerspectiveFieldOfView(FOV,AspectRatio,near, far);
        }

        public override void Update(ref GameTime gameTime)
        {
            View = Matrix.CreateLookAt(Position, Target, Up);
            Frustrum = new BoundingFrustum(View * Projection);
        }
    }
}
