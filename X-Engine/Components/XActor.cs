using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using XSIXNARuntime;
using JigLibX.Geometry;

namespace XEngine
{
    public class XActor : XComponent, XDrawable
    {
        public XModel model;
        public int modelNumber; //the number to the Xmodel we are using
        public XPhysicsObject PhysicsObject;

        //not used yet
        public Vector3 modeloffset;
        public Vector3 rotation;
        
        #region Editor Properties


        //Some of these are still used by the engine and should be copied, renamed with _editor and the null checking taken off for speed
        public XPhysicsObject PhysicsObject_editor
        {
            get { return PhysicsObject; }
            set { PhysicsObject = value; }
        }

        public XModel model_editor
        {
            get { return model; }
            set { model = value; modelNumber = model.Number; }
        }

        public int PhysicsID { get { if (PhysicsObject != null) return PhysicsObject.PhysicsBody.ID; else return 0; } }

        public BoundingBox boundingBox
        { 
            get 
            {
                if (PhysicsObject != null)
                {
                    if (PhysicsObject.PhysicsBody.CollisionSkin != null)
                        return PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox;
                    else
                        return new BoundingBox(Position, Position);
                }
                else
                    return new BoundingBox();
            }
            /*set
            {
                PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox = value;
            }*/
        }

       
        List<int> collisions = new List<int>();
        public List<int> Collisions
        {
            get
            {
                if(PhysicsObject != null)
                {
                    collisions.Clear();
                    for (int i = 0; i < PhysicsObject.PhysicsBody.CollisionSkin.Collisions.Count; i++)
                        collisions.Add(PhysicsObject.PhysicsBody.CollisionSkin.Collisions[i].SkinInfo.Skin1.ID);
                    return collisions;
                }
                else return collisions;
            }
        }

        public Vector3 Position
        {
            get
            {
                if (PhysicsObject != null)
                    return PhysicsObject.PhysicsBody.Position;
                else
                    return Vector3.Zero;
            }
            set
            {
                if (PhysicsObject != null)
                    PhysicsObject.PhysicsBody.MoveTo(value, Matrix.Identity);
                else ;
            }
        }

        public bool Immovable
        {
            get { if (PhysicsObject != null) return PhysicsObject.PhysicsBody.Immovable; else return true; }
            set { if (PhysicsObject != null) PhysicsObject.PhysicsBody.Immovable = value; else ; }
        }

        public Matrix Orientation 
        {
            get { if (PhysicsObject != null) return PhysicsObject.PhysicsBody.Orientation; else return Matrix.Identity; }
            set { if (PhysicsObject != null) PhysicsObject.PhysicsBody.Orientation = value; else ; }
        }

        public Vector3 Velocity
        {
            get { if (PhysicsObject != null) return PhysicsObject.PhysicsBody.Velocity; else return Vector3.Zero; }
            set { if (PhysicsObject != null) PhysicsObject.PhysicsBody.Velocity = value; else ; }
        }

        public Vector3 Scale
        {
            get { if (PhysicsObject != null) return PhysicsObject.scale; else return Vector3.One; }
            set { if (PhysicsObject != null) PhysicsObject.scale = value; else ; }
        }

        
        public float Mass_editor
        {
            get { if (PhysicsObject != null) return PhysicsObject.PhysicsBody.Mass; else return 0; }
            set 
            {
                if (PhysicsObject != null)
                {
                    PhysicsObject.SetMass(value);
                }
                else ;
            }
        }

