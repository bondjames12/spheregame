using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using XEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace XEngine
{
    public class XWaterFast : XComponent, XIDrawable, XIUpdateable
    {
        public struct VertexMultitextured
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TextureCoordinate;
            public Vector3 Tangent;
            public Vector3 BiNormal;

            public static int SizeInBytes = (3 + 3 + 2 + 3 + 3) * 4;
            public static VertexElement[] VertexElements = new VertexElement[]
             {
                 new VertexElement( 0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0 ),
                 new VertexElement( 0, sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0 ),
                 new VertexElement( 0, sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0 ),
                 new VertexElement( 0, sizeof(float) * 8, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Tangent, 0 ),
                 new VertexElement( 0, sizeof(float) * 11, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Binormal, 0 ),
             };

        }

        #region Fields
        private VertexBuffer vb;
        private IndexBuffer ib;
        VertexMultitextured[] myVertices;
        private int myHeight = 128;
        private int myWidth = 128;

        private Vector3 myPosition;
        private Vector3 myScale;
        private Quaternion myRotation;

        Effect effect;

        public Vector3 basePosition;
        private string EnvAsset;
        float bumpHeight = 0.5f;
        Vector2 textureScale = new Vector2(4, 4);
        Vector2 bumpSpeed = new Vector2(0, .003f);
        float fresnelBias = .025f;
        float fresnelPower = 1.0f;
        float hdrMultiplier = 1.0f;
        Color deepWaterColor = Color.Blue;
        Color shallowWaterColor = Color.SkyBlue;
        Color reflectionColor = Color.LightBlue;
        float reflectionAmount = 0.7f;
        float waterAmount = 0.0f;
        float waveAmplitude = 0.1f;
        float waveFrequency = 1.2f;
        #endregion

        #region Properties
        public Vector3 Position
        {
            get { return basePosition; }
            set { basePosition = value; }
        }

        /// <summary>
        /// Height of water bump texture.
        /// Min 0.0 Max 2.0 Default = .5
        /// </summary>
        public float BumpHeight
        {
            get { return bumpHeight; }
            set { bumpHeight = value; }
        }
        /// <summary>
        /// Scale of bump texture.
        /// </summary>
        public Vector2 TextureScale
        {
            get { return textureScale; }
            set { textureScale = value; }
        }
        /// <summary>
        /// Velocity of water flow
        /// </summary>
        public Vector2 BumpSpeed
        {
            get { return bumpSpeed; }
            set { bumpSpeed = value; }
        }
        /// <summary>
        /// Min 0.0 Max 1.0 Default = .025
        /// </summary>
        public float FresnelBias
        {
            get { return fresnelBias; }
            set { fresnelBias = value; }
        }
        /// <summary>
        /// Min 0.0 Max 10.0 Default = 1.0;
        /// </summary>
        public float FresnelPower
        {
            get { return fresnelPower; }
            set { fresnelPower = value; }
        }
        /// <summary>
        /// Min = 0.0 Max = 100 Default = 1.0
        /// </summary>
        public float HDRMultiplier
        {
            get{return hdrMultiplier;}
            set { hdrMultiplier = value; }
        }
        /// <summary>
        /// Color of deep water Default = Black;
        /// </summary>
        public Color DeepWaterColor
        {
            get { return deepWaterColor; }
            set { deepWaterColor = value; }
        }
        /// <summary>
        /// Color of shallow water Default = SkyBlue
        /// </summary>
        public Color ShallowWaterColor
        {
            get { return shallowWaterColor; }
            set { shallowWaterColor = value; }
        }
        /// <summary>
        /// Default = White
        /// </summary>
        public Color ReflectionColor
        {
            get { return reflectionColor; }
            set { reflectionColor = value; }
        }
        /// <summary>
        /// Min = 0.0 Max = 2.0 Default = .5
        /// </summary>
        public float ReflectionAmount
        {
            get { return reflectionAmount; }
            set { reflectionAmount = value; }
        }
        /// <summary>
        /// Amount of water color to use.
        /// Min = 0 Max = 2 Default = 0;
        /// </summary>
        public float WaterAmount
        {
            get { return waterAmount; }
            set { waterAmount = value; }
        }
        /// <summary>
        /// Min = 0.0 Max = 10 Defatult = 0.5
        /// </summary>
        public float WaveAmplitude
        {
            get { return waveAmplitude; }
            set { waveAmplitude = value; }
        }
        /// <summary>
        /// Min = 0 Max = 1 Default .1
        /// </summary>
        public float WaveFrequency
        {
            get { return waveFrequency; }
            set { waveFrequency = value; }
        }

        /// <summary>
        /// Default 128
        /// </summary>
        public int Height
        {
            get { return myHeight; }
            set { myHeight = value; }
        }
        /// <summary>
        /// Default 128
        /// </summary>
        public int Width
        {
            get { return myWidth; }
            set { myWidth = value; }
        }
        #endregion

        public XWaterFast(ref XMain X, string EnvironmentMap)
            : base(ref X)
        {
            myWidth = 256;
            myHeight = 256;

            myPosition = new Vector3(0, 0, 0);
            myScale = new Vector3(10, 1, 10); //Vector3.One;
            myRotation = new Quaternion(0, 0, 0, 1);

            EnvAsset = EnvironmentMap;
        }

        public void SetDefault()
        {
            bumpHeight = 0.5f;
            textureScale = new Vector2(4, 4);
            bumpSpeed = new Vector2(0, .05f);
            fresnelBias = .025f;
            fresnelPower = 1.0f;
            hdrMultiplier = 1.0f;
            deepWaterColor = Color.Black;
            shallowWaterColor = Color.SkyBlue;
            reflectionColor = Color.White;
            reflectionAmount = 0.5f;
            waterAmount = 0f;
            waveAmplitude = 0.5f;
            waveFrequency = 0.1f;
        }
 
        public override void Load(ContentManager Content)
        {
            effect = Content.Load<Effect>(@"Content\XEngine\Effects\WaterFast");

            if (!string.IsNullOrEmpty(EnvAsset))//won't work without this but avoid this error anyway
                effect.Parameters["tEnvMap"].SetValue(X.Content.Load<TextureCube>(EnvAsset));
            effect.Parameters["tNormalMap"].SetValue(X.Content.Load<Texture2D>(@"Content\XEngine\Textures\WaterBumpMap2"));
            
            myPosition = new Vector3(basePosition.X - (myWidth / 2), basePosition.Y, basePosition.Z - (myHeight / 2));

            // Vertices
            myVertices = new VertexMultitextured[myWidth * myHeight];

            for (int x = 0; x < myWidth; x++)
                for (int y = 0; y < myHeight; y++)
                {
                    myVertices[x + y * myWidth].Position = new Vector3(y, 0, x);
                    myVertices[x + y * myWidth].Normal = new Vector3(0, -1, 0);
                    myVertices[x + y * myWidth].TextureCoordinate.X = (float)x / 30.0f;
                    myVertices[x + y * myWidth].TextureCoordinate.Y = (float)y / 30.0f;
                }

 	        // Calc Tangent and Bi Normals.
            for (int x = 0; x < myWidth; x++)
                for (int y = 0; y < myHeight; y++)
                {
                    // Tangent Data.
                    if (x != 0 && x < myWidth - 1)
                        myVertices[x + y * myWidth].Tangent = myVertices[x - 1 + y * myWidth].Position - myVertices[x + 1 + y * myWidth].Position;
                    else
                        if (x == 0)
                            myVertices[x + y * myWidth].Tangent = myVertices[x + y * myWidth].Position - myVertices[x + 1 + y * myWidth].Position;
                        else
                            myVertices[x + y * myWidth].Tangent = myVertices[x - 1 + y * myWidth].Position - myVertices[x + y * myWidth].Position;

                    // Bi Normal Data.
                    if (y != 0 && y < myHeight - 1)
                        myVertices[x + y * myWidth].BiNormal = myVertices[x + (y - 1) * myWidth].Position - myVertices[x + (y + 1) * myWidth].Position;
                    else
                        if (y == 0)
                            myVertices[x + y * myWidth].BiNormal = myVertices[x + y * myWidth].Position - myVertices[x + (y + 1) * myWidth].Position;
                        else
                            myVertices[x + y * myWidth].BiNormal = myVertices[x + (y - 1) * myWidth].Position - myVertices[x + y * myWidth].Position;
                }


            vb = new VertexBuffer(X.GraphicsDevice, VertexMultitextured.SizeInBytes * myWidth * myHeight,BufferUsage.WriteOnly);
            vb.SetData(myVertices);

           short[] terrainIndices = new short[(myWidth - 1) * (myHeight - 1) * 6];
            for (short x = 0; x < myWidth - 1; x++)
            {
                for (short y = 0; y < myHeight - 1; y++)
                {
                    terrainIndices[(x + y * (myWidth - 1)) * 6] = (short)((x + 1) + (y + 1) * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 1] = (short)((x + 1) + y * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 2] = (short)(x + y * myWidth);

                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 3] = (short)((x + 1) + (y + 1) * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 4] = (short)(x + y * myWidth);
                    terrainIndices[(x + y * (myWidth - 1)) * 6 + 5] = (short)(x + (y + 1) * myWidth);
                }
            }

            ib = new IndexBuffer(X.GraphicsDevice, typeof(short), (myWidth - 1) * (myHeight - 1) * 6,BufferUsage.WriteOnly);
            ib.SetData(terrainIndices);

            X.GraphicsDevice.VertexDeclaration = new VertexDeclaration(X.GraphicsDevice, VertexMultitextured.VertexElements);

            base.Load(Content);
        }

        public override void Draw(ref GameTime gameTime, ref XCamera Camera)
        {
            Matrix World = 
                            Matrix.CreateFromQuaternion(myRotation) *
                            Matrix.CreateTranslation(myPosition) * Matrix.CreateScale(myScale);

            Matrix WVP = World * Camera.View * Camera.Projection;
            Matrix WV = World * Camera.View;
            Matrix viewI = Matrix.Invert(Camera.View);

            effect.Parameters["matWorldViewProj"].SetValue(WVP);
            effect.Parameters["matWorld"].SetValue(World);
            effect.Parameters["matWorldView"].SetValue(WV);
            effect.Parameters["matViewI"].SetValue(viewI);

            effect.Parameters["fBumpHeight"].SetValue(bumpHeight);
            effect.Parameters["vTextureScale"].SetValue(textureScale);
            effect.Parameters["vBumpSpeed"].SetValue(bumpSpeed);
            effect.Parameters["fFresnelBias"].SetValue(fresnelBias);
            effect.Parameters["fFresnelPower"].SetValue(fresnelPower);
            effect.Parameters["fHDRMultiplier"].SetValue(hdrMultiplier);
            effect.Parameters["vDeepColor"].SetValue(deepWaterColor.ToVector4());
            effect.Parameters["vShallowColor"].SetValue(shallowWaterColor.ToVector4());
            effect.Parameters["vReflectionColor"].SetValue(reflectionColor.ToVector4());
            effect.Parameters["fReflectionAmount"].SetValue(reflectionAmount);
            effect.Parameters["fWaterAmount"].SetValue(waterAmount);
            effect.Parameters["fWaveAmp"].SetValue(waveAmplitude);
            effect.Parameters["fWaveFreq"].SetValue(waveFrequency);

            X.GraphicsDevice.Vertices[0].SetSource(vb, 0, VertexMultitextured.SizeInBytes);
            X.GraphicsDevice.Indices = ib;

            //Game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            
            effect.Begin(SaveStateMode.SaveState);
            for (int p = 0; p < effect.CurrentTechnique.Passes.Count; p++)
            {
                effect.CurrentTechnique.Passes[p].Begin();

                X.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, myWidth * myHeight, 0, (myWidth - 1) * (myHeight - 1) * 2);

                effect.CurrentTechnique.Passes[p].End();
            }
            effect.End();

            //Game.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
        }

        public override void Update(ref GameTime gameTime)
        {
            effect.Parameters["fTime"].SetValue((float)gameTime.TotalRealTime.TotalSeconds);
        }
    }//end class
}//end namespace
