using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace XEngine
{
    public class XActor : XComponent, XDrawable
    {
        public enum ActorType { Box, Capsule, Plane, Sphere, Mesh, BowlingPin }

        public int ModelNumber;

        ActorType type;
        public ActorType Type
        {
            get { return type; }
            set
            {
                type = value;
                Size = size;
            }
        }

        XModel mod;
        public XModel model
        {
            get { return mod; }
            set { mod = value; ModelNumber = mod.Number; }
        }

        internal PhysicsObject PhysicsBody;

        public Vector3 modeloffset { get; set; }

        public int PhysicsID { get { return PhysicsBody.PhysicsBody.ID; } }

        public bool ShowBoundingBox { get; set; }

        List<int> collisions;
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

        Vector3 size;
        public Vector3 Size
        {
            get { return size; }
            set
            {
                PhysicsObject obj = new PlaneObject();
                switch (Type)
                {
                    case ActorType.Box:
                        obj = new BoxObject(value, Orientation, Position); break;
                    case ActorType.Capsule:
                        obj = new CapsuleObject(value.X, value.Y, Orientation, Position); break;
                    case ActorType.Mesh:
                        obj = new TriangleMeshObject(model.Model, Orientation, Position, value); break;
                    case ActorType.Plane:
                        obj = new PlaneObject(); break;
                    case ActorType.Sphere:
                        obj = new SphereObject(value.X, Orientation, Position); break;
                    case ActorType.BowlingPin:
                        obj = new BowlingPinObject(Orientation, Position); break;
                }
                PhysicsBody = obj;
                size = value;
            }
        }

        Vector3 rotation;
        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; Orientation = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotation.Y), MathHelper.ToRadians(rotation.X), MathHelper.ToRadians(rotation.Z)); }
        }

        public XActor(XMain X, ActorType type, XModel model, Vector3 Position, Matrix Rotation, Vector3 ModelScale, Vector3 ModelOffset, Vector3 Size, Vector3 Velocity, float Mass) : base(X)
        {
            this.model = model;
            this.type = type;
            this.size = Size;
            this.mass = Mass;

            switch (type)
            {
                case ActorType.Box:
                    PhysicsBody = new BoxObject(Size, Rotation, Position); break;
                case ActorType.Capsule:
                    PhysicsBody = new CapsuleObject(Size.X, Size.Y, Rotation, Position); break;
                case ActorType.Mesh:
                    PhysicsBody = new TriangleMeshObject(model.Model, Rotation, Position, Size); break;
                case ActorType.Plane:
                    PhysicsBody = new PlaneObject(); break;
                case ActorType.Sphere:
                    PhysicsBody = new SphereObject(Size.X, Rotation, Position); break;
                case ActorType.BowlingPin:
                    PhysicsBody = new BowlingPinObject(Rotation, Position); break;
            }

            PhysicsBody.SetMass(Mass);
            PhysicsBody.scale = ModelScale;
            PhysicsBody.PhysicsBody.Velocity = Velocity;
            modeloffset = ModelOffset;
        }

        public override void Draw(GameTime gameTime, XCamera Camera)
        {
            if (model != null && model.Loaded)
                X.Renderer.DrawModel(model, Camera, PhysicsBody.GetWorldMatrix(model.Model, modeloffset));
            
            if (ShowBoundingBox)
                X.DebugDrawer.DrawCube(PhysicsBody.PhysicsSkin.WorldBoundingBox.Min, PhysicsBody.PhysicsSkin.WorldBoundingBox.Max, Color.White, Matrix.Identity, Camera);
        }

        public void Disable()
        {
            PhysicsBody.PhysicsBody.DisableBody();
        }
    }
}
