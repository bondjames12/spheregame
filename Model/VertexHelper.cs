using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace ContentModel
{
    class VertexHelper
    {
        /// <summary>
        /// Helper for extracting a list of all the vertex positions in a model.
        /// </summary>
        public static List<object> FindVertices(NodeContent node)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            int i = 0;


            DoFindWork(node, vertices, indices, i);


            // Recursively scan over the children of this node.
            foreach (NodeContent child in node.Children)
            {
                DoFindWork(child, vertices, indices, i);
            }

            List<object> verts = new List<object>();
            verts.Add(vertices);
            verts.Add(indices);

            return verts;
        }

        static void DoFindWork(NodeContent node, List<Vector3> vertices, List<int> indices, int i)
        {
            // Is this node a mesh?
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                // Look up the absolute transform of the mesh.
                Matrix absoluteTransform = mesh.AbsoluteTransform;

                // Loop over all the pieces of geometry in the mesh.
                foreach (GeometryContent geometry in mesh.Geometry)
                {

                    // Loop over all the indices in this piece of geometry.
                    // Every group of three indices represents one triangle.
                    foreach (int index in geometry.Indices)
                    {
                        // Look up the position of this vertex.
                        Vector3 vertex = geometry.Vertices.Positions[index];

                        // Transform from local into world space.
                        vertex = Vector3.Transform(vertex, absoluteTransform);

                        // Store this vertex.
                        vertices.Add(vertex);
                        indices.Add(i);
                        i++;
                    }
                }
            }
            else
            {
                //throw new InvalidContentException("Mesh is null");
            }
        }
    }
}
