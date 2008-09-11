using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using XSIXNARuntime;
using JigLibX.Geometry;

namespace XEngine
{
    public class XActor : XComponent, XIDrawable, XIBoundedTransform
    {
        public XModel model;
        public int modelNumber; //the number to the Xmodel we are using
        public XPhysicsObject PhysicsObject;

        //not used yet
        //public Vector3 modeloffset;
        
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

        public BoundingBox Bounds
        { 
            get 
            {
                if (PhysicsObject != null)
                {
                    if (PhysicsObject.PhysicsBody.CollisionSkin != null)
                        return PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox;
                    else
                        return new BoundingBox(PhysicsObject.PhysicsBody.Position, PhysicsObject.PhysicsBody.Position);
                }
                else
                    return new BoundingBox();
            }
            /*set
            {
                PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox = value;
            }*/
        }

        public Vector3 Translation
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

        public Quaternion Rotation
        {
            get { if (PhysicsObject != null) return Quaternion.CreateFromRotationMatrix(PhysicsObject.PhysicsBody.Orientation); else return Quaternion.Identity; }
            set { if (PhysicsObject != null) PhysicsObject.PhysicsBody.Orientation = Matrix.CreateFromQuaternion(value); else ; }
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

        /*public Vector3 Position
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
        }*/

        public bool Immovable
        {
            get { if (PhysicsObject != null) return PhysicsObject.PhysicsBody.Immovable; else return true; }
            set { if (PhysicsObject != null) PhysicsObject.PhysicsBody.Immovable = value; else ; }
        }

        /*public Matrix Orientation 
        {
            get { if (PhysicsObject != null) return PhysicsObject.PhysicsBody.Orientation; else return Matrix.Identity; }
            set { if (PhysicsObject != null) PhysicsObject.PhysicsBody.Orientation = value; else ; }
        }*/

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

 #endregion

        public XActor(ref XMain X, XPhysicsObject Object, XModel model, Vector3 ModelScale, Vector3 Velocity, float Mass)
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
            this.PhysicsObject.SetMass(Mass);
            this.PhysicsObject.scale = ModelScale;
            this.PhysicsObject.PhysicsBody.Velocity = Velocity;
            this.PhysicsObject.PhysicsBody.SetDeactivationTime(0.1f);
            this.PhysicsObject.PhysicsBody.SetActivityThreshold(5f, 5f);
        }

        public XActor(ref XMain X, XModel model, Vector3 Position, Vector3 Velocity, float Mass)
            : base(ref X)
        {
            DrawOrder = 100;
            this.Scale = Vector3.One;

            if (model != null)
            {
                this.model = model;
                model.Parent = this;
                this.modelNumber = model.Number;
                if (model.Model != null)
                {
                    RebuildCollisionSkin(Position);
                    this.PhysicsObject.SetMass(Mass);
                    this.PhysicsObject.PhysicsBody.Velocity = Velocity;
                }
            }
        }

        public void RebuildCollisionSkin(Vector3 position)
        {
            this.PhysicsObject = new XSIBoneMapObject(Vector3.Zero, ref model);
            this.PhysicsObject.PhysicsBody.SetDeactivationTime(0.1f);
            this.PhysicsObject.PhysicsBody.SetActivityThreshold(5f, 5f);
        }

        public Matrix GetWorldMatrix()
        {
            return PhysicsObject.GetWorldMatrix(model.Model, Vector3.Zero);//modeloffset);
        }

        public Vector3 GetScreenCoordinates(XCamera Camera)
        {
            return X.Tools.UnprojectVector3(this.PhysicsObject.PhysicsBody.Position, Camera, GetWorldMatrix());
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

        /// <summary>
        /// Draws the model attached into the pick buffer
        /// </summary>
        public override void DrawPick(XPickBuffer pick_buf, XICamera camera)
        {
            // don't draw if we don't have a model
            //null checks are for editor!
            //remove these eventually maybe using a compiler directive for an editorer version of the DLL?
            if (model == null || !model.loaded)
                return;

            //if (!mPickEnabled)
            //    return;

            pick_buf.PushMatrix(MatrixMode.World, this.PhysicsObject.GetWorldMatrix(model.Model, Vector3.Zero));
            pick_buf.PushPickID(this.ComponentID);

            foreach (ModelMesh mesh in model.Model.Meshes)
            {
                pick_buf.PushVertexBuffer(mesh.VertexBuffer);
                pick_buf.PushIndexBuffer(mesh.IndexBuffer);

                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    pick_buf.PushVertexDeclaration(part.VertexDeclaration);

                    pick_buf.QueueIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, part.StreamOffset,
                        part.BaseVertex, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);

                    pick_buf.PopVertexDeclaration();
                }

                pick_buf.PopVertexBuffer();
                pick_buf.PopIndexBuffer();
            }

            pick_buf.PopPickID();
            pick_buf.PopMatrix(MatrixMode.World);
        }

        public override void Disable()
        {
            if (PhysicsObject != null) PhysicsObject.PhysicsBody.DisableBody();
            this.model.Disable();
            base.Disable();
        }
    }
}
