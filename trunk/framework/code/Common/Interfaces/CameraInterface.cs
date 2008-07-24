//
// CameraInterface.cs
// 
// This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
// for license details.
//

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using QuickStart.Entities;

namespace QuickStart.Interfaces
{
    /// <summary>
    /// All camera messages will be handled by the camera interface. It will allow
    /// anyone in the code that has access to this interface to request camera information.
    /// This is still being designed however, and is up for debate, nothing final just yet.
    /// </summary>
    public class CameraInterface : QSInterface
    {
        /// <summary>
        /// This is the current render camera for the game.
        /// </summary>
        /// <remarks>
        /// Upon creating split-screen support we will need multiple render cameras. We'd need at
        /// least one camera per viewport.
        /// </remarks>
        public Camera RenderCamera
        {
            get { return renderCamera; }
        }
        protected Camera renderCamera;

        /// <summary>
        /// Create a <see cref="CameraInterface"/>.
        /// </summary>
        /// <param name="sceneMgr">SceneManager which for which this interface resides in.</param>
        public CameraInterface(SceneManager sceneMgr)
            : base(sceneMgr)
        {
        }

        /// <summary>
        /// Update the camera interface.
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Updates the current render camera
            renderCamera.Update(gameTime);
        }

        /// <summary>
        /// Attaches a camera to an entity.
        /// </summary>
        /// <param name="inCamera">Camera to add</param>
        public void AttachCamera(BaseEntity parentEntity, Camera inCamera)
        {
            inCamera.ProcessEntityAttach(parentEntity);

            parentEntity.Cameras.Add(inCamera);
        }

        /// <summary>
        /// Finds a camera attached to parentEntity, of a specific type <see cref="CameraType"/>
        /// </summary>
        /// <param name="parentEntity">Entity to get the camera from</param>
        /// <param name="cameraType">Type of camera to get</param>
        /// <returns>Returns the requested camera, if not found then null is returned</returns>
        public Camera GetCamera(BaseEntity parentEntity, CameraType cameraType)
        {
            for (int i = parentEntity.Cameras.Count - 1; i >= 0; i--)
            {
                if (parentEntity.Cameras[i].CameraType == cameraType)
                {
                    return parentEntity.Cameras[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the camera for which rendering will occur.
        /// </summary>
        /// <param name="inCamera">Desired camera to render from</param>        
        public void SetRenderCamera(Camera inCamera)
        {
            this.renderCamera = inCamera;
        }
    }
}
