// GraphicsSystem.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using QuickStart.Entities;

namespace QuickStart.Graphics
{
    /// <summary>
    /// The QuickStart graphics system.  Manages all rendering-related tasks, including device configuration.
    /// </summary>
    public class GraphicsSystem
    {
        /// <summary>
        /// Enumeration of all queriable variable matrix types.
        /// </summary>
        public enum VariableMatrixID
        {
            World = 0,
            View,
            Projection,
            WorldView,
            ViewProjection,
            WorldViewProjection,

            Identity,
        }

        /// <summary>
        /// Enumeration of all queriable variable float types.
        /// </summary>
        public enum VariableFloatID
        {
            TotalTime = 0,

            Unity,
        }

        /// <summary>
        /// Enumeration of all queriable variable float4 types.
        /// </summary>
        public enum VariableFloat4ID
        {
            ViewPos = 0,
            ViewForward,

            Unity,
        }

        private GraphicsDeviceManager graphics;

        private GraphicsSettings settings;

        private QSGame game;

        private Matrix[] variableMatrix;
        private float[] variableFloat;
        private Vector4[] variableFloat4;

        private List<RenderChunk> currentChunkList;
        private List<RenderChunk> deadChunkList;

        private List<IRenderChunkProvider> renderChunkProviders;

        private const int numChunksToPreallocate = 30;

        /// <summary>
        /// Initializes a new instance of the graphics system.
        /// </summary>
        /// <param name="gameInstance">The instance of QSGame for the game.</param>
        public GraphicsSystem(QSGame gameInstance)
        {
            this.game = gameInstance;

            this.graphics = new GraphicsDeviceManager(game);
            this.graphics.PreparingDeviceSettings += PreparingDeviceSettings;

            this.variableMatrix = new Matrix[(int)VariableMatrixID.Identity + 1];
            for(int i = 0; i < this.variableMatrix.Length; ++i)
            {
                this.variableMatrix[i] = Matrix.Identity;
            }

            this.variableFloat = new float[(int)VariableFloatID.Unity + 1];
            for(int i = 0; i < this.variableFloat.Length; ++i)
            {
                this.variableFloat[i] = 0.0f;
            }

            this.variableFloat4 = new Vector4[(int)VariableFloat4ID.Unity + 1];
            for(int i = 0; i < this.variableFloat4.Length; ++i)
            {
                this.variableFloat4[i] = Vector4.Zero;
            }

            this.currentChunkList = new List<RenderChunk>(numChunksToPreallocate);
            this.deadChunkList = new List<RenderChunk>();
            for(int i = 0; i < numChunksToPreallocate; ++i)
            {
                RenderChunk chunk = new RenderChunk();
                this.deadChunkList.Add(chunk);
            }


            this.renderChunkProviders = new List<IRenderChunkProvider>();
        }

        /// <summary>
        /// Toggles between fullscreen and windowed mode
        /// </summary>
        public void ToggleFullscreen()
        {
            this.graphics.IsFullScreen = !this.graphics.IsFullScreen;
            this.graphics.ApplyChanges();
        }

        /// <summary>
        /// Recreates the graphics device with the given settings.
        /// </summary>
        /// <param name="newSettings">The settings to use when recreating the device.</param>
        public void ApplySettings(GraphicsSettings newSettings)
        {
            this.settings = newSettings;

            this.graphics.PreferredBackBufferWidth = this.settings.BackBufferWidth;
            this.graphics.PreferredBackBufferHeight = this.settings.BackBufferHeight;

            this.graphics.PreferMultiSampling = this.settings.EnableMSAA;

            this.graphics.IsFullScreen = this.settings.EnableFullScreen;

            this.graphics.SynchronizeWithVerticalRetrace = this.settings.EnableVSync;

            // We have to wait until the PreparingDeviceSettings event to set the MSAA level.

            this.graphics.ApplyChanges();
        }

