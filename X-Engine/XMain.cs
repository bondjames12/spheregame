using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using JigLibX.Physics;
using JigLibX.Collision;
using System;
using Microsoft.Xna.Framework.Content;
using System.Threading;

namespace XEngine
{
    public class XMain
    {
        //public Dictionary<uint, XComponent> Components;
        public List<XComponent> Components;

        public SpriteBatch spriteBatch;

        public XFont SystemFont;

        public XDebug Debug;
        public XFrameRate FrameRate;
        public XConsole Console;
        public XRenderer Renderer;
        public XDebugDrawer DebugDrawer;
        public XTools Tools;
        public XDepthMap DepthMap;
        public XEnvironmentParameters Environment;
        public PhysicsSystem Physics;
        public Vector3 Gravity { get { return Physics.Gravity; } set { Physics.Gravity = value; } }

        public bool UpdatePhysics = true;

        public ContentManager Content;
        public GraphicsDevice GraphicsDevice;
        public IServiceProvider Services;
        public XCamera DefaultCamera;
        public ParticleCloudSystem cloudSystem;

        public XMain(GraphicsDevice GraphicsDevice,IServiceProvider Services, string ContentRootDir, XCamera defaultCamera)
        {
            Components = new List<XComponent>();

            this.Content = new ContentManager(Services);
            this.Content.RootDirectory = ContentRootDir;
            this.GraphicsDevice = GraphicsDevice;
            this.Services = Services;

            Tools = new XTools(this);

            SystemFont = new XFont(this, @"Content\XEngine\Fonts\System");
            Debug = new XDebug(this);
            FrameRate = new XFrameRate(this);
            Console = new XConsole(this);
            DebugDrawer = new XDebugDrawer(this);
            DepthMap = new XDepthMap(this);
           
            Environment = new XEnvironmentParameters(this);


            Physics = new PhysicsSystem();
            Physics.EnableFreezing = true;
            Physics.SolverType = PhysicsSystem.Solver.Normal;
            Physics.CollisionSystem = new CollisionSystemGrid(32, 32, 32, 32, 32, 32);
            Physics.CollisionSystem.UseSweepTests = true;
            
            //New system
            //Physics.CollisionSystem = new CollisionSystemSAP();

            //Rendering should be the last thing! The constructor requires some componnets like debug, debugdrawer to allready exist!
            Renderer = new XRenderer(this);

            this.DefaultCamera = defaultCamera;
            // Initialize the particle cloud system for tree leaves!
            cloudSystem = new ParticleCloudSystem(GraphicsDevice, Content, "Content/XEngine/Effects/ParticleCloud");
        }

        public void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SystemFont.Load(Content);
            Console.Load(Content);
            DebugDrawer.Load(Content);
            Environment.Load(Content);
            Renderer.Load(Content);

            cloudSystem.Initialize();
            if(DefaultCamera != null) cloudSystem.Projection = DefaultCamera.Projection;

            GC.Collect();
        }

        const float StepSize = 1.00f / 100.0f;  // or whatever your sim rate is
        double phase_;

        public void Update(GameTime gameTime)
        {
            if (UpdatePhysics)
            {
                phase_ += gameTime.ElapsedGameTime.TotalSeconds;
                if (phase_ > 0.1) phase_ = 0.1; // lock to 10 fps assumed worst case
                while (phase_ > 0)
                {
                    PhysicsSystem.CurrentPhysicsSystem.Integrate(StepSize);
                    phase_ -= StepSize;
                }
                this.Debug.Write("Physics Integration dt coefficient:" + StepSize.ToString(), false);
            }


            for (int i = 0; i < Components.Count; i++)
            {
                XComponent Component = Components[i];
                if (Component is XIUpdateable)
                    Component.Update(ref gameTime);
            }
            
                
        }

        

    }  
}