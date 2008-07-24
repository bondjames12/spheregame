//
// GamepadHandler.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.
//

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace QuickStart
{
    /// <summary>
    /// This class handles gamepad input and broadcasts all input messages.
    /// </summary>
    public class GamePadHandler : InputHandler
    {
        /// <summary>
        /// Stores info about each player's current gamepad state.
        /// </summary>
        private readonly GamePadState[] currentGamePadStates;

        /// <summary>
        /// Stores info about each player's previous gamepad state (last update loop's gamepad state).
        /// </summary>
        private readonly GamePadState[] previousGamePadStates;

        /// <summary>
        /// Maximum number of supported gamepads.
        /// </summary>
        private const int MaxNumGamepads = 4;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The <see cref="QSGame"/> instance for the game</param>
        public GamePadHandler(QSGame game)
            : base(game)
        {
            this.currentGamePadStates = new GamePadState[MaxNumGamepads];
            this.previousGamePadStates = new GamePadState[MaxNumGamepads];
        }

        /// <summary>
        /// Updates the gamepad state, retriving the current state
        /// </summary>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        protected override void UpdateCore(GameTime gameTime)
        {
            // Check each player's controller (up to 4 controls)
            for (int i = 0; i < MaxNumGamepads; i++ )
            {            
                // If this player's control is connected, process its functions.
                if (GamePad.GetState((PlayerIndex)i).IsConnected)
                {
                    ProcessGamePad(i, gameTime);
                }
            }
        }

        /// <summary>
        /// Setup gamepad states.
        /// </summary>
        /// <param name="pIndex">Int index of the <see cref="PlayerIndex"/></param>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        private void ProcessGamePad(int pIndex, GameTime gameTime)
        {
            this.currentGamePadStates[pIndex] = GamePad.GetState((PlayerIndex)pIndex);

            // Check all functions of the controller for currentPlayer
            ProcessButtons(pIndex, gameTime);
            ProcessTriggers(pIndex, gameTime);
            ProcessThumbsticks(pIndex, gameTime);

            // Update previous gamepad state before input processing finishes
            this.previousGamePadStates[pIndex] = this.currentGamePadStates[pIndex];           
        }

        /// <summary>
        /// Process all <see cref="Gamepadbuttons"/>
        /// </summary>
        /// <param name="pIndex">Int index of the <see cref="PlayerIndex"/></param>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        private void ProcessButtons(int pIndex, GameTime gameTime)
        {
            PlayerIndex player = (PlayerIndex)pIndex;
            GamePadState currentState = currentGamePadStates[pIndex];
            GamePadState previousState = previousGamePadStates[pIndex];

            ProcessButton(currentState.Buttons.A, previousState.Buttons.A, Buttons.A, pIndex, gameTime);
            ProcessButton(currentState.Buttons.B, previousState.Buttons.B, Buttons.B, pIndex, gameTime);
            ProcessButton(currentState.Buttons.X, previousState.Buttons.X, Buttons.X, pIndex, gameTime);
            ProcessButton(currentState.Buttons.Y, previousState.Buttons.Y, Buttons.Y, pIndex, gameTime);
            ProcessButton(currentState.Buttons.Back, previousState.Buttons.Back, Buttons.Back, pIndex, gameTime);
            ProcessButton(currentState.Buttons.Start, previousState.Buttons.Start, Buttons.Start, pIndex, gameTime);
            ProcessButton(currentState.Buttons.LeftShoulder, previousState.Buttons.LeftShoulder, Buttons.LeftShoulder, pIndex, gameTime);
            ProcessButton(currentState.Buttons.RightShoulder, previousState.Buttons.RightShoulder, Buttons.RightShoulder, pIndex, gameTime);
            ProcessButton(currentState.Buttons.LeftStick, previousState.Buttons.LeftStick, Buttons.LeftStick, pIndex, gameTime);
            ProcessButton(currentState.Buttons.RightStick, previousState.Buttons.RightStick, Buttons.RightStick, pIndex, gameTime);
        }

        /// <summary>
        /// Process a gamepad button.
        /// </summary>
        /// <param name="currentButtonState">Current state of the button</param>
        /// <param name="previousButtonState">Previous state of the button</param>
        /// <param name="button">A gamepad button</param>
        /// <param name="pIndex">Player index for which player is pressing this button</param>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        private void ProcessButton(ButtonState currentButtonState, ButtonState previousButtonState, Buttons button, 
                                   int pIndex, GameTime gameTime)
        {
            PlayerIndex player = (PlayerIndex)pIndex;
            int messageTypeInt = -1;

            if (currentButtonState != previousButtonState)
            {
                if (currentGamePadStates[pIndex].IsButtonUp(button))
                {
                    messageTypeInt = MessageType.ButtonUp;
                }
                else
                {
                    messageTypeInt = MessageType.ButtonUp;
                }
            }
            else
            {
                if (currentGamePadStates[pIndex].IsButtonDown(button))
                {
                    messageTypeInt = MessageType.ButtonHeld;
                }
            }

            if (messageTypeInt != -1)
            {
                SendButtonMessage(messageTypeInt, button, player, gameTime);
            }
        }

        /// <summary>
        /// Process all <see cref="GamePadTriggers"/>
        /// </summary>
        /// <param name="pIndex">Int index of the <see cref="PlayerIndex"/></param>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        private void ProcessTriggers(int pIndex, GameTime gameTime)
        {
            PlayerIndex player = (PlayerIndex)pIndex;
            GamePadState currentState = this.currentGamePadStates[pIndex];

            if (currentState.Triggers.Left > 0.0f)
            {
                SendTriggerMessage(GamePadInputSide.Left, currentState.Triggers.Left, player, gameTime);
            }
            
            if (currentState.Triggers.Right > 0.0f)
            {
                SendTriggerMessage(GamePadInputSide.Right, currentState.Triggers.Right, player, gameTime);
            }
        }

        /// <summary>
        /// Process all <see cref="GamePadThumbsticks"/>
        /// </summary>
        /// <param name="pIndex">Int index of the <see cref="PlayerIndex"/></param>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        private void ProcessThumbsticks(int pIndex, GameTime gameTime)
        {
            PlayerIndex player = (PlayerIndex)pIndex;
            GamePadState currentState = this.currentGamePadStates[pIndex];

            if ( ( Math.Abs(currentState.ThumbSticks.Left.X) > 0.0f) || ( Math.Abs(currentState.ThumbSticks.Left.Y) > 0.0f) )
            {
                SendThumbstickMessage(GamePadInputSide.Left, currentState.ThumbSticks.Left.X, currentState.ThumbSticks.Left.Y, 
                                      player, gameTime);
            }

            if ( ( Math.Abs(currentState.ThumbSticks.Right.X) > 0.0f) || ( Math.Abs(currentState.ThumbSticks.Right.Y) > 0.0f) )
            {
                SendThumbstickMessage(GamePadInputSide.Right, -currentState.ThumbSticks.Right.X, currentState.ThumbSticks.Right.Y,
                                      player, gameTime);
            }
        }

        /// <summary>
        /// Sends a message containing information about a specific button.
        /// </summary>
        /// <param name="buttonState"></param>
        /// <param name="button"></param>
        /// <param name="player">Player whose controller this message pertains to</param>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        private void SendButtonMessage(int buttonState, Buttons button, PlayerIndex player, GameTime gameTime)
        {
            GamePadButton buttonData;
            buttonData.Button = button;
            buttonData.Player = player;

            Message<GamePadButton> buttonMessage = ObjectPool.Aquire<Message<GamePadButton>>();
            buttonMessage.Type = buttonState;
            buttonMessage.Data = buttonData;
            buttonMessage.Time = gameTime;

            this.Game.SendMessage(buttonMessage);                      
        }

        /// <summary>
        /// Sends a message containing information about a specific trigger.
        /// </summary>
        /// <param name="triggerType">Type of trigger</param>
        /// <param name="triggerValue">Value of trigger (0.0f to 1.0f)</param>
        /// <param name="player">Player whose controller this message pertains to</param>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        private void SendTriggerMessage(GamePadInputSide triggerType, float triggerValue, PlayerIndex player, GameTime gameTime)
        {
            GamePadTrigger triggerData;
            triggerData.TriggerType = triggerType;
            triggerData.TriggerValue = triggerValue;
            triggerData.Player = player;

            Message<GamePadTrigger> triggerMessage = ObjectPool.Aquire<Message<GamePadTrigger>>();
            triggerMessage.Type = MessageType.Trigger;
            triggerMessage.Data = triggerData;
            triggerMessage.Time = gameTime;

            this.Game.SendMessage(triggerMessage);
        }

        /// <summary>
        /// Sends a message containing information about a specific trigger
        /// </summary>
        /// <param name="stickType">Type of thumbstick (left or right)</param>
        /// <param name="XValue">Value of X-axis of thumbstick (-1.0f to 1.0f)</param>
        /// <param name="YValue">Value of Y-axis of thumbstick (-1.0f to 1.0f)</param>
        /// <param name="player">Player whose controller this message pertains to</param>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        private void SendThumbstickMessage(GamePadInputSide stickType, float XValue, float YValue, PlayerIndex player, GameTime gameTime)
        {
            GamePadThumbStick thumbstickData;
            thumbstickData.StickType = stickType;
            thumbstickData.StickValues.X = XValue;
            thumbstickData.StickValues.Y = YValue;
            thumbstickData.Player = player;

            Message<GamePadThumbStick> thumbstickMessage = ObjectPool.Aquire<Message<GamePadThumbStick>>();
            thumbstickMessage.Type = MessageType.Thumbstick;
            thumbstickMessage.Data = thumbstickData;
            thumbstickMessage.Time = gameTime;

            this.Game.SendMessage(thumbstickMessage);
        }
    }
}
