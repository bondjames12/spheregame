/*
 * Terrain.cs
 * 
 * This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
 * for license details.
 */

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using QuickStart;
using QuickStart.Graphics;
using QuickStart.Physics;

namespace QuickStart.Entities
{
    /*
     * Shader Input Structure
     * 
     *   struct VS_INPUT
         {
             float4 Position            : POSITION0;     
             float3 Normal              : NORMAL0;    
         };
     * */

    /// <summary>
    /// Used to hold vertex information for <see cref="Terrain"/>.
    /// </summary>
    public struct VertexTerrain
    {
        public Vector3 Position;
        public Vector3 Normal;

        public static int SizeInBytes = (3 + 3) * sizeof(float);
        public static VertexElement[] VertexElements = new VertexElement[]
        {
            new VertexElement( 0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0 ),
            new VertexElement( 0, sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0 ),            
        };
    }

    /// <summary>
    /// Used for level-of-detail values throughout the <see cref="Terrain"/> and <see cref="TerrainPatch"/> system.
    /// </summary>
    public enum LOD
    {
        NumOfLODs = 9,
        Minimum = 8,
        Low = 4,
        Med = 2,
        High = 1
    }

    /// <summary>
    /// The terrain class wraps up the entire terrain system, including all <see cref="QuadTree"/> sections it holds, and
    /// all <see cref="TerrainPatch"/> sections within the QuadTrees. The <see cref="Terrain"/> class also performs all
    /// operations dealing with terrain information, like determining the height of a point on the terrain.
    /// </summary>
    public class Terrain : BaseEntity
    {
        /// <summary>
        /// Retrieves the <see cref="VertexBuffer"/> associated with this terrain.
        /// </summary>
        public VertexBuffer VertexBuffer
        {
            get { return this.vertexBuffer; }
        }

        /// <summary>
        /// Holds entire <see cref="VertexBuffer"/> for this entire terrain section.
        /// </summary>
        private VertexBuffer vertexBuffer;

        /// <summary>
        /// Retrieves the <see cref="VertexDeclaration"/> associated with this terrain.
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get { return this.vertDeclaration; }
        }

        /// <summary>
        /// The vertex declaration for the vertex type for terrain.
        /// </summary>
        private VertexDeclaration vertDeclaration;

        /// <summary>
        /// Holds vertices for entire <see cref="Terrain"/> section until all <see cref="QuadTree"/> sections and
        /// <see cref="TerrainPatch"/> sections have finished loading, and then this list is released.
        /// </summary>
        public List<VertexTerrain> VertexList
        {
            get { return this.vertexList; }
            set
            {
                this.vertexList = value;
            }
        }
        private List<VertexTerrain> vertexList;

        /// <summary>
        /// The root <see cref="QuadTree"/> node for the entire <see cref="Terrain"/> section.
        /// </summary>
        public QuadTree RootQuadTree
        {
            get { return this.rootQuadTree; }
        }
        private QuadTree rootQuadTree;

        /// <summary>
        /// Height/Width of the <see cref="Terrain"/> heightfield. Must be power-of-two.
        /// </summary>
        public int Size
        {
            get { return this.size; }
            set
            {
                // Anytime size is changed we make sure it is being set to a power-of-two value.
                if (QSMath.IsPowerOf2(value))
                {
                    this.size = value;
                }
                else
                {
                    throw new Exception("Terrain size (height and width) must be a power-of-two!");
                }
            }
        }
        private int size;

        /// <summary>
        /// <see cref="Terrain"/> scaling factor. Larger values will result in a larger terrain.
        /// </summary>
        public int ScaleFactor
        {
            get { return this.scaleFactor; }
        }
        private int scaleFactor = 1;

        /// <summary>
        /// Elevation strength of <see cref="Terrain"/>. Larger value will give you taller, and steeper <see cref="Terrain"/>.
        /// </summary>
        public float ElevationStrength
        {
            get { return this.elevationStrength; }
            set 
            {
                if (value < float.Epsilon)
                {
                    value = QSConstants.DefaultTerrainElevStr;
                }

                this.elevationStrength = value; 
            }
        }
        private float elevationStrength = 1.0f;

        /// <summary>
        /// Holds the height (Y coordinate) of each [x, z] coordinate. Height data is loaded from a heightmap image.
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public float[,] heightData;

        /// <summary>
        /// Holds information about the texture type, which is stored in each vertex after the vertex buffers 
        /// are created.
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public Color[,] textureTypeData;

