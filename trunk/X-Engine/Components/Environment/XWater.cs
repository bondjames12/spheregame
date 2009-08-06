using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace XEngine
{
    public class XWater : XComponent, XIUpdateable, XIDrawable, XIBoundedTransform
    {
        public Vector2 PointOne;
        public Vector2 PointTwo;
        Vector2 Size;

        public BoundingBox boundingBox = new BoundingBox();
        public float height;

        bool reflect;
        bool refract;
        Effect effect;
        RenderTarget2D reflection;
        RenderTarget2D refraction;
        Texture2D texreflect;
        Texture2D texrefract;
        DepthStencilBuffer Depth;
        VertexPositionTexture[] vertices;
        XCamera reflectionCamera;
        XCamera camera;
        Matrix reflectionViewMatrix = Matrix.Identity;
        float waveLength = .25f;
        float waveHeight = 1.15f;
        float windForce = 5.0f;
        float windDirection = 90.0f;

        #region Editor Properties
        public BoundingBox Bounds
        {
            get
            {
                if (boundingBox != null)
                    return boundingBox;
                else
                    return new BoundingBox();
            }
        }

        public Vector3 Translation
        {
            get { return new Vector3(PointOne.X, height, PointOne.Y); }
            set 
            { 
                PointOne = new Vector2(value.X, value.Z);
                PointTwo = new Vector2(value.X + Size.X, value.Z + Size.Y);
                height = value.Y;
                CreateWaterPlane();
            }
        }

        public Quaternion Rotation
        {
            get
            {
                //return Quaternion.CreateFromRotationMatrix(orientation);
                return Quaternion.Identity;
            }
            set
            {
                //orientation = Matrix.CreateFromQuaternion(value);
            }
        }

        public Vector3 Scale
        {
            get
            {
                return new Vector3(Size.X, 0, Size.Y);
            }
            set
            {
                Size = new Vector2(value.X,value.Z);
                CreateWaterPlane();
            }
        }
        #endregion

        public float Height
        {
            get { return height; }
            set { height = value; CreateWaterPlane();}
        }

        public bool DoesReflect
        {
            get { return reflect; }
            set { reflect = value; effect.Parameters["reflect"].SetValue(value); }
        }
        public bool DoesRefract
        {
            get { return refract; }
            set { refract = value; effect.Parameters["refract"].SetValue(value); }
        }

        public XCamera ReflectionCamera
        {
            get { return reflectionCamera; }
            set { reflectionCamera = value; }
        }

        public float WaveLength
        {
            get { return waveLength; }
            set { waveLength = value; effect.Parameters["xWaveLength"].SetValue(value); }
        }

        public float WaveHeight
        {
            get { return waveHeight; }
            set { waveHeight = value; effect.Parameters["xWaveHeight"].SetValue(value); }
        }

        public float WindForce
        {
            get { return windForce; }
            set { windForce = value; effect.Parameters["xWindForce"].SetValue(value); }
        }

        public float WindDirection
        {
            get { return windDirection; }
            set { windDirection = value; effect.Parameters["xWindDirection"].SetValue(Matrix.CreateRotationZ(MathHelper.ToRadians(value))); }
        }

        public XWater(ref XMain X)
            : base(ref X)
        {
            DrawOrder = 22;
            height = 4;
            Size = new Vector2(128*2,128*2);
            PointOne = new Vector2(-128,-128);
            PointTwo = Vector2.Add(PointOne, Size);

            reflectionCamera = new XCamera(ref X,1,100);
        }

        public XWater(ref XMain X, Vector2 pointOne, Vector2 size, float height) : base(ref X)
        {
            DrawOrder = 22;
            PointOne = pointOne;
            PointTwo = Vector2.Add(pointOne, size);
            this.Size = size;

            this.height = height;

            reflectionCamera = new XCamera(ref X,1,100);
        }

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            effect = Content.Load<Effect>(@"Content\XEngine\Effects\Water");

            effect.Parameters["xWaterBumpMap"].SetValue(Content.Load<Texture2D>(@"Content\XEngine\Textures\WaterBumpMap"));
            effect.Parameters["xWaveLength"].SetValue(waveLength);
            effect.Parameters["xWaveHeight"].SetValue(waveHeight);
            effect.Parameters["xWindForce"].SetValue(windForce);
            effect.Parameters["xWindDirection"].SetValue(Matrix.CreateRotationZ(MathHelper.ToRadians(windDirection)));

            reflection = new RenderTarget2D(X.GraphicsDevice, X.GraphicsDevice.Viewport.Width, X.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
            refraction = new RenderTarget2D(X.GraphicsDevice, X.GraphicsDevice.Viewport.Width, X.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
            Depth = new DepthStencilBuffer(X.GraphicsDevice, X.GraphicsDevice.Viewport.Width, X.GraphicsDevice.Viewport.Height, X.GraphicsDevice.DepthStencilBuffer.Format);

            CreateWaterPlane();

            DoesReflect = true;
            DoesRefract = true;

            base.Load(Content);
        }

        public void CreateWaterPlane()
        {
            //Recacl PointTwo
            PointTwo = Vector2.Add(PointOne, Size);

            //requires valid PointOne, height, PointTwo
            vertices = new VertexPositionTexture[6];

            // Create the water plane
            vertices[0] = new VertexPositionTexture(new Vector3(PointOne.X, height, PointOne.Y), new Vector2(0, 1));
            vertices[1] = new VertexPositionTexture(new Vector3(Size.X + PointOne.X, height, Size.Y + PointOne.Y), new Vector2(1, 0));
            vertices[2] = new VertexPositionTexture(new Vector3(PointOne.X, height, Size.Y + PointOne.Y), new Vector2(0, 0));
            vertices[3] = new VertexPositionTexture(new Vector3(PointOne.X, height, PointOne.Y), new Vector2(0, 1));
            vertices[4] = new VertexPositionTexture(new Vector3(Size.X + PointOne.X, height, PointOne.Y), new Vector2(1, 1));
            vertices[5] = new VertexPositionTexture(new Vector3(Size.X + PointOne.X, height, Size.Y + PointOne.Y), new Vector2(1, 0));

            boundingBox = new BoundingBox(new Vector3(PointOne.X, height, PointOne.Y), new Vector3(PointTwo.X, height, PointTwo.Y));
        }

        public override void Update(ref GameTime gameTime)
        {
            if (X.GraphicsDevice.Viewport.Width != reflection.Width || X.GraphicsDevice.Viewport.Height != reflection.Height)
            {
                reflection = new RenderTarget2D(X.GraphicsDevice, X.GraphicsDevice.Viewport.Width, X.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
                refraction = new RenderTarget2D(X.GraphicsDevice, X.GraphicsDevice.Viewport.Width, X.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
                Depth = new DepthStencilBuffer(X.GraphicsDevice, X.GraphicsDevice.Viewport.Width, X.GraphicsDevice.Viewport.Height, X.GraphicsDevice.DepthStencilBuffer.Format);
            }

            if (camera != null)
            {
                float reflectionCamYCoord = -camera.Position.Y + 2 * height;
                Vector3 reflectionCamPosition = new Vector3(camera.Position.X, reflectionCamYCoord, camera.Position.Z);

                float reflectionTargetYCoord = -camera.Target.Y + 2 * height;
                Vector3 reflectionCamTarget = new Vector3(camera.Target.X, reflectionTargetYCoord, camera.Target.Z);

                Vector3 forwardVector = camera.Target - camera.Position;
                Vector3 sideVector = Vector3.Transform(new Vector3(1, 0, 0), camera.RotationMatrix);
                Vector3 reflectionCamUp = Vector3.Cross(sideVector, forwardVector);

                reflectionViewMatrix = Matrix.CreateLookAt(reflectionCamPosition, reflectionCamTarget, reflectionCamUp);

                reflectionCamera.View = reflectionViewMatrix;
                reflectionCamera.Projection = camera.Projection;
                reflectionCamera.Position = reflectionCamPosition;
                reflectionCamera.Up = reflectionCamUp;
                reflectionCamera.Target = reflectionCamTarget;

                camera.Update(ref gameTime);
                reflectionCamera.Update(ref gameTime);

                Refract(gameTime, camera);
                Reflect(gameTime, reflectionCamera);
            }
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            CullMode Cullprevious = X.GraphicsDevice.RenderState.CullMode;
            X.GraphicsDevice.RenderState.CullMode = CullMode.None;

            this.camera = Camera;

            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(Camera.View);
            effect.Parameters["xProjection"].SetValue(Camera.Projection);

            if (reflectionViewMatrix != Matrix.Identity)
            {
                effect.Parameters["xReflectionView"].SetValue(reflectionViewMatrix);
                effect.Parameters["xReflectionMap"].SetValue(texreflect);//reflection.GetTexture());
                effect.Parameters["xRefractionMap"].SetValue(texrefract);//refraction.GetTexture());
            }

            effect.Parameters["xCamPos"].SetValue(Camera.Position);

            effect.Parameters["xTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds / 100);
            
            //if rendering a depthmap
            if (Camera.RenderType == RenderTypes.Depth)
            {
                //override any techniques with DepthMap technique shader
                if (effect.Techniques["DepthMapStatic"] != null)
                    effect.CurrentTechnique = effect.Techniques["DepthMapStatic"];
                //continue;
            }
            else
            {
                if (effect.Techniques["Water"] != null)
                    effect.CurrentTechnique = effect.Techniques["Water"];
            }

            // Draw the water
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                X.GraphicsDevice.VertexDeclaration = new VertexDeclaration(X.GraphicsDevice, VertexPositionTexture.VertexElements);
                X.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, 2);
                pass.End();
            }
            effect.End();
            X.GraphicsDevice.RenderState.CullMode = Cullprevious;
        }

#if XBOX == FALSE
        /// <summary>
        /// Draws the model attached into the pick buffer
        /// </summary>
        public override void DrawPick(XPickBuffer pick_buf, XICamera camera)
        {
            if (!this.loaded)
                return;

            //if (!mPickEnabled)
            //    return;

            pick_buf.PushMatrix(MatrixMode.World, Matrix.Identity);
            pick_buf.PushPickID(this.ComponentID);
    
            pick_buf.PushVertexDeclaration(new VertexDeclaration(X.GraphicsDevice, VertexPositionTexture.VertexElements));

            pick_buf.QueueUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);

            pick_buf.PopVertexDeclaration();

            pick_buf.PopPickID();
            pick_buf.PopMatrix(MatrixMode.World);
        }
#endif

        void Refract(GameTime gameTime, XCamera Camera)
        {
            // Create the clip plane
            Vector3 planeNormalDirection = new Vector3(0, -1, 0);
            planeNormalDirection.Normalize();
            Vector4 planeCoefficients = new Vector4(planeNormalDirection, height + .05f);

            // Create a view matrix
            Matrix camMatrix = Camera.View * Camera.Projection;
            Matrix invCamMatrix = Matrix.Invert(camMatrix);
            invCamMatrix = Matrix.Transpose(invCamMatrix);

            // Setup the clip plane
            planeCoefficients = Vector4.Transform(planeCoefficients, invCamMatrix);
            Plane refractionClipPlane = new Plane(planeCoefficients);

            // Enable the clip plane
            X.GraphicsDevice.ClipPlanes[0].Plane = refractionClipPlane;
            X.GraphicsDevice.ClipPlanes[0].IsEnabled = true;

            DepthStencilBuffer prev = X.GraphicsDevice.DepthStencilBuffer;
            X.GraphicsDevice.DepthStencilBuffer = Depth;

            // Set the rener target to the refraction target
            X.GraphicsDevice.SetRenderTarget(0, refraction);
            X.GraphicsDevice.Clear(Color.Black);

            X.GraphicsDevice.RenderState.DepthBufferEnable = true;
            X.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            DrawRefractedScene(gameTime, Camera);

            X.GraphicsDevice.SetRenderTarget(0, null);

            X.GraphicsDevice.DepthStencilBuffer = prev;

            texrefract = refraction.GetTexture();

            X.GraphicsDevice.ClipPlanes[0].IsEnabled = false;
        }

        void Reflect(GameTime gameTime, XCamera Camera)
        {
            Vector3 planeNormalDirection = new Vector3(0, 1, 0);
            planeNormalDirection.Normalize();
            Vector4 planeCoefficients = new Vector4(planeNormalDirection, -height + .05f);

            Matrix camMatrix = Camera.View * Camera.Projection;
            Matrix invCamMatrix = Matrix.Invert(camMatrix);
            invCamMatrix = Matrix.Transpose(invCamMatrix);

            planeCoefficients = Vector4.Transform(planeCoefficients, invCamMatrix);
            Plane reflectionClipPlane = new Plane(planeCoefficients);

            X.GraphicsDevice.ClipPlanes[0].Plane = reflectionClipPlane;
            X.GraphicsDevice.ClipPlanes[0].IsEnabled = true;

            DepthStencilBuffer prev = X.GraphicsDevice.DepthStencilBuffer;
            X.GraphicsDevice.DepthStencilBuffer = Depth;

            X.GraphicsDevice.SetRenderTarget(0, reflection);
            X.GraphicsDevice.Clear(Color.Black);

            X.GraphicsDevice.RenderState.DepthBufferEnable = true;
            X.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            DrawReflectedScene(gameTime, Camera);

            X.GraphicsDevice.SetRenderTarget(0, null);

            X.GraphicsDevice.DepthStencilBuffer = prev;

            texreflect = reflection.GetTexture();

            X.GraphicsDevice.ClipPlanes[0].IsEnabled = false;
        }

        public virtual void DrawRefractedScene(GameTime gameTime, XCamera Camera)
        {
            List<XComponent> NoDraw = new List<XComponent>();
            NoDraw.Add(this);
            NoDraw.Add(X.Debug);
            NoDraw.Add(X.Console);
            NoDraw.Add(X.DebugDrawer);

            X.Renderer.DrawScene(ref gameTime,ref  Camera, NoDraw, null);
        }

        public virtual void DrawReflectedScene(GameTime gameTime, XCamera Camera)
        {
            List<XComponent> NoDraw = new List<XComponent>();
            NoDraw.Add(this);
            NoDraw.Add(X.Debug);
            NoDraw.Add(X.Console);
            NoDraw.Add(X.DebugDrawer);

            X.Renderer.DrawScene(ref gameTime,ref Camera, NoDraw, null);
        }
    }
}
