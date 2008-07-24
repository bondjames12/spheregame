using System;

using Microsoft.Xna.Framework;

namespace QuickStart.Entities
{
    /// <summary>
    /// FPSCameras are used for first person points of view.
    /// </summary>
    public class FPSCamera : Camera
    {
        /// <summary>
        /// Holds the default field of view. Storing the default FOV allows you to make changes to the camera's
        /// FOV and return back to your original values.
        /// </summary>
        protected float defaultFOV = QSConstants.DefaultFOV;

        /// <summary>
        /// Offset of the camera from the <see cref="attachedEntity"/>'s position. This offset is in relative coordinates
        /// and rotation to the <see cref="attachedEntity"/>.
        /// </summary>
        protected Vector3 positionOffset = new Vector3(0.0f, 5.0f, -1.0f);      // 5.0 above origin, 1.0 in front of origin.

        /// <summary>
        /// Creates an FPS camera.
        /// </summary>
        /// <param name="FOV">Field-of-view, in degrees</param>
        /// <param name="aspectRatio">Aspect Ratio is the screen's width (resolution in pixels) divided by the 
        /// screen's height (resolution in pixels). Most standard monitors and televisions are 4:3, or 1.333r. 
        /// Most widescreen monitors are 16:9, or 1.777r. So something like 1024x768 would be 4:3 because 1024/768 is 1.333r.</param>
        /// <param name="nearPlane">Near plane distance</param>
        /// <param name="farPlane">Far plane distance</param>
        /// <param name="game">The <see cref="QSGame"/> the camera belongs to</param>
        public FPSCamera(QSGame game, float FOV, float aspectRatio, float nearPlane, float farPlane)
            : base(game, FOV, aspectRatio, nearPlane, farPlane)
        {
            this.cameraType = CameraType.FPS;   // This must not be removed
            this.defaultFOV = FOV;
        }

        /// <summary>
        /// Creates an FPS camera.
        /// </summary>
        /// <param name="FOV">Field-of-view, in degrees</param>
        /// <param name="screenWidth">Screen's width (resolution in pixels).</param>
        /// <param name="screenHeight">Screen's height (resolution in pixels).</param>
        /// <param name="nearPlane">Near plane distance</param>
        /// <param name="farPlane">Far plane distance</param>
        /// <param name="game">The <see cref="QSGame"/> the camera belongs to</param>
        public FPSCamera(QSGame game, float FOV, float screenWidth, float screenHeight, float nearPlane, float farPlane)
            : base(game, FOV, screenWidth, screenHeight, nearPlane, farPlane)
        {
            this.cameraType = CameraType.FPS;   // This must not be removed
            this.defaultFOV = FOV;
        }

        /// <summary>
        /// Update the camera. Update the view matrix and frustum if needed. Take into account any movement of the camera.
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public override void Update(GameTime gameTime)
        {
            // Update camera's position
            this.Position = this.attachedEntity.Position + this.positionOffset;
            this.Forward = this.attachedEntity.rotation.Forward;
            this.Up = attachedEntity.rotation.Up;

            this.Position = Vector3.Transform(this.Position, this.attachedEntity.rotation);
            
            // Compute view matrix
            this.viewMatrix = Matrix.CreateLookAt(this.Position, this.Position + this.Forward, this.Up);

            // @todo: Temporary, this should be updated with a flag of some sort. - N.Foster
            UpdateFrustum();

            // Set hasChanged back to false...
        }
    }
}