        /// <summary>
        /// Holds the normal vectors for each vertex in the terrain.
        /// The normals for lighting are later stored in each vertex, but
        /// we want to store these values permanentally for proper physics
        /// collisions with the ground.
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public Vector3[,] normals;

        /// <summary>
        /// Holds information about billboards in on the terrain.
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public int[,] billboardData;

        /// <summary>
        /// Heightmap image which is used to setup the heightfield for this <see cref="Terrain"/>
        /// </summary>
        /// <remarks>Can be set to <see cref="null"/> after heightfield is created</remarks>
        private Texture2D heightMap;

        /// <summary>
        /// Terrainmap image which is used to determine terrain texture splatting.
        /// </summary>
        /// <remarks>Can be set to <see cref="null"/> after <see cref="terrainTypeData"/> is created</remarks>
        private Texture2D terrainMap;

        /// <summary>
        /// Billboard image which is used to determine where billboards and vegetation will be placed.
        /// </summary>
        /// <remarks>For use with future vegetation system</remarks>
        //private Texture2D billboardMap;

        /// <summary>
        /// Holds the different textures used for this <see cref="Terrain"/> section.
        /// </summary>
        private Texture2D[] terrainTextures;

        /// <summary>
        /// Holds the different texture normal map images used for this <see cref="Terrain"/> section.
        /// </summary>
        private Texture2D[] terrainTextureNormals;

        /// <summary>
        /// Holds the different billboard textures used for this <see cref="Terrain"/> section.
        /// </summary>
        /// <remarks>For use with future vegetation system</remarks>
        //private Texture2D[] billboardTextures;

        /// <summary>
        /// HLSL Effect for terrain rendering.
        /// </summary>
        public Effect TerrainEffect
        {
            get { return this.terrainEffect; }
        }
        private Effect terrainEffect;

        IPhysicsActor terrainActor;

        /// <summary>
        /// HLSL Effect for vegetation/billboard rendering.
        /// </summary>
        /// <remarks>For use with future vegetation system</remarks>
        //private Effect vegEffect;

        /// <summary>
        /// Must be power of two values.
        /// ALL <see cref="QuadTree"/> sections leafs will result in this size.
        /// </summary>
        public int MinLeafSize
        {
            get { return this.minLeafSize; }
            private set
            {
                if (QSMath.IsPowerOf2(value))
                {
                    this.minLeafSize = value;
                }
                else
                {
                    throw new Exception("Leaf size must be set to a power-of-two");
                }
            }
        }
        private int minLeafSize = QSConstants.DefaultQuadTreeWidth * QSConstants.DefaultQuadTreeWidth;

        /// <summary>
        /// Default <see cref="Terrain"/> level-of-detail setting.
        /// </summary>
        public LOD DetailDefault
        {
            get { return this.detailDefault; }
            set { this.detailDefault = value; }
        }
        private LOD detailDefault = LOD.High;

        /// <summary>
        /// Current <see cref="Terrain"/> level-of-detail setting.
        /// </summary>
        public LOD Detail
        {
            get { return this.detail; }
        }
        private LOD detail = LOD.High;

        /// <summary>
        /// Retrieves the material used for the terrain.
        /// </summary>
        public Material Material
        {
            get { return this.material; }
        }

        private Material material;

        /// <summary>
        /// Create an entire <see cref="Terrain"/> section.
        /// </summary>
        /// <param name="game">QSGame reference</param>
        public Terrain(QSGame game)
            : base(game)
        {
            this.terrainEffect = game.Content.Load<Effect>("./Effects/Terrain");

            if (QSConstants.DetailLevel >= GraphicsLevel.High)
            {
                this.terrainEffect.CurrentTechnique = terrainEffect.Techniques["MultiTexturedNormaled"];
            }
            else
            {
                this.terrainEffect.CurrentTechnique = terrainEffect.Techniques["MultiTextured"];
            }

            this.vertexList = new List<VertexTerrain>();

            this.vertDeclaration = new VertexDeclaration(this.Game.GraphicsDevice, VertexTerrain.VertexElements);
        }

        /// <summary>
        /// Create an entire <see cref="Terrain"/> section.
        /// </summary>
        /// <param name="game">QSGame reference</param>
        /// <param name="detailLevel">Level-of-detail of this terrain. Higher is better quality, lower is better performance.</param>
        public Terrain(QSGame game, LOD detailLevel)
            : base(game)
        {
            this.detailDefault = detailLevel;
            this.detail = detailDefault;

            this.terrainEffect = this.Game.Content.Load<Effect>("./Effects/Terrain");

            if (QSConstants.DetailLevel >= GraphicsLevel.High)
            {
                this.terrainEffect.CurrentTechnique = this.terrainEffect.Techniques["MultiTexturedNormaled"];
            }
            else
            {
                this.terrainEffect.CurrentTechnique = this.terrainEffect.Techniques["MultiTextured"];
            }

            this.vertexList = new List<VertexTerrain>();

            this.vertDeclaration = new VertexDeclaration(this.Game.GraphicsDevice, VertexTerrain.VertexElements);
        }        

        /// <summary>
        /// Setup terrain shader parameters, and tell the root <see cref="QuadTree"/> node to begin drawing. The
        /// QuadTree will recursively determine if the node is in view, and if not then none of its children
        /// will be drawn or checked for collision with the camera's frustum.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentCam"></param>
        /// <param name="sceneLight"></param>
        public void Draw(GameTime gameTime, Camera currentCam, Light sceneLight)
        {
            //Game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexTerrain.SizeInBytes);
            //Game.GraphicsDevice.VertexDeclaration = vertDeclaration;

            //terrainEffect.CurrentTechnique = terrainEffect.Techniques["MultiTexturedNormaled"];

            //terrainEffect.Parameters["CameraForward"].SetValue(currentCam.Forward);
            
            //terrainEffect.Parameters["World"].SetValue(Matrix.Identity);
            //terrainEffect.Parameters["WorldViewProj"].SetValue(currentCam.viewMatrix * currentCam.projectionMatrix);

            //terrainEffect.Parameters["LightDirection"].SetValue(sceneLight.direction);
            //terrainEffect.Parameters["AmbientColor"].SetValue(sceneLight.ambientColor);
            //terrainEffect.Parameters["AmbientPower"].SetValue(sceneLight.AmbientPower);
            //terrainEffect.Parameters["SpecularColor"].SetValue(sceneLight.specularColor);
            //terrainEffect.Parameters["SpecularPower"].SetValue(sceneLight.SpecularPower);
            //terrainEffect.Parameters["DiffuseColor"].SetValue(sceneLight.diffuseColor);

            //rootQuadTree.Draw(currentCam.Frustum);
        }

        public void QueryForRenderChunks(ref RenderPassDesc desc)
        {
            this.rootQuadTree.QueryForRenderChunks(desc.RenderCamera.Frustum);
        }

        /// <summary>
        /// Initialize main terrain settings.
        /// </summary>
        /// <param name="heightImagePath">File path for heightmap image, image must be a power of 2 in height and width</param>
        /// <param name="terrainTexturePath">File path for terrain texture image, image must be a power of 
        /// 2 in height and width. Texture image defines where multi-texture splatting occurs. You can use
        /// this to draw out paths or sections in the <see cref="Terrain"/>.</param>
        /// <param name="scaleFactor">Scale (size) of <see cref="Terrain"/>.</param>
        /// <param name="smoothingPasses">Smoothes out the terrain using averages of height. The number of
        /// smoothing passes you choose to make is up to you. If you have sharp elevations on your map, 
        /// you have the elevation strength turned up then you may want a higher value. If your terrain 
        /// is already smooth or has very small elevation strength you may not need any passes. Default 
        /// value is 5. Use value of 0 to skip smoothing.</param>
        public void Initialize(string heightImagePath, string terrainTexturePath, int scaleFactor, int? smoothingPasses, IPhysicsScene physicsScene)
        {
            this.scaleFactor = (int)MathHelper.Clamp(scaleFactor, 1, QSConstants.MaxTerrainScale);

            //terrainEffect.Parameters["TerrainScale"].SetValue(scaleFactor);

            LoadHeightData(heightImagePath);                                // Load heightfield from heightmap image
            LoadTerrainTypeData(terrainTexturePath);                        // Load terrain texture splatting

            SmoothTerrain(smoothingPasses);                                 // Smooth out the Terrain
            SetupTerrainNormals();                                          // Setup the normals for each terrain vertex

            this.rootQuadTree = new QuadTree(this.Game, this, this.normals.Length); // Initialize the root quad-tree node

            SetupTerrainVertexBuffer();             // QuadTree sections have setup the vertex list, now this creates a VertexBuffer.

            textureTypeData = null;                 // Free terrain data to GC now that each quad-tree section has its own data.
            vertexList.Clear();                // Free terrain vertex list, as all vertex data is loaded into vertex buffer.


            // Create terrain physics actor
            ActorDesc actorDesc = new ActorDesc();
            actorDesc.Density = 0;
            actorDesc.Dynamic = false;
            actorDesc.Orientation = Matrix.Identity;
            actorDesc.Position = Vector3.Zero;

            // Create physics shape
            HeightFieldShapeDesc heightFieldDesc = new HeightFieldShapeDesc();
            heightFieldDesc.HeightField = this.heightData;
            heightFieldDesc.SizeX = this.heightData.GetLength(0) * this.scaleFactor;
            heightFieldDesc.SizeZ = this.heightData.GetLength(1) * this.scaleFactor;

            // Bind shape to actor
            actorDesc.Shapes.Add(heightFieldDesc);

            // Create actor
            this.terrainActor = physicsScene.CreateActor(actorDesc);
        }

        /// <summary>
        /// Loads all content needed for the terrain.
        /// </summary>
        public void LoadContent()
        {
            this.material = this.Game.Content.Load<Material>("Material/Terrain");
        }

        /// <summary>
        /// Unloads all content previously loaded by the terrain.
        /// </summary>
        public void UnloadContent()
        {
        }

        /// <summary>
        /// Load the heightfield data from the heightmap image
        /// </summary>
        /// <param name="heightImagePath">Path of heightmap image to read from</param>
        private void LoadHeightData(string heightImagePath)
        {
            this.heightMap = Game.Content.Load<Texture2D>(heightImagePath);

            if((QSMath.IsPowerOf2(this.heightMap.Width) && QSMath.IsPowerOf2(this.heightMap.Height) == false))
            {
                throw new Exception("Height maps must have a width and height that is a power of two.");
            }

            float minimumHeight = 1000;             // Set a high number because this will go drop with the math below
            float maximumHeight = 0;                // Opposite of line above

            this.Size = heightMap.Width;             // Sets the map width to the same as the heightmap texture.

            // We setup the map for colors so we can use the color to determine elevations of the map
            Color[] heightMapColors = new Color[this.size * this.size];

            // XNA Built-in feature automatically copies pixel data into the heightmap.
            this.heightMap.GetData(heightMapColors);

            this.heightData = new float[size, size];  // Create an array to hold elevations from heightMap

            // Find minimum and maximum values for the heightmap file we read in
            for (int x = 0; x < size; x++)
                for (int z = 0; z < size; z++)
                {
                    this.heightData[x, z] = heightMapColors[x + z * size].R;
                    if(this.heightData[x, z] < minimumHeight) minimumHeight = this.heightData[x, z];
                    if(this.heightData[x, z] > maximumHeight) maximumHeight = this.heightData[x, z];
                }


            // Set height by color, and then alter height by min and max amounts
            for (int x = 0; x < size; x++)
                for (int z = 0; z < size; z++)
                {
                    this.heightData[x, z] = (heightMapColors[z + x * size].R) + (heightMapColors[z + x * this.size].G) + (heightMapColors[z + x * size].B);
                    this.heightData[x, z] = (this.heightData[x, z] - minimumHeight) / (maximumHeight - minimumHeight) * elevationStrength * this.scaleFactor;
                }

            this.heightMap = null;

            this.terrainEffect.Parameters["TerrainWidth"].SetValue(this.size);
        }

        private void LoadTerrainTypeData(string terrainTexturePath)
        {
            this.terrainMap = Game.Content.Load<Texture2D>(terrainTexturePath);
            this.terrainEffect.Parameters["TextureMap"].SetValue(this.terrainMap);

            if((QSMath.IsPowerOf2(this.terrainMap.Width) && QSMath.IsPowerOf2(this.terrainMap.Height)) == false)
            {
                throw new Exception("Terrain maps must have a width and height that is a power of two.");
            }

            // We setup the map with colors so we can splat terrain in the quad-tree sections.
            Color[] terrainTypeColors = new Color[size * size];

            this.terrainMap.GetData(terrainTypeColors);        // XNA GetData feature automatically copies
                                                          // pixel data into a Color array for us.

            this.textureTypeData = new Color[size, size];

            for (int x = 0; x < size; x++)
                for (int z = 0; z < size; z++)
                {
                    this.textureTypeData[x, z] = new Color(terrainTypeColors[z + x * size].R,
                                                      terrainTypeColors[z + x * size].G,
                                                      terrainTypeColors[z + x * size].B);
                }
        }

        /// <summary>
        /// Smooths the terrain.
        /// </summary>
        /// <param name="smoothingPasses">Number of smoothing passes to use. More passes will result in a smoother terrain,
        /// however more smoothing takes more loading time.</param>
        private void SmoothTerrain(int? smoothingPasses)
        {
            if (smoothingPasses < 0 || smoothingPasses == null)
            {
                smoothingPasses = QSConstants.DefaultTerrainSmoothing;
            }

            int passes = (int)smoothingPasses;

            float[,] newHeightData;

            while (passes > 0)
            {
                passes--;
                newHeightData = new float[size, size];

                for(int x = 0; x < this.size; x++)
                {
                    for(int z = 0; z < this.size; z++)
                    {
                        int adjacentSections = 0;
                        float sectionsTotal = 0.0f;

                        // =================================================================
                        if ((x - 1) > 0)            // Check to left
                        {
                            sectionsTotal += this.heightData[x - 1, z];
                            adjacentSections++;

                            if ((z - 1) > 0)        // Check up and to the left
                            {
                                sectionsTotal += this.heightData[x - 1, z - 1];
                                adjacentSections++;
                            }

                            if ((z + 1) < size)        // Check down and to the left
                            {
                                sectionsTotal += this.heightData[x - 1, z + 1];
                                adjacentSections++;
                            }
                        }
                        // =================================================================

                        // =================================================================
                        if ((x + 1) < size)     // Check to right
                        {
                            sectionsTotal += this.heightData[x + 1, z];
                            adjacentSections++;

                            if ((z - 1) > 0)        // Check up and to the right
                            {
                                sectionsTotal += this.heightData[x + 1, z - 1];
                                adjacentSections++;
                            }

                            if ((z + 1) < size)        // Check down and to the right
                            {
                                sectionsTotal += this.heightData[x + 1, z + 1];
                                adjacentSections++;
                            }
                        }
                        // =================================================================

                        // =================================================================
                        if ((z - 1) > 0)            // Check above
                        {
                            sectionsTotal += this.heightData[x, z - 1];
                            adjacentSections++;
                        }
                        // =================================================================

                        // =================================================================
                        if ((z + 1) < size)    // Check below
                        {
                            sectionsTotal += this.heightData[x, z + 1];
                            adjacentSections++;
                        }
                        // =================================================================

                        newHeightData[x, z] = (this.heightData[x, z] + (sectionsTotal / adjacentSections)) * 0.5f;
                    }
                }

                // Overwrite the HeightData info with our new smoothed info
                for(int x = 0; x < this.size; x++)
                    for(int z = 0; z < this.size; z++)
                    {
                        this.heightData[x, z] = newHeightData[x, z];
                    }
            }
        }

        /// <summary>
        /// Setup <see cref="Terrain"/> normals. Normals are used for lighting, normal mapping, and physics with terrain.
        /// </summary>
        private void SetupTerrainNormals()
        {
            VertexTerrain[] terrainVertices = new VertexTerrain[this.size * this.size];
            this.normals = new Vector3[size, size];

            // Determine vertex positions so we can figure out normals in section below.
            for(int x = 0; x < this.size; x++)
                for(int z = 0; z < this.size; z++)
                {
                    terrainVertices[x + z * size].Position = new Vector3(x * this.scaleFactor, this.heightData[x, z], z * this.scaleFactor);
                }

            // Setup normals for lighting and physics (Credit: Riemer's method)
            for(int x = 1; x < this.size - 1; x++)
                for(int z = 1; z < this.size - 1; z++)
                {
                    Vector3 normX = new Vector3((terrainVertices[x - 1 + z * this.size].Position.Y - terrainVertices[x + 1 + z * this.size].Position.Y) / 2, 1, 0);
                    Vector3 normZ = new Vector3(0, 1, (terrainVertices[x + (z - 1) * this.size].Position.Y - terrainVertices[x + (z + 1) * this.size].Position.Y) / 2);
                    terrainVertices[x + z * size].Normal = normX + normZ;
                    terrainVertices[x + z * size].Normal.Normalize();

                    this.normals[x, z] = terrainVertices[x + z * size].Normal;    // Stored for use in physics and for the
                                                                                    // quad-tree component to reference.
                }
        }

