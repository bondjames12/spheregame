/*
 * QuadTree.cs
 * 
 * This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
 * for license details.
 */

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using QuickStart.Graphics;

namespace QuickStart.Entities
{
    enum TreeSection
    {
        TopLeft = 0,
        TopRight = 1,
        BottomLeft = 2,
        BottomRight = 3
    }

    public class QuadTree : BaseEntity
    {
        /// <summary>
        /// <see cref="Terrain"/> that is <see cref="QuadTree"/> belongs to.
        /// </summary>
        private Terrain terrain;

        /// <summary>
        /// Bounding box that surrounds this <see cref="QuadTree"/> section.
        /// </summary>
        private BoundingBox boundingBox;    // Holds bounding box used for culling

        /// <summary>
        /// Top left <see cref="QuadTree"/> child section of this <see cref="QuadTree"/>.
        /// </summary>
        private QuadTree topLeft;

        /// <summary>
        /// Top right <see cref="QuadTree"/> child section of this <see cref="QuadTree"/>.
        /// </summary>
        private QuadTree topRight;

        /// <summary>
        /// Bottom left <see cref="QuadTree"/> child section of this <see cref="QuadTree"/>.
        /// </summary>
        private QuadTree bottomLeft;

        /// <summary>
        /// Bottom right <see cref="QuadTree"/> child section of this <see cref="QuadTree"/>.
        /// </summary>
        private QuadTree bottomRight;

        /// <summary>
        /// Array of <see cref="QuadTree"/> sections
        /// </summary>        
        private QuadTree[] childQuadTrees;

        /// <summary>
        /// Terrain patch for this <see cref="QuadTree"/> section. A single terrain patch can hold multiple
        /// LODs.
        /// </summary>
        private TerrainPatch leafPatch;

        /// <summary>
        /// Whether or not thie is a leaf node in the <see cref="QuadTree"/>. A leaf node has no child nodes
        /// branched off from it.
        /// </summary>
        private bool isLeaf = false;

        /// <summary>
        /// The first corner is the corner formed by the x, and z coordinates of the first vertex in the <see cref="QuadTree"/>, and
        /// the y coordinate of the lowest elevation value for the <see cref="QuadTree"/>.
        /// </summary>
        private Vector2 firstCorner = Vector2.Zero;

        /// <summary>
        /// The last corner is the corner formed by the x, and z coordinates of the last vertex in the <see cref="QuadTree"/> and the
        /// y coordinate of the highest elevation value for the <see cref="QuadTree"/>.
        /// </summary>
        private Vector2 lastCorner = Vector2.Zero;

        /// <summary>
        /// The dimensions (in the x and z axes) of this <see cref="QuadTree"/>.
        /// </summary>
        public int Width
        {
            get { return width; }
        }
        private int width;

        /// <summary>
        /// This is the offset along the X-axis for the beginning of this QuadTree section from the origin of the Terrain.
        /// </summary>
        public int offsetX;

        /// <summary>
        /// This is the offset along the Z-axis for the beginning of this QuadTree section from the origin of the Terrain.
        /// </summary>
        public int offsetZ;

        /// <summary>
        /// Contains the vertex offset for the first vertex in this <see cref="QuadTree"/> section.
        /// </summary>
        public int vertexBufferOffset = 0;

        /// <summary>
        /// Contains the vertex offset for the last vertex in this <see cref="QuadTree"/> section.
        /// </summary>
        public int vertexBufferOffsetEnd = 0;

        /// <summary>
        /// Holds mesh vertices that represent the bounding box created by this <see cref="QuadTree"/> section.
        /// </summary>
        public VertexPositionColor[] BoundingBoxMesh
        {
            get { return boundingBoxMesh; }
        }
        private VertexPositionColor[] boundingBoxMesh;

        /// <summary>
        /// Effect for drawing lines. Used for bounding box rendering.
        /// </summary>
        private BasicEffect lineEffect;

        /// <summary>
        /// Vertex declaration for vertices used to draw lines.
        /// </summary>
        private VertexDeclaration lineVertexDeclaration;

        /// <summary>
        /// Value is true whenever this <see cref="QuadTree"/>'s bounding box lies with a bounding frustum.
        /// </summary>
        public bool InView
        {
            get { return inView; }
            set { inView = value; }
        }
        private bool inView = false;

