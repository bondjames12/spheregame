// KeyboardHandler.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace QuickStart
{
    /// <summary>
    /// This class handles keyboard input
    /// </summary>
    public class KeyboardHandler : InputHandler
    {
        private List<Keys> downKeys;
        private List<Keys> pressedKeys;
        private List<Keys> tempKeys;
        private List<Keys> newDownKeys;

        private bool leftControl = false;
        private bool leftShift = false;
        private bool leftAlt = false;
        private bool rightAlt = false;
        private bool rightControl = false;
        private bool rightShift = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The <see cref="QSGame"/> instance for the game</param>
        public KeyboardHandler(QSGame game)
            : base(game)
        {
            this.downKeys = new List<Keys>();
            this.pressedKeys = new List<Keys>();
            this.tempKeys = new List<Keys>();
            this.newDownKeys = new List<Keys>();
        }

        /// <summary>
        /// Updates the keyboard state, retriving the current state and sending the needed <see cref="KeyMessage"/>
        /// </summary>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        protected override void UpdateCore(GameTime gameTime)
        {
            Keys[] pressed = Keyboard.GetState().GetPressedKeys();
            List<Keys> released = this.pressedKeys;
            this.pressedKeys = this.tempKeys;

            this.VerifyControlKeys(pressed);

            // Loop through all currently pressed keys
            for (int i = pressed.Length - 1; i >= 0; i--)
            {
                Keys key = pressed[i];

                // If it's already being pressed send the KeyPress message
                bool found = false;
                for (int j = released.Count - 1; j >= 0; j--)
                {
                    if (released[j] == key)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == true)
                {
                    released.Remove(key);
                    this.pressedKeys.Add(key);
                    this.SendKeyMessage(MessageType.KeyPress, key, gameTime);

                    continue;
                }

                // If it was pressed last cycle send the KeyPress message
                found = false;
                for (int j = this.downKeys.Count - 1; j >= 0; j--)
                {
                    if (this.downKeys[j] == key)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == true)
                {
                    released.Remove(key);
                    this.downKeys.Remove(key);
                    this.pressedKeys.Add(key);
                    this.SendKeyMessage(MessageType.KeyPress, key, gameTime);
                    continue;
                }

                this.newDownKeys.Add(key);
            }
            released.AddRange(this.downKeys);
            this.downKeys.Clear();

            // If the key hasn't been pressed before send the KeyDown message
            this.downKeys.AddRange(this.newDownKeys);
            this.newDownKeys.Clear();
            for (int i = this.downKeys.Count - 1; i >= 0; i--)
            {
                Keys key = this.downKeys[i];

                this.SendKeyMessage(MessageType.KeyDown, key, gameTime);
            }

            // If the key isn't pressed anymore send the KeyUp message
            for (int i = released.Count - 1; i >= 0; i--)
            {
                Keys key = released[i];

                this.SendKeyMessage(MessageType.KeyUp, key, gameTime);
            }

            released.Clear();
            released.AddRange(this.pressedKeys);
            this.pressedKeys = released;
            this.tempKeys.Clear();
            this.leftAlt = false;
            this.leftControl = false;
            this.leftShift = false;
            this.rightAlt = false;
            this.rightControl = false;
            this.rightShift = false;
        }

        /// <summary>
        /// Verifies which control (Shift/Alt/Control) keys are currently pressed
        /// </summary>
        /// <param name="pressed">The set of <see cref="Keys"/> currently pressed</param>
        private void VerifyControlKeys(Keys[] pressed)
        {
            for (int i = pressed.Length - 1; i > 0; i--)
            {
                Keys key = pressed[i];
                switch (key)
                {
                    case Keys.LeftAlt:
                        this.leftAlt = true;
                        break;

                    case Keys.LeftControl:
                        this.leftControl = true;
                        break;

                    case Keys.LeftShift:
                        this.leftShift = true;
                        break;

                    case Keys.RightAlt:
                        this.rightAlt = true;
                        break;

                    case Keys.RightControl:
                        this.rightControl = true;
                        break;

                    case Keys.RightShift:
                        this.rightShift = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Sends a message containing information about a specific key
        /// </summary>
        /// <param name="keyState">The state of the key Down/Pressed/Up</param>
        /// <param name="key">The <see cref="Keys"/> that changed it's state</param>
        private void SendKeyMessage(int keyState, Keys key, GameTime gameTime)
        {
            KeyMessage keyMessage = ObjectPool.Aquire<KeyMessage>();
            keyMessage.Type = keyState;
            keyMessage.Data.Key = key;
            keyMessage.Time = gameTime;

            keyMessage.Data.LeftAlt = this.leftAlt;
            keyMessage.Data.LeftControl = this.leftControl;
            keyMessage.Data.LeftShift = this.leftShift;
            keyMessage.Data.RightAlt = this.rightAlt;
            keyMessage.Data.RightControl = this.rightControl;
            keyMessage.Data.RightShift = this.rightShift;

            this.Game.SendMessage(keyMessage);
        }
    }
}
