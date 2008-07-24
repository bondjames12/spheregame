using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using QuickStart;
using QuickStart.Graphics;
using QuickStart.Entities;
using QuickStart.Compositor;
using QuickStart.Interfaces;
//using QuickStart.Physics;

namespace Sphere
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : QuickStart.QSGame
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// Sample compositor screens
        /// </summary>
        private FPSScreen fpsScreen;

        public Game()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = QSConstants.MouseDefaultVisible;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SendDebugMessage("Initialize");
            // Set up screen compositor
            fpsScreen = new FPSScreen(10, 10);
            Compositor.InsertScreen(fpsScreen, false);
            //Debug Messages Screen
            Compositor.InsertScreen(new DebugScreen(10, 100, this), false);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            // Render the whole frame with one call!
            Compositor.DrawCompositorChain(gameTime);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Handles game messages
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> send to the game</param>
        protected override void OnGameMessage(IMessage message)
        {
            switch (message.Type)
            {
                case MessageType.KeyDown:
                    KeyMessage msg = message as KeyMessage;
                    switch (msg.Data.Key)
                    {
                        case Keys.Escape:
                            this.SendMessage(new ExitMessage());
                            break;

                        case Keys.Enter:
                            if (msg.Data.LeftAlt == true && msg.Data.LeftShift == true)
                            {
                                this.Graphics.ToggleFullscreen();
                            }
                            break;

                    }
                    break;
            }

            base.OnGameMessage(message);
        }

        /// <summary>
        /// Sends a text message to the debug output
        /// </summary>
        /// <param name="message">Text to send to the debug output</param>
        private void SendDebugMessage(string message)
        {
            Message<string> msg = ObjectPool.Aquire<Message<string>>();
            msg.Data = message;
            msg.Type = 20000;

            SendMessage(msg);
        }
    }
}
