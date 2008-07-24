/*
 * FreeCamera.cs
 * 
 * This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
 * for license details.
 */

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace QuickStart.Entities
{
    /// <summary>
    /// <see cref="FreeCamera"/> is a free-moving camera. Free camera's can be "flown" around as you please using
    /// input, and have little restriction on their movement.
    /// </summary>
    public class FreeCamera : Camera
    {
        /// <summary>
        /// Thumbstick Rotation sensitivity
        /// </summary>
        private const float thumbstickRotModifier = 0.002f;

        /// <summary>
        /// Thumbstick Movement sensitivity
        /// </summary>
        private const float thumbstickMoveModifier = 0.3f;

        /// <summary>
        /// Does the camera handle input inverted?
        /// </summary>
        private bool cameraRotInverted = true;

        private int speed = 1;

        /// <summary>
        /// Current zoom level, 1 would be (1x), 2 would be (2x), etc...
        /// </summary>
        private int zoomLevel = 1;

        /// <summary>
        /// Holds the default field of view. Storing the default FOV allows you to make changes to the camera's
        /// FOV and return back to your original values.
        /// </summary>
        private float defaultFOV = QSConstants.DefaultFOV;

        private Vector3 flatFront = Vector3.Zero;
        private Vector3 tiltedFront = Vector3.Zero;

        /// <summary>
        /// Rotation matrix which contains the pitch rotation data for this camera.
        /// </summary>
        private Matrix pitchMatrix = Matrix.Identity;      // Public for performance reasons

        /// <summary>
        /// Rotation matrix which contains the turn rotation data for this camera.
        /// </summary>
        private Matrix turnMatrix = Matrix.Identity;      // Public for performance reasons

        /// <summary>
        /// Sets camera's pitch (rotation along camera's right vector)
        /// </summary>
        public float Pitch
        {
            get { return this.pitch; }
            set
            {
                this.pitch = value;
                // Set hasChanged flag to true (if we use a bool)
            }
        }
        private float pitch = 0.0f;

        /// <summary>
        /// Sets camera's turn (rotation along camera's up vector)
        /// </summary>
        public float Turn
        {
            get { return this.turn; }
            set
            {
                this.turn = value;
                // Set hasChanged flag to true (if we use a bool)
            }
        }
        private float turn = 0.0f;                                        // Along Z axis (Up axis)

        /// <summary>
        /// Creates a free moving camera.
        /// </summary>
        /// <param name="FOV">Field-of-view, in degrees</param>
        /// <param name="screenWidth">Screen's width (resolution in pixels).</param>
        /// <param name="screenHeight">Screen's height (resolution in pixels).</param>
        /// <param name="nearPlane">Near plane distance</param>
        /// <param name="farPlane">Far plane distance</param>
        /// <param name="game">The <see cref="QSGame"/> the camera belongs to</param>
        public FreeCamera(QSGame game, float FOV, float screenWidth, float screenHeight, float nearPlane, float farPlane)
            : base(game, FOV, screenWidth, screenHeight, nearPlane, farPlane)
        {
            this.cameraType = CameraType.Free;   // This must not be removed
            this.defaultFOV = FOV;
            this.Game.GameMessage += this.Game_GameMessage;
        }

        /// <summary>
        /// Handles the <see cref="Game.GameMessage"/> event
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> recieved</param>
        private void Game_GameMessage(IMessage message)
        {
            switch (message.Type)
            {
                case MessageType.KeyPress:
                    KeyMessage keyMessage = message as KeyMessage;
                    if (keyMessage == null)
                    {
                        throw new ArgumentException("Passed message was not a KeyMessage");
                    }
                    this.HandleKeyPress(keyMessage.Data.Key, keyMessage.Time);
                    break;

                case MessageType.MouseScroll:
                    Message<int> scrollMessage = message as Message<int>;
                    if (scrollMessage == null)
                    {
                        throw new ArgumentException("Passed message was not a Message<int>");
                    }
                    this.HandleMouseScroll(scrollMessage.Data);
                    break;

                case MessageType.MouseDown:
                    Message<MouseButton> mouseDownMessage = message as Message<MouseButton>;
                    if (mouseDownMessage == null)
                    {
                        throw new ArgumentException("Passed message was not a Message<MouseButton>");
                    }
                    this.HandleButtonDown(mouseDownMessage.Data);
                    break;

                case MessageType.MouseUp:
                    Message<MouseButton> mouseUpMessage = message as Message<MouseButton>;
                    if (mouseUpMessage == null)
                    {
                        throw new ArgumentException("Passed message was not a Message<MouseButton>");
                    }
                    this.HandleButtonUp(mouseUpMessage.Data);
                    break;

                case MessageType.MouseMove:
                    Message<Vector2> moveMessage = message as Message<Vector2>;
                    if (moveMessage == null)
                    {
                        throw new ArgumentException("Passed message was not a Message<Vector2>");
                    }
                    this.HandleMouseMove(moveMessage.Data);
                    break;

                case MessageType.Thumbstick:
                    Message<GamePadThumbStick> thumbstickMessage = message as Message<GamePadThumbStick>;
                    if (thumbstickMessage == null)
                    {
                        throw new ArgumentException("Passed message was not a Message<GamePadThumbStick>");
                    }
                    HandleThumbsticks(thumbstickMessage.Data, thumbstickMessage.Time);
                    break;
            }
        }

        /// <summary>
        /// Handles mouse move messages
        /// </summary>
        /// <param name="distance">Distance the mouse has moved expressed through a <see cref="Vector2"/></param>
        private void HandleMouseMove(Vector2 distance)
        {
            this.PitchCamera(distance.Y);
            this.YawCamera(-distance.X);
        }

        /// <summary>
        /// Handles mouse button up messages
        /// </summary>
        /// <param name="mouseButton">The <see cref="MouseButton"/> that was released</param>
        private void HandleButtonUp(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Right:
                    this.speed /= 4;
                    break;
            }
        }

        /// <summary>
        /// Handles mouse button down messages
        /// </summary>
        /// <param name="mouseButton">The <see cref="MouseButton"/> that was pressed</param>
        private void HandleButtonDown(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Right:
                    this.speed *= 4;
                    break;
            }
        }

        /// <summary>
        /// Handles the mouse scroll event
        /// </summary>
        /// <param name="distance">Distance the mouse has scrolled</param>
        private void HandleMouseScroll(int distance)
        {
            if (distance > 0)
            {
                this.ZoomIn();
            }
            else
            {
                this.ZoomOut();
            }
        }

        /// <summary>
        /// Handles key presses and moves the camera accordently
        /// </summary>
        /// <param name="keys">The <see cref="Keys"/> being pressed</param>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        private void HandleKeyPress(Keys keys, GameTime gameTime)
        {
            float timeDelta = gameTime.ElapsedGameTime.Milliseconds;
            if (timeDelta > 100)
            {
                timeDelta = 100;
            }

            switch (keys)
            {
                case Keys.Up:
                    this.PitchCamera(0.001f * timeDelta);
                    break;
                case Keys.Down:
                    this.PitchCamera(-0.001f * timeDelta);
                    break;
                case Keys.Left:
                    this.YawCamera(0.001f * timeDelta);
                    break;
                case Keys.Right:
                    this.YawCamera(-0.001f * timeDelta);
                    break;

                case Keys.W:
                    this.MoveFrontBack(0.15f * timeDelta * this.speed);
                    break;
                case Keys.S:
                    this.MoveFrontBack(-0.15f * timeDelta * this.speed);
                    break;
                case Keys.A:
                    this.Strafe(-0.15f * timeDelta * this.speed);
                    break;
                case Keys.D:
                    this.Strafe(0.15f * timeDelta * this.speed);
                    break;
            }
        }

        /// <summary>
        /// Handles thumbstick input from gamepads for this camera.
        /// </summary>
        /// <param name="thumbstickData">Incoming data about a gamepad thumbstick</param>
        /// <param name="gameTime">XNA Timing snapshot</param>
        private void HandleThumbsticks(GamePadThumbStick thumbstickData, GameTime gameTime)
        {
            float timeDelta = gameTime.ElapsedGameTime.Milliseconds;
            if (timeDelta > 100)
            {
                timeDelta = 100;
            }

            // Left thumbstick moves the camera's position
            if (thumbstickData.StickType == GamePadInputSide.Left)
            {
                if (Math.Abs(thumbstickData.StickValues.Y) > float.Epsilon)
                {
                    this.MoveFrontBack(thumbstickData.StickValues.Y * timeDelta * thumbstickMoveModifier);
                }

                if (Math.Abs(thumbstickData.StickValues.X) > float.Epsilon)
                {
                    this.Strafe(thumbstickData.StickValues.X * timeDelta * thumbstickMoveModifier);
                }
            }

            // Right thumbstick rotates the camera
            if (thumbstickData.StickType == GamePadInputSide.Right)
            {
                int invertMod = (cameraRotInverted) ? -1 : 1;

                if (Math.Abs(thumbstickData.StickValues.Y) > float.Epsilon)
                {
                    this.PitchCamera(thumbstickData.StickValues.Y * timeDelta * thumbstickRotModifier * -invertMod);
                }

                if (Math.Abs(thumbstickData.StickValues.X) > float.Epsilon)
                {
                    this.YawCamera(thumbstickData.StickValues.X * timeDelta * thumbstickRotModifier);
                }
            }
        }

        /// <summary>
        /// Update the camera. Update the view matrix and frustum if needed. Take into account any movement of the camera.
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public override void Update(GameTime gameTime)
        {
            this.Right = Vector3.Cross(this.Up, this.Forward);
            this.flatFront = Vector3.Cross(this.Right, this.Up);

            this.pitchMatrix = Matrix.CreateFromAxisAngle(this.Right, this.Pitch);
            this.turnMatrix = Matrix.CreateFromAxisAngle(this.Up, this.Turn);

            Matrix pitchTurnMat = this.pitchMatrix * this.turnMatrix;
            this.tiltedFront = Vector3.TransformNormal(this.Forward, pitchTurnMat);

            float frontDot = 0.0f;
            Vector3.Dot(ref this.tiltedFront, ref this.flatFront, out frontDot);

            // Check angle so we can't flip over
            if (frontDot > 0.001f)
            {
                this.Forward = Vector3.Normalize(this.tiltedFront);
            }

            // Compute view matrix
            this.viewMatrix = Matrix.CreateLookAt(this.Position, this.Position + this.Forward, this.Up);

            // @todo: Temporary, this should be updated with a flag of some sort. - N.Foster
            this.UpdateFrustum();

            // Reset pitch and turn values so the next input loop can start fresh
            this.Pitch = 0.0f;
            this.Turn = 0.0f;

            // Set hasChanged back to false...
        }

        /// <summary>
        /// Strafes camera left or right (along its right vector)
        /// </summary>
        /// <param name="StrafeValue">Left is negative value, right is positive. 
        /// Value ranges (-1 to 1)</param>
        public void Strafe(float strafeValue)
        {
            this.Position -= this.Right * strafeValue;
        }

        /// <summary>
        /// Moves camera up or down
        /// </summary>
        /// <param name="UpDownValue">Up is negative value, down is positive
        /// Value ranges (-1 to 1)</param>
        public void MoveUpDown(float upDownValue)
        {
            this.Position += this.Up * upDownValue;
        }

        /// <summary>
        /// Moves camera forwards or backwards
        /// </summary>
        /// <param name="FrontBackValue">Forwards is a positive value, backwards is negative.
        /// Values range from -1 to 1</param>
        public void MoveFrontBack(float frontBackValue)
        {
            this.Position += this.Forward * frontBackValue;
        }

        /// <summary>
        /// Yaws camera (rotates along Z axis). Represents a yaw rotation.
        /// </summary>
        /// <param name="RotationValue">Amount to yaw</param>
        public void YawCamera(float rotationValue)
        {
            this.turn += rotationValue;
        }

        /// <summary>
        /// Pitches camera vertically (along its right axis). Represents a pitch rotation.
        /// </summary>
        /// <param name="RotationValue">Amount to pitch</param>
        public void PitchCamera(float rotationValue)
        {
            this.Pitch += rotationValue;
        }

        /// <summary>
        /// Zooms in on current view by 2x the current value (by restricting the <see cref="FOV"/>).
        /// Returns back to default zoom when zoomed past the <see cref="FreeCamera.MaxZoomLevel"/>
        /// </summary>
        public void ZoomIn()
        {
            if (this.zoomLevel < QSConstants.MaxZoomLevel)
            {
                this.zoomLevel *= 2;
            }

            this.FOV = this.defaultFOV / this.zoomLevel;

            this.UpdateProjMatrix();
        }

        /// <summary>
        /// Zooms out on current view (by widening the <see cref="FOV"/>)
        /// </summary>
        public void ZoomOut()
        {
            if (this.zoomLevel > 1)
            {
                this.zoomLevel /= 2;

                this.FOV = this.defaultFOV / this.zoomLevel;

                this.UpdateProjMatrix();
            }
        }
    }
}
