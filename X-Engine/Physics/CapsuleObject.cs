#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XEngine
{

    public class CapsuleObject : XPhysicsObject
    {

        public CapsuleObject(float radius, float length, Matrix orientation, Vector3 position)
        {
            body = new Body();
            collision = new CollisionSkin(body);
            collision.AddPrimitive(new Capsule(Vector3.Transform(new Vector3(-0.5f, 0, 0), orientation), orientation, radius, length), new MaterialProperties(0.8f, 0.7f, 0.6f));
            body.CollisionSkin = this.collision;
            Vector3 com = SetMass(10.0f);
            body.MoveTo(position + com, Matrix.Identity);

            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            body.EnableBody();
            this.scale = new Vector3(radius, radius, length / 2);
        }
    }
}