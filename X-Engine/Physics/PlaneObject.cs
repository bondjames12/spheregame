using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using JigLibX.Physics;

namespace XEngine
{
    public class PlaneObject : XPhysicsObject
    {
        public PlaneObject()
        {
            body = new Body();
            collision = new CollisionSkin(null);
            collision.AddPrimitive(new JigLibX.Geometry.Plane(Vector3.Up,0.0f), (int)MaterialTable.MaterialID.UserDefined, new MaterialProperties(0.2f, 0.7f, 0.6f));
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(collision);
        }
    }
}
