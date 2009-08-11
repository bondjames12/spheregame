using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using XEngine;

namespace XEngine
{
    /// <summary>
    /// Helps to combine the physics with the graphics.
    /// </summary>
    public abstract class XPhysicsObject
    {
        protected Body body;     
        protected CollisionSkin collision;


        public Vector3 scale = Vector3.One;
        public Vector3 centerOfMass = Vector3.Zero;

        public Body PhysicsBody { get { return body; } }
        /// <summary>
        /// The Main Collision Skin (primitives collection) that we check collisions against for all the X-Engine Objects
        /// </summary>
        public CollisionSkin PhysicsSkin { get { return collision; } }

        public XPhysicsObject()
        {
        }
#if DEBUG
        public VertexPositionColor[] GetCollisionWireframe()
        {
            VertexPositionColor[] wf = collision.GetLocalSkinWireframe();

            // if the collision skin was also added to the body
            // we have to transform the skin wireframe to the body space
            if (body.CollisionSkin != null)        
                body.TransformWireframe(wf);

            return wf;
        }
#endif
        public Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties =
                new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid, PrimitiveProperties.MassTypeEnum.Density, mass);
            
            float junk; Vector3 com; Matrix it, itCoM;

            collision.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);
            body.BodyInertia = itCoM;
            body.Mass = junk;
            centerOfMass = com;

            return com;
        }

        public float GetMass()
        {
            return body.Mass;
        }



        //Matrix[] boneTransforms;
        //int boneCount = 0;

        public virtual Matrix GetWorldMatrix(Model model, Vector3 ModelOffset)
        {
            //THIS DOES NOTHING COMMENTED OUT!!!
            /*boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            if (boneTransforms == null || boneCount != model.Bones.Count)
            {
                boneTransforms = new Matrix[model.Bones.Count];
                boneCount = model.Bones.Count;
            }

            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            */
            Matrix World;
            // the body has an orientation but also the primitives in the collision skin
            // owned by the body can be rotated!
            if (body.CollisionSkin != null)
                World = Matrix.CreateScale(scale) * body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation * body.Orientation * Matrix.CreateTranslation(body.Position + Vector3.Transform(ModelOffset, PhysicsBody.Orientation));
            else
                World = Matrix.CreateScale(scale) * body.Orientation * Matrix.CreateTranslation(body.Position + Vector3.Transform(ModelOffset, PhysicsBody.Orientation));

            return World;
        }
    }
}
