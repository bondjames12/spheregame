// StaticModelProcessor.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace QuickStart.ContentPipeline.Model
{
    /// <summary>
    /// XNA content processor for handling static models.  Static models are baked into one vertex/index buffer for efficient rendering.
    /// </summary>
    [ContentProcessor(DisplayName="QuickStart Static Model Processor")]
    public class StaticModelProcessor : ContentProcessor<NodeContent, StaticModelContent>
    {
        /// <summary>
        /// Gets/sets the property indicating whether or not the coordinate system in the imported model should be transformed into the
        /// Quick Start coordinate system.  Set to true to transform the coordinate system.
        /// </summary>
        [DisplayName("Swap Coordinate System"), Description("True if model's coordinate system should be transformed into Quick Start coordinate system.  False, otherwise."), DefaultValue(true)]
        public bool SwapCoordinateSystem
        {
            get
            {
                return swapCoordSystem;
            }
            set
            {
                swapCoordSystem = value;
            }
        }

        private bool swapCoordSystem = true;

        /// <summary>
        /// Converts a NodeContent object into a StaticModelContent object.  All geometry is collapsed into one vertex/index buffer, and NTB data is generated.
        /// </summary>
        /// <param name="input">The NodeContent (from a model importer) to convert.</param>
        /// <param name="context">The context of the content processing environment.</param>
        /// <returns></returns>
        public override StaticModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            StaticModelContent staticModel = new StaticModelContent();

            BakeTransform(input);
            //MeshHelper.TransformScene(input, input.Transform);

            int vertexOffset = 0;
            int indexOffset = 0;

            ProcessNode(input, staticModel, context, ref vertexOffset, ref indexOffset);

            return staticModel;
        }

        /// <summary>
        /// Processes a single node in the node hierarchy.  Recursively processes children.
        /// </summary>
        /// <param name="node">The node to process.</param>
        private void ProcessNode(NodeContent node, StaticModelContent staticModel, ContentProcessorContext context, ref int indexOffset, ref int vertexOffset)
        {
            MeshContent mesh = node as MeshContent;

            if(mesh != null)
            {
                GenerateNTBData(mesh);

                if(swapCoordSystem)
                {
                    for(int i = 0; i < mesh.Positions.Count; ++i)
                    {
                        float temp;
                        Vector3 vec = mesh.Positions[i];

                        temp = vec.Y;
                        vec.Y = -vec.Z;
                        vec.Z = temp;

                        mesh.Positions[i] = vec;
                    }
                }

                foreach(GeometryContent geometry in mesh.Geometry)
                {
                    if(!geometry.Vertices.Channels.Contains("Normal0") || !geometry.Vertices.Channels.Contains("TextureCoordinate0") || !geometry.Vertices.Channels.Contains("Binormal0") || !geometry.Vertices.Channels.Contains("Tangent0"))
                    {
                        // Only complain about normals/texture coordinates, since binormal and tangent data should have been computed for us and would only have failed if normal/texture coordinate data did not exist.
                        throw new Exception("Geometry must contain Normal/Texture Coordinate channels.");
                    }

                    VertexChannel<Vector3> normals = geometry.Vertices.Channels.Get<Vector3>("Normal0");
                    VertexChannel<Vector3> binormals = geometry.Vertices.Channels.Get<Vector3>("Binormal0");
                    VertexChannel<Vector3> tangents = geometry.Vertices.Channels.Get<Vector3>("Tangent0");
                    VertexChannel<Vector2> texCoord0 = geometry.Vertices.Channels.Get<Vector2>("TextureCoordinate0");

                    if(swapCoordSystem)
                    {
                        for(int i = 0; i < geometry.Vertices.Positions.Count; ++i)
                        {
                            float temp;

                            Vector3 vec = normals[i];
                            temp = vec.Y;
                            vec.Y = -vec.Z;
                            vec.Z = temp;
                            normals[i] = vec;

                            vec = binormals[i];
                            temp = vec.Y;
                            vec.Y = -vec.Z;
                            vec.Z = temp;
                            binormals[i] = vec;

                            vec = tangents[i];
                            temp = vec.Y;
                            vec.Y = -vec.Z;
                            vec.Z = temp;
                            tangents[i] = vec;
                        }
                    }
                  
                    staticModel.VertexContent.Write<Vector3>(sizeof(float) * 0 + vertexOffset, sizeof(float) * 14, geometry.Vertices.Positions, context.TargetPlatform);
                    staticModel.VertexContent.Write<Vector2>(sizeof(float) * 3 + vertexOffset, sizeof(float) * 14, texCoord0, context.TargetPlatform);
                    staticModel.VertexContent.Write<Vector3>(sizeof(float) * 5 + vertexOffset, sizeof(float) * 14, normals, context.TargetPlatform);
                    staticModel.VertexContent.Write<Vector3>(sizeof(float) * 8 + vertexOffset, sizeof(float) * 14, binormals, context.TargetPlatform);
                    staticModel.VertexContent.Write<Vector3>(sizeof(float) * 11 + vertexOffset, sizeof(float) * 14, tangents, context.TargetPlatform);

                    vertexOffset += geometry.Vertices.Positions.Count * sizeof(float) * 14;

                    int[] indices = new int[geometry.Indices.Count];
                    geometry.Indices.CopyTo(indices, 0);

                    for(int i = 0; i < indices.Length; ++i)
                    {
                        indices[i] += indexOffset;
                    }

                    indexOffset += geometry.Vertices.Positions.Count;

                    staticModel.IndexContent.AddRange(indices);
                }
            }

            foreach(NodeContent child in node.Children)
            {
                ProcessNode(child, staticModel, context, ref indexOffset, ref vertexOffset);
            }
        }

        /// <summary>
        /// Transforms a node by its world transformation matrix.  Recursively transforms children.
        /// </summary>
        /// <param name="node">The node to transform.</param>
        private void BakeTransform(NodeContent node)
        {
            MeshHelper.TransformScene(node, node.Transform);
            node.Transform = Matrix.Identity;

            foreach(NodeContent child in node.Children)
            {
                BakeTransform(child);
            }
        }

        /// <summary>
        /// Generate tangents and binormals for the model data at the given node.  Recursively generates for children.
        /// </summary>
        /// <param name="content">The node to process.</param>
        private void GenerateNTBData(NodeContent content)
        {
            MeshContent mesh = content as MeshContent;

            if(mesh != null)
            {
                MeshHelper.CalculateTangentFrames(mesh, VertexChannelNames.TextureCoordinate(0), VertexChannelNames.Tangent(0), VertexChannelNames.Binormal(0));
            }

            foreach(NodeContent child in content.Children)
            {
                GenerateNTBData(child);
            }
        }
    }
}