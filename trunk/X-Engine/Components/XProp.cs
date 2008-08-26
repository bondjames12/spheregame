﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using XSIXNARuntime;
using JigLibX.Geometry;

namespace XEngine
{
    public class XProp : XComponent, XDrawable
    {
        public XModel model;
        public int modelNumber; //the number to the Xmodel we are using
        
        public Vector3 position;
        public Vector3 modeloffset;
        public Matrix orientation;
        public Vector3 scale;
        public XGifTexture gif;

        #region Editor Properties


        //Some of these are still used by the engine and should be copied, renamed with _editor and the null checking taken off for speed
        public XModel model_editor
        {
            get { return model; }
            set { model = value; modelNumber = model.Number; }
        }

        public Vector3 Position_editor
        {
            get { return position;}
            set { position = value;}
        }

        public Vector3 ModelOffset_editor
        {
            get { return modeloffset;}
            set { modeloffset = value;}
        }

        public Matrix Orientation_editor
        {
            get { return orientation;}
            set { orientation = value;}
        }

        public Vector3 Scale_editor
        {
            get { return scale;}
            set { scale = value;}
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
                this.modelNumber = model.Number;
                //if its not loaded try and load!
                if (!model.loaded) model.Load(X.Content);
            }
            this.position = position;
            this.modeloffset = modeloffset;
            this.orientation = orient;
            this.scale = scale;
            gif = new XGifTexture(ref X, @"Content\Textures\Simple");
            gif.Load(X.Content);
            gif.Start();
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
                model.SASData.Model = this.GetWorldMatrix();
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

        public override void Disable()
        {
            this.model.Disable();
            base.Disable();
        }
    }
}