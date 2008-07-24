/*
 * SceneManager.cs
 * 
 * This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
 * for license details.
 */

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using QuickStart.Interfaces;
using QuickStart.Entities;
using QuickStart.Graphics;
using QuickStart.Compositor;

namespace QuickStart
{
    /// <summary>
    /// The scene manager will eventually be the "brains" of the game. It will update and run
    /// each of the interfaces and store some core information (TBD).
    /// </summary>
    public class SceneManager : IRenderChunkProvider, IScreen
    {
        /// <summary>
        /// Exposes <see cref="QSGame"/> to the <see cref="SceneManager"/>
        /// </summary>
        private QSGame game;
        
        /// <summary>
        /// All interfaces controlled by the <see cref="SceneManager"/> are run through this list
        /// </summary>
        private List<QSInterface> interfaces;

        /// <summary>
        /// Holds all of the scenes in memory.
        /// </summary>
        public List<Scene> Scenes
        {
            get { return scenes; }
        }
        private List<Scene> scenes; 
        
        /// <summary>
        /// Give the <see cref="SceneManager"/> a <see cref="CameraInterface"/>
        /// </summary>
        /// <remarks>This accessor is temporary until we get messaging running for the camera</remarks>
        public CameraInterface CameraInterface
        {
            get { return cameraInterface; }
        }
        private CameraInterface cameraInterface;

        /// <summary>
        /// A root entity for the scene manager to contain things like cameras.
        /// </summary>
        /// <remarks>@todo: This may be TEMPORARY. Still testing out the idea of a root entity for the camera system.</remarks>
        public StaticEntity SceneMgrRootEntity
        {
            get { return sceneMgrRootEntity; }
        }
        private StaticEntity sceneMgrRootEntity;

        private ContentManager content;

        public bool NeedBackgroundAsTexture
        {
            get { return false; }
        }

        public string Name
        {
            get { return name; }
        }

        private const string name = "SceneManager";

        /// <summary>
        /// Creates and initializes a scene manager.
        /// </summary>
        /// <param name="game">Base engine game class</param>
        public SceneManager(QSGame game)
        {
            this.game = game;

            this.interfaces = new List<QSInterface>();
            this.scenes = new List<Scene>();

            this.cameraInterface = new CameraInterface(this);
            AddInterface(cameraInterface);

            this.sceneMgrRootEntity = new StaticEntity(game);

            // Create a free camera for the scene manager to use
            FreeCamera sceneMgrCamera = new FreeCamera(game, 60.0f, QSConstants.ScreenWidth, QSConstants.ScreenHeight, 0.5f, 2000.0f);

            sceneMgrCamera.Position = new Vector3(600.0f, 400.0f, 600.0f);
            sceneMgrCamera.LookAt(new Vector3(900.0f, 400.0f, 900.0f));

            //sceneMgrCamera.ZoomIn();      // Zoom to 2x
            //sceneMgrCamera.ZoomIn();      // Zoom to 4x
            //sceneMgrCamera.ZoomOut();     // Test zoom out

            // Create a fixed camera for the scene manager to use 
            // (testing only, comment the free camera and position set above if you use this)
            //FixedCamera sceneMgrCamera = new FixedCamera(game, 60.0f, QSConstants.ScreenWidth, QSConstants.ScreenHeight, 0.5f, 1000.0f);            

            // Create an arc ball camera for the scene manager to use 
            // (testing only, comment the free camera and position set above if you use this)
            //ArcBallCamera sceneMgrCamera = new ArcBallCamera(game, 60.0f, QSConstants.ScreenWidth, QSConstants.ScreenHeight, 0.5f, 1000.0f);  

            this.cameraInterface.AttachCamera(sceneMgrRootEntity, sceneMgrCamera);
            this.cameraInterface.SetRenderCamera(sceneMgrCamera);
        }        

        /// <summary>
        /// Updates each scene, which allows each scene to run.
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public void Update(GameTime gameTime)
        {
            // Update all interfaces
            for (int i = 0; i < this.interfaces.Count; i++)
            {
                this.interfaces[i].Update(gameTime);
            }

            // Update each scene
            for (int i = 0; i < this.scenes.Count; i++)
            {
                if (this.scenes[i].Active)
                {
                    this.scenes[i].Update(gameTime, cameraInterface.RenderCamera);
                }
            }

            // @todo: Turn these into messages once the message system is ready. - N.Foster
            //cameraInterface.RenderCamera.MoveFrontBack(0.006f * gameTime.ElapsedGameTime.Milliseconds);  // Tests forward movement, negative for backward
            //cameraInterface.RenderCamera.MoveUpDown(0.006f * gameTime.ElapsedGameTime.Milliseconds);       // Tests upward movement, negative for downward
            //cameraInterface.RenderCamera.Strafe(0.006f * gameTime.ElapsedGameTime.Milliseconds);           // Tests right movement, negative for left
            //cameraInterface.RenderCamera.PitchCamera(0.0001f * gameTime.ElapsedGameTime.Milliseconds);   // Tests pitch
            //cameraInterface.RenderCamera.YawCamera(0.0001f * gameTime.ElapsedGameTime.Milliseconds);     // Tests yaw (still seems strange at sharp angles to a model)

            //cameraInterface.RenderCamera.LookAtAttached();    // Tests a simple camera lock-on
        }

        /// <summary>
        /// Draws each scene.
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        /// <remarks>@todo: Currently only tells the graphics system to draw the render queue.</remarks>
        public void Draw(GameTime gameTime)
        {

        }

        /// <summary>
        /// Queries for all potentially visible RenderChunk instances given a RenderPassDesc descriptor.
        /// </summary>
        /// <param name="desc">A descriptor for the current rendering pass.</param>
        public void QueryForRenderChunks(ref RenderPassDesc desc)
        {
            for(int i = 0; i < scenes.Count; ++i)
            {
                this.scenes[i].QueryForRenderChunks(ref desc);
            }
        }

        /// <summary>
        /// Draws the scene for the compositor.
        /// </summary>
        /// <param name="batch">The <see cref="SpriteBatch"/> instance to use for 2D rendering.</param>
        /// <param name="background">The previous screen's output as a texture, if NeedBackgroundAsTexture is true.  Null, otherwise.</param>
        /// <param name="gameTime">The <see cref="GameTime"/> structure for the current Game.Draw() cycle.</param>
        public void DrawScreen(SpriteBatch batch, Texture2D background, GameTime gameTime)
        {
            this.game.Graphics.DrawFrame(this.cameraInterface.RenderCamera, gameTime);
        }

        /// <summary>
        /// Adds a <see cref="QSInterface"/> to the <see cref="SceneManager"/>.
        /// </summary>
        /// <param name="inQSInterface"><see cref="QSInterface"/> to add.</param>
        private void AddInterface(QSInterface inQSInterface)
        {
            this.interfaces.Add(inQSInterface);
        }

        /// <summary>
        /// Adds a <see cref="Scene"/> to the <see cref="SceneManager"/>.
        /// </summary>
        /// <param name="inScene">Scene to add</param>
        private void AddScene(Scene inScene)
        {
            // @todo: Should we check here if scene is finished loading?
            this.scenes.Add(inScene);
        }

        /// <summary>
        /// Buffer and load a <see cref="Scene"/> into memory.
        /// </summary>
        /// <param name="targetScene"><see cref="Scene"/> to load</param>
        /// <param name="autoBegin">Begin a <see cref="Scene"/> once loaded</param>
        public void LoadScene( Scene targetScene, bool autoBegin )
        {
            // We need a scene loader, which should probably be its own class.
            // Scene loading should be buffered, preferably in its own thread.
            
            // If a scene is to be buffered in, would we ever want to begin before all of it
            // is loaded?

            // Should there be an order to what is loaded? For instance, I would say sky and terrain
            // should load before entities.

            // *** TEMP ***
            // Currently this method doesn't buffer anything, it simple adds a given scene
            // to a list. Eventually the scene should be loaded from an XML file. XML file can contain
            // all pertainant info about the scene, like which models to load, where to place them, what
            // kind of sky dome, what strength of gravity, etc. The loader will create a scene out of this
            // and then start it if needed.

            AddScene(targetScene);

            if (autoBegin)
            {
                BeginScene(targetScene);
            }
        }

        /// <summary>
        /// Unload a <see cref="Scene"/> from memory.
        /// </summary>
        /// <param name="targetScene">Scene to unload</param>
        public void UnloadScene(Scene targetScene)
        {
            // We need a scene unload, which should probably be in a separate class.
            // Could be combined with the scene loader. Scene unloading doesn't need
            // to be buffered, but should still be done in its own scene.
            
            // Scene should not be able to unload unless stopped first.

            // Any other conditions required for unloading?
            StopScene(targetScene);

            this.scenes.Remove(targetScene);
        }

        /// <summary>
        /// Start running a new <see cref="Scene"/>. After beginning a scene it will continue to run
        /// until stopped. Scene must be loaded before it can begin.
        /// </summary>
        /// <param name="targetScene"><see cref="Scene"/> to run.</param>
        public void BeginScene(Scene targetScene)
        {
            targetScene.Activate();
        }

        /// <summary>
        /// Stops <see cref="Scene"/> from being processes. This step must be done before unloading a scene.
        /// </summary>
        /// <param name="targetScene"><see cref="Scene"/> to stop.</param>
        public void StopScene(Scene targetScene)
        {
            targetScene.Deactiviate();
        }

        /// <summary>
        /// Loads all needed ContentManager content.
        /// </summary>
        public void LoadContent(ContentManager contentManager)
        {
            this.content = contentManager;
            this.content.RootDirectory = "Content";

            // @todo:  Give each scene a separate ContentManager
            for (int i = 0; i < scenes.Count; ++i)
            {
                this.scenes[i].LoadContent();
            }
        }

        /// <summary>
        /// Unloads all ContentManager content.
        /// </summary>
        public void UnloadContent()
        {
            for(int i = 0; i < scenes.Count; ++i)
            {
                this.scenes[i].UnloadContent();
            }
        }
    }
}
