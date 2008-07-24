// DebugScreen.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace QuickStart.Compositor
{
    /// <summary>
    /// Compositor screen for a simple debug output.
    /// </summary>
    public class DebugScreen : IScreen
    {
        /// <summary>
        /// Gets/sets the horizontal position.
        /// </summary>
        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// Gets/sets the vertical position.
        /// </summary>
        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        /// <summary>
        /// The <see cref="DebugScreen"/> never needs the previous output as texture
        /// </summary>
        /// <remarks>
        /// This always returns false
        /// </remarks>
        public bool NeedBackgroundAsTexture
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the name of the <see cref="DebugScreen"/>
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        private int x;
        private int y;
        private ContentManager content;
        private SpriteFont font;
        private QSGame game;
        private readonly List<Keys> keys = new List<Keys>();

        private const string name = "Debug Information";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="xPos">The horizontal position</param>
        /// <param name="yPos">The vertical position</param>
        public DebugScreen(int xPos, int yPos, QSGame game)
        {
            this.game = game;
            this.game.GameMessage += this.Game_GameMessage;
            this.x = xPos;
            this.y = yPos;
        }
        
        /// <summary>
        /// Handles game messages
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> sent</param>
        private void Game_GameMessage(IMessage message)
        {
            KeyMessage keyMessage;
            switch (message.Type)
            {
                case MessageType.KeyDown:
                    keyMessage = message as KeyMessage;
                    if (keyMessage == null)
                    {
                        throw new ArgumentException("Message is not a KeyMessage");
                    }

                    this.keys.Add(keyMessage.Data.Key);
                    break;

                case MessageType.KeyUp:
                    keyMessage = message as KeyMessage;
                    if (keyMessage == null)
                    {
                        throw new ArgumentException("Message is not a KeyMessage");
                    }
                    this.keys.Remove(keyMessage.Data.Key);
                    break;
            }
        }

        /// <summary>
        /// Loads all content needed
        /// </summary>
        /// <param name="contentManager">The <see cref="ContentManager"/> instance to use for all content loading.</param>
        public void LoadContent(ContentManager contentManager)
        {
            this.content = contentManager;
            this.content.RootDirectory = "Content";

            this.font = this.content.Load<SpriteFont>("Fonts/Verdana8");
        }

        /// <summary>
        /// Unloads all previously loaded content.
        /// </summary>
        public void UnloadContent()
        {
            this.content.Unload();
        }

        /// <summary>
        /// Draws the debug scene
        /// </summary>
        /// <param name="batch">The <see cref="SpriteBatch"/> to use for 2D drawing.</param>
        /// <param name="background">The previous screen's output as a texture, if NeedBackgroundAsTexture is true.</param>
        /// <param name="gameTime">The <see cref="GameTime"/> structure for the current Game.Draw() cycle.</param>
        public void DrawScreen(SpriteBatch batch, Texture2D background, GameTime gameTime)
        {
            batch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

            StringBuilder sb = new StringBuilder();

            sb.Append("Current keys: ");
            for (int i = this.keys.Count - 1; i >= 0; i--)
            {
                sb.Append(this.keys[i].ToString());
                sb.Append(" ");
            }
            sb.AppendLine();
            sb.AppendLine();

            sb.Append("Number of ObjectPool types: ");
            sb.AppendLine(ObjectPool.free.Count.ToString());
            int count = 0;
            foreach (List<IPoolItem> list in ObjectPool.free.Values)
            {
                count += list.Count;
            }
            sb.Append(" Total free objects: ");
            sb.AppendLine(count.ToString());

            count = 0;
            foreach (List<IPoolItem> list in ObjectPool.locked.Values)
            {
                count += list.Count;
            }
            sb.Append(" Total locked objects: ");
            sb.AppendLine(count.ToString());

            sb.AppendLine();

            sb.Append("Number of current messages: ");
            sb.AppendLine(this.game.currentMessages.Count.ToString());
            sb.Append("Number of queued messages: ");
            sb.AppendLine(this.game.queuedMessages.Count.ToString());
            sb.AppendLine(" Queued message types:");
            for (int i = this.game.queuedMessages.Count - 1; i >= 0; i--)
            {
                IMessage message = this.game.queuedMessages[i];
                sb.AppendLine(string.Format("  {0} - {1}", message.Type, message.GetType().Name));
            }

            batch.DrawString(font, sb.ToString(), new Vector2(x, y), Color.White);

            batch.End();
        }
    }
}