        /// <summary>
        /// Minimum height of this <see cref="QuadTree"/> section's bounding box.
        /// </summary>
        private float minHeight = 1000000.0f;

        /// <summary>
        /// Maximum height of this <see cref="QuadTree"/> section's bounding box.
        /// </summary>
        private float maxHeight = 0.0f;

        /// <summary>
        /// Width of the root <see cref="QuadTree"/>.
        /// </summary>
        private int rootWidth = 0;

        /// <summary>
        /// Creates a root <see cref="QuadTree"/> node. This should only be called from the
        /// parent Terrain section.
        /// </summary>
        /// <param name="game">QSGame reference</param>
        /// <param name="terrain">Parent terrain section</param>
        /// <param name="verticesLength">Length of vertices in this QuadTree</param>
        public QuadTree(QSGame game, Terrain terrain, int verticesLength)
            : base(game)
        {
            this.terrain = terrain;

            // Line effect is used for rendering debug bounding boxes
            this.lineEffect = new BasicEffect(game.GraphicsDevice, null);
            this.lineEffect.VertexColorEnabled = true;

            this.lineVertexDeclaration = new VertexDeclaration(game.GraphicsDevice, VertexPositionColor.VertexElements);

            // This truncation requires all heightmap images to be
            // a power of two in height and width
            this.width = (int)Math.Sqrt(verticesLength);
            this.rootWidth = width;

            // Vertices are only used for setting up the dimensions of
            // the bounding box. The vertices used in rendering are
            // located in the terrain class.
            SetupBoundingBox();

            // If this tree is the smallest allowable size, set it as a leaf
            // so that it will not continue branching smaller.
            if (verticesLength <= this.terrain.MinLeafSize)
            {
                this.isLeaf = true;

                CreateBoundingBoxMesh();
            }

            if (this.isLeaf)
            {
                SetupTerrainVertices();

                this.leafPatch = new TerrainPatch(this.Game, this.terrain, this);
            }
            else
            {
                BranchOffRoot();
            }
        }

        /// <summary>
        /// Creates a child <see cref="QuadTree"/> section.
        /// </summary>
        /// <param name="SourceTerrain">Parent <see cref="Terrain"/> section</param>
        /// <param name="verticesLength"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public QuadTree(QSGame game, Terrain sourceTerrain, int verticesLength, int offsetX, int offsetZ)
            : base(game)
        {            
            this.terrain = sourceTerrain;

            this.lineEffect = new BasicEffect(game.GraphicsDevice, null);
            this.lineEffect.VertexColorEnabled = true;

            this.lineVertexDeclaration = new VertexDeclaration(game.GraphicsDevice, VertexPositionColor.VertexElements);

            this.offsetX = offsetX;
            this.offsetZ = offsetZ;

            // This truncation requires all heightmap images to be
            // a power of two in height and width
            this.width = ((int)Math.Sqrt(verticesLength) / 2) + 1;

            SetupBoundingBox();

            // If this tree is the smallest allowable size, set it as a leaf
            // so that it will not continue branching smaller.
            if ((this.width - 1) * (this.width - 1) <= this.terrain.MinLeafSize)
            {
                this.isLeaf = true;

                CreateBoundingBoxMesh();
            }

            if (this.isLeaf)
            {
                SetupTerrainVertices();

                this.leafPatch = new TerrainPatch(this.Game, this.terrain, this);
            }
            else
            {
                BranchOff();
            }
        }

        /// <summary>
        /// Setup <see cref="BoundingBox"/> for this <see cref="QuadTree"/> section.
        /// </summary>
        private void SetupBoundingBox()
        {
            this.firstCorner = new Vector2(this.offsetX * this.terrain.ScaleFactor, this.offsetZ * this.terrain.ScaleFactor);
            this.lastCorner = new Vector2((this.width - 1 + this.offsetX) * this.terrain.ScaleFactor, 
                                          (this.width - 1 + this.offsetZ) * this.terrain.ScaleFactor);

            // Determine heights for use with the bounding box
            for (int x = 0; x < this.width; x++)
            {
                for (int z = 0; z < this.width; z++)
                {
                    if (this.terrain.heightData[x + this.offsetX, z + this.offsetZ] < this.minHeight)
                    {
                        this.minHeight = this.terrain.heightData[x + this.offsetX, z + this.offsetZ];
                    }
                    else if (this.terrain.heightData[x + this.offsetX, z + this.offsetZ] > this.maxHeight)
                    {
                        this.maxHeight = this.terrain.heightData[x + this.offsetX, z + this.offsetZ];
                    }
                }
            }

            boundingBox = new BoundingBox(new Vector3(this.firstCorner.X, this.minHeight, this.firstCorner.Y),
                                          new Vector3(this.lastCorner.X, this.maxHeight, this.lastCorner.Y));
        }

        /// <summary>
        /// This sets up the vertices for all of the triangles in this quad-tree section
        /// passes them to the main terrain component.
        /// </summary>
        private void SetupTerrainVertices()
        {
            int offset = this.terrain.VertexList.Count;

            // Texture the level
            for (int x = 0; x < width; x++)
                for (int z = 0; z < width; z++)
                {
                    VertexTerrain tempVert = new VertexTerrain();
                    tempVert.Position = new Vector3((offsetX + x) * terrain.ScaleFactor,
                                                    terrain.heightData[offsetX + x, offsetZ + z],
                                                    (offsetZ + z) * terrain.ScaleFactor );

                    tempVert.Normal = terrain.normals[offsetX + x, offsetZ + z];

                    this.terrain.VertexList.Add(tempVert);
                }

            this.vertexBufferOffset = offset;
            this.vertexBufferOffsetEnd = terrain.VertexList.Count;
        }

        /// <summary>
        /// Create a simple bounding box mesh so that we can show a QuadTree's <see cref="BoundingBox"/>
        /// </summary>
        private void CreateBoundingBoxMesh()
        {
            List<Vector3> boxList = new List<Vector3>();

            // 36 because there are 12 triangles in to make a cube, and each triangle has 3 points.
            this.boundingBoxMesh = new VertexPositionColor[36];

            for (int i = 0; i < 36; i++)
            {
                this.boundingBoxMesh[i].Color = Color.Magenta;
            }

            foreach (Vector3 thisVector in boundingBox.GetCorners())
            {
                boxList.Add(thisVector);
            }

            // Front
            boundingBoxMesh[0].Position = boxList[0];
            boundingBoxMesh[1].Position = boxList[1];
            boundingBoxMesh[2].Position = boxList[2];

            boundingBoxMesh[3].Position = boxList[2];
            boundingBoxMesh[4].Position = boxList[3];
            boundingBoxMesh[5].Position = boxList[0];

            // Top
            boundingBoxMesh[6].Position = boxList[0];
            boundingBoxMesh[7].Position = boxList[5];
            boundingBoxMesh[8].Position = boxList[1];

            boundingBoxMesh[9].Position = boxList[0];
            boundingBoxMesh[10].Position = boxList[4];
            boundingBoxMesh[11].Position = boxList[5];

            // Left
            boundingBoxMesh[12].Position = boxList[0];
            boundingBoxMesh[13].Position = boxList[3];
            boundingBoxMesh[14].Position = boxList[7];

            boundingBoxMesh[15].Position = boxList[7];
            boundingBoxMesh[16].Position = boxList[4];
            boundingBoxMesh[17].Position = boxList[0];

            // Right
            boundingBoxMesh[18].Position = boxList[1];
            boundingBoxMesh[19].Position = boxList[5];
            boundingBoxMesh[20].Position = boxList[6];

            boundingBoxMesh[21].Position = boxList[6];
            boundingBoxMesh[22].Position = boxList[3];
            boundingBoxMesh[23].Position = boxList[1];

            // Bottom
            boundingBoxMesh[24].Position = boxList[3];
            boundingBoxMesh[25].Position = boxList[7];
            boundingBoxMesh[26].Position = boxList[6];

            boundingBoxMesh[27].Position = boxList[6];
            boundingBoxMesh[28].Position = boxList[2];
            boundingBoxMesh[29].Position = boxList[3];

            // Back
            boundingBoxMesh[30].Position = boxList[7];
            boundingBoxMesh[31].Position = boxList[4];
            boundingBoxMesh[32].Position = boxList[6];

            boundingBoxMesh[33].Position = boxList[6];
            boundingBoxMesh[34].Position = boxList[4];
            boundingBoxMesh[35].Position = boxList[5];
        }

