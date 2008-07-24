// StaticModel.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace QuickStart.Graphics
{
    /// <summary>
    /// A model container that is optimized for static geometry.  No animation data is imported and all geometry is collapsed into single vertex and index buffers.
    /// </summary>
    public class StaticModel
    {
        // @todo: The interface to this class needs to be reworked to make it easy to use through the new render chunk system.

        /// <summary>
        /// Retrieves the number of vertices in the model.
        /// </summary>
        public int VertexCount
        {
            get { return numVertices; }
        }

        /// <summary>
        /// Retrieves the number of primitives (triangles) in the model.
        /// </summary>
        public int PrimitiveCount
        {
            get { return numPrimitives; }
        }

        /// <summary>
        /// Retrieves the type of geometry composing the model.
        /// </summary>
        public PrimitiveType GeometryType
        {
            get { return PrimitiveType.TriangleList; }
        }

        /// <summary>
        /// Retrieves the <see cref="VertexBuffer"/> for the model.
        /// </summary>
        public VertexBuffer VertexBuffer
        {
            get { return this.vertexBuffer; }
        }

        /// <summary>
        /// Retrieves the <see cref="IndexBuffer"/> for the model.
        /// </summary>
        public IndexBuffer IndexBuffer
        {
            get { return this.indexBuffer; }
        }

        /// <summary>
        /// Retrieves the <see cref="VertexDeclaration"/> for the model.
        /// </summary>
        public VertexDeclaration Declaration
        {
            get { return this.vertexDecl; }
        }

        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private VertexDeclaration vertexDecl;
        private int numPrimitives;
        private int numVertices;


        /// <summary>
        /// Creates a new StaticModel instance from a content pipeline data file.
        /// </summary>
        /// <param name="reader">The content pipeline data file reader.</param>
        internal StaticModel(ContentReader reader)
        {
            vertexBuffer = reader.ReadObject<VertexBuffer>();
            indexBuffer = reader.ReadObject<IndexBuffer>();
            vertexDecl = reader.ReadObject<VertexDeclaration>();

            if (indexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
            {
                numPrimitives = indexBuffer.SizeInBytes / 2 / 3;
            }
            else
            {
                numPrimitives = indexBuffer.SizeInBytes / 4 / 3;
            }

            numVertices = vertexBuffer.SizeInBytes / vertexDecl.GetVertexStrideSize(0);
        }

        /// <summary>
        /// Binds the vertex buffer, index buffer, and vertex declaration to the current graphics device.
        /// </summary>
        /// <param name="stream">The vertex stream index for the data.</param>
        public void BindBuffers(int stream)
        {
            vertexBuffer.GraphicsDevice.Vertices[stream].SetSource(vertexBuffer, 0, vertexDecl.GetVertexStrideSize(0));
            indexBuffer.GraphicsDevice.Indices = indexBuffer;
            vertexDecl.GraphicsDevice.VertexDeclaration = vertexDecl;
        }

        /// <summary>
        /// Renders the associated geometry in one call.  Equivalent to calling BindBuffers(0) followed by DrawIndexedPrimitive().
        /// </summary>
        public void DrawGeometry()
        {
            BindBuffers(0);

            vertexBuffer.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numPrimitives);
        }
    }
}
