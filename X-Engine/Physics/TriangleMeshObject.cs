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
    public class TriangleMeshObject : XPhysicsObject
    {
        TriangleMesh triangleMesh;

        public TriangleMeshObject(XModel model, Matrix orientation, Vector3 position)
        {
            body = new Body();
            collision = new CollisionSkin(null);

            triangleMesh = new TriangleMesh();

            foreach(ModelMesh mesh in model.Model.Meshes)
            {
                //declare an array of correct size to hold the models vertices
                VertexPositionNormalTexture[] vertices =
                    new VertexPositionNormalTexture[mesh.VertexBuffer.SizeInBytes / VertexPositionNormalTexture.SizeInBytes];
                //do again, debug this see which one is right??????????
                vertices =
                    new VertexPositionNormalTexture[mesh.VertexBuffer.SizeInBytes / mesh.MeshParts[0].VertexStride];
                
                //get a copy of the vertices
                mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices);

                List<Vector3> verts = new List<Vector3>();
                //Loop through each vertex and store in vector array
                for (int k = 0; k < vertices.Length; k++)
                    verts.Add(vertices[k].Position);  //may have to transform position relative to something??????


                //Create a list of TriangleVertexIndices from the Index buffer taking into account it could be ints or shorts
                List<TriangleVertexIndices> indexList = new List<TriangleVertexIndices>();
                if(mesh.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
                {
                    short[] tempshorts = new short[mesh.IndexBuffer.SizeInBytes / sizeof(short)];
                    mesh.IndexBuffer.GetData<short>(tempshorts);
                    for (int i = 0; i < tempshorts.Length / 3; i++)
                        indexList.Add(new TriangleVertexIndices((int)tempshorts[i * 3 + 2], (int)tempshorts[i * 3 + 1], (int)tempshorts[i * 3 + 0]));
                }

                if (mesh.IndexBuffer.IndexElementSize == IndexElementSize.ThirtyTwoBits)
                {
                    //create index array of correct size
                    int[] tempInts = new int[mesh.IndexBuffer.SizeInBytes / sizeof(int)];
                    mesh.IndexBuffer.GetData<int>(tempInts);
                    for (int i = 0; i < tempInts.Length / 3; i++)
                        indexList.Add(new TriangleVertexIndices(tempInts[i * 3 + 2], tempInts[i * 3 + 1], tempInts[i * 3 + 0]));
                }



                triangleMesh.CreateMesh(verts, indexList, 4, 1.0f);
                collision.AddPrimitive(triangleMesh, 1, new MaterialProperties(0.8f, 0.7f, 0.6f));
               
            }//do next mesh

            /*
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
             */
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(collision);

            body.CollisionSkin = collision;
            body.MoveTo(position, orientation);
        }

        /// <summary>
        /// Creates the bounding box for the model. This should only be called during model
        /// initialization, as it is much too slow to be used during runtime, or each frame for each model.
        /// </summary>
        public BoundingBox GetBoundingBox(Model model, ref Matrix[] BoneTransforms)
        {
            BoundingBox tempBox = new BoundingBox();

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                ModelMesh mesh = model.Meshes[i];
                VertexPositionNormalTexture[] vertices =
                    new VertexPositionNormalTexture[mesh.VertexBuffer.SizeInBytes / VertexPositionNormalTexture.SizeInBytes];

                mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices);

                Vector3[] vertexs = new Vector3[vertices.Length];

                for (int k = 0; k < vertexs.Length; k++)
                    vertexs[k] = Vector3.Transform(vertices[k].Position, BoneTransforms[mesh.ParentBone.Index]);// * WorldMatrix); 

                BoundingBox b = BoundingBox.CreateFromPoints(vertexs);

                if (i == 0)
                    tempBox = b;
                else
                    tempBox = BoundingBox.CreateMerged(tempBox, b);
            }

            return tempBox;
        }

    }
}