        /// <summary>
        /// Setup vertex buffer for entire terrain section.
        /// </summary>
        private void SetupTerrainVertexBuffer()
        {
            VertexTerrain[] verticesArray = new VertexTerrain[vertexList.Count];

            for(int i = 0; i < this.vertexList.Count; i++)
            {
                verticesArray[i] = this.vertexList[i];
            }

            this.vertexBuffer = new VertexBuffer(this.Game.GraphicsDevice, VertexTerrain.SizeInBytes * (this.size + (this.size / QSConstants.DefaultQuadTreeWidth)) * (this.size + (this.size / QSConstants.DefaultQuadTreeWidth)), BufferUsage.WriteOnly);
            this.vertexBuffer.SetData(verticesArray);
        }

        /// <summary>
        /// Sets up three different textures that the terrain can use.
        /// </summary>
        /// <param name="Texture0">Terrain texture #0</param>
        /// <param name="Texture1">Terrain texture #1</param>
        /// <param name="Texture2">Terrain texture #2</param>
        public void InitTerrainTextures(string texture0, string texture1, string texture2)
        {
            this.terrainTextures = new Texture2D[3];

            this.terrainTextures[0] = Game.Content.Load<Texture2D>(texture0);
            this.terrainTextures[1] = Game.Content.Load<Texture2D>(texture1);
            this.terrainTextures[2] = Game.Content.Load<Texture2D>(texture2);

            this.terrainEffect.Parameters["GrassTexture"].SetValue(this.terrainTextures[0]);
            this.terrainEffect.Parameters["SandTexture"].SetValue(this.terrainTextures[1]);
            this.terrainEffect.Parameters["RockTexture"].SetValue(this.terrainTextures[2]);
        }

        /// <summary>
        /// Sets up three different normal textures for use with terrain normal mapping. Normal textures
        /// should match the order of the terrain textures. If grass was used for texture0, then grass normal texture
        /// should be used for texture0Normal.
        /// </summary>
        /// <param name="texture0Normal">Normal texture #0</param>
        /// <param name="texture1Normal">Normal texture #1</param>
        /// <param name="texture2Normal">Normal texture #2</param>
        public void InitTerrainNormalsTextures(string texture0Normal, string texture1Normal, string texture2Normal)
        {
            this.terrainTextureNormals = new Texture2D[3];

            this.terrainTextureNormals[0] = Game.Content.Load<Texture2D>(texture0Normal);
            this.terrainTextureNormals[1] = Game.Content.Load<Texture2D>(texture1Normal);
            this.terrainTextureNormals[2] = Game.Content.Load<Texture2D>(texture2Normal);

            this.terrainEffect.Parameters["GrassNormal"].SetValue(this.terrainTextureNormals[0]);
            this.terrainEffect.Parameters["SandNormal"].SetValue(this.terrainTextureNormals[1]);
            this.terrainEffect.Parameters["RockNormal"].SetValue(this.terrainTextureNormals[2]);
        }

        //public void InitTerrainVegetation(string vegTexture0, string vegTexture1, string vegTexture2, string billboardMapPath)
        //{
        //    billboardTextures = new Texture2D[3];
        //    billboardTextures[0] = Game.Content.Load<Texture2D>(vegTexture0);
        //    billboardTextures[1] = Game.Content.Load<Texture2D>(vegTexture1);
        //    billboardTextures[2] = Game.Content.Load<Texture2D>(vegTexture2);

        //    billboardMap = Game.Content.Load<Texture2D>(billboardMapPath);

        //    if ((QSMathHelper.IsPowerOf2(billboardMap.Width) && QSMathHelper.IsPowerOf2(billboardMap.Height)) == false)
        //    {
        //        throw new Exception("Billboard maps must have a width and height that is a power of two.");
        //    }
        //}

