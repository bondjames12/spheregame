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
        public List<XComponent> Components;

        public SpriteBatch spriteBatch;

        public XFont SystemFont;

        public XDebug Debug;
        public XFrameRate FrameRate;
        public XConsole Console;
        public XRenderer Renderer;
        public XDebugDrawer DebugDrawer;
        public XTools Tools;

        public PhysicsSystem Physics;
        public Vector3 Gravity { get { return Physics.Gravity; } set { Physics.Gravity = value; } }

        public bool UpdatePhysics = true;

        public ContentManager Content;
        public GraphicsDevice GraphicsDevice;
        public IServiceProvider Services;

        public XMain(GraphicsDevice GraphicsDevice, IServiceProvider Services)
        {
            Components = new List<XComponent>();

            this.Content = new ContentManager(Services);
            this.GraphicsDevice = GraphicsDevice;
            this.Services = Services;

            SystemFont = new XFont(this, @"Content\XEngine\Fonts\System");

            Debug = new XDebug(this);
            FrameRate = new XFrameRate(this);
            Console = new XConsole(this);
            Renderer = new XRenderer(this);
            DebugDrawer = new XDebugDrawer(this);
            Tools = new XTools(this);

            Physics = new PhysicsSystem();
            Physics.EnableFreezing = true;
            Physics.SolverType = PhysicsSystem.Solver.Normal;
            Physics.CollisionSystem = new CollisionSystemSAP();
        }

        public void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SystemFont.Load(Content);
            Console.Load(Content);
            DebugDrawer.Load(Content);
            Renderer.Load(Content);

            GC.Collect();
        }

        public void Update(GameTime gameTime)
        {
            if (UpdatePhysics)
                PhysicsSystem.CurrentPhysicsSystem.Integrate((float)gameTime.ElapsedGameTime.TotalSeconds);

            foreach(XComponent Component in Components)
                if (Component is XUpdateable)
                    Component.Update(gameTime);
        }
    }
}