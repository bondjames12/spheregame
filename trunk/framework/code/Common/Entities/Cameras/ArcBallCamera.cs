/*
 * ArcBallCamera.cs
 * 
 * This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
 * for license details.
 */

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace QuickStart.Entities
{
    /// <summary>
    /// <see cref="ArcBallCamera"/> is a <see cref="Camera"/> that rotates around an origin.
    /// </summary>
    public class ArcBallCamera : Camera
    {
        /// <summary>
        /// Vertical angle (degrees) inclination/declination from the camera to the target. 
        /// Example: If <see cref="cameraArc"/> is 45.0f, then the camera will look down 45 degrees to the target.
        /// </summary>
        protected float cameraArc = 45.0f;

        /// <summary>
        /// Horizontal rotation angle (degrees) around the target.
        /// </summary>
        protected float cameraRotation = 0.0f;

        /// <summary>
        /// Distance from the attached target.
        /// </summary>
        protected float cameraDistance = 10.0f;

        /// <summary>
        /// Maximum allowed distance from attached target.
        /// </summary>
        public float CameraMaxDistance
        {
            get { return cameraMaxDistance; }
            set
            {
                if (value < cameraMinDistance)
                {
                    throw new Exception("cameraMaxDistance must be greater than cameraMinDistance.");
                }

                cameraMaxDistance = value;
            }
        }
        protected float cameraMaxDistance = 35.0f;

        /// <summary>
        /// Minimum allowed distance from the attached target.
        /// </summary>
        public float CameraMinDistance
        {
            get { return cameraMinDistance; }
            set
            {
                if (value < float.Epsilon)
                {
                    throw new Exception("An ArcBallCamera must have a distance greater than zero.");
                }

                if (value > cameraMaxDistance)
                {
                    throw new Exception("cameraMinDistance must be less than cameraMaxDistance.");
                }

                cameraMinDistance = value;
            }
        }
        protected float cameraMinDistance = 10.0f;

        /// <summary>
        /// Maximum declination angle between camera and the attached target.
        /// </summary>
        protected float cameraMaxVertAngle = 65.0f;

        /// <summary>
        /// Minimum inclination angle between camera and the attached target.
        /// </summary>
        protected float cameraMinVertAngle = -65.0f;

        /// <summary>
        /// Creates a <see cref="ArcBallCamera"/>
        /// </summary>
        /// <param name="FOV">Field-of-view, in degrees</param>
        /// <param name="aspectRatio">Aspect Ratio is the screen's width (resolution in pixels) divided by the 
        /// screen's height (resolution in pixels). Most standard monitors and televisions are 4:3, or 1.333r. 
        /// Most widescreen monitors are 16:9, or 1.777r. So something like 1024x768 would be 4:3 because 1024/768 is 1.333r.</param>
        /// <param name="nearPlane">Near plane distance</param>
        /// <param name="farPlane">Far plane distance</param>
        /// <param name="game">The <see cref="QSGame"/> the camera belongs to</param>
        public ArcBallCamera(QSGame game, float FOV, float aspectRatio, float nearPlane, float farPlane)
            : base(game, FOV, aspectRatio, nearPlane, farPlane)
        {
            cameraType = CameraType.ArcBall;     // This must not be removed
        }

        /// <summary>
        /// Creates a <see cref="ArcBallCamera"/>
        /// </summary>
        /// <param name="FOV">Field-of-view, in degrees</param>
        /// <param name="screenWidth">Screen's width (resolution in pixels).</param>
        /// <param name="screenHeight">Screen's height (resolution in pixels).</param>
        /// <param name="nearPlane">Near plane distance</param>
        /// <param name="farPlane">Far plane distance</param>
        /// <param name="game">The <see cref="QSGame"/> the camera belongs to</param>
        public ArcBallCamera(QSGame game, float FOV, float screenWidth, float screenHeight, float nearPlane, float farPlane)
            : base(game, FOV, screenWidth, screenHeight, nearPlane, farPlane)
        {
            cameraType = CameraType.ArcBall;       // This must not be removed
        }

        /// <summary>
        /// Update this camera's position, view matrix, and bounding frustum.
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public override void Update(GameTime gameTime)
        {
            // Keeps the camera from going too far or too close
            cameraDistance = MathHelper.Clamp(cameraDistance, cameraMinDistance, cameraMaxDistance);

            // Keeps the camera from going straight above or below the target.
            cameraArc = MathHelper.Clamp(cameraArc, cameraMinVertAngle, cameraMaxVertAngle);

            Vector3 cameraOffset = new Vector3(0, 0, -cameraDistance);

            Matrix unrotatedView = Matrix.CreateLookAt( cameraOffset, Vector3.Zero, Up);

            viewMatrix = Matrix.CreateTranslation(-attachedEntity.Position) *
                         Matrix.CreateRotationZ(cameraRotation) *
                         Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                         unrotatedView;

            // Store the position of the camera after it has been rotated
            Vector3 viewTrans = Matrix.Invert(viewMatrix).Translation;
            Position = viewTrans;

            // Now that position has been determined, set the forward vector
            Forward = Vector3.Normalize(attachedEntity.Position - Position);
        }

        /// <summary>
        /// Strafe camera around <see cref="attachedEntity"/>. Rotates around the Z(up) axis.
        /// </summary>
        /// <param name="strafeValue">Strafe amount</param>
        public void Strafe(float strafeValue)
        {
            cameraRotation += strafeValue;
        }

        /// <summary>
        /// Rotate camera around <see cref="attachedEntity"/>. Rotates around the attachedEntity's right axis.
        /// </summary>
        /// <param name="upDownValue">Rotation amount</param>
        public void MoveUpDown(float upDownValue)
        {
            cameraArc += upDownValue;
        }

        /// <summary>
        /// Zoom in the camera.
        /// </summary>
        public void ZoomIn()
        {
            cameraDistance *= 0.75f;
        }

        /// <summary>
        /// Zoom out the camera.
        /// </summary>
        public void ZoomOut()
        {
            cameraDistance *= 1.25f;
        }
    }
}
