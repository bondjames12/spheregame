using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace XEngine
{
    public class XWater : XComponent, XUpdateable, XDrawable
    {
        public Vector2 PointOne;
        public Vector2 PointTwo;

        public Vector3 v3PointOne { get { return new Vector3(PointOne.X, Height, PointOne.Y); } }
        public Vector3 v3PointTwo { get { return new Vector3(PointTwo.X, Height, PointTwo.Y); } }

        bool reflect;
        bool refract;
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

        Vector2 Size { get { return PointTwo - PointOne; } }

        public float Height;
        Vector2 Position { get { return PointOne; } }

        Effect effect;
        RenderTarget2D reflection;
        RenderTarget2D refraction;
        DepthStencilBuffer Depth;

        VertexPositionTexture[] vertices;

        XCamera reflectionCamera;

        public BoundingBox boundingBox = new BoundingBox();

        public XWater(ref XMain X)
            : base(ref X)
        {
            DrawOrder = 22;
            PointOne = new Vector2(-128);
            PointTwo = new Vector2(128);
            Height = 4;
            reflectionCamera = new XCamera(ref X,1,100);
        }

        public XWater(ref XMain X, Vector2 pointOne, Vector2 pointTwo, float Height) : base(ref X)
        {
            PointOne = pointOne;
            PointTwo = pointTwo;

            this.Height = Height;

            reflectionCamera = new XCamera(ref X,1,100);
        }

        float waveLength = .25f;
        float waveHeight = 1.15f;
        float windForce = 5.0f;
        float windDirection = 90.0f;

        public float WaveLenth
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

            vertices = new VertexPositionTexture[6];

            // Create the water plane
            vertices[0] = new VertexPositionTexture(new Vector3(Position.X, Height, Position.Y), new Vector2(0, 1));
            vertices[1] = new VertexPositionTexture(new Vector3(Size.X + Position.X, Height, Size.Y + Position.Y), new Vector2(1, 0));
            vertices[2] = new VertexPositionTexture(new Vector3(Position.X, Height, Size.Y + Position.Y), new Vector2(0, 0));
            vertices[3] = new VertexPositionTexture(new Vector3(Position.X, Height, Position.Y), new Vector2(0, 1));
            vertices[4] = new VertexPositionTexture(new Vector3(Size.X + Position.X, Height, Position.Y), new Vector2(1, 1));
            vertices[5] = new VertexPositionTexture(new Vector3(Size.X + Position.X, Height, Size.Y + Position.Y), new Vector2(1, 0));

            boundingBox = new BoundingBox(v3PointOne, v3PointTwo);

            DoesReflect = true;
            DoesRefract = true;

            base.Load(Content);
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
                float reflectionCamYCoord = -camera.Position.Y + 2 * Height;
                Vector3 reflectionCamPosition = new Vector3(camera.Position.X, reflectionCamYCoord, camera.Position.Z);

                float reflectionTargetYCoord = -camera.Target.Y + 2 * Height;
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

        XCamera camera;
        Matrix reflectionViewMatrix = Matrix.Identity;

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
                effect.Parameters["xReflectionMap"].SetValue(reflection.GetTexture());
                effect.Parameters["xRefractionMap"].SetValue(refraction.GetTexture());
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

        void Refract(GameTime gameTime, XCamera Camera)
        {
            // Create the clip plane
            Vector3 planeNormalDirection = new Vector3(0, -1, 0);
            planeNormalDirection.Normalize();
            Vector4 planeCoefficients = new Vector4(planeNormalDirection, Height + .05f);

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

            X.GraphicsDevice.ClipPlanes[0].IsEnabled = false;
        }

        void Reflect(GameTime gameTime, XCamera Camera)
        {
            Vector3 planeNormalDirection = new Vector3(0, 1, 0);
            planeNormalDirection.Normalize();
            Vector4 planeCoefficients = new Vector4(planeNormalDirection, -Height + .05f);

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
