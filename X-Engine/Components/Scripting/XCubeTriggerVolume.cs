using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XCubeTriggerVolume : XTriggerVolume
    {
        BoundingBox box;

        public XCubeTriggerVolume(ref XMain X, XActor Actor, bool Continuous, Vector3 Point1, Vector3 Point2) : base(ref X, Actor, Continuous)
        {
            box = new BoundingBox(Point1, Point2);
        }

        protected override bool CheckForIntersection()
        {
            if (box.Contains(actor.PhysicsObject.PhysicsBody.Position) == ContainmentType.Intersects || box.Contains(actor.PhysicsObject.PhysicsBody.Position) == ContainmentType.Contains)
                return true;

            return false;
        }
    }
}
