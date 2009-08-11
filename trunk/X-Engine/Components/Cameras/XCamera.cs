using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public enum RenderTypes
    {
        Normal, Depth 
    }


    public class XCamera : XComponent, XICamera, XIUpdateable
    {
        public enum ProjectionTypes { Perspective, Orthographic }

        public XCamera Base;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Target = new Vector3(0, 0, -10);
        public Vector3 Up = new Vector3(0, 1, 0);
        public RenderTypes RenderType;
        public ProjectionTypes ProjectionType;

        public Matrix View;
        public Matrix Projection;
        public Matrix ViewProjection;
        public Matrix ViewInverse;
        public Matrix RotationMatrix;
        public Matrix ShadowView;
        public Matrix ShadowProjection;
        public float AspectRatio;
        public float FOV;
        public float NearPlane;
        public float FarPlane;
        public BoundingFrustum Frustrum;

        public Vector3 Direction { get { return Target - Position; } }

        #region Editor Properties
        public Vector3 position
        {
            get { return Position; }
            set { Position = value; }
        }
        public Vector3 target
        {
            get { return Target; }
            set { Target = value; }
        }
        public Vector3 up
        {
            get { return Up; }
            set { Up = value; }
        }
        public float nearplane
        {
            get {return NearPlane;}
            set { NearPlane = value; GenerateProjection(ref X, MathHelper.PiOver4, this.ProjectionType, NearPlane, FarPlane); }
        }
        public float farplane
        {
            get {return FarPlane;}
            set { FarPlane = value; GenerateProjection(ref X, MathHelper.PiOver4, this.ProjectionType, NearPlane, FarPlane); }
        }
        public float fov
        {
            get { return FOV; }
            set { FOV = value; GenerateProjection(ref X, MathHelper.PiOver4, this.ProjectionType, NearPlane, FarPlane); }
        }
        public ProjectionTypes projectiontype
        {
            get { return ProjectionType; }
            set { ProjectionType = value; GenerateProjection(ref X, MathHelper.PiOver4, this.ProjectionType, NearPlane, FarPlane); }
        }

        public RenderTypes rendertype
        {
            get { return RenderType; }
            set { RenderType = value; }
        }

        public Matrix ViewMatrix
        {
            get { return View; }
        }
        public Matrix ProjectionMatrix
        {
            get { return Projection; }
        }
        #endregion


        public XCamera(ref XMain X, float nearplane, float farplane)
            : base(ref X)
        {
            ProjectionType = ProjectionTypes.Perspective;
            RenderType = RenderTypes.Normal;
            NearPlane = nearplane;
            FarPlane = farplane;
            FOV = MathHelper.PiOver4;
            GenerateProjection(ref X, this.FOV, this.ProjectionType, NearPlane, FarPlane);
            
            Base = this;
            DrawOrder = 50000;
        }

        

        public void GenerateProjection(ref XMain X, float FoV, ProjectionTypes type, float nearplane, float farplane)
        {
            AspectRatio = (float)X.GraphicsDevice.Viewport.Width / (float)X.GraphicsDevice.Viewport.Height;
            Projection = GenerateProjection(ref X, FoV, AspectRatio, type,nearplane,farplane);
            Frustrum = new BoundingFrustum(View * Projection);
        }

        public Matrix GenerateProjection(ref XMain X, float FoV, float AspectRatio, ProjectionTypes type, float nearplane, float farplane)
        {
            if (type == ProjectionTypes.Perspective)
            {
                return Matrix.CreatePerspectiveFieldOfView(FoV, AspectRatio, nearplane, farplane);
            }
            else if (type == ProjectionTypes.Orthographic)
            {
                return Matrix.CreateOrthographic(100, 100, nearplane, farplane);
            }

            return Matrix.Identity;
        }

        public void SetOrthographic(float width, float height, float near, float far)
        {
            NearPlane = near;
            FarPlane = far;
            Projection = Matrix.CreateOrthographic(width, height, near, far);
        }

        public void SetPerspective(float FOV, float AspectRatio, float near, float far)
        {
            AspectRatio = AspectRatio;
            NearPlane = near;
            FarPlane = far;
            Projection = Matrix.CreatePerspectiveFieldOfView(FOV,AspectRatio,near, far);
        }

        public override void Update(ref GameTime gameTime)
        {
            Matrix.CreateLookAt(ref Position,ref Target,ref Up, out View);
            Matrix.Multiply(ref View, ref Projection, out ViewProjection);
            Matrix.Invert(ref View, out ViewInverse);
            Frustrum.Matrix = ViewProjection;
            //Frustrum = new BoundingFrustum(ViewProjection); //replaced by above line *faster
        }
    }
}
