using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
//using XSIXNARuntime;
using JigLibX.Geometry;

namespace XEngine
{
    public class XProp : XComponent, XIDrawable, XIBoundedTransform
    {
        public XModel model;
        public int modelNumber; //the number to the Xmodel we are using
        
        public Vector3 position;
        public Vector3 modeloffset;
        public Matrix orientation;
        public Vector3 scale;

        #region Editor Properties


        //Some of these are still used by the engine and should be copied, renamed with _editor and the null checking taken off for speed
        public XModel model_editor
        {
            get { return model; }
            set { model = value; modelNumber = model.Number; }
        }

        public Vector3 ModelOffset_editor
        {
            get { return modeloffset;}
            set { modeloffset = value;}
        }

        public BoundingBox Bounds
        {
            get
            {
                if (model != null)
                    return model.Boundingbox;
                else
                    return new BoundingBox();
            }
        }

        public Vector3 Translation
        {
            get { return position; }
            set { position = value; }
        }

        public Quaternion Rotation
        {
            get
            {
                return Quaternion.CreateFromRotationMatrix(orientation);
            }
            set
            {
                orientation = Matrix.CreateFromQuaternion(value);
            }
        }

        public Vector3 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

 #endregion

        public XProp(ref XMain X, XModel model, Vector3 position, Vector3 modeloffset, Matrix orient, Vector3 scale)
            : base(ref X)
        {
            DrawOrder = 100;

            if (model != null)
            {
                this.model = model;
                model.Parent = this;
                modelNumber = model.Number;
                //if its not loaded try and load!
                //if (!model.loaded) model.Load(X.Content);
            }
            this.position = position;
            this.modeloffset = modeloffset;
            this.orientation = orient;
            this.scale = scale;
        }

        public Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(scale) * orientation * Matrix.CreateTranslation(position + modeloffset); //Vector3.Transform(ModelOffset, orientation));
        }

        public Vector3 GetScreenCoordinates(ref XCamera Camera)
        {
            return X.Tools.UnprojectVector3(this.position, Camera, GetWorldMatrix());
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            //null checks are for editor!
            //remove these eventually maybe using a compiler directive for an editorer version of the DLL?
            if (model != null && model.loaded)
            {
                if (AlphaBlendable)
                {
                    X.GraphicsDevice.RenderState.AlphaBlendEnable = true;
                    X.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha; // source rgb * source alpha
                    X.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One; // don't modify source alpha
                    X.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha; // dest rgb * (255 - source alpha)
                    X.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha; // dest alpha * (255 - source alpha)
                    X.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add; // add source and dest results
                }
                else
                    X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

                //Set camera params, compute matrices
                //model.SASData.Camera.NearFarClipping.X = Camera.NearPlane;
                //model.SASData.Camera.NearFarClipping.Y = Camera.FarPlane;
                //model.SASData.Camera.Position.X = Camera.Position.X;
                //model.SASData.Camera.Position.Y = Camera.Position.Y;
                //model.SASData.Camera.Position.Z = Camera.Position.Z;
                model.SASData.Projection = Camera.Projection;
                model.SASData.View = Camera.View;
                model.SASData.World = this.GetWorldMatrix();
                model.SASData.ComputeViewAndProjection();
                //model.SASData.ComputeModel();

                X.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

                X.Renderer.DrawModel(ref model,ref Camera);

                //restore render modes (shader files might have changes this!
                X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

                if (DebugMode)
                {
                    
                }
            }//end if (model != null && model.loaded)
        }

        /// <summary>
        /// Draws the model attached into the pick buffer
        /// </summary>
        public override void DrawPick(XPickBuffer pick_buf, XICamera camera)
        {
            // don't draw if we don't have a model
            //null checks are for editor!
            //remove these eventually maybe using a compiler directive for an editorer version of the DLL?
            if (model == null || !model.loaded)
                return;

            //if (!mPickEnabled)
            //    return;

            pick_buf.PushMatrix(MatrixMode.World, this.GetWorldMatrix());
            pick_buf.PushPickID(this.ComponentID);

            foreach (ModelMesh mesh in model.Model.Meshes)
            {
                pick_buf.PushVertexBuffer(mesh.VertexBuffer);
                pick_buf.PushIndexBuffer(mesh.IndexBuffer);

                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    pick_buf.PushVertexDeclaration(part.VertexDeclaration);

                    pick_buf.QueueIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, part.StreamOffset,
                        part.BaseVertex, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);

                    pick_buf.PopVertexDeclaration();
                }

                pick_buf.PopVertexBuffer();
                pick_buf.PopIndexBuffer();
            }

            pick_buf.PopPickID();
            pick_buf.PopMatrix(MatrixMode.World);
        }

        public override void Disable()
        {
            //this.model.Disable();
            base.Disable();
        }
    }
}
