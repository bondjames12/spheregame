using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XEngine
{
    public class XHeightMap : XComponent, XDrawable
    {
        VertexBuffer terrainVertexBuffer;
        IndexBuffer terrainIndexBuffer;

        Effect effect;

        int WIDTH;
        int HEIGHT;

        HeightmapObject Object;
        public HeightMapInfo Heights;

        public int environmentalParametersNumber;
        XEnvironmentParameters parameters;
        public XEnvironmentParameters Params
        {
            get { return parameters; }
            set { parameters = value; environmentalParametersNumber = value.number; }
        }

        Matrix World;

        public string TextureMapFile;
        public string RTextureFile;
        public string GTextureFile;
        public string BTextureFile;
        public string HeightMapFile;

        public BoundingBox boundingBox = new BoundingBox();

        public XHeightMap(XMain X, string HeightMap, XEnvironmentParameters Params, string RTexture, string GTexture, string BTexture, string TextureMap) : base(X) 
        {
            if (Params != null)
                this.Params = Params;

            HeightMapFile = HeightMap;
            TextureMapFile = TextureMap;
            RTextureFile = RTexture;
            GTextureFile = GTexture;
            BTextureFile = BTexture;
            DrawOrder = 21;
        }

        public override void Load(ContentManager Content)
        {
            Heights = HeightMapInfo.GenerateFromHeightmap(X, X.Content.Load<Texture2D>(HeightMapFile), 1.0f);

            effect = Content.Load<Effect>(@"Content\XEngine\Effects\Terrain");

            World = Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateScale(new Vector3(-1, 1, 1));

            effect.Parameters["WorldIT"].SetValue(Matrix.Transpose(Matrix.Invert(World)));
            effect.Parameters["World"].SetValue(World);

            effect.Parameters["SkyTextureNight"].SetValue(Params.Night);
            effect.Parameters["SkyTextureSunset"].SetValue(Params.Sunset);
            effect.Parameters["SkyTextureDay"].SetValue(Params.Day);

            effect.Parameters["isSkydome"].SetValue(false);

            effect.Parameters["LightDirection"].SetValue(-Params.LightDirection);
            effect.Parameters["LightColor"].SetValue(Params.LightColor);
            effect.Parameters["LightColorAmbient"].SetValue(Params.LightColorAmbient);
            effect.Parameters["FogColor"].SetValue(Params.FogColor);
            effect.Parameters["fDensity"].SetValue(Params.FogDensity);
            effect.Parameters["SunLightness"].SetValue(Params.SunLightness);
            effect.Parameters["sunRadiusAttenuation"].SetValue(Params.SunRadiusAttenuation);
            effect.Parameters["largeSunLightness"].SetValue(Params.LargeSunLightness);
            effect.Parameters["largeSunRadiusAttenuation"].SetValue(Params.LargeSunRadiusAttenuation);
            effect.Parameters["dayToSunsetSharpness"].SetValue(Params.DayToSunsetSharpness);
            effect.Parameters["hazeTopAltitude"].SetValue(Params.HazeTopAltitude);

            if (!string.IsNullOrEmpty(RTextureFile))
                effect.Parameters["TextureR"].SetValue(Content.Load<Texture2D>(RTextureFile));

            if (!string.IsNullOrEmpty(GTextureFile))
                effect.Parameters["TextureG"].SetValue(Content.Load<Texture2D>(GTextureFile));

            if (!string.IsNullOrEmpty(BTextureFile))
                effect.Parameters["TextureB"].SetValue(Content.Load<Texture2D>(BTextureFile));

            effect.Parameters["TextureMap"].SetValue(Content.Load<Texture2D>(TextureMapFile));

            float[,] heightData = Heights.heights;

            WIDTH = heightData.GetLength(0);
            HEIGHT = heightData.GetLength(1);

            VertexPositionNormalTexture[] terrainVertices = new VertexPositionNormalTexture[WIDTH * HEIGHT];

            float maxheight = 0;

            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    terrainVertices[x + y * WIDTH].Position = new Vector3(x - WIDTH / 2, y - WIDTH / 2, heightData[x, y]);
                    terrainVertices[x + y * WIDTH].Normal = new Vector3(0, 0, 1);

                    terrainVertices[x + y * WIDTH].TextureCoordinate.X = (float)x / WIDTH;
                    terrainVertices[x + y * WIDTH].TextureCoordinate.Y = (float)y / HEIGHT;

                    maxheight = maxheight < heightData[x, y] ? heightData[x, y] : maxheight;
                }

            for (int x = 1; x < WIDTH - 1; x++)
            {
                for (int y = 1; y < HEIGHT - 1; y++)
                {
                    Vector3 normX = new Vector3((terrainVertices[x - 1 + y * WIDTH].Position.Z - terrainVertices[x + 1 + y * WIDTH].Position.Z) / 2, 0, 1);
                    Vector3 normY = new Vector3(0, (terrainVertices[x + (y - 1) * WIDTH].Position.Z - terrainVertices[x + (y + 1) * WIDTH].Position.Z) / 2, 1);
                    terrainVertices[x + y * WIDTH].Normal = normX + normY;
                    terrainVertices[x + y * WIDTH].Normal.Normalize();
                }
            }

            terrainVertexBuffer = new VertexBuffer(X.GraphicsDevice, VertexPositionNormalTexture.SizeInBytes * WIDTH * HEIGHT, BufferUsage.WriteOnly);
            terrainVertexBuffer.SetData(terrainVertices);

            int[] terrainIndices = new int[(WIDTH - 1) * (HEIGHT - 1) * 6];
            for (int x = 0; x < WIDTH - 1; x++)
            {
                for (int y = 0; y < HEIGHT - 1; y++)
                {
                    terrainIndices[(x + y * (WIDTH - 1)) * 6] = (x + 1) + (y + 1) * WIDTH;
                    terrainIndices[(x + y * (WIDTH - 1)) * 6 + 1] = (x + 1) + y * WIDTH;
                    terrainIndices[(x + y * (WIDTH - 1)) * 6 + 2] = x + y * WIDTH;

                    terrainIndices[(x + y * (WIDTH - 1)) * 6 + 3] = (x + 1) + (y + 1) * WIDTH;
                    terrainIndices[(x + y * (WIDTH - 1)) * 6 + 4] = x + y * WIDTH;
                    terrainIndices[(x + y * (WIDTH - 1)) * 6 + 5] = x + (y + 1) * WIDTH;
                }
            }

            terrainIndexBuffer = new IndexBuffer(X.GraphicsDevice, typeof(int), (WIDTH - 1) * (HEIGHT - 1) * 6, BufferUsage.WriteOnly);
            terrainIndexBuffer.SetData(terrainIndices);

            //create physics object for collisions!
            Object = new HeightmapObject(Heights, Vector2.Zero);

            boundingBox = new BoundingBox(new Vector3(-(WIDTH / 2), 0, -(HEIGHT / 2)), new Vector3((WIDTH / 2), maxheight, (HEIGHT / 2)));

            base.Load(Content);
        }

        public override void Draw(ref GameTime gameTime, ref XCamera Camera)
        {
            if (loaded)
            {
                X.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;

                effect.Parameters["WorldViewProj"].SetValue(World * Camera.View * Camera.Projection);
                effect.Parameters["ViewInv"].SetValue(Matrix.Invert(Camera.View));
                effect.Parameters["LightDirection"].SetValue(-Params.LightDirection);

                //if rendering a depthmap
                if (Camera.RenderType == RenderTypes.Depth)
                {
                    //override any techniques with DepthMap technique shader
                    if (effect.Techniques["DepthMapStatic"] != null)
                        effect.CurrentTechnique = effect.Techniques["DepthMapStatic"];
                    //continue;
                }
                else
                {
                    if (effect.Techniques["SkyDome"] != null)
                        effect.CurrentTechnique = effect.Techniques["SkyDome"];
                }

                effect.Begin();
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    X.GraphicsDevice.Vertices[0].SetSource(terrainVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
                    X.GraphicsDevice.Indices = terrainIndexBuffer;
                    X.GraphicsDevice.VertexDeclaration = new VertexDeclaration(X.GraphicsDevice, VertexPositionNormalTexture.VertexElements);

                    X.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, WIDTH * HEIGHT, 0, (WIDTH - 1) * (HEIGHT - 1) * 2);

                    pass.End();
                }
                effect.End();

                X.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

                if (DebugMode)
                    X.DebugDrawer.DrawCube(boundingBox.Min, boundingBox.Max, Color.Yellow, World, Camera);
            }
        }
    }
}
