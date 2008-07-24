// RenderChunk.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuickStart.Graphics
{
    /// <summary>
    /// Definition of a renderable chunk of geometry handled by the graphics system.
    /// </summary>
    public class RenderChunk
    {
        /// <summary>
        /// The <see cref="VertexBuffer"/>s used for the geometry chunk.
        /// </summary>
        public List<VertexBuffer> VertexStreams = new List<VertexBuffer>(1);

        /// <summary>
        /// The <see cref="IndexBuffer"/> used for the geometry chunk.
        /// </summary>
        public IndexBuffer Indices;

        /// <summary>
        /// The <see cref="VertexDeclaration"/> used for the geometry chunk.
        /// </summary>
        public VertexDeclaration Declaration;

        /// <summary>
        /// The starting index in the index buffer used for the geometry chunk.
        /// </summary>
        public int StartIndex;

        /// <summary>
        /// The number of vertices in the geometry chunk.
        /// </summary>
        public int VertexCount;

        /// <summary>
        /// The number of primitives in the geometry chunk.
        /// </summary>
        public int PrimitiveCount;

        /// <summary>
        /// The offset into the vertex stream for the geometry chunk.
        /// </summary>
        public int VertexStreamOffset;

        /// <summary>
        /// The type of primitive defined in the geometry chunk.
        /// </summary>
        public PrimitiveType Type;

        /// <summary>
        /// The world transformation matrix of the geometry chunk.
        /// </summary>
        public Matrix WorldTransform;

        /// <summary>
        /// The material assigned to the geometry chunk.
        /// </summary>
        public Material Material;

        /// <summary>
        /// Recycles and prepares the <see cref="RenderChunk"/> the reallocation.
        /// </summary>
        public void Recycle()
        {
            Indices = null;
            Declaration = null;
            VertexStreams.Clear();
            Material = null;
        }
    }
}
