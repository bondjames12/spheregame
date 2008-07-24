//
// QSGame.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.
//

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using QuickStart.Graphics;
using QuickStart.Audio;
using QuickStart.Compositor;
using QuickStart.Physics;

namespace QuickStart
{
    public delegate void EngineMessageHandler(IMessage message);

    /// <summary>
    /// The QuickStart game base class.  This is the central point of control for all games built on the QuickStart engine.
    /// </summary>
    public abstract class QSGame : Game
    {
        private readonly ConfigurationManager configurationManager;
        private readonly GraphicsSystem graphics;
        private readonly ModelLoader modelLoader;
        private readonly ScreenCompositor compositor;
        private PhysicsManager physics;

        private bool exiting = false;
        internal List<IMessage> currentMessages;
        internal List<IMessage> queuedMessages;
        private static object messageLock = new object();

        private readonly List<BaseManager> managers;

        private readonly AudioSystem audio;

        /// <summary>
        /// Gets the <see cref="ModelLoader"/>
        /// </summary>
        public ModelLoader ModelLoader
        {
            get { return this.modelLoader; }
        }

        /// <summary>
        /// Retrieves the global configuration manager.
        /// </summary>
        public ConfigurationManager ConfigurationManager
        {
            get { return this.configurationManager; }
        }

        /// <summary>
        /// Retrieves the graphics system.
        /// </summary>
        public GraphicsSystem Graphics
        {
            get { return this.graphics; }
        }

        /// <summary>
        /// Retrieves the audio system.
        /// </summary>
        public AudioSystem Audio
        {
            get { return this.audio; }
        }

        /// <summary>
        /// Retrieves the compositor.
        /// </summary>
        public ScreenCompositor Compositor
        {
            get { return this.compositor; }
        }

        /// <summary>
        /// Retrieves the physics system.
        /// </summary>
        public PhysicsManager Physics
        {
            get { return this.physics; }
        }

        /// <summary>
        /// This event is raised when there are messages in the queue
        /// </summary>
        /// <remarks>
        /// The message is only raised during a update loop
        /// </remarks>
        public event EngineMessageHandler GameMessage;

        /// <summary>
        /// Creates a new instance of the QuickStart game base class.
        /// </summary>
        public QSGame()
        {
            this.configurationManager = new ConfigurationManager();

            this.graphics = new GraphicsSystem(this);
            this.audio = new AudioSystem();

            this.compositor = new ScreenCompositor(this);

            this.modelLoader = new ModelLoader(this.Content);

            this.currentMessages = new List<IMessage>();
            this.queuedMessages = new List<IMessage>();

            this.managers = new List<BaseManager>();
        }

        /// <summary>
        /// Sends messages to the system
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> which should be send</param>
        /// <remarks>Use <see cref="Message<T>"/> when sending errors</remarks>
        public void SendMessage(IMessage message)
        {
            if (message.Type == MessageType.Shutdown)
            {
                this.exiting = true;
            }

            if (this.exiting == true)
            {
                return;
            }

            // Lock the message queue before adding message
            lock (QSGame.messageLock)
            {
                this.queuedMessages.Add(message);
            }
        }

        /// <summary>
        /// Initializes the game instance.
        /// </summary>
        protected override void Initialize()
        {
            this.InitializeConfiguration();
            this.InitializeManagers();
            this.InitializePhysics();

            base.Initialize();
        }

        /// <summary>
        /// Loads all core content that will be used through-out entire lifetime of game.
        /// </summary>
        protected override void LoadContent()
        {
            this.compositor.LoadContent();

            base.LoadContent();
        }

        /// <summary>
        /// Unloads the content that lasts the entire lifetime of the game.
        /// </summary>
        protected override void UnloadContent()
        {
            this.compositor.UnloadContent();

            base.UnloadContent();
        }

