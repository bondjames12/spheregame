// MouseHandler.cs
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
    public class MouseHandler : InputHandler
    {
        private class MouseBottomComparer : IEqualityComparer<MouseButton>
        {
            #region IEqualityComparer<MouseButton> Members

            public bool Equals(MouseButton x, MouseButton y)
            {
                return x == y;
            }

            public int GetHashCode(MouseButton obj)
            {
                return 0;
            }

            #endregion
        }

        private GameTime gameTime;

        /// <summary>
        /// List of <see cref="MouseButton"/> and their last <see cref="ButtonState"/>
        /// </summary>
        private readonly Dictionary<MouseButton, ButtonState> lastState = new Dictionary<MouseButton, ButtonState>(6, new MouseBottomComparer());

        /// <summary>
        /// We want releative values not cumulative
        /// </summary>
        private int lastScrollValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The <see cref="QSGame"/> instance for the game</param>
        public MouseHandler(QSGame game)
            : base(game)
        {
            MouseState state = Mouse.GetState();
            this.lastState[MouseButton.Left] = state.LeftButton;
            this.lastState[MouseButton.Middle] = state.MiddleButton;
            this.lastState[MouseButton.Right] = state.RightButton;
            this.lastState[MouseButton.XButton1] = state.XButton1;
            this.lastState[MouseButton.XButton2] = state.XButton2;

            this.lastScrollValue = state.ScrollWheelValue;

            Mouse.SetPosition(100, 100);

            this.Game.Activated += this.Game_Activated;
            this.Game.Deactivated += this.Game_Deactivated;
        }

        /// <summary>
        /// Updates the mouse state, retriving the current state and sending the needed <see cref="Message<???>"/>
        /// </summary>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        protected override void UpdateCore(GameTime gameTime)
        {
            this.gameTime = gameTime;

            MouseState state = Mouse.GetState();
            this.ValidateButtons(state);

            this.ValidateScroll(state);

            this.ValidatePosition(state);

            this.gameTime = null;
        }

        /// <summary>
        /// Handles the <see cref="Game.Deactivate"/> event
        /// </summary>
        /// <param name="sender">The <see cref="Game"/> sending the event</param>
        /// <param name="e">Empty event arguments</param>
        private void Game_Deactivated(object sender, EventArgs e)
        {
            this.Game.IsMouseVisible = true;
        }

        /// <summary>
        /// Handles the <see cref="Game.Activate"/> event
        /// </summary>
        /// <param name="sender">The <see cref="Game"/> sending the event</param>
        /// <param name="e">Empty event arguments</param>
        private void Game_Activated(object sender, EventArgs e)
        {
            Mouse.SetPosition(100, 100);
            this.Game.IsMouseVisible = false;
        }

        /// <summary>
        /// Validates the mouse scroll value and sends a <see cref="Message<int>"/> if changed
        /// </summary>
        /// <param name="state">Current <see cref="MouseState"/></param>
        private void ValidateScroll(MouseState state)
        {
            if (state.ScrollWheelValue != this.lastScrollValue)
            {
                Message<int> scrollMessage = ObjectPool.Aquire<Message<int>>();
                scrollMessage.Data = state.ScrollWheelValue - this.lastScrollValue;
                scrollMessage.Time = this.gameTime;
                scrollMessage.Type = MessageType.MouseScroll;

                this.lastScrollValue = state.ScrollWheelValue;
                this.Game.SendMessage(scrollMessage);
            }
        }

        /// <summary>
        /// Validates the mouse position and sends a <see cref="Message<Vector2>"/> if changed
        /// </summary>
        /// <param name="state">Current <see cref="MouseState"/></param>
        private void ValidatePosition(MouseState state)
        {
            // If the mouse hasn't move just return
            if (state.X == 100 && state.Y == 100)
            {
                return;
            }

            // Reset the mouse position
            Mouse.SetPosition(100, 100);

            Message<Vector2> moveMessage = ObjectPool.Aquire<Message<Vector2>>();
            moveMessage.Type = MessageType.MouseMove;
            moveMessage.Time = this.gameTime;
            moveMessage.Data = new Vector2((float)(state.X - 100) / QSConstants.MouseSensitivity, (float)(state.Y - 100) / QSConstants.MouseSensitivity);

            this.Game.SendMessage(moveMessage);
        }

        /// <summary>
        /// Validates the mouse button states and sends a <see cref="Message<MouseButton>"/> if changed
        /// </summary>
        /// <param name="state">Current <see cref="MouseState"/></param>
        private void ValidateButtons(MouseState state)
        {
            this.ValidateButtonState(MouseButton.Left, state.LeftButton);
            this.ValidateButtonState(MouseButton.Middle, state.MiddleButton);
            this.ValidateButtonState(MouseButton.Right, state.RightButton);
            this.ValidateButtonState(MouseButton.XButton1, state.XButton1);
            this.ValidateButtonState(MouseButton.XButton2, state.XButton2);
        }

        /// <summary>
        /// Validates the state of a specific mouse button
        /// </summary>
        /// <param name="currentButton">The <see cref="MouseButton"/> which should be tested</param>
        /// <param name="currentState">The current <see cref="ButtonState"/> of the button</param>
        private void ValidateButtonState(MouseButton currentButton, ButtonState currentState)
        {
            Message<MouseButton> buttonMessage;
            // If there is a change in the state the button has just been pressed or released
            if (currentState != this.lastState[currentButton])
            {
                buttonMessage = ObjectPool.Aquire<Message<MouseButton>>();
                buttonMessage.Time = this.gameTime;
                buttonMessage.Type = MessageType.MouseDown;
                buttonMessage.Data = currentButton;

                // Determine the state of the button
                switch (currentState)
                {
                    case ButtonState.Pressed:
                        buttonMessage.Type = MessageType.MouseDown;
                        break;

                    case ButtonState.Released:
                        buttonMessage.Type = MessageType.MouseUp;
                        break;
                }

                this.lastState[currentButton] = currentState;
                this.Game.SendMessage(buttonMessage);
            }
            else
            {
                // If the state here is pressed then send the MousePress message
                if (currentState == ButtonState.Pressed)
                {
                    buttonMessage = ObjectPool.Aquire<Message<MouseButton>>();
                    buttonMessage.Time = this.gameTime;
                    buttonMessage.Type = MessageType.MousePress;
                    buttonMessage.Data = currentButton;

                    this.Game.SendMessage(buttonMessage);
                }
            }
        }
    }
}
