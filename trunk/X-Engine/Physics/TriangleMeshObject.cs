using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using JigLibX.Geometry;
using JigLibX.Physics;
using JigLibX.Collision;

namespace XEngine
{
    class TriangleMeshObject : PhysicsObject
    {
        TriangleMesh triangleMesh;

        public TriangleMeshObject(Model model, Matrix orientation, Vector3 position, Vector3 scale)
        {
            body = new Body();
            collision = new CollisionSkin(null);

            triangleMesh = new TriangleMesh();

            this.scale = scale;

            Dictionary<string, object> tagData = model.Tag as Dictionary<string, object>;
            Vector3[] vertices = tagData["Vertices"] as Vector3[];
            int[] indices = tagData["Indices"] as int[];

            List<Vector3> vertexList = new List<Vector3>();
            List<TriangleVertexIndices> indexList = new List<TriangleVertexIndices>();

            for (int i = 0; i < indices.Length / 3; i++)
            {
                indexList.Add(new TriangleVertexIndices(indices[i * 3 + 2], indices[i * 3 + 1], indices[i * 3 + 0]));
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertexList.Add(vertices[i]);
            }

            triangleMesh.CreateMesh(vertexList, indexList, 4, 1.0f);
            collision.AddPrimitive(triangleMesh, 1, new MaterialProperties(0.8f, 0.7f, 0.6f));
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(collision);
        }
    }
}