        /// <summary>
        /// Get the height of the terrain at given horizontal coordinates.
        /// </summary>
        /// <param name="xPos">X coordinate</param>
        /// <param name="zPos">Z coordinate</param>
        /// <returns>Height at given coordinates</returns>
        public float GetTerrainHeight(float xPos, float zPos)
        {
            // we first get the height of four points of the quad underneath the point
            // Check to make sure this point is not off the map at all
            int x = (int)(xPos / this.scaleFactor);
            if(x > this.size - 2)
            {
                return -10000.0f;      // Terrain height is considered -10000 (or any really low number will do)
                                       // if it is outside the heightmap.
            }
            else if (x < 0)
            {
                return -10000.0f;
            }

            int z = (int)(zPos / this.scaleFactor);
            if(z > this.size - 2)
            {
                return -10000.0f;
            }
            else if (z < 0)
            {
                return -10000.0f;
            }

            float triZ0 = (this.heightData[x, z]);
            float triZ1 = (this.heightData[x + 1, z]);
            float triZ2 = (this.heightData[x, z + 1]);
            float triZ3 = (this.heightData[x + 1, z + 1]);

            float height;
            float sqX = (xPos / this.scaleFactor) - x;
            float sqZ = (zPos / this.scaleFactor) - z;
            if ((sqX + sqZ) < 1)
            {
                height = triZ0;
                height += (triZ1 - triZ0) * sqX;
                height += (triZ2 - triZ0) * sqZ;
            }
            else
            {
                height = triZ3;
                height += (triZ1 - triZ3) * (1.0f - sqZ);
                height += (triZ2 - triZ3) * (1.0f - sqX);
            }
            return height;
        }

        /// <summary>
        /// Checks if a position is above terrain.
        /// </summary>
        /// <param name="xPos">X Coordinate of position</param>
        /// <param name="zPos">Z Coordinate of position</param>
        /// <returns>True if position is above terrain.</returns>
        public bool IsAboveTerrain(float xPos, float zPos)
        {
            // Keep object from going off the edge of the map
            if((xPos / this.scaleFactor) > size)
            {
                return false;
            }
            else if((xPos / this.scaleFactor) < 0)
            {
                return false;
            }

            // Keep object from going off the edge of the map
            if((zPos / this.scaleFactor) > size)
            {
                return false;
            }
            else if((zPos / this.scaleFactor) < 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the normal of a position on the heightmap.
        /// </summary>
        /// <param name="xPos">X position on the map</param>
        /// <param name="zPos">Z position on the map</param>
        /// <returns>Normal vector of this spot on the terrain</returns>
        public Vector3 GetNormal(float xPos, float zPos)
        {
            int x = (int)(xPos / this.scaleFactor);

            if(x > this.size - 2)
            {
                x = this.size - 2;
            }
            // if it is outside the heightmap.
            else if (x < 0)
            {
                x = 0;
            }

            int z = (int)(zPos / this.scaleFactor);

            if(z > this.size - 2)
            {
                z = this.size - 2;
            }
            else if (z < 0)
            {
                z = 0;
            }

            Vector3 triZ0 = (this.normals[x, z]);
            Vector3 triZ1 = (this.normals[x + 1, z]);
            Vector3 triZ2 = (this.normals[x, z + 1]);
            Vector3 triZ3 = (this.normals[x + 1, z + 1]);

            Vector3 avgNormal;
            float sqX = (xPos / this.scaleFactor) - x;
            float sqZ = (zPos / this.scaleFactor) - z;
            if ((sqX + sqZ) < 1)
            {
                avgNormal = triZ0;
                avgNormal += (triZ1 - triZ0) * sqX;
                avgNormal += (triZ2 - triZ0) * sqZ;
            }
            else
            {
                avgNormal = triZ3;
                avgNormal += (triZ1 - triZ3) * (1.0f - sqZ);
                avgNormal += (triZ2 - triZ3) * (1.0f - sqX);
            }
            return avgNormal;
        }

        /// <summary>
        /// Sets the minimum leaf size for quad-tree patches. Must be a power of two value.
        /// </summary>
        /// <param name="Width">Minimum leaf size width (also sets height to match)</param>
        public void SetLeafSize(int width)
        {
            this.MinLeafSize = (width * width);
        }

        /// <summary>
        /// Sets the <see cref="TerrainDetail"/> level
        /// </summary>
        /// <param name="detailLevel">Detail level setting</param>
        public void SetDetailLevel(LOD detailLevel)
        {
            this.detail = detailLevel;
        }

        /// <summary>
        /// Recursively sets up all LODs lookups for all quad-tree leaves.
        /// </summary>
        public void SetupLODS()
        {
            this.RootQuadTree.SetupLODs();
        }

        /// <summary>
        /// Adds a new level of detail to all quad-tree leaves.
        /// </summary>
        /// <param name="detailLevel">Detail level to add to all terrain patches in the Terrain</param>
        public void AddNew(LOD detailLevel)
        {
            this.RootQuadTree.AddNewPatchLOD(detailLevel);
        }
    }
}
