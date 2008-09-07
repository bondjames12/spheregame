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

        public XTree(ref XMain X, XPhysicsObject Object, XModel model,Vector3 ModelScale,
            Vector3 ModelOffset, Vector3 Velocity, float Mass)
            : base(ref X, Object, model, ModelScale, ModelOffset, Velocity, Mass)
        {
           
        }

        /// <summary>
        /// Render all trees in the camera frustrum (trees that you can see)
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="Camera"></param>
        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            if (Camera.Frustrum.Contains(tree.boundingBox) != ContainmentType.Disjoint || NoCull)
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

                X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

                if (DebugMode)
                {
                    //model.Model.Meshes[0].BoundingSphere.
                    //Draw Frustum (Yellow) and Physics Bounding (White), In XActor these should be the same but draw then both anyway just in case
                    X.DebugDrawer.DrawCube(tree.boundingBox.Min, tree.boundingBox.Max, Color.Yellow, Matrix.Identity, Camera);
                    X.DebugDrawer.DrawCube(PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox.Min, PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox.Max, Color.White, Matrix.Identity, Camera);
                    //X.DebugDrawer.DrawCube(PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox.Min, PhysicsObject.PhysicsBody.CollisionSkin.WorldBoundingBox.Max, Color.White, Matrix.Identity, Camera);
                }
            }
        }
    }
}
