//
// Camera.cs
// 
// This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
// for license details.


using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using QuickStart;

namespace QuickStart.Entities
{
    /// <summary>
    /// Specifies a type of camera.
    /// </summary>
    public enum CameraType
    {
        Free = 0,
        Fixed = 1,
        ArcBall = 2,
        FPS = 3
    }

    /// <summary>
    /// Super class for all Cameras. This class is abstract, which means you cannot just create a
    /// Camera instance, you must create a type of camera, like <see cref="FreeCamera"/>. To render something in the 
    /// QuickStart Engine you must have a <see cref="Camera"/>.
    /// </summary>
    public abstract class Camera : BaseEntity
    {
        /// <summary>
        /// Type of camera that this camera is. Based on <see cref="CameraType"/> enumeration.
        /// </summary>
        public CameraType CameraType
        {
            get { return cameraType; }
        }
        protected CameraType cameraType;

        /// <summary>
        /// Entity that camera is attached to. All cameras must be attached to an entity, even if it is a null entity.
        /// </summary>
        protected BaseEntity attachedEntity;

        /// <summary>
        /// Camera's view matrix.
        /// </summary>
        public Matrix viewMatrix = Matrix.Identity;             // Public for performance reasons

        /// <summary>
        /// Camera's projection matrix.
        /// </summary>
        public Matrix projectionMatrix = Matrix.Identity;       // Public for performance reasons  