        /// <summary>
        /// Applies changes based on the user settings to the device descriptor before device creation.
        /// </summary>
        /// <param name="obj">The calling GraphicsDeviceManager instance.</param>
        /// <param name="args">The device creation event arguments.</param>
        private void PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs args)
        {
            args.GraphicsDeviceInformation.PresentationParameters.SwapEffect = SwapEffect.Discard;

            if(this.settings.EnableMSAA)
            {
                // Handle the multi-sampling settings ourselves.  For now, we just select the best possible non-maskable multi-sampling type.  In the future, make this user customizable.
                int levels;
                MultiSampleType msaaType;

#if XBOX360
                msaaType = MultiSampleType.TwoSamples;
#else
                msaaType = MultiSampleType.NonMaskable;
#endif

                if(!args.GraphicsDeviceInformation.Adapter.CheckDeviceMultiSampleType(args.GraphicsDeviceInformation.DeviceType, args.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat, args.GraphicsDeviceInformation.PresentationParameters.IsFullScreen, msaaType, out levels))
                {
                    // TODO: Real logging
                    System.Diagnostics.Debug.WriteLine("[GraphicsSystem]: Hardware does not support non-maskable MSAA.");
                }

                args.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = msaaType;
                args.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = levels - 1;
            }
        }

        /// <summary>
        /// Loads all global content needed by the graphics system.
        /// </summary>
        public void LoadContent()
        {
        }

        /// <summary>
        /// Unloads all previously loaded global content needed by the graphics system.
        /// </summary>
        public void UnloadContent()
        {
        }

        /// <summary>
        /// Retrieves the current value of the specified queriable variable matrix.
        /// </summary>
        /// <param name="matID">The enumerated ID of the variable matrix to retrieve.</param>
        /// <param name="mat">The matrix value.</param>
        public void GetVariableMatrix(VariableMatrixID matID, out Matrix mat)
        {
            mat = this.variableMatrix[(int)matID];
        }

        /// <summary>
        /// Retrieves the current value of the specified queriable variable float.
        /// </summary>
        /// <param name="varID">The enumerated ID of the variable float to retrieve.</param>
        /// <param name="val">The float value.</param>
        public void GetVariableFloat(VariableFloatID varID, out float val)
        {
            val = this.variableFloat[(int)varID];
        }

        /// <summary>
        /// Retrieves the current value of the specified queriable variable float4.
        /// </summary>
        /// <param name="varID">The enuemrated ID of the variable float4 to retrieve.</param>
        /// <param name="val">The float4 value.</param>
        public void GetVariableFloat4(VariableFloat4ID varID, out Vector4 val)
        {
            val = this.variableFloat4[(int)varID];
        }

        /// <summary>
        /// Allocate a new <see cref="RenderChunk"/> and inserts into the render chunk queue.  A recycling scheme is used to minimize garbage creation.
        /// </summary>
        /// <returns>The allocated instance of <see cref="RenderChunk"/></returns>
        public RenderChunk AllocateRenderChunk()
        {
            if(this.deadChunkList.Count > 0)
            {
                RenderChunk chunk = deadChunkList[this.deadChunkList.Count - 1];
                this.deadChunkList.RemoveAt(this.deadChunkList.Count - 1);

                this.currentChunkList.Add(chunk);

                return chunk;
            }
            else
            {
                RenderChunk chunk = new RenderChunk();

                this.currentChunkList.Add(chunk);

                return chunk;
            }
        }

        /// <summary>
        /// Deallocates an unused <see cref="RenderChunk"/> instance.  Use this method if AllocateRenderChunk was called but the returned <see cref="RenderChunk"/>
        /// instance should be ignored and deleted without being processed for rendering.  This is the "undo" method for AllocateRenderChunk().
        /// </summary>
        /// <param name="chunk">The <see cref="RenderChunk"/> to deallocate.</param>
        public void ReleaseRenderChunk(RenderChunk chunk)
        {
            chunk.Recycle();

            this.deadChunkList.Add(chunk);
        }

        /// <summary>
        /// Inserts an IRenderChunkProvider instance into the per-frame processing list.  All registered IRenderChunkProviders
        /// will be queried for every rendering frame every frame.
        /// </summary>
        /// <param name="provider">The IRenderChunkProvider to register.</param>
        public void RegisterRenderChunkProvider(IRenderChunkProvider provider)
        {
            this.renderChunkProviders.Add(provider);
        }

