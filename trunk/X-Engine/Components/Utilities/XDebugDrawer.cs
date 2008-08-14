using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

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

    public class XDebugDrawer : XComponent, XDrawable
    {
        #region Variables
        private Effect lineRender;
        private int numOfLines = 0;
        private List<Line> lines = new List<Line>();

        private bool buildVertexBuffer = false;
        static VertexDeclaration decl = null;

        static VertexPositionColor[] lineVertices;
        private int numOfPrimitives = 0;
        private int MaxNumOfLines = 5024;

        #endregion

        #region Methods

        public XDebugDrawer(XMain X) : base(X)
        {
            DrawOrder = 200;
        }

        public override void Load(ContentManager Content)
        {
            decl = new VertexDeclaration(X.GraphicsDevice, VertexPositionColor.VertexElements);
            lineRender = Content.Load<Effect>(@"Content\XEngine\Effects\DebugDrawer");

            lineVertices = new VertexPositionColor[MaxNumOfLines * 2];

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

                lineRender.CurrentTechnique.Passes[0].End();
                lineRender.End();

                numOfLines = 0;
                lines.Clear();
            }
        }

        #endregion
    }
}