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
    public class SphereObject : XPhysicsObject
    {
        public SphereObject(float radius, Matrix orientation, Vector3 position)
        {
            body = new Body();
            collision = new CollisionSkin(body);
            collision.AddPrimitive(new JigLibX.Geometry.Sphere(Vector3.Zero * 5.0f, radius), (int)MaterialTable.MaterialID.UserDefined, new MaterialProperties(0.5f, 0.7f, 0.6f));
            body.CollisionSkin = this.collision;
            Vector3 com = SetMass(10.0f);
            body.MoveTo(position + com, orientation);
            // collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            body.EnableBody();
            this.scale = Vector3.One * radius;
        }
    }
}
