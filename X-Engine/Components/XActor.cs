using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using XSIXNARuntime;

namespace XEngine
{
    public class XActor : XComponent, XDrawable
    {
        public int ModelNumber;
        public bool AlphaBlendable = false;

        XModel mod;
        public XModel model
        {
            get { return mod; }
            set { mod = value; ModelNumber = mod.Number; }
        }

        public PhysicsObject PhysicsBody;

        public Vector3 modeloffset;

        public int PhysicsID { get { return PhysicsBody.PhysicsBody.ID; } }

        public BoundingBox boundingBox { get { if (PhysicsBody.PhysicsBody.CollisionSkin != null) return PhysicsBody.PhysicsBody.CollisionSkin.WorldBoundingBox; else return new BoundingBox(Position, Position);  } }

        List<int> collisions = new List<int>();
        public List<int> Collisions
        {
            get
            {
                collisions.Clear();
                for (int i = 0; i < PhysicsBody.PhysicsBody.CollisionSkin.Collisions.Count; i++)
                    collisions.Add(PhysicsBody.PhysicsBody.CollisionSkin.Collisions[i].SkinInfo.Skin1.ID);
                return collisions;
            }
        }

        public Vector3 Position
        {
            get { return PhysicsBody.PhysicsBody.Position; }
            set { PhysicsBody.PhysicsBody.MoveTo(value, Matrix.Identity);  }
        }

        public bool Immovable
        {
            get { return PhysicsBody.PhysicsBody.Immovable; }
            set { PhysicsBody.PhysicsBody.Immovable = value; }
        }

        public Matrix Orientation 
        { 
            get { return PhysicsBody.PhysicsBody.Orientation; }
            set { PhysicsBody.PhysicsBody.Orientation = value; }
        }

        public Vector3 Velocity
        {
            get { return PhysicsBody.PhysicsBody.Velocity; }
            set { PhysicsBody.PhysicsBody.Velocity = value; }
        }

        public Vector3 Scale
        {
            get { return PhysicsBody.scale; }
            set { PhysicsBody.scale = value; }
        }

        float mass;
        public float Mass
        {
            get { return mass; }
            set { PhysicsBody.SetMass(value); mass = value; }
        }

        bool collisionenabled = true;
        public bool CollisionEnabled
        {
            set { collisionenabled = value; if (value) PhysicsBody.PhysicsBody.EnableBody(); else PhysicsBody.PhysicsBody.DisableBody(); }
            get { return collisionenabled; }
        }

        Vector3 rotation;
        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; Orientation = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotation.Y), MathHelper.ToRadians(rotation.X), MathHelper.ToRadians(rotation.Z)); }
        }

        public XActor(XMain X, PhysicsObject Object, XModel model, Vector3 ModelScale, Vector3 ModelOffset, Vector3 Velocity, float Mass) : base(X)
        {
            model.ParentActor = this;
            this.model = model;
            this.mass = Mass;
            this.PhysicsBody = Object;

            PhysicsBody.SetMass(Mass);
            PhysicsBody.scale = ModelScale;
            PhysicsBody.PhysicsBody.Velocity = Velocity;
            modeloffset = ModelOffset;
        }

        

        public Matrix GetWorldMatrix()
        {
            return PhysicsBody.GetWorldMatrix(model.Model, modeloffset);
        }

        public Vector3 GetScreenCoordinates(XCamera Camera)
        {
            return X.Tools.UnprojectVector3(this.Position, Camera, GetWorldMatrix());
        }

        public override void Draw(GameTime gameTime, XCamera Camera)
        {
            if (model != null && model.Loaded)
            {
                Matrix World = PhysicsBody.GetWorldMatrix(model.Model, modeloffset);

                if (AlphaBlendable)
                {
                    X.GraphicsDevice.RenderState.AlphaBlendEnable = true;
                    X.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha; // source rgb * source alpha
                    X.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One; // don't modify source alpha
                    X.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha; // dest rgb * (255 - source alpha)
                    X.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha; // dest alpha * (255 - source alpha)
                    X.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add; // add source and dest results
                }
                else
                    X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

                //X.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
                
                //Set camera params, compute matrices
                model.SASData.Camera.NearFarClipping.X = Camera.NearPlane;
                model.SASData.Camera.NearFarClipping.Y = Camera.FarPlane;
                model.SASData.Camera.Position.X = Camera.Position.X;
                model.SASData.Camera.Position.Y = Camera.Position.Y;
                model.SASData.Camera.Position.Z = Camera.Position.Z;
                model.SASData.Projection = Camera.Projection;
                model.SASData.View = Camera.View;
                model.SASData.Model = World;
                model.SASData.ComputeViewAndProjection();
                model.SASData.ComputeModel();

                X.Renderer.DrawModel(model, Camera);
            }

            if (DebugMode)
                X.DebugDrawer.DrawCube(boundingBox.Min, boundingBox.Max, Color.White, Matrix.Identity, Camera);
        }

        public override void Disable()
        {
            PhysicsBody.PhysicsBody.DisableBody();
            base.Disable();
        }
    }
}
