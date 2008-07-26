using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XHingeJoint : XComponent, XUpdateable
    {
        HingeJoint hinge;

        public XHingeJoint(XMain X, XActor actor1, XActor actor2, Vector3 HingeAxis, Vector3 Offset, float Width, float ForwardMax, float BackWardMax, float Sideways, float Damping) : base(X)
        {
            hinge = new HingeJoint();
            hinge.Initialise(actor1.PhysicsBody.PhysicsBody, actor2.PhysicsBody.PhysicsBody, HingeAxis, Offset, Width / 2, ForwardMax, BackWardMax, Sideways, Damping);
            hinge.EnableController();
            hinge.EnableHinge();
        }

        public override void Update(GameTime gameTime)
        {
            hinge.UpdateController((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