        /// <summary>
        /// Handles game activated. (Windows-only)
        /// </summary>
        /// <param name="sender">The calling Game instance</param>
        /// <param name="args">The activation arguments</param>
        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
        }

        /// <summary>
        /// Handles game deactivation. (Windows only)
        /// </summary>
        /// <param name="sender">The calling Game instance.</param>
        /// <param name="args">The activation arguments.</param>
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
        }

        /// <summary>
        /// Updates all game and engine logic for one frame.
        /// </summary>
        /// <param name="gameTime">Time snapshot for the current update.</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.exiting == true)
            {
                // Clear all other messages
                this.currentMessages.Clear();
                this.queuedMessages.Clear();

                // Send a single shutdown message to all, bypass the pool as we are shutting down
                this.OnGameMessage(new ExitMessage());

                // Empty out the pool
                ObjectPool.ReleaseAll();

                this.Exit();
                return;
            }

            this.HandleMessages();

            for (int i = this.managers.Count - 1; i >= 0; i--)
            {
                BaseManager manager = this.managers[i];
                manager.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Begins a rendering frame.
        /// </summary>
        /// <returns>True if rendering can proceed, false otherwise.</returns>
        protected override bool BeginDraw()
        {
            return base.BeginDraw();
        }

        /// <summary>
        /// Renders a single frame for the current update cycle.
        /// </summary>
        /// <param name="gameTime">Time snapshot for the current frame.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        /// <summary>
        /// Ends a rendering frame.
        /// </summary>
        protected override void EndDraw()
        {
            base.EndDraw();
        }

        /// <summary>
        /// This handles messages and raises the <see cref="QSGame.GameMessage"/> if needed
        /// </summary>
        private void HandleMessages()
        {
            if (this.GameMessage != null)
            {
                lock (QSGame.messageLock)
                {
                    List<IMessage> temp = this.currentMessages;
                    this.currentMessages = this.queuedMessages;
                    this.queuedMessages = temp;
                    this.queuedMessages.Clear();
                }

                for (int i = 0; i < this.currentMessages.Count; i++)
                {
                    if (this.exiting == true)
                    {
                        break;
                    }
                    this.OnGameMessage(this.currentMessages[i]);
                }
            }

            this.ClearMessages(this.currentMessages);
        }

        /// <summary>
        /// Clears the list of messages and release the aquired messages
        /// </summary>
        /// <param name="list">List of <see cref="IMessage"/> to be cleared</param>
        private void ClearMessages(List<IMessage> list)
        {
            while (list.Count > 0)
            {
                IMessage message = list[0];
                list.RemoveAt(0);

                ObjectPool.Release(message);
            }
        }

        /// <summary>
        /// Raises the <see cref="GameMessage"/> event
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to raise</param>
        protected virtual void OnGameMessage(IMessage message)
        {
            if (this.GameMessage != null)
            {
                this.GameMessage(message);
            }
        }

        /// <summary>
        /// Initializes the <see cref="ConfigurationManager"/>
        /// </summary>
        private void InitializeConfiguration()
        {
            this.configurationManager.Initialize();
        }

        /// <summary>
        /// Initializes the input managers
        /// </summary>
        private void InitializeManagers()
        {
            this.managers.AddRange(this.ConfigurationManager.GetManagers(this));

            this.managers.Sort(delegate(BaseManager left, BaseManager right)
            {
                return right.UpdateOrder.CompareTo(left.UpdateOrder);
            });

            for (int i = this.managers.Count - 1; i >= 0; i--)
            {
                BaseManager manager = this.managers[i];
                manager.Initialize();
            }
        }

        /// <summary>
        /// Initializes the physics manager and configures the Physics property on QSGame.
        /// </summary>
        private void InitializePhysics()
        {
            for(int i = this.managers.Count - 1; i >= 0; i--)
            {
                BaseManager manager = this.managers[i];

                this.physics = manager as PhysicsManager;

                if(this.physics != null)
                    break;
            }
        }
    }
}