        /// <summary>
        /// Branch the root <see cref="QuadTree"/> node into four child QuadTree nodes.
        /// </summary>
        /// <remarks>Only called from the main root node</remarks>
        private void BranchOffRoot()
        {
            this.topLeft = new QuadTree(Game, terrain, width * width, 0, 0);
            this.bottomLeft = new QuadTree(Game, terrain, width * width, 0, width / 2 - 1);
            this.topRight = new QuadTree(Game, terrain, width * width, width / 2 - 1, 0);
            this.bottomRight = new QuadTree(Game, terrain, width * width, width / 2 - 1, width / 2 - 1);

            this.childQuadTrees = new QuadTree[4];

            this.childQuadTrees[(int)TreeSection.TopLeft] = topLeft;
            this.childQuadTrees[(int)TreeSection.TopRight] = topRight;
            this.childQuadTrees[(int)TreeSection.BottomLeft] = bottomLeft;
            this.childQuadTrees[(int)TreeSection.BottomRight] = bottomRight;
        }

        /// <summary>
        /// Branch a child <see cref="QuadTree"/> node into four more children QuadTree nodes
        /// </summary>
        private void BranchOff()
        {
            this.topLeft = new QuadTree(Game, terrain, width * width, offsetX, offsetZ);
            this.bottomLeft = new QuadTree(Game, terrain, width * width, offsetX, (width - 1) / 2 + offsetZ);
            this.topRight = new QuadTree(Game, terrain, width * width, (width - 1) / 2 + offsetX, offsetZ);
            this.bottomRight = new QuadTree(Game, terrain, width * width, (width - 1) / 2 + offsetX, (width - 1) / 2 + offsetZ);

            this.childQuadTrees = new QuadTree[4];

            this.childQuadTrees[(int)TreeSection.TopLeft] = topLeft;
            this.childQuadTrees[(int)TreeSection.TopRight] = topRight;
            this.childQuadTrees[(int)TreeSection.BottomLeft] = bottomLeft;
            this.childQuadTrees[(int)TreeSection.BottomRight] = bottomRight;

            //treeList = new List<QuadTree>();
            //treeList.Add(topLeft);
            //treeList.Add(topRight);
            //treeList.Add(bottomLeft);
            //treeList.Add(bottomRight);
        }

        /// <summary>
        /// If this <see cref="QuadTree"/> section is a leaf, and it is in view, draw it. If it is not a leaf
        /// then recursively call the draw functions of all four of its children.
        /// </summary>
        /// <param name="bFrustum">Frustum used to check section for culling</param>
        public void Draw(BoundingFrustum bFrustum)
        {
            // View is kept track of for later when vegetation is drawn.
            // This keeps the program from having to calculate the frustum intersections
            // again for each node.
            this.inView = false;

            // Check if QuadTree bounding box intersection the current view frustum
            if (bFrustum.Intersects(boundingBox))
            {
                this.InView = true;

                // Only draw leaves on the tree, never the main tree branches themselves.
                if (this.isLeaf)
                {
                    int detail = (int)terrain.Detail;

                    Game.GraphicsDevice.Indices = leafPatch.indexBuffers[detail];

                    this.terrain.TerrainEffect.Begin();

                    for (int i = 0; i < this.terrain.TerrainEffect.CurrentTechnique.Passes.Count; i++)
                    {
                        this.terrain.TerrainEffect.CurrentTechnique.Passes[i].Begin();

                        Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexBufferOffset, 0, width * width, 0, leafPatch.numTris[detail]);

                        this.terrain.TerrainEffect.CurrentTechnique.Passes[i].End();
                    }
                    this.terrain.TerrainEffect.End();

                    //leavesDrawn++;
                }                 
                // If there are branches on this node, move down through them recursively
                else if (this.childQuadTrees.Length > 0)
                {
                    for (int i = 0; i < this.childQuadTrees.Length; i++)
                    {
                        this.childQuadTrees[i].Draw(bFrustum);
                    }
                }
            }
        }

