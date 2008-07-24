// TriangleMeshShapeDesc.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using Microsoft.Xna.Framework;

namespace QuickStart.Physics
{
    /// <summary>
    /// Descriptor for triangle mesh physics shape.
    /// </summary>
    public class TriangleMeshShapeDesc : ShapeDesc
    {
        private Vector3[] vertices;
        private Vector3[] normals;
        private int[] indices;

        /// <summary>
        /// Position array.
        /// </summary>
        public Vector3[] Vertices
        {
            get { return this.vertices; }
            set { this.vertices = value; }
        }

        /// <summary>
        /// Normal array.
        /// </summary>
        public Vector3[] Normals
        {
            get { return this.normals; }
            set { this.normals = value; }
        }

        /// <summary>
        /// Triangle index array.
        /// </summary>
        public int[] Indices
        {
            get { return this.indices; }
            set { this.indices = value; }
        }
    }
}
