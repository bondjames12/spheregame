using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using JigLibX.Geometry;
using JigLibX.Physics;
using JigLibX.Collision;

//#if XBOX == FALSE
using ContentModel;
//#endif

namespace XEngine
{
    public class TriangleMeshObject : PhysicsObject
    {
        TriangleMesh triangleMesh;

        public TriangleMeshObject(XModel model, Matrix orientation, Vector3 position)
        {
            body = new Body();
            collision = new CollisionSkin(null);

            triangleMesh = new TriangleMesh();

            List<object> tagData = ((ModelData)model.Model.Tag).VertexData as List<object>;
            List<Vector3> vertices = (List<Vector3>)tagData[0];
            List<int> indices = (List<int>)tagData[1];

            Vector3[] verts = new Vector3[vertices.Count];
            int[] inds = new int[indices.Count];

            for (int i = 0; i < vertices.Count; i++)
                verts[i] = vertices[i];

            for (int i = 0; i < indices.Count; i++)
                inds[i] = indices[i];

            List<Vector3> vertexList = new List<Vector3>();
            List<TriangleVertexIndices> indexList = new List<TriangleVertexIndices>();

            for (int i = 0; i < inds.Length / 3; i++)
            {
                indexList.Add(new TriangleVertexIndices(indices[i * 3 + 2], indices[i * 3 + 1], indices[i * 3 + 0]));
            }

            for (int i = 0; i < verts.Length; i++)
            {
                vertexList.Add(vertices[i]);
            }

            triangleMesh.CreateMesh(vertexList, indexList, 4, 1.0f);

            collision.AddPrimitive(triangleMesh, 1, new MaterialProperties(0.8f, 0.7f, 0.6f));
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(collision);

            body.CollisionSkin = collision;
            body.MoveTo(position, orientation);
        }
    }
}