        public void QueryForRenderChunks(BoundingFrustum bFrustum)
        {
            // View is kept track of for later when vegetation is drawn.
            // This keeps the program from having to calculate the frustum intersections
            // again for each node.
            this.inView = false;

            // Check if QuadTree bounding box intersection the current view frustum
            if(bFrustum.Intersects(boundingBox))
            {
                this.InView = true;

                // Only draw leaves on the tree, never the main tree branches themselves.
                if(isLeaf)
                {
                    int detail = (int)terrain.Detail;

                    RenderChunk chunk = Game.Graphics.AllocateRenderChunk();

                    //Game.GraphicsDevice.Indices = leafPatch.indexBuffers[detail];
                    chunk.Indices = leafPatch.indexBuffers[detail];
                    chunk.VertexStreams.Add(terrain.VertexBuffer);
                    chunk.Declaration = terrain.VertexDeclaration;
                    chunk.WorldTransform = Matrix.Identity;
                    chunk.VertexCount = width * width;
                    chunk.StartIndex = 0;
                    chunk.VertexStreamOffset = vertexBufferOffset;
                    chunk.PrimitiveCount = leafPatch.numTris[detail];
                    chunk.Material = terrain.Material;
                    chunk.Type = PrimitiveType.TriangleList;


                    //terrain.TerrainEffect.Begin();

                    //for(int i = 0; i < terrain.TerrainEffect.CurrentTechnique.Passes.Count; i++)
                    //{
                    //    terrain.TerrainEffect.CurrentTechnique.Passes[i].Begin();

                    //    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexBufferOffset, 0, width * width, 0, leafPatch.numTris[detail]);

                    //    terrain.TerrainEffect.CurrentTechnique.Passes[i].End();
                    //}
                    //terrain.TerrainEffect.End();

                    //leavesDrawn++;
                }
                // If there are branches on this node, move down through them recursively
                else if(childQuadTrees.Length > 0)
                {
                    for(int i = 0; i < childQuadTrees.Length; i++)
                    {
                        childQuadTrees[i].QueryForRenderChunks(bFrustum);
                    }
                }
            }
        }

        /// <summary>
        /// This recursive function makes sure that every array of index buffers in each
        /// <see cref="QuadTree"/> node have proper pointers. This allows for the user to call any
        /// <see cref="LOD"/> draw on the terrain and the <see cref="Terrain"/> should draw that LOD or the next highest
        /// LOD possible available.
        /// </summary>
        public void SetupLODs()
        {
            // Only setup LODs for leaves on the tree, never the main tree branches themselves.
            if (this.isLeaf)
            {
                int highestLODUsed = 0;

                for (int i = (int)LOD.NumOfLODs - 1; i > 0; i--)
                {
                    if (this.leafPatch.indexBuffers[i] != null)
                    {
                        highestLODUsed = i;
                    }
                }

                for (int i = 1; i < (int)LOD.NumOfLODs; i++)
                {
                    if (i < highestLODUsed)
                    {
                        this.leafPatch.indexBuffers[i] = leafPatch.indexBuffers[highestLODUsed];
                        this.leafPatch.numTris[i] = leafPatch.numTris[highestLODUsed];
                    }
                    else if (i > highestLODUsed)
                    {
                        if (this.leafPatch.indexBuffers[i] == null)
                        {
                            this.leafPatch.indexBuffers[i] = leafPatch.indexBuffers[i - 1];
                            this.leafPatch.numTris[i] = leafPatch.numTris[i - 1];
                        }
                    }
                }
            }
            // If there are branches on this node, move down through them recursively
            else if (childQuadTrees.Length > 0)
            {
                for (int i = 0; i < childQuadTrees.Length; i++)
                {
                    childQuadTrees[i].SetupLODs();
                }
            }
        }

        /// <summary>
        /// Adds a new LOD to the <see cref="TerrainPatch"/> owned by this <see cref="QuadTree"/>.
        /// </summary>
        /// <param name="detailLevel">LOD to add to the TerrainPatch owned by this QuadTree.</param>
        public void AddNewPatchLOD(LOD detailLevel)
        {
            // Only setup LODs for leaves on the tree, never the main tree branches themselves.
            if (isLeaf)
            {
                this.leafPatch.SetupTerrainIndices(this.width, detailLevel);

                SetupLODs();        // Update LOD array to account for new LOD
            }

            // If there are branches on this node, move down through them recursively
            //else if (treeList != null)
            else if (childQuadTrees.Length > 0)
            {
                for (int i = 0; i < childQuadTrees.Length; i++)
                {
                    this.childQuadTrees[i].AddNewPatchLOD(detailLevel);
                }
            }

        }
    }
}

