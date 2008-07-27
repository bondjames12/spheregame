using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using JigLibX.Physics;
using JigLibX.Collision;
using System;
using Microsoft.Xna.Framework.Content;
#if !XBOX
    using System.ComponentModel.Design;
#endif

namespace XEngine
{
    public class XMain : DrawableGameComponent
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
        public float Gravity { get { return Physics.Gravity.Length(); } }

        public Game game;

        public XMain(Game game) : base(game)
        {
            this.game = game;
            game.Components.Add(this);

            Components = new List<XComponent>();

            SystemFont = new XFont(this, @"Content\XEngine\Fonts\System");

            Debug = new XDebug(this);
            FrameRate = new XFrameRate(this);
            Console = new XConsole(this);
            Renderer = new XRenderer(this);
            DebugDrawer = new XDebugDrawer(this);
            Tools = new XTools();

            Physics = new PhysicsSystem();
            Physics.EnableFreezing = true;
            Physics.SolverType = PhysicsSystem.Solver.Normal;
            Physics.CollisionSystem = new CollisionSystemGrid(32, 32, 32, 32, 32, 32);
            Physics.CollisionSystem.UseSweepTests = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            SystemFont.Load(game.Content);
            Console.Load(game.Content);
            DebugDrawer.Load(game.Content);

            GC.Collect();
        }

        public override void Update(GameTime gameTime)
        {
            PhysicsSystem.CurrentPhysicsSystem.Integrate((float)gameTime.ElapsedGameTime.TotalSeconds);

            foreach(XComponent Component in Components)
                if (Component is XUpdateable)
                    Component.Update(gameTime);

            base.Update(gameTime);
        }
    }
}