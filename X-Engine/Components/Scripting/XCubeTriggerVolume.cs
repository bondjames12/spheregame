using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XCubeTriggerVolume : XTriggerVolume
    {
        BoundingBox box;

        public XCubeTriggerVolume(XMain X, XActor Actor, bool Continuous, Vector3 Point1, Vector3 Point2) : base(X, Actor, Continuous)
        {
            box = new BoundingBox(Point1, Point2);
        }

        protected override bool CheckForIntersection()
        {
            if (box.Contains(actor.Position) == ContainmentType.Intersects || box.Contains(actor.Position) == ContainmentType.Contains)
                return true;

            return false;
        }
    }
}
