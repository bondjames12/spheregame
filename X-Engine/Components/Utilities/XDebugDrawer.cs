﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SMath = System.Math;

namespace XEngine
{
    #region Line
    public struct Line
    {
        //Positions
        public Vector3 startPoint, endPoint;
        //Colors
        public Color startColor, endColor;

        public Line(Vector3 setStartPoint, Color setStartColor,
            Vector3 setEndPoint, Color setEndColor)
        {
            startPoint = setStartPoint;
            startColor = setStartColor;
            endPoint = setEndPoint;
            endColor = setEndColor;
        }

        public static bool operator ==(Line a, Line b)
        {
            return
                a.startPoint == b.startPoint &&
                a.endPoint == b.endPoint &&
                a.startColor == b.startColor &&
                a.endColor == b.endColor;
        }

        public static bool operator !=(Line a, Line b)
        {
            return
                a.startPoint != b.startPoint ||
                a.endPoint != b.endPoint ||
                a.startColor != b.startColor ||
                a.endColor != b.endColor;
        }

        public override bool Equals(object a)
        {
            if (a.GetType() == typeof(Line))
                return (Line)a == this;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
    #endregion

    public class XDebugDrawer : XComponent, XIDrawable
    {
        #region Variables
        private Effect lineRender;
        private int numOfLines = 0;
        private List<Line> lines = new List<Line>();

        private bool buildVertexBuffer = false;
        static VertexDeclaration decl = null;

        static VertexPositionColor[] lineVertices;
        private int numOfPrimitives = 0;
        private int MaxNumOfLines = 500000;

        //secondary vertex buffer for linestrips rendering
        List<VertexPositionColor> stripVertexData;

        #endregion

        #region Methods

        public XDebugDrawer(XMain X)
            : base(ref X)
        {
            DrawOrder = 200;
        }

        public override void Load(ContentManager Content)
        {
            decl = new VertexDeclaration(X.GraphicsDevice, VertexPositionColor.VertexElements);
            lineRender = Content.Load<Effect>(@"Content\XEngine\Effects\DebugDrawer");

            lineVertices = new VertexPositionColor[MaxNumOfLines * 2];
            stripVertexData = new List<VertexPositionColor>(500000);

            base.Load(Content);
        }

        public void DrawLine(Vector3 startPoint, Color startColor,
                             Vector3 endPoint, Color endColor)
        {
            if (numOfLines < MaxNumOfLines)
            {
                Line line = new Line(startPoint, startColor, endPoint, endColor);

                if (lines.Count > numOfLines)
                {
                    if ((Line)lines[numOfLines] != line)
                    {
                        lines[numOfLines] = line;
                        buildVertexBuffer = true;
                    }
                }
                else
                {
                    lines.Add(line);
                    buildVertexBuffer = true;
                }

                numOfLines++;
            }
        }

        public void DrawLine(Vector3 startPoint, Vector3 endPoint, Color color)
        {
            DrawLine(startPoint, color, endPoint, color);
        }

        public void DrawCube(Vector3 min, Vector3 max, Color color, Matrix world, XCamera camera)
        {
            DrawLine(new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, min.Y, min.Z), color);
            DrawLine(new Vector3(max.X, min.Y, max.Z), new Vector3(min.X, min.Y, max.Z), color);
            DrawLine(new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, min.Y, min.Z), color);
            DrawLine(new Vector3(min.X, min.Y, min.Z), new Vector3(max.X, min.Y, min.Z), color);
            DrawLine(new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, max.Y, max.Z), color);
            DrawLine(new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color);
            DrawLine(new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);
            DrawLine(new Vector3(min.X, min.Y, min.Z), new Vector3(min.X, max.Y, min.Z), color);
            DrawLine(new Vector3(max.X, max.Y, max.Z), new Vector3(max.X, max.Y, min.Z), color);
            DrawLine(new Vector3(max.X, max.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color);
            DrawLine(new Vector3(min.X, max.Y, max.Z), new Vector3(min.X, max.Y, min.Z), color);
            DrawLine(new Vector3(min.X, max.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);

            //Draw(world, camera.View, camera.Projection);
        }

        public void DrawBoundingBox(BoundingBox boundingBox, Color color, Matrix world, XCamera camera)
        {
            DrawCube(boundingBox.Min, boundingBox.Max, color, world, camera);
        }

        public void DrawNormals(Model model, Color color, Matrix world, XCamera camera)
        {
            X.GraphicsDevice.Indices = null;
            X.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);

            foreach (ModelMesh mesh in model.Meshes)
            {
                int vertexCount = mesh.VertexBuffer.SizeInBytes / VertexPositionNormalTexture.SizeInBytes;

                VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[vertexCount];
                mesh.VertexBuffer.GetData(verts);

                for (int i = 0; i < vertexCount; i++)
                {
                    DrawLine(verts[i].Position, verts[i].Position + verts[i].Normal, color);
                }
            }

            //Draw(world, camera.View, camera.Projection);
        }

        void UpdateVertexBuffer()
        {
            if (numOfLines == 0 || lines.Count < numOfLines)
            {
                numOfPrimitives = 0;
                return;
            }

            for (int lineNum = 0; lineNum < numOfLines; lineNum++)
            {
                Line line = (Line)lines[lineNum];
                lineVertices[lineNum * 2 + 0] = new VertexPositionColor(
                    line.startPoint, line.startColor);
                lineVertices[lineNum * 2 + 1] = new VertexPositionColor(
                    line.endPoint, line.endColor);
            }
            numOfPrimitives = numOfLines;
            buildVertexBuffer = false;
        }

        #region Methods to add Linestrips (wireframes) to the stripVertexData buffer

        public void DrawShape(List<Vector3> shape, Color color)
        {
            if (stripVertexData.Count > 0)
            {
                Vector3 v = stripVertexData[stripVertexData.Count - 1].Position;
                stripVertexData.Add(new VertexPositionColor(v, new Color(0, 0, 0, 0)));
                stripVertexData.Add(new VertexPositionColor(shape[0], new Color(0, 0, 0, 0)));
            }

            foreach (Vector3 p in shape)
            {
                stripVertexData.Add(new VertexPositionColor(p, color));
            }
        }

        public void DrawShape(List<Vector3> shape, Color color, bool closed)
        {
            DrawShape(shape, color);

            Vector3 v = shape[0];
            stripVertexData.Add(new VertexPositionColor(v, color));
        }

        public void DrawShape(List<VertexPositionColor> shape)
        {
            if (stripVertexData.Count > 0)
            {
                Vector3 v = stripVertexData[stripVertexData.Count - 1].Position;
                stripVertexData.Add(new VertexPositionColor(v, new Color(0, 0, 0, 0)));
                stripVertexData.Add(new VertexPositionColor(shape[0].Position, new Color(0, 0, 0, 0)));
            }

            foreach (VertexPositionColor vps in shape)
            {
                stripVertexData.Add(vps);
            }
        }

        public void DrawShape(VertexPositionColor[] shape)
        {
            //Don't connect adjacent objects in the linestrip
            //add an invisible line alpha=0
            if (stripVertexData.Count > 0)
            {
                Vector3 v = stripVertexData[stripVertexData.Count - 1].Position;
                stripVertexData.Add(new VertexPositionColor(v, new Color(0, 0, 0, 0)));
                stripVertexData.Add(new VertexPositionColor(shape[0].Position, new Color(0, 0, 0, 0)));
            }

            foreach (VertexPositionColor vps in shape)
            {
                stripVertexData.Add(vps);
            }
        }

        public void DrawShape(VertexPositionColor[] shape, Color color)
        {
            //Don't connect adjacent objects in the linestrip
            //add an invisible line alpha=0
            if (stripVertexData.Count > 0)
            {
                Vector3 v = stripVertexData[stripVertexData.Count - 1].Position;
                stripVertexData.Add(new VertexPositionColor(v, new Color(0, 0, 0, 0)));
                stripVertexData.Add(new VertexPositionColor(shape[0].Position, new Color(0, 0, 0, 0)));
            }

            for (int i = 0; i < shape.Length; i++)
            {
                //if line is invisiale don't change color 
                if(shape[i].Color.A != 0) shape[i].Color = color;
                stripVertexData.Add(shape[i]);
            }
        }

        public void DrawShape(List<VertexPositionColor> shape, bool closed)
        {
            DrawShape(shape);

            VertexPositionColor v = shape[0];
            stripVertexData.Add(v);
        }

        #endregion

        public override void Draw(ref GameTime gameTime,ref XCamera camera)//Matrix world, Matrix view, Matrix projection)
        {
            if (buildVertexBuffer || numOfPrimitives != numOfLines)
            {
                UpdateVertexBuffer();
            }

            if (numOfPrimitives > 0)
            {
                X.GraphicsDevice.RenderState.AlphaBlendEnable = true;
                X.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                X.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                X.GraphicsDevice.RenderState.DepthBufferEnable = true;
                X.GraphicsDevice.VertexDeclaration = decl;

                lineRender.Parameters["World"].SetValue(Matrix.Identity);
                lineRender.Parameters["View"].SetValue(camera.View);
                lineRender.Parameters["Projection"].SetValue(camera.Projection);
                lineRender.CurrentTechnique = lineRender.Techniques["LineRendering3D"];

                lineRender.Begin();
                lineRender.CurrentTechnique.Passes[0].Begin();

                X.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                                PrimitiveType.LineList, lineVertices, 0, numOfPrimitives);
                if (stripVertexData.Count != 0)
                    X.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                            stripVertexData.ToArray(), 0, stripVertexData.Count - 1);

                lineRender.CurrentTechnique.Passes[0].End();
                lineRender.End();

