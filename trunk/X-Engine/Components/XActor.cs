using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace XEngine
{
    public class XActor : XComponent, XDrawable
    {
        public int ModelNumber;

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

        public bool ShowBoundingBox = true;

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
            this.model = model;
            this.mass = Mass;
            this.PhysicsBody = Object;

            PhysicsBody.SetMass(Mass);
            PhysicsBody.scale = ModelScale;
            PhysicsBody.PhysicsBody.Velocity = Velocity;
            modeloffset = ModelOffset;
            //this seems to assign 1 material for the whole actor?!
            //CHANGE: Modify XActor contruct to create XMaterial objects for every Texture/Effect in the model

            try
            {
                this.material = new XMaterial(X, model.Model.Meshes[0].Effects[0].Parameters["Texture"].GetValueTexture2D(), true, null, false, null, false, 10);
            }
            catch
            {
                this.material = new XMaterial(X, model.Model.Meshes[0].Effects[0].Parameters["BasicTexture"].GetValueTexture2D(), true, null, false, null, false, 10);
            }

        }

        public Matrix GetWorldMatrix()
        {
            return PhysicsBody.GetWorldMatrix(model.Model, modeloffset);
        }

        public Vector3 GetScreenCoordinates(XCamera Camera)
        {
            return X.Tools.UnprojectVector3(this.Position, Camera, GetWorldMatrix());
        }

        //CHANGE: Changed to a List<Material> to add support for multiple materials per model/actor
        XMaterial material;
        public XMaterial Material
        {
            get { return material; }
            set { material = value; }
        }


        public override void Draw(GameTime gameTime, XCamera Camera, XEnvironmentParameters environment)
        {
            if (model != null && model.Loaded)
            {
                Matrix[] mat = new Matrix[1];
                mat[0] = PhysicsBody.GetWorldMatrix(model.Model, modeloffset);

                if (material.AlphaBlendable)
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

                X.GraphicsDevice.RenderState.CullMode = material.VertexWinding;

                X.Renderer.DrawModel(model, Camera, mat, material, environment);
            }

            if (ShowBoundingBox)
                X.DebugDrawer.DrawCube(boundingBox.Min, boundingBox.Max, Color.White, Matrix.Identity, Camera);
        }

        public override void Disable()
        {
            PhysicsBody.PhysicsBody.DisableBody();
            base.Disable();
        }
    }

    /*public class XAnimatedActor : XActor, XUpdateable
    {
        AnimationPlayer animator;

        public XAnimatedActor(XMain X, PhysicsObject Object, XModel model, Vector3 ModelScale, Vector3 ModelOffset, Vector3 Velocity, float Mass) :
            base(X, Object, model, ModelScale, ModelOffset, Velocity, Mass)
        {
            animator = new AnimationPlayer((SkinningData)model.Model.Tag);
        }

        public override void Update(GameTime gameTime)
        {
            animator.Update(gameTime.ElapsedGameTime, true, PhysicsBody.GetWorldMatrix(model.Model, modeloffset));
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, XCamera Camera)
        {
            if (model != null && model.Loaded)
            {
                Matrix[] mat = new Matrix[1];
                mat[0] = PhysicsBody.GetWorldMatrix(model.Model, modeloffset);

                if (AlphaBlendAble)
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

                X.GraphicsDevice.RenderState.CullMode = VertexWinding;

                X.Renderer.DrawModel(model, Camera, mat);
            }

            if (ShowBoundingBox)
                X.DebugDrawer.DrawCube(boundingBox.Min, boundingBox.Max, Color.White, PhysicsBody.GetWorldMatrix(model.Model, Vector3.Zero), Camera);
        }
    }*/
}
