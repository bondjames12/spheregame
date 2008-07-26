using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XEngine;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Sphere
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        XMain X;
        XResourceGroup resources;

        XKeyboard keyboard;
        XMouse mouse;

        XFreeLookCamera camera;

        XModel model;
        XModel duckModel;

        XHeightMap heightmap;
        XDynamicSky sky;
        XWater water;
        XEnvironmentParameters environment;

        XActor duckActor;
        List<XActor> boxes = new List<XActor>();

        XModel Chassis;
        XModel Wheel;
        XCar Car;

        Fire fire;

        XChaseCamera chase;

        XModel plane;
        XActor planeActor;

        XModel housemodel;
        XActor houseactor;

        public Game()
        {
            //Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);

            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            X = new XMain(this);
            X.FrameRate.DisplayFrameRate = false;   
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            resources = new XResourceGroup(X);

            keyboard = new XKeyboard(X);
            mouse = new XMouse(X);

            camera = new XFreeLookCamera(X);
            camera.Position = new Vector3(0, 10, 0);

            environment = new XEnvironmentParameters(X);
            sky = new XDynamicSky(X, environment);
            heightmap = new XHeightMap(X, @"Content\Textures\Heightmap", environment, @"Content\Textures\Grass", @"Content\Textures\Sand", null, @"Content\Textures\TextureMap");

            resources.AddComponent(environment);
            resources.AddComponent(heightmap);
            resources.AddComponent(sky);

            model = new XModel(X, @"Content\Models\Box");
            resources.AddComponent(model);

            duckModel = new XModel(X, @"Content\Models\Duck");
            resources.AddComponent(duckModel);

            Chassis = new XModel(X, @"Content\Models\Chassis");
            Wheel = new XModel(X, @"Content\Models\Wheel");

            resources.AddComponent(Chassis);
            resources.AddComponent(Wheel);

            chase = new XChaseCamera(X);

            housemodel = new XModel(X, @"Content\Models\tree01");
            resources.AddComponent(housemodel);

            //plane = new XModel(X, @"Content\plane");
            //resources.AddComponent(plane);
            //planeActor = new XActor(X, XActor.ActorType.Box, plane, new Vector3(0, 20, 0), Matrix.Identity, new Vector3(800), new Vector3(0), new Vector3(1000, .01f, 1000), Vector3.Zero, 1);
            //planeActor.Immovable = true;
            //planeActor.ShowBoundingBox = true;
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {  
            // TODO: use this.Content to load your game content here
            resources.Load();

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
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();
            if (keyboard.KeyPressed(Keys.Escape))
                Exit();

            camera.Rotate(new Vector3(mouse.Delta.Y * .0016f, mouse.Delta.X * .0016f, 0));

            if (keyboard.KeyDown(Keys.W))
                camera.Translate(Vector3.Forward * 40f);
            if (keyboard.KeyDown(Keys.S))
                camera.Translate(Vector3.Backward * 40);
            if (keyboard.KeyDown(Keys.A))
                camera.Translate(Vector3.Left * 40);
            if (keyboard.KeyDown(Keys.D))
                camera.Translate(Vector3.Right * 40);

            if (Car != null)
            {
                if (keyboard.KeyDown(Keys.Left))
                    Car.Steer(-1);
                if (keyboard.KeyDown(Keys.Right))
                    Car.Steer(1);
                if (keyboard.KeyDown(Keys.Up))
                    Car.Accelerate(2);
                if (keyboard.KeyDown(Keys.Down))
                    Car.Accelerate(-1);
            }

            if (mouse.ButtonPressed(XMouse.Buttons.Left))
                boxes.Add(new XActor(X, XActor.ActorType.Box, model, camera.Position, Matrix.Identity, new Vector3(10), new Vector3(0, 0, 0), new Vector3(1), Vector3.Normalize(camera.Target - camera.Position) * 30, 10));

            //sky.Theta += mouse.ScrollDelta * .0004f;

            if (keyboard.KeyPressed(Keys.F1))
                if (fire == null)
                    fire = new Fire(X);

            if (keyboard.KeyDown(Keys.F2))
                sky.Theta -= .5f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboard.KeyPressed(Keys.F3))
                if (water == null)
                {
                    water = new XWater(X, new Vector2(-128, -128), new Vector2(128, 128), 3);
                    water.Load(Content);
                }

            if (keyboard.KeyPressed(Keys.F4))
                for (int x = 0; x < 10; x++)
                    for (int e = x; e < 10; e++)
                        boxes.Add(new XActor(X, XActor.ActorType.Box, model, new Vector3(20, x * 1.01f + 1, e - 0.5f * x), Matrix.Identity, new Vector3(10), new Vector3(0, 0, 0), new Vector3(1), Vector3.Zero, 10));

            if (keyboard.KeyPressed(Keys.F5))
            {
                List<XActor> chainBoxes = new List<XActor>();

                for (int i = 0; i < 25; i++)
                {
                    XActor actor = new XActor(X, XActor.ActorType.Box, model, new Vector3(i + 10, 45 - i, 0), Matrix.Identity, new Vector3(10), Vector3.Zero, Vector3.One, Vector3.Zero, 1);
                    if (i == 0) actor.Immovable = true;
                    chainBoxes.Add(actor);
                }

                for (int i = 1; i < 25; i++)
                {
                    XHingeJoint hinge = new XHingeJoint(X, chainBoxes[i - 1], chainBoxes[i], Vector3.Backward, new Vector3(0.5f, -0.5f, 0.0f), 1.0f, 90.0f, 90.0f, 0.2f, 0.2f);
                }
            }

            if (keyboard.KeyPressed(Keys.F6))
                if (Car == null)
                    Car = new XCar(X, Chassis, Wheel, true, true, 30.0f, 5.0f, 4.7f, 5.0f, 0.20f, 0.4f, 0.05f, 0.45f, 0.3f, 1, 520.0f, X.Gravity, new Vector3(0, 3, 0));

            if (keyboard.KeyPressed(Keys.F7))
                if (duckActor == null)
                {
                    duckActor = new XActor(X, XActor.ActorType.Box, duckModel, new Vector3(0, 7, 0), Matrix.Identity, new Vector3(20), new Vector3(0, -2, 0), new Vector3(3, 3, 3), new Vector3(0), 10);
                    duckActor.Immovable = true;
                }

            if (keyboard.KeyPressed(Keys.F8))
                if (houseactor == null)
                {
                    houseactor = new XActor(X, XActor.ActorType.Box, housemodel, new Vector3(10, 10, 0), Matrix.Identity, new Vector3(0.05f), new Vector3(0, -1, 0), new Vector3(3, 3, 3), new Vector3(0), 10);
                    houseactor.Immovable = false;
                }

            if (duckActor != null)
                duckActor.Position = new Vector3(0, (float)Math.Sin((float)gameTime.TotalGameTime.TotalSeconds) * 1.5f + 7, 0);

            if (Car != null)
            {
                chase.ChaseTargetPosition = Car.Position;
                chase.ChaseTargetForward = Car.Orientation.Forward;
                chase.Up = Car.Orientation.Up;
            }
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            X.Renderer.Draw(gameTime, camera);
           
            base.Draw(gameTime);
        }
    }
}