                numOfLines = 0;
                lines.Clear();
                stripVertexData.Clear();
            }
        }

        #endregion
    }
    
	public static class XDebugVolumeRenderer
	{
		static GraphicsDevice device;
		static VertexDeclaration vertexDeclaration;
		static BasicEffect effect;

		static VertexBuffer sphereBuffer;
		static int numberOfSphereVerts;

        static VertexBuffer boxBuffer;
        const int numberOfBoxVerts = 19;
	
		public static void InitializeBuffers(GraphicsDevice device, int numberOfSphereVertices)
		{
            XDebugVolumeRenderer.device = device;
			vertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
			effect = new BasicEffect(device, null);

			//generate the sphere buffer **************************************************************
			numberOfSphereVerts = numberOfSphereVertices;
			VertexPositionColor[] vertices = new VertexPositionColor[numberOfSphereVerts];

			float step = MathHelper.TwoPi / (float)(numberOfSphereVerts - 1);

			for (int i = 0; i < numberOfSphereVerts; i++)
			{
				float angle = step * (float)i;
				vertices[i] = new VertexPositionColor(
					new Vector3((float)SMath.Cos(angle), 0f, (float)SMath.Sin(angle)),
					Color.White);
			}

			sphereBuffer = new VertexBuffer(device,VertexPositionColor.SizeInBytes * vertices.Length,
                BufferUsage.None);

			sphereBuffer.SetData<VertexPositionColor>(vertices);

            //Generate Box vertex buffer*************************************************************
            Vector3[] corners = { new Vector3(-1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(-1, -1, 1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1), new Vector3(1, -1, -1), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1), new Vector3(-1, 1, 1), new Vector3(-1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, 1, -1), new Vector3(1, -1, 1), new Vector3(1, -1, -1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1) };
            VertexPositionColor[] boxvertices = new VertexPositionColor[corners.Length];

            for (int i = 0; i < corners.Length; i++)
            {
                boxvertices[i] = new VertexPositionColor(corners[i], Color.White);
            }
            boxBuffer = new VertexBuffer(device, VertexPositionColor.SizeInBytes * boxvertices.Length, BufferUsage.None);
            boxBuffer.SetData<VertexPositionColor>(boxvertices);
            //***************************************************************************************
		}

		public static void RenderSphere(BoundingSphere sphere, Color color, ref Matrix view, ref Matrix projection)
		{
            //this null check is here in case we never ran the InitializeBuffers method of this class
            //if this is the case the device will be null
            if (device == null) return;

			device.VertexDeclaration = vertexDeclaration;
			device.Vertices[0].SetSource(sphereBuffer, 0, VertexPositionColor.SizeInBytes);

			effect.View = view;
			effect.Projection = projection;

			Matrix scale = Matrix.CreateScale(sphere.Radius);
			Matrix translation = Matrix.CreateTranslation(sphere.Center);

            effect.Begin(SaveStateMode.SaveState);

			for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
			{
				EffectPass pass = effect.CurrentTechnique.Passes[i];

				pass.Begin();

                effect.DiffuseColor = color.ToVector3();//Vector3.UnitY;
				effect.World = scale * translation;
				effect.CommitChanges();
				device.DrawPrimitives(PrimitiveType.LineStrip, 0, numberOfSphereVerts - 1);

                effect.DiffuseColor = color.ToVector3(); //Vector3.UnitZ;
				effect.World = scale * Matrix.CreateRotationX(MathHelper.PiOver2) * translation;
				effect.CommitChanges();
				device.DrawPrimitives(PrimitiveType.LineStrip, 0, numberOfSphereVerts - 1);

                effect.DiffuseColor = color.ToVector3(); //Vector3.UnitX;
				effect.World = scale * Matrix.CreateRotationZ(MathHelper.PiOver2) * translation;
				effect.CommitChanges();
				device.DrawPrimitives(PrimitiveType.LineStrip, 0, numberOfSphereVerts - 1);
				
				pass.End();
			}

			effect.End();
		}

        public static void RenderSphere(Vector3 center, float radius, Color color, ref Matrix view, ref Matrix projection)
        {
            //this null check is here in case we never ran the InitializeBuffers method of this class
            //if this is the case the device will be null
            if (device == null) return;

            device.VertexDeclaration = vertexDeclaration;
            device.Vertices[0].SetSource(sphereBuffer, 0, VertexPositionColor.SizeInBytes);

            effect.View = view;
            effect.Projection = projection;

            Matrix scale = Matrix.CreateScale(radius);
            Matrix translation = Matrix.CreateTranslation(center);

            effect.Begin(SaveStateMode.SaveState);
            for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
            {
                EffectPass pass = effect.CurrentTechnique.Passes[i];

                pass.Begin();

                effect.DiffuseColor = color.ToVector3();
                effect.World = scale * translation;
                effect.CommitChanges();
                device.DrawPrimitives(PrimitiveType.LineStrip, 0, numberOfSphereVerts - 1);

                effect.DiffuseColor = color.ToVector3();
                effect.World = scale * Matrix.CreateRotationX(MathHelper.PiOver2) * translation;
                effect.CommitChanges();
                device.DrawPrimitives(PrimitiveType.LineStrip, 0, numberOfSphereVerts - 1);

                effect.DiffuseColor = color.ToVector3();
                effect.World = scale * Matrix.CreateRotationZ(MathHelper.PiOver2) * translation;
                effect.CommitChanges();
                device.DrawPrimitives(PrimitiveType.LineStrip, 0, numberOfSphereVerts - 1);

                pass.End();
            }

            effect.End();
        }

        public static void RenderBox(Vector3 pos, Matrix orient, Vector3 sideLengths, Color color, ref Matrix view, ref Matrix projection)
        {
            //this null check is here in case we never ran the InitializeBuffers method of this class
            //if this is the case the device will be null
            if (device == null) return;

            device.VertexDeclaration = vertexDeclaration;
            device.Vertices[0].SetSource(boxBuffer, 0, VertexPositionColor.SizeInBytes);

            effect.View = view;
            effect.Projection = projection;

            Matrix scale = Matrix.CreateScale(sideLengths);
            Matrix translation = Matrix.CreateTranslation(pos);

            effect.Begin(SaveStateMode.SaveState);

            for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
            {
                EffectPass pass = effect.CurrentTechnique.Passes[i];

                pass.Begin();

                effect.DiffuseColor = color.ToVector3();
                effect.World = scale * translation * orient;
                effect.CommitChanges();
                device.DrawPrimitives(PrimitiveType.LineStrip, 0, numberOfBoxVerts - 1);

                pass.End();
            }

            effect.End();
        }
	}
    
}