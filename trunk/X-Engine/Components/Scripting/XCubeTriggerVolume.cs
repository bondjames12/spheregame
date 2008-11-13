using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public class XCubeTriggerVolume : XTriggerVolume, XIBoundedTransform
    {
        BoundingBox box;

        public XCubeTriggerVolume(ref XMain X,XPhysicsObject obj, bool Continuous, Vector3 Point1,
            Vector3 Point2, Vector3 translation, Quaternion rotation, Vector3 scale)
            : base(ref X, obj, Continuous, translation, rotation, scale)
        {
            box = new BoundingBox(Point1, Point2);
        }

        public BoundingBox Bounds
        {
            get { return box; }
            set { box = value; }
        }

        protected override bool CheckForIntersection()
        {
            if (obj == null) return false;

            //calc any translation, rotation, or scale
            Vector3.Multiply(ref box.Min, ref translation, out box.Min);
            Vector3.Multiply(ref box.Max, ref translation, out box.Max);
            Vector3.Transform(ref box.Min, ref rotation, out box.Min);
            Vector3.Transform(ref box.Max, ref rotation, out box.Max);
            Vector3.Multiply(ref box.Min, ref scale, out box.Min);
            Vector3.Multiply(ref box.Max, ref scale, out box.Max);

            if (box.Contains(obj.PhysicsBody.Position) == ContainmentType.Intersects || box.Contains(obj.PhysicsBody.Position) == ContainmentType.Contains)
                return true;

            return false;
        }

        public override void Draw(ref GameTime gameTime, ref XCamera Camera)
        {
            X.DebugDrawer.DrawBoundingBox(box, Color.Red, Matrix.Identity, Camera);
            //BoundingVolumeRenderer.RenderBoundingBox(box.GetCentre(), Matrix.Identity, box.SideLengths, Color.White, ref Camera.View, ref Camera.Projection);
            base.Draw(ref gameTime, ref Camera);
        }
        /*
        public override void DrawPick(XPickBuffer pick_buf, XICamera camera)
        {
            //don't draw if we don't have anything
            //null checks are for editor!
            //remove these eventually maybe using a compiler directive for an editorer version of the DLL?
            if (obj == null) return;

            //if (!mPickEnabled)
            //    return;

            pick_buf.PushMatrix(MatrixMode.World, Matrix.Identity);
            pick_buf.PushPickID(this.ComponentID);

           
                pick_buf.PushVertexBuffer(mesh.VertexBuffer);
                pick_buf.PushIndexBuffer(mesh.IndexBuffer);

               
                pick_buf.PushVertexDeclaration(new VertexDeclaration(X.GraphicsDevice, VertexPositionColor.VertexElements));

                pick_buf.QueuePrimitives(PrimitiveType.LineStrip,0,0,7);

                pick_buf.PopVertexDeclaration();


                pick_buf.PopVertexBuffer();
                pick_buf.PopIndexBuffer();


            pick_buf.PopPickID();
            pick_buf.PopMatrix(MatrixMode.World);
        }*/
    }
}
