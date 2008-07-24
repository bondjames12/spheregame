/*
 * FixedCamera.cs
 * 
 * This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
 * for license details.
 */

using System;

using Microsoft.Xna.Framework;

namespace QuickStart.Entities
{
    /// <summary>
    /// <see cref="FixedCamera"/> is a <see cref="Camera"/> that operates at a fixed distance from an origin.
    /// </summary>
    public class FixedCamera : Camera
    {
        /// <summary>
        /// Position of the camera will be offset by these amounts in each axis. The axes are reletive to the target's rotation, not
        /// world axes.
        /// </summary>
        protected Vector3 positionOffset = new Vector3(0.0f, 5.0f, -10.0f);

        /// <summary>
        /// View will be offset from the target by this much. This vector is in reletive coordinates to the object,
        /// so if you offset by 5.0f in the Z value, you'll offset the camera 5.0 units above the target according to
        /// the target's up axis.
        /// </summary>
        protected Vector3 viewOffset = Vector3.Zero;

        /// <summary>
        /// Creates a <see cref="FixedCamera"/>
        /// </summary>
        /// <param name="FOV">Field-of-view, in degrees</param>
        /// <param name="aspectRatio">Aspect Ratio is the screen's width (resolution in pixels) divided by the 
        /// screen's height (resolution in pixels). Most standard monitors and televisions are 4:3, or 1.333r. 
        /// Most widescreen monitors are 16:9, or 1.777r. So something like 1024x768 would be 4:3 because 1024/768 is 1.333r.</param>
        /// <param name="nearPlane">Near plane distance</param>
        /// <param name="farPlane">Far plane distance</param>
        /// <param name="game">The <see cref="QSGame"/> the camera belongs to</param>
        public FixedCamera(QSGame game, float FOV, float aspectRatio, float nearPlane, float farPlane)
            : base(game, FOV, aspectRatio, nearPlane, farPlane)
        {
            this.cameraType = CameraType.Fixed;     // This must not be removed
        }

        /// <summary>
        /// Creates a <see cref="FixedCamera"/>
        /// </summary>
        /// <param name="FOV">Field-of-view, in degrees</param>
        /// <param name="screenWidth">Screen's width (resolution in pixels).</param>
        /// <param name="screenHeight">Screen's height (resolution in pixels).</param>
        /// <param name="nearPlane">Near plane distance</param>
        /// <param name="farPlane">Far plane distance</param>
        /// <param name="game">The <see cref="QSGame"/> the camera belongs to</param>
        public FixedCamera(QSGame game, float FOV, float screenWidth, float screenHeight, float nearPlane, float farPlane)
            : base(game, FOV, screenWidth, screenHeight, nearPlane, farPlane)
        {
            this.cameraType = CameraType.Fixed;       // This must not be removed
        }

        /// <summary>
        /// Update the camera. Update the view matrix and frustum if needed. Take into account any movement of the camera.
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public override void Update(GameTime gameTime)
        {
            // Position camera should look at with respect to its position offset.
            Vector3 forwardDistance = Vector3.Transform(this.viewOffset, this.attachedEntity.Rotation);

            // Position camera in space
            this.Position = this.positionOffset;

            // Keeps camera behind its target
            this.Position = Vector3.Transform(this.Position, this.attachedEntity.Rotation);

            // Keeps camera "attached" to its target
            this.Position = Vector3.Transform(this.Position, Matrix.CreateTranslation(this.attachedEntity.Position + forwardDistance));

            this.Forward = Vector3.Normalize(this.attachedEntity.Position + forwardDistance - this.Position);

            this.viewMatrix = Matrix.CreateLookAt(this.Position, this.attachedEntity.Position + forwardDistance, Vector3.Up);

            UpdateFrustum();        // TEMPORARY
        }
    }
}