        /// <summary>
        /// Camera's aspect ratio (used for <see cref="projectionMatrix"/>)
        /// </summary>
        public float AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; }
        }
        protected float aspectRatio = 1.33333f;        

        /// <summary>
        /// Camera's forward vector. Direction camera is facing.
        /// </summary>
        public Vector3 Forward
        {
            get { return forward; }

            protected set
            {
                forward = value;
                // Set hasChanged flag to true (if we use a bool)
            }
        }
        private Vector3 forward = Vector3.Forward;

        /// <summary>
        /// Camera's right vector. Points to the right of the camera. Negative right (-right) points
        /// to the left of the camera.
        /// </summary>
        protected Vector3 Right
        {
            get { return right; }

            set
            {
                right = value;
                // Set hasChanged flag to true (if we use a bool)
            }
        }
        private Vector3 right = Vector3.Right;

        /// <summary>
        /// Camera's up vector. Points up from the top of the camera, not up according to the world,
        /// which would always be (0, 0, 1).
        /// </summary>
        protected Vector3 Up
        {
            get { return up; }

            set
            {
                up = value;
                // Set hasChanged flag to true (if we use a bool)
            }
        }
        private Vector3 up = Vector3.Up;

        /// <summary>
        /// Camera's Field-of-view (Degrees). Field of view determines the angle of the camera's
        /// vision. Most FPS style games use between 70-110 degrees.
        /// </summary>
        public float FOV
        {
            protected get { return fov; }
            set
            {
                if (value < float.Epsilon)
                {
                    throw new Exception("FOV (field of view) cannot be zero or a negative value");
                }

                if (value > QSConstants.MaxFOV)
                {
                    throw new Exception("FOV (field of view) cannot be greater than " + QSConstants.MaxFOV + "degrees");
                }

                fov = value;
                // Set hasChanged flag to true (if we use a bool)

                // Update projection matrix
            }
        }
        private float fov = QSConstants.DefaultFOV;

        /// <summary>
        /// Camera's near plane distance. The camera will only draw things between the near and far planes.
        /// The camera's bounding frustum near-plane is based on this value as well.
        /// </summary>
        protected float NearPlane
        {
            get { return nearPlane; }
            set
            {
                if (value >= farPlane)
                {
                    throw new Exception("Near-plane distance cannot be greater than or equal to the far-plane distance");
                }

                if (value < QSConstants.MinNearPlane)
                {
                    throw new Exception("Near-plane distance cannot be less than " + QSConstants.MinNearPlane);
                }

                nearPlane = value;
                // Set hasChanged flag to true (if we use a bool)

                // Update projection matrix
            }
        }
        private float nearPlane = QSConstants.DefaultNearPlane;

        /// <summary>
        /// Camera's far plane distance. The camera will only draw things between the near and far planes.
        /// The camera's bounding frustum far-plane is based on this value as well.
        /// </summary>
        public float FarPlane
        {
            protected get { return farPlane; }
            set
            {
                if (value <= nearPlane)
                {
                    throw new Exception("Far-plane distance cannot be less than or equal to the near-plane distance");
                }

                if (value > QSConstants.MaxFarPlane)
                {
                    throw new Exception("Far-plane distance cannot be greater than " + QSConstants.MaxFarPlane);
                }

                farPlane = value;
                // Set hasChanged flag to true (if we use a bool)

                // Update projection matrix
            }
        }
        private float farPlane = QSConstants.DefaultFarPlane;

        /// <summary>
        /// Camera's bounding frustum. A bounding frustum is a box that is the same size as our
        /// camera's view, because of that it can be used to check collision (whether or not something
        /// is inside of it, or touching it). This allows us to determine is something should be
        /// drawn or not.
        /// </summary>
        public BoundingFrustum Frustum
        {
            get { return frustum; }
            protected set { frustum = value; }
        }
        public BoundingFrustum frustum;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Camera(QSGame game)
            : base(game)
        {
        }

        /// <summary>
        /// Creates a camera.
        /// </summary>
        /// <param name="FOV">Field-of-view, in degrees</param>
        /// <param name="aspectRatio">Aspect Ratio is the screen's width (resolution in pixels) divided by the 
        /// screen's height (resolution in pixels). Most standard monitors and televisions are 4:3, or 1.333r. 
        /// Most widescreen monitors are 16:9, or 1.777r. So something like 1024x768 would be 4:3 because 1024/768 is 1.333r.</param>
        /// <param name="nearPlane">Near plane distance</param>
        /// <param name="farPlane">Far plane distance</param>
        /// <param name="game">The <see cref="QSGame"/> the camera belongs to</param>
        /// <remarks>@todo: exceptions should be changed once a error handling system is in place. - N.Foster</remarks>
        public Camera(QSGame game, float FOV, float aspectRatio, float nearPlane, float farPlane)
            : base(game)
        {
            this.FOV = FOV;
            this.NearPlane = nearPlane;
            this.FarPlane = farPlane;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectRatio, nearPlane, farPlane);

            UpdateFrustum();

            // Set hasChanged flag to true (if we use a bool)
        }

        /// <summary>
        /// Creates a camera.
        /// </summary>
        /// <param name="FOV">Field-of-view, in degrees</param>
        /// <param name="screenWidth">Screen's width (resolution in pixels).</param>
        /// <param name="screenHeight">Screen's height (resolution in pixels).</param>
        /// <param name="nearPlane">Near plane distance</param>
        /// <param name="farPlane">Far plane distance</param>
        /// <param name="game">The <see cref="QSGame"/> the camera belongs to</param>
        public Camera(QSGame game, float FOV, float screenWidth, float screenHeight, float nearPlane, float farPlane)
            : base(game)
        {
            if (screenHeight < float.Epsilon)
            {
                throw new Exception("screenHeight cannot be zero or a negative value");
            }

            if (screenWidth < float.Epsilon)
            {
                throw new Exception("screenWidth cannot be zero or a negative value");
            }

            this.FOV = FOV;
            this.NearPlane = nearPlane;
            this.FarPlane = farPlane;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), screenWidth / screenHeight, nearPlane, farPlane);

            UpdateFrustum();
            // Set hasChanged flag to true (if we use a bool)
        }

        /// <summary>
        /// Updates this camera.        
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        new abstract public void Update(GameTime gameTime);

        /// <summary>
        /// Updates the camera's frustum based on its current view and projection matrices.
        /// </summary>
        protected void UpdateFrustum()
        {
            this.frustum = new BoundingFrustum(viewMatrix * projectionMatrix);
        }

        /// <summary>
        /// Updates the projection matrix based on current camera settings.
        /// </summary>
        protected void UpdateProjMatrix()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectRatio, nearPlane, farPlane);
        }

        /// <summary>
        /// Any actions that should occur on an attachment to an entity should
        /// occur here.
        /// </summary>
        public virtual void ProcessEntityAttach(BaseEntity attachedEntity)
        {
            this.attachedEntity = attachedEntity;
        }

        /// <summary>
        /// Points camera in direction of any position.
        /// </summary>
        /// <param name="targetPos">Target position for camera to face.</param>
        public void LookAt(Vector3 targetPos)
        {
            this.Forward = targetPos - this.Position;

            NormDirectionVectors();     // Now that Forward has changed, update all direction vectors
        }

        /// <summary>
        /// Points camera in direction of its <see cref="attachedEntity"/>
        /// </summary>
        public void LookAtAttached()
        {
            this.Forward = attachedEntity.Position - this.Position;

            NormDirectionVectors();     // Now that Forward has changed, update all direction vectors
        }

        /// <summary>
        /// Updates all rotation vectors and normalizes them.
        /// </summary>
        public void NormDirectionVectors()
        {
            this.forward.Normalize();

            this.right = Vector3.Cross(up, forward);

            this.up = Vector3.Cross(forward, right);
        }

        /// <summary>
        /// Creates a <see cref="Ray"/> from the camera's position in the direction the camera is facing.
        /// </summary>
        /// <returns>Ray in direction camera faces</returns>
        public Ray CreateRayFromForward()
        {
            Ray cameraRay = new Ray(this.Position, this.forward);

            return cameraRay;
        }

        /// <summary>
        /// Create a <see cref="Ray"/> from the camera's position through any other specified position.        
        /// </summary>
        /// <param name="destinationPos">Point to cast a ray through</param>
        /// <returns>Ray to specified position from the camera</returns>
        public Ray CreateRayFromEye(Vector3 destinationPos)
        {
            Vector3 direction = destinationPos - this.Position;

            Ray cameraRay = new Ray(this.Position, direction);

            return cameraRay;
        }
    }
}
