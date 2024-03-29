using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XEngine;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;


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
        public XWater water;
        public XTreeSystem trees;
        
        public List<XActor> boxes = new List<XActor>();

        public XModel Chassis;
        public XModel Wheel;
        public XCar Car;

        public XModel plane;
        public XActor planeActor;

        public XModel housemodel;
        public XAnimatedActor houseactor;

        public Game()
        {
            //Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);

            IsFixedTimeStep = false;
            graphics.IsFullScreen = false;
            graphics.SynchronizeWithVerticalRetrace =true;
            graphics.PreferredDepthStencilFormat = SelectStencilMode();
            //graphics.PreferMultiSampling = true;

            // use this for 720P
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;
 
            // for NTSC, use a 4:3 ratio
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 480;
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
            X = new XMain(graphics.GraphicsDevice,Services, "", freeCamera);
            //We are going to use a right hand corrdinate system switch default cull mode to refelect this
            //NOTE: Using the default model processor will not work with this since its winding order is backwards
            X.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
#if DEBUG
            XDebugVolumeRenderer.InitializeBuffers(X.GraphicsDevice, 50);
#endif
            X.FrameRate.DisplayFrameRate = true;
            X.Console.AutoDraw = false;
            X.Debug.StartPosition.Y = 200;

            resources = new XResourceGroup(ref X);
            input = new InputProcessor(ref X, this);
            menus = new MenuManager(ref X);
            //menu to the debugnodraw list
            X.Renderer.DebugNoDraw.Add(menus);

            //Make some Cameras
            freeCamera = new XFreeLookCamera(ref X, .1f, 1000f);
            freeCamera.Position = new Vector3(0, 10, 50);
            driverCamera = new XFreeLookCamera(ref X, .1f, 1000f);
            chase = new XChaseCamera(ref X, .1f, 1000f);
            currentCamera = chase;
            X.DefaultCamera = freeCamera;

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
            //sky = new XDynamicSky(ref X,X.Environment);
            //XSkyBox sky = new XSkyBox(ref X, @"Content\XEngine\Textures\Sky\front", @"Content\XEngine\Textures\Sky\back", @"Content\XEngine\Textures\Sky\left", @"Content\XEngine\Textures\Sky\right", @"Content\XEngine\Textures\Sky\top", @"Content\XEngine\Textures\Sky\bottom");
            XSkyBox sky = new XSkyBox(ref X, @"Content\XEngine\Textures\Sky\GreenWaterSky");
            heightmap = new XHeightMap(ref X, @"Content\Images\Heightmaps\Level2", X.Environment, @"Content\Textures\Grass", @"Content\Textures\Sand", null, @"Content\Images\Terrainmaps\Island1");
 
            //resources.AddComponent(environment);
            resources.AddComponent(heightmap);
            resources.AddComponent(sky);

            model = new XModel(ref X, @"Content\Models\cube");
            resources.AddComponent(model);

            Chassis = new XModel(ref X, @"Content\Models\chassis");
            Wheel = new XModel(ref X, @"Content\Models\wheel");

            resources.AddComponent(Chassis);
            resources.AddComponent(Wheel);

            housemodel = new XModel(ref X, @"Content\Models\spider");
            resources.AddComponent(housemodel);
            
            resources.Load();

            //Car = new XCar(ref X, Chassis, Wheel, true, true, 30.0f, 5.0f, 4.7f, 5.0f, 0.20f, 0.4f, 0.05f, 0.45f, 0.3f, 1, 520.0f, Math.Abs(X.Gravity.Y), new Vector3(-10, heightmap.Heights.GetHeight(new Vector3(-10, 3, 0))+5, 0));

            Car = new XCar(ref X, Chassis, Wheel,
                true,   // front wheel drive 
                true,   // rear wheel drive 
                35.0f,  // max steering angle 
                5f,     // steering rate 
                3.6f,   // front lateral traction 
                3.1f,   // rear lateral traction 
                1f,     // front longintudinal traction 
                2f,     // rear longitudinal traction 
                2.5f,   // handbrake rear lateral traction  
                .75f,   // handbrake rear longitudinal traction 
                6f,     // starting slide factor 
                15f,    // threshold 1 slide factor 
                30f,    // threshold 2 slide factor 
                .7f,    // slip threshold 1 
                10.0f,  // slip threshold 2 
                2.0f,   // slide speed factor 
                1.7f,   // traction loss factor on slip 
                0.3f,   // suspension travel 
                0.45f,  // wheel radius 
                -0.10f, // wheel mounting point  
                0.6f,   // spring rate 
                0.6f,   // shock dampening 
                2,      // wheel rays 
                2.5f,   // roll resistance 
                300.0f, // top speed 
                1200.0f, // torque
                X.Physics.Gravity.Length(), // gravity  
                new Vector3(-10, heightmap.Heights.GetHeight(new Vector3(-10, 3, 0)) + 5, 0)
                );
            
            Car.Load(X.Content);

            trees = new XTreeSystem(ref X, @"Content\Images\Treemaps\Level1", heightmap);
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
            //System.Threading.Thread.Sleep(100);

            

            //Input processor update KB,Mouse,Gamepad
            input.Update(gameTime);


            //this logic should be integrated into the main update loop somewhere
            //must be after physics and position updates then set the final camera positions then call update
  /*          if (currentCamera == driverCamera)
            {
                //set position is Head bone of the Car model!
                driverCamera.Position = Vector3.Transform(Vector3.Zero, Matrix.Identity * Car.Car.GetWorldMatrix(Car.Chassis.Model, Vector3.Zero)[0] * Car.Chassis.Model.Bones["Head"].Transform);
                //driverCamera.Update(ref gameTime);
            }
*/
            //Advance physics engine
            X.AdvancePhysics(ref gameTime);

            //After physics run, update chase cameras positions/orientation.
            if (currentCamera == chase)
            {
                chase.ChaseTargetPosition = Car.Position;
                chase.ChaseTargetForward = Car.Orientation.Forward;
                chase.Up = Car.Orientation.Up;
            }

            //Call engine update will call update on all updateable engine components
            X.UpdateComponents(ref gameTime);
            

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