        /// <summary>
        /// Removes an IRenderChunkProvider instance from the per-frame processing list.
        /// </summary>
        /// <param name="provider"></param>
        public void UnRegisterRenderChunkProvider(IRenderChunkProvider provider)
        {
            this.renderChunkProviders.Remove(provider);
        }

        /// <summary>
        /// Draws a single frame.
        /// </summary>
        public void DrawFrame(Camera camera, GameTime gameTime)
        {
            //this.graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            this.graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            this.graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            this.graphics.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            RenderPassDesc passDesc = new RenderPassDesc();
            passDesc.RenderCamera = camera;

            // Process initial view
            for(int i = 0; i < this.renderChunkProviders.Count; ++i)
            {
                this.renderChunkProviders[i].QueryForRenderChunks(ref passDesc);
            }




            // @todo: Implement real rendering algorithm

            this.variableMatrix[(int)VariableMatrixID.View] = camera.viewMatrix;
            this.variableMatrix[(int)VariableMatrixID.Projection] = camera.projectionMatrix;
            Matrix.Multiply(ref this.variableMatrix[(int)VariableMatrixID.View], ref this.variableMatrix[(int)VariableMatrixID.Projection], out this.variableMatrix[(int)VariableMatrixID.ViewProjection]);

            this.variableFloat[(int)VariableFloatID.TotalTime] = (float)gameTime.TotalGameTime.Ticks / (float)TimeSpan.TicksPerSecond;

            // @todo: Assign this
            this.variableFloat4[(int)VariableFloat4ID.ViewPos] = Vector4.Zero;

            this.variableFloat4[(int)VariableFloat4ID.ViewForward] = new Vector4(camera.viewMatrix.Forward, 0.0f);

            //graphics.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

            ProcessRenderChunks(this.currentChunkList);

            //graphics.GraphicsDevice.RenderState.FillMode = FillMode.Solid;




            for(int i = 0; i < this.currentChunkList.Count; ++i)
            {
                this.currentChunkList[i].Recycle();
                this.deadChunkList.Add(currentChunkList[i]);
            }

            this.currentChunkList.Clear();
        }

        /// <summary>
        /// Processes a <see cref="RenderChunk"/> list.
        /// </summary>
        /// <param name="chunkList">The <see cref="RenderChunk"/> list to process.</param>
        private void ProcessRenderChunks(List<RenderChunk> chunkList)
        {
            for(int i = 0; i < chunkList.Count; ++i)
            {
                RenderChunk chunk = chunkList[i];

                this.variableMatrix[(int)VariableMatrixID.World] = chunk.WorldTransform;
                Matrix.Multiply(ref this.variableMatrix[(int)VariableMatrixID.World], ref this.variableMatrix[(int)VariableMatrixID.View], out this.variableMatrix[(int)VariableMatrixID.WorldView]);
                Matrix.Multiply(ref this.variableMatrix[(int)VariableMatrixID.WorldView], ref this.variableMatrix[(int)VariableMatrixID.Projection], out this.variableMatrix[(int)VariableMatrixID.WorldViewProjection]);

                for(int j = 0; j < chunk.VertexStreams.Count; ++j)
                {
                    this.graphics.GraphicsDevice.Vertices[j].SetSource(chunk.VertexStreams[j], 0, chunk.Declaration.GetVertexStrideSize(j));
                }

                this.graphics.GraphicsDevice.Indices = chunk.Indices;
                this.graphics.GraphicsDevice.VertexDeclaration = chunk.Declaration;

                int passes = chunk.Material.BindMaterial(this);
                for(int j = 0; j < passes; ++j)
                {
                    chunk.Material.BeginPass(j);

                    this.graphics.GraphicsDevice.DrawIndexedPrimitives(chunk.Type, chunk.VertexStreamOffset, 0, chunk.VertexCount, chunk.StartIndex, chunk.PrimitiveCount);

                    chunk.Material.EndPass();
                }
                chunk.Material.UnBindMaterial();
            }
        }
    }
}
