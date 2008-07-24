// StaticModelContent.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace QuickStart.ContentPipeline.Model
{
    /// <summary>
    /// Container for static models during content creation.
    /// </summary>
    public class StaticModelContent
    {
        /// <summary>
        /// Retrieves the vertex content for the model.
        /// </summary>
        public VertexBufferContent VertexContent
        {
            get
            {
                return vertexContent;
            }
        }

        /// <summary>
        /// Retrieves the index content for the model.
        /// </summary>
        public IndexCollection IndexContent
        {
            get
            {
                return indexContent;
            }
        }

        private VertexBufferContent vertexContent;
        private IndexCollection indexContent;
        private VertexElement[] vertexElements;

        /// <summary>
        /// Initializes a new instance of a static model.
        /// </summary>
        public StaticModelContent()
        {
            vertexContent = new VertexBufferContent();
            indexContent = new IndexCollection();

            // Predefined vertex declaration
            vertexElements = new VertexElement[] {
                new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                new VertexElement(0, sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(0, sizeof(float) * 5, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
                new VertexElement(0, sizeof(float) * 8, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Binormal, 0),
                new VertexElement(0, sizeof(float) * 11, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Tangent, 0) };
        }

        /// <summary>
        /// Write the static model to a content pipeline data file.
        /// </summary>
        /// <param name="output">The ContentWriter for the data file.</param>
        public void Write(ContentWriter output)
        {
            output.WriteObject<VertexBufferContent>(vertexContent);
            output.WriteObject<IndexCollection>(indexContent);
            output.WriteObject<VertexElement[]>(vertexElements);
        }
    }
}
