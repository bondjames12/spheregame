using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public class XTree : XActor
    {
        public XTreeModel tree;
        public bool renderLeaves;

        #region Editor Properties
        public new XTreeModel model_editor
        {
            get { return tree; }
            set { tree = value; }
        }

        public bool RenderLeaves
        {
            get { return renderLeaves; }
            set { renderLeaves = value; }
        }

        public new Vector3  Translation
        {
            get
            {
                if (PhysicsObject != null)
                    return PhysicsObject.PhysicsBody.Position;
                else
                    return Vector3.Zero;
            }
            set
            {
                translation = value;
                if (PhysicsObject != null)
                {
                    PhysicsObject.PhysicsBody.MoveTo(value, Matrix.Identity);
                    GenerateFrustumBB();
                }
                else ;
            }
        }

        public Vector3 Scale
        {
            get { if (PhysicsObject != null) return PhysicsObject.scale; else return Vector3.One; }
            set 
            { 
                scale = value;
                if (PhysicsObject != null)
                {
                    PhysicsObject.scale = value;
                    GenerateFrustumBB();
                }
                else ;
            }
        }

        #endregion

        public XTree(ref XMain X, XTreeModel model, Vector3 translation, Vector3 ModelScale)
            : base(ref X, new BoxObject(new Vector3(ModelScale.X, 10, ModelScale.Z), Matrix.Identity, translation), null, ModelScale, Vector3.Zero, 10000)
        {
            this.tree = model;
            this.Immovable = true;
            this.renderLeaves = true;
            // Enable/disable leaf sorting
            if(tree != null)
                if(tree.Leaves != null)
                    tree.Leaves.SortingEnabled = false;
        }

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            GenerateFrustumBB();
            base.Load(Content);
        }

        public void GenerateFrustumBB()
        {
            if (tree == null || this.PhysicsObject == null) return;

            Matrix world = this.PhysicsObject.GetWorldMatrix(null, Vector3.Zero);
            //this is only used for frustrum culling 
            //we do however need to translate the frustrum box to the position of the tree
            Vector3[] bbcorners = tree.boundingBox.GetCorners();
            for (int i = 0; i < bbcorners.Length; i++)
            {
                bbcorners[i] = Vector3.Transform(bbcorners[i], world);
            }
            tree.boundingBox = BoundingBox.CreateFromPoints(bbcorners);
        }

        /// <summary>
        /// Render this tree in the camera frustrum
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="Camera"></param>
        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            if (!loaded || tree == null) return;
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

                Matrix world = PhysicsObject.GetWorldMatrix(null, Vector3.Zero);

                // Draw the tree's trunk
                //winding is backwards for the trees
                X.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
                tree.Trunk.Draw(world, Camera.View);
                X.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;

                // Draw the tree's leaves (this has its own effect)
                if(renderLeaves) tree.Leaves.Draw(world, Camera.View, Camera.Position);

#if DEBUG
                    //model.Model.Meshes[0].BoundingSphere.
                    //Draw Frustum (Yellow) and Physics Bounding (White), In XActor these should be the same but draw then both anyway just in case
                    X.DebugDrawer.DrawCube(tree.boundingBox.Min, tree.boundingBox.Max, Color.Yellow, Matrix.Identity, Camera);
                    //Draw physics collisionskin
                    X.DebugDrawer.DrawShape(PhysicsObject.GetCollisionWireframe(),Color.White);
#endif
            }
        }//end draw
#if XBOX == FALSE
        /// <summary>
        /// Draws the model attached into the pick buffer
        /// </summary>
        public override void DrawPick(XPickBuffer pick_buf, XICamera camera)
        {
            // don't draw if we don't have a model
            //null checks are for editor!
            //remove these eventually maybe using a compiler directive for an editorer version of the DLL?
            if (!tree.loaded)
                return;

            //if (!mPickEnabled)
            //    return;

            pick_buf.PushMatrix(MatrixMode.World, this.PhysicsObject.GetWorldMatrix(null, Vector3.Zero));
            pick_buf.PushPickID(this.ComponentID);

            //trunk
            pick_buf.PushVertexBuffer(tree.Trunk.vertexBuffer);
            pick_buf.PushIndexBuffer(tree.Trunk.indexBuffer);

            pick_buf.PushVertexDeclaration(tree.Trunk.vertexDeclaration);

            pick_buf.QueueIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0,
                0, 0, tree.Trunk.vertices.Length, 0, tree.Trunk.indices.Length / 3);

            pick_buf.PopVertexDeclaration();
            pick_buf.PopVertexBuffer();
            pick_buf.PopIndexBuffer();



            pick_buf.PopPickID();
            pick_buf.PopMatrix(MatrixMode.World);
        }
#endif
    }
}
