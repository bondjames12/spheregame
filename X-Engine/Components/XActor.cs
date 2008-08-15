using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using XSIXNARuntime;

namespace XEngine
{
    public class XActor : XComponent, XDrawable
    {
        public XModel model;

        public XPhysicsObject PhysicsObject;

        public Vector3 modeloffset;

        #region Properties
        public int PhysicsID { get { return PhysicsObject.PhysicsBody.ID; } }

        public BoundingBox boundingBox { get { if (PhysicsObject.PhysicsBody.CollisionSkin != null) return PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox; else return new BoundingBox(Position, Position);  } }

       
        List<int> collisions = new List<int>();
        public List<int> Collisions
        {
            get
            {
                collisions.Clear();
                for (int i = 0; i < PhysicsObject.PhysicsBody.CollisionSkin.Collisions.Count; i++)
                    collisions.Add(PhysicsObject.PhysicsBody.CollisionSkin.Collisions[i].SkinInfo.Skin1.ID);
                return collisions;
            }
        }

        public Vector3 Position
        {
            get { return PhysicsObject.PhysicsBody.Position; }
            set { PhysicsObject.PhysicsBody.MoveTo(value, Matrix.Identity);  }
        }

        public bool Immovable
        {
            get { return PhysicsObject.PhysicsBody.Immovable; }
            set { PhysicsObject.PhysicsBody.Immovable = value; }
        }

        public Matrix Orientation 
        { 
            get { return PhysicsObject.PhysicsBody.Orientation; }
            set { PhysicsObject.PhysicsBody.Orientation = value; }
        }

        public Vector3 Velocity
        {
            get { return PhysicsObject.PhysicsBody.Velocity; }
            set { PhysicsObject.PhysicsBody.Velocity = value; }
        }

        public Vector3 Scale
        {
            get { return PhysicsObject.scale; }
            set { PhysicsObject.scale = value; }
        }

        float mass;
        public float Mass
        {
            get { return mass; }
            set { PhysicsObject.SetMass(value); mass = value; }
        }

        bool collisionenabled = true;
        public bool CollisionEnabled
        {
            set { collisionenabled = value; if (value) PhysicsObject.PhysicsBody.EnableBody(); else PhysicsObject.PhysicsBody.DisableBody(); }
            get { return collisionenabled; }
        }

        Vector3 rotation;
        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; Orientation = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotation.Y), MathHelper.ToRadians(rotation.X), MathHelper.ToRadians(rotation.Z)); }
        }
 #endregion

        public XActor(XMain X, XPhysicsObject Object, XModel model, Vector3 ModelScale, Vector3 ModelOffset, Vector3 Velocity, float Mass)
            : base(X)
        {
            DrawOrder = 100;

            if (model != null)
            {
                this.model = model;
                model.ParentActor = this;
            }
            this.PhysicsObject = Object;
            this.mass = Mass;
            this.PhysicsObject.SetMass(Mass);
            this.PhysicsObject.scale = ModelScale;
            this.PhysicsObject.PhysicsBody.Velocity = Velocity;
            this.PhysicsObject.PhysicsBody.SetDeactivationTime(0.1f);
            this.PhysicsObject.PhysicsBody.SetActivityThreshold(5f, 5f);
            this.modeloffset = ModelOffset;
        }

        public Matrix GetWorldMatrix()
        {
            return PhysicsObject.GetWorldMatrix(model.Model, modeloffset);
        }

        public Vector3 GetScreenCoordinates(XCamera Camera)
        {
            return X.Tools.UnprojectVector3(this.Position, Camera, GetWorldMatrix());
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            if (model != null && model.loaded)
            {
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

                //Set camera params, compute matrices
                //model.SASData.Camera.NearFarClipping.X = Camera.NearPlane;
                //model.SASData.Camera.NearFarClipping.Y = Camera.FarPlane;
                //model.SASData.Camera.Position.X = Camera.Position.X;
                //model.SASData.Camera.Position.Y = Camera.Position.Y;
                //model.SASData.Camera.Position.Z = Camera.Position.Z;
                model.SASData.Projection = Camera.Projection;
                model.SASData.View = Camera.View;
                model.SASData.Model = PhysicsObject.GetWorldMatrix(model.Model, modeloffset);
                model.SASData.ComputeViewAndProjection();
                //model.SASData.ComputeModel();

                X.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

                X.Renderer.DrawModel(ref model,ref Camera);

                //restore render modes (shader files might have changes this!
                X.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            }

            if (DebugMode)
            {
                //Draw Frustum (Yellow) and Physics Bounding (White), In XActor these should be the same but draw then both anyway just in case
                X.DebugDrawer.DrawCube(boundingBox.Min, boundingBox.Max, Color.Yellow, Matrix.Identity, Camera);
                //X.DebugDrawer.DrawCube(PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox.Min, PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox.Max, Color.White, Matrix.Identity, Camera);
                //Not sure if this is different or not from the one above
                //X.DebugDrawer.DrawCube(PhysicsObject.PhysicsSkin.WorldBoundingBox.Min, PhysicsObject.PhysicsSkin.WorldBoundingBox.Max, Color.White, Matrix.Identity, Camera);
                //
            }
        }

        public override void Disable()
        {
            PhysicsObject.PhysicsBody.DisableBody();
            base.Disable();
        }

        /*
        List<Boundary> boundaries;

        public void UpdateDebug(ref GameTime gameTime)
        {
            boundaries.Clear();
            ReadOnlyCollection<Body> bodies = PhysicsSystem.CurrentPhysicsSystem.Bodies;
            foreach (Body body in bodies)
            {
                for (int i = 0; i < body.CollisionSkin.NumPrimitives; i++)
                {
                    Primitive p = body.CollisionSkin.GetPrimitiveOldWorld(i);
                    if (p.Type == (int)JigLibX.Geometry.PrimitiveType.Sphere)
                    {
                        Boundary b = new Boundary();
                        Sphere s = p as Sphere;
                        b.Scale = s.Radius;
                        b.Model = sphere;
                        b.Transform = Matrix.CreateTranslation(s.Position);
                        boundaries.Add(b);
                    }
                }
            }
        }

        public void DrawDebug(ref GameTime gameTime,ref  XCamera Camera) 
        { 
            foreach (Boundary b in boundaries) 
            { 
                Matrix[] transforms = new Matrix[b.Model.Bones.Count]; 
                b.Model.CopyAbsoluteBoneTransformsTo(transforms); 
                foreach (ModelMesh m in b.Model.Meshes) 
                { 
                    foreach (BasicEffect effect in m.Effects) 
                    { 
                        effect.World = Matrix.CreateScale(b.Scale) * b.Transform; 
                        effect.View = Camera.View; 
                        effect.Projection = CameraProjection; 
                    } 
                    m.Draw(); 
                } 
            } 
        } */
    }
}
