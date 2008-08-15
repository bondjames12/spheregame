using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XEngine;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Sphere
{
    /// <summary>
    /// This is the Main Type for your Game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        XMain X;

        public MenuManager menus;
        public InputProcessor input;
        public XResourceGroup resources;

        public XFreeLookCamera freeCamera;
        public XFreeLookCamera driverCamera;
        public XChaseCamera chase;
        public XCamera currentCamera;

        public XModel model;

        public XHeightMap heightmap;
        public XDynamicSky sky;
        public XWater water;
        public XTreeSystem trees;
        
        public List<XActor> boxes = new List<XActor>();

        public XModel Chassis;
        public XModel Wheel;
        public XCar Car;

        public Fire fire;

        

        public XModel plane;
        public XActor planeActor;

        public XModel housemodel;
        public XAnimatedActor houseactor;

        public Game()
        {
            //Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);

            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferredDepthStencilFormat = SelectStencilMode();
            //graphics.PreferMultiSampling = true;

            // use this for 720P
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
 
            // for NTSC, use a 4:3 ratio
            //graphics.PreferredBackBufferWidth = 720;
            //graphics.PreferredBackBufferHeight = 480;
        }


        private DepthFormat SelectStencilMode()
        {
            // Check stencil formats
            GraphicsAdapter adapter = GraphicsAdapter.DefaultAdapter;
            SurfaceFormat format = adapter.CurrentDisplayMode.Format;
            if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth24Stencil8))
                return DepthFormat.Depth24Stencil8;
            else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth24Stencil8Single))
                return DepthFormat.Depth24Stencil8Single;
            else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth24Stencil4))
                return DepthFormat.Depth24Stencil4;
            else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth15Stencil1))
                return DepthFormat.Depth15Stencil1;
            else
                throw new ApplicationException("Could Not Find Stencil Buffer for Default Adapter");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            X = new XMain(graphics.GraphicsDevice, Services);
            X.Gravity = new Vector3(0, -40, 0);
            X.FrameRate.DisplayFrameRate = true;
            X.Console.AutoDraw = false;
            X.Debug.StartPosition.Y = 200;

            resources = new XResourceGroup(X);
            input = new InputProcessor(X, this);
            menus = new MenuManager(X);
            //menu to the debugnodraw list
            X.Renderer.DebugNoDraw.Add(menus);

            //Make some Cameras
            freeCamera = new XFreeLookCamera(X, .01f, 1000f);
            freeCamera.Position = new Vector3(0, 10, 50);
            driverCamera = new XFreeLookCamera(X, 0.01f, 1000f);
            chase = new XChaseCamera(X, .01f, 1000f);
            currentCamera = chase;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {  
            //Load engine base content
            X.LoadContent();
            
            //Game base elements
            menus.Load(Content);


            // load scene objects/content
            sky = new XDynamicSky(X, X.Environment);
            heightmap = new XHeightMap(X, @"Content\Images\Heightmaps\Level1", X.Environment, @"Content\Textures\Grass", @"Content\Textures\Sand", null, @"Content\Images\Terrainmaps\Level1");
 
            //resources.AddComponent(environment);
            resources.AddComponent(heightmap);
            resources.AddComponent(sky);

            model = new XModel(X, @"Content\Models\barrel_low");
            resources.AddComponent(model);

            Chassis = new XModel(X, @"Content\Models\chassis");
            Wheel = new XModel(X, @"Content\Models\wheel");

            resources.AddComponent(Chassis);
            resources.AddComponent(Wheel);

            housemodel = new XModel(X, @"Content\Models\barrel_low");
            resources.AddComponent(housemodel);

            Car = new XCar(X, Chassis, Wheel, true, true, 30.0f, 5.0f, 4.7f, 5.0f, 0.20f, 0.4f, 0.05f, 0.45f, 0.3f, 1, 520.0f, Math.Abs(X.Gravity.Y), new Vector3(-10, 3, 0));
            resources.AddComponent(Car);

            resources.Load();

            trees = new XTreeSystem(X, @"Content\Images\Treemaps\Level1", heightmap.Heights);
            trees.Load(Content);
            trees.GenerateTrees(freeCamera);

            input.Load();
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // If enabling this option does NOT reduce frame rate,
            // we must be GPU bound
            //System.Threading.Thread.Sleep(1);

            
            //Call engine update
            X.Update(gameTime);
            
            //Input processor update KB,Mouse,Gamepad
            input.Update(gameTime);

            if (Car != null)
            {
                chase.ChaseTargetPosition = Car.Position;
                chase.ChaseTargetForward = Car.Orientation.Forward;
                chase.Up = Car.Orientation.Up;
            }

            if (currentCamera == driverCamera)
            {
                //set position is Head bone of the Car model!
                driverCamera.Position = Vector3.Transform(Vector3.Zero, Matrix.Identity * Car.Car.GetWorldMatrix(Car.Chassis.Model, Vector3.Zero)[0] * Car.Chassis.Model.Bones["Head"].Transform);
            }

            //XNA Update
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            X.Renderer.Draw(ref gameTime, ref currentCamera);

            
            base.Draw(gameTime);
        }
    }
}
