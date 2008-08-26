using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public class XQuadDrawer : XComponent
    {
        VertexDeclaration vertexDecl = null;
        VertexPositionTexture[] verts = null;
        short[] ib = null;


        public XQuadDrawer(ref XMain X)
            : base(ref X)
        {
        }

        public override void Load(ContentManager Content)
        {
            vertexDecl = new VertexDeclaration(X.GraphicsDevice,
                                               VertexPositionTexture.VertexElements);

            verts = new VertexPositionTexture[6];

            verts[0] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(1, 1));
            verts[1] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));
            verts[2] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0));
            verts[3] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(1, 0));

            ib = new short[] { 0, 1, 2, 2, 3, 0 };

            verts[0].Position.X = 1.0f;
            verts[0].Position.Y = -1.0f;

            verts[1].Position.X = -1.0f;
            verts[1].Position.Y = -1.0f;

            verts[2].Position.X = -1.0f;
            verts[2].Position.Y = 1.0f;

            verts[3].Position.X = 1.0f;
            verts[3].Position.Y = 1.0f;

            base.Load(Content);
        }

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            X.GraphicsDevice.VertexDeclaration = vertexDecl;
            X.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, verts, 0, 4, ib, 0, 2);
        }
    }
}
