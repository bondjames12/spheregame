using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XHingeJoint : XComponent, XIUpdateable
    {
        HingeJoint hinge;

        public XHingeJoint(ref XMain X, XActor actor1, XActor actor2, Vector3 HingeAxis, Vector3 Offset, float Width, float ForwardMax, float BackWardMax, float Sideways, float Damping)
            : base(ref X)
        {
            hinge = new HingeJoint();
            hinge.Initialise(actor1.PhysicsObject.PhysicsBody, actor2.PhysicsObject.PhysicsBody, HingeAxis, Offset, Width / 2, ForwardMax, BackWardMax, Sideways, Damping);
            hinge.EnableController();
            hinge.EnableHinge();
        }

        public override void Update(ref GameTime gameTime)
        {
            hinge.UpdateController((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