        bool collisionenabled = true;
        public bool CollisionEnabled
        {
            set 
            {
                if (PhysicsObject != null)
                {
                    collisionenabled = value;
                    if (value) PhysicsObject.PhysicsBody.EnableBody(); else PhysicsObject.PhysicsBody.DisableBody();
                }
                else ;
            }
            get { return collisionenabled; }
        }

        
        public Vector3 Rotation_editor
        {
            get { return rotation; }
            set { rotation = value; Orientation = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotation.Y), MathHelper.ToRadians(rotation.X), MathHelper.ToRadians(rotation.Z)); }
        }
 #endregion

        public XActor(ref XMain X, XPhysicsObject Object, XModel model, Vector3 ModelScale, Vector3 ModelOffset, Vector3 Velocity, float Mass)
            : base(ref X)
        {
            DrawOrder = 100;

            if (model != null)
            {
                this.model = model;
                model.Parent = this;
                this.modelNumber = model.Number;
            }
            this.PhysicsObject = Object;
            PhysicsObject.SetMass(Mass);
            //this.mass = Mass;
            this.PhysicsObject.SetMass(Mass);
            this.PhysicsObject.scale = ModelScale;
            this.PhysicsObject.PhysicsBody.Velocity = Velocity;
            this.PhysicsObject.PhysicsBody.SetDeactivationTime(0.1f);
            this.PhysicsObject.PhysicsBody.SetActivityThreshold(5f, 5f);
            //this.modeloffset = ModelOffset;
        }

        public XActor(ref XMain X, XModel model, Vector3 Position, Vector3 ModelOffset, Vector3 Velocity, float Mass)
            : base(ref X)
        {
            DrawOrder = 100;

            if (model != null)
            {
                this.model = model;
                model.Parent = this;
                this.modelNumber = model.Number;
            }
            if (model.Model != null)
            {
                this.PhysicsObject = new XSIBoneMapObject(Position,ref model);
                //this.PhysicsObject = Object;
                //this.PhysicsObject.scale = ModelScale;
                this.Scale = Vector3.One;
                //this.mass = Mass;
                PhysicsObject.SetMass(Mass);
                this.PhysicsObject.SetMass(Mass);
                this.PhysicsObject.PhysicsBody.Velocity = Velocity;
                this.PhysicsObject.PhysicsBody.SetDeactivationTime(0.1f);
                this.PhysicsObject.PhysicsBody.SetActivityThreshold(5f, 5f);
                //this.modeloffset = ModelOffset;
            }
        }

        public Matrix GetWorldMatrix()
        {
            return PhysicsObject.GetWorldMatrix(model.Model, Vector3.Zero);//modeloffset);
        }

        public Vector3 GetScreenCoordinates(XCamera Camera)
        {
            return X.Tools.UnprojectVector3(this.Position, Camera, GetWorldMatrix());
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            //null checks are for editor!
            //remove these eventually maybe using a compiler directive for an editorer version of the DLL?
            if (model != null && PhysicsObject != null && model.loaded)
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
                model.SASData.Model = PhysicsObject.GetWorldMatrix(model.Model, Vector3.Zero); //modeloffset);
                model.SASData.ComputeViewAndProjection();
                //model.SASData.ComputeModel();

                X.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

                X.Renderer.DrawModel(ref model,ref Camera);

                //restore render modes (shader files might have changes this!
                X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

                if (DebugMode)
                {
                    for (int i = 0; i < PhysicsObject.PhysicsSkin.NumPrimitives; i++)
                    {
                        Primitive prim = PhysicsObject.PhysicsSkin.GetPrimitiveNewWorld(i);
                        if (prim is JigLibX.Geometry.Box)
                        {
                            JigLibX.Geometry.Box box = (JigLibX.Geometry.Box)prim;
                            //BoundingVolumeRenderer.RenderBoundingBox(box.GetCentre(), box., box.SideLengths, Color.White, ref Camera.View, ref Camera.Projection);
                        }
                        if (prim is JigLibX.Geometry.Sphere)
                        {
                            JigLibX.Geometry.Sphere sph = (JigLibX.Geometry.Sphere)prim;
                            BoundingVolumeRenderer.RenderBoundingSphere(sph.Position, sph.Radius, Color.White, ref Camera.View, ref Camera.Projection);
                        }
                    }
                }
            }//end if (model != null && model.loaded)
        }

        public override void Disable()
        {
            if (PhysicsObject != null) PhysicsObject.PhysicsBody.DisableBody();
            this.model.Disable();
            base.Disable();
        }
    }
}
