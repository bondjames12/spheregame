using System;
using System.Collections.Generic;
using System.Text;
using Feldthaus.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public class XTree : XActor
    {
        public TreeFile File;
        public TreeGenerator Generator;
        public TreeModel tree;
        public int Seed;
        public int RadialSegments;
        public int CutOffLevel;
        public bool RenderLeaves;
        public Matrix World;

        public XTree(XMain X, ActorType Type, XModel model,Vector3 Position, Matrix Rotation, Vector3 ModelScale,
            Vector3 ModelOffset, Vector3 Size, Vector3 Velocity, float Mass)
            : base(X,  Type,  model, Position,Rotation, ModelScale, ModelOffset, Size, Velocity,Mass)
        {

            
        }

        /// <summary>
        /// Render all trees in the camera frustrum (trees that you can see)
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="Camera"></param>
        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            if (Camera.RenderType == RenderTypes.Depth)
            {
                tree.Trunk.Effect.CurrentTechnique = tree.Trunk.Effect.Techniques["DepthMapStatic"];
                tree.Leaves.DepthMapRendering = true;
            }
            else
            {
                tree.Trunk.Effect.CurrentTechnique = tree.Trunk.Effect.Techniques["Static"];
                tree.Leaves.DepthMapRendering = false;
            }

            // Draw the tree's trunk
            tree.Trunk.Draw(World, Camera.View);

            // Draw the tree's leaves (this has its own effect)
            tree.Leaves.Draw(World, Camera.View, Camera.Position);

            if (DebugMode)
            {

                //Draw Frustum (Yellow) and Physics Bounding (White), In XActor these should be the same but draw then both anyway just in case
                X.DebugDrawer.DrawCube(boundingBox.Min, boundingBox.Max, Color.Yellow, Matrix.Identity, Camera);
                //X.DebugDrawer.DrawCube(PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox.Min, PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox.Max, Color.White, Matrix.Identity, Camera);
            }
        }
    }
}
