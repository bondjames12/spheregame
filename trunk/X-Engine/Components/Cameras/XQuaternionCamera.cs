//
//      coded by un
//            --------------
//                     mindshifter.com
//

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    /// <summary>
    /// Defines the vector space in which camera transforms are performed
    /// </summary>
    public enum TransformSpace
    {
        CameraSpace,
        WorldSpace,
    }

    /// <summary>
    /// QuaternionCamera is a input-based vector camera with support for translations
    /// and rotations in both world and local space
    /// </summary>
    public sealed class XQuaternionCamera : XCamera, XICamera
    {
        //private Matrix          mViewMatrix;
        private Matrix          mInverseViewMatrix;
        //private Matrix          mProjectionMatrix;

        //private float           mFieldOfView;
        //private float           mAspectRatio;
        //private float           mNearPlaneDistance;
        //private float           mFarPlaneDistance;

        //private Vector3         mPosition;
        private Quaternion      mOrientation;

        private bool            mBuildView;
        private bool            mBuildProjection;

        /// <summary>
        /// Gets the camera's view matrix
        /// </summary>
        public Matrix ViewMatrix
        {
            get
            {
                if (mBuildView)
                    BuildViewMatrix();

                return View;
            }
        }

        /// <summary>
        /// Gets the camera's inverse view matrix
        /// </summary>
        public Matrix InverseViewMatrix
        {
            get
            {
                if (mBuildView)
                    BuildViewMatrix();

                return mInverseViewMatrix;
            }
        }

        /// <summary>
        /// Gets the camera's projection matrix
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get 
            {
                if (mBuildProjection)
                    BuildProjectionMatrix();

                return Projection; 
            }
        }

        /// <summary>
        /// Gets or sets the camera's field of view
        /// </summary>
        public float FieldOfView
        {
            get { return FOV; }
            set
            {
                FOV = value;
                mBuildProjection = true;
            }
        }

        /// <summary>
        /// Gets or sets the camera's aspect ratio
        /// </summary>
        public float aspectratio
        {
            get { return AspectRatio; }
            set
            {
                AspectRatio = value;
                mBuildProjection = true;
            }
        }

        /// <summary>
        /// Gets or sets the camera's near frustum plane distance
        /// </summary>
        public float NearPlaneDistance
        {
            get { return NearPlane; }
            set
            {
                NearPlane = value;
                mBuildProjection = true;
            }
        }

        /// <summary>
        /// Gets or sets the camera's far frustum plane distance
        /// </summary>
        public float FarPlaneDistance
        {
            get { return FarPlane; }
            set
            {
                FarPlane = value;
                mBuildProjection = true;
            }
        }

        /// <summary>
        /// Gets or sets the camera's position
        /// </summary>
        public Vector3 position
        {
            get { return Position; }
            set
            {
                Position = value;
                mBuildView = true;
            }
        }

        /// <summary>
        /// Gets or sets the camera's orientation
        /// </summary>
        public Quaternion Orientation
        {
            get { return mOrientation; }
            set
            {
                Quaternion.Normalize(ref value, out mOrientation);
                mBuildView = true;
            }
        }

        /// <summary>
        /// Gets or sets the camera's world X position
        /// </summary>
        public float X
        {
            get { return Position.X; }
            set
            {
                Position.X = value;
                mBuildView = true;
            }
        }

        /// <summary>
        /// Gets or sets the camera's world Y position
        /// </summary>
        public float Y
        {
            get { return Position.Y; }
            set
            {
                Position.Y = value;
                mBuildView = true;
            }
        }

        /// <summary>
        /// Gets or sets the camera's world Z position
        /// </summary>
        public float Z
        {
            get { return Position.Z; }
            set
            {
                Position.Z = value;
                mBuildView = true;
            }
        }

        /// <summary>
        /// Gets the camera's up vector
        /// </summary>
        public Vector3 Up
        {
            get
            {
                if (mBuildView)
                    BuildViewMatrix();

                return mInverseViewMatrix.Up;
            }
        }

        /// <summary>
        /// Gets the camera's forward vector
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                if (mBuildView)
                    BuildViewMatrix();

                return mInverseViewMatrix.Forward;
            }
        }

        /// <summary>
        /// Gets the camera's right vector
        /// </summary>
        public Vector3 Right
        {
            get
            {
                if (mBuildView)
                    BuildViewMatrix();

                return mInverseViewMatrix.Right;
            }
        }

        /// <summary>
        /// Gets the camera's bounding frustum
        /// </summary>
        public BoundingFrustum Frustum
        {
            get { return new BoundingFrustum(ViewMatrix * ProjectionMatrix); }
        }

        /// <summary>
        /// Creates a new instance of QuaternionCamera with a position at the origin looking
        /// down the negative z axis using a perspective projection with a 45 degree field of 
        /// view, 4/3 aspect ratio, near plane distance of 1 and far plane distance of 1000
        /// </summary>
        public XQuaternionCamera(ref XMain X) 
            : this(ref X, Vector3.Zero, Quaternion.Identity, (float)Math.PI * 0.25f, 4f / 3f, 1f, 1000f)
        {

        }

        /// <summary>
        /// Creates a new instance of QuaternionCamera with the specified position
        /// and orientation using a perspective projection with a 45 degree field of view, 4/3
        /// aspect ratio, near plane distance of 1 and far plane distance of 1000
        /// </summary>
        /// <param name="position">The position of the camera's eye point</param>
        /// <param name="orientation">The orientation of the camera</param>
        public XQuaternionCamera(ref XMain X, Vector3 position, Quaternion orientation)
            : this(ref X,position, orientation, (float)Math.PI * 0.25f, 4f / 3f, 1f, 1000f)
        {

        }

        /// <summary>
        /// Creates a new instance of QuaternionCamera with a position at the origin
        /// using a perspective projection with the specified projection parameters
        /// </summary>
        /// <param name="position">The position of the camera's eye point</param>
        /// <param name="orientation">The orientation of the camera</param>
        /// <param name="fov">The camera's perspective projection field of view</param>
        /// <param name="aspect">The camera's perspective projection aspect ratio</param>
        /// <param name="near">The near frustum plane distance of the camera's projection</param>
        /// <param name="far">The far frustum plane distance of the camera's projection</param>
        public XQuaternionCamera(ref XMain X, Vector3 position, Quaternion orientation, float fov, 
                                    float aspect, float near, float far) : base(ref X, near, far)
        {
            Position = position;
            mOrientation = orientation;
            View = Matrix.Identity;
            mInverseViewMatrix = Matrix.Identity;
            Projection = Matrix.Identity;

            FOV = fov;
            AspectRatio = aspect;
            NearPlane = near;
            FarPlane = far;

            mBuildView = true;
            mBuildProjection = true;
        }

        /// <summary>
        /// Pitches the camera's orientation (x axis rotation)
        /// </summary>
        /// <param name="pitchAmount">The angle to pitch the camera's orientation (in radians)</param>
        /// <param name="space">The vector space in which to perform the transform</param>
        public void Pitch(float pitchAmount, TransformSpace space)
        {
            if (mBuildView)
                BuildViewMatrix();

            switch (space)
            {
                case TransformSpace.CameraSpace:
                    mOrientation *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(mInverseViewMatrix.Right), pitchAmount);
                    break;

                case TransformSpace.WorldSpace:
                    mOrientation *= Quaternion.CreateFromAxisAngle(Vector3.Right, pitchAmount);
                    break;
            }

            mBuildView = true;
        }

        /// <summary>
        /// Pitches the camera's orientation in camera space (x axis rotation)
        /// </summary>
        /// <param name="pitchAmount">The angle to pitch the camera's orientation (in radians)</param>
        public void Pitch(float pitchAmount)
        {
            Pitch(pitchAmount, TransformSpace.CameraSpace);
        }

        /// <summary>
        /// Yaws the camera's orientation (y axis rotation)
        /// </summary>
        /// <param name="yawAmount">The angle to yaw the camera's orientation (in radians)</param>
        /// <param name="space">The vector space in which to perform the transform</param>
        public void Yaw(float yawAmount, TransformSpace space)
        {
            if (mBuildView)
                BuildViewMatrix();

            switch (space)
            {
                case TransformSpace.CameraSpace:
                    mOrientation *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(mInverseViewMatrix.Up), yawAmount);
                    break;

                case TransformSpace.WorldSpace:
                    mOrientation *= Quaternion.CreateFromAxisAngle(Vector3.Up, yawAmount);
                    break;
            }

            mBuildView = true;
        }

        /// <summary>
        /// Yaws the camera's orientation in camera space (y axis rotation)
        /// </summary>
        /// <param name="yawAmount">The angle to yaw the camera's orientation (in radians)</param>
        public void Yaw(float yawAmount)
        {
            Yaw(yawAmount, TransformSpace.CameraSpace);
        }

        /// <summary>
        /// Rolls the camera's orientation (z axis rotation)
        /// </summary>
        /// <param name="rollAmount">The angle to roll the camera's orientation (in radians)</param>
        /// <param name="space">The vector space in which to perform the transform</param>
        public void Roll(float rollAmount, TransformSpace space)
        {
            if (mBuildView)
                BuildViewMatrix();

            switch (space)
            {
                case TransformSpace.CameraSpace:
                    mOrientation *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(mInverseViewMatrix.Backward), rollAmount);
                    break;

                case TransformSpace.WorldSpace:
                    mOrientation *= Quaternion.CreateFromAxisAngle(Vector3.Backward, rollAmount);
                    break;
            }

            mBuildView = true;
        }

        /// <summary>
        /// Rolls the camera's orientation in camera space (z axis rotation)
        /// </summary>
        /// <param name="rollAmount">The angle to roll the camera's orientation (in radians)</param>
        public void Roll(float rollAmount)
        {
            Roll(rollAmount, TransformSpace.CameraSpace);
        }

        /// <summary>
        /// Translates the camera's position by the specified vector
        /// </summary>
        /// <param name="amount">The amount by which to translate the camera's position</param>
        /// <param name="space">The vector space in which to perform the transform</param>
        public void Translate(Vector3 amount, TransformSpace space)
        {
            if (mBuildView)
                BuildViewMatrix();

            switch (space)
            {
                case TransformSpace.CameraSpace:
                    Position += mInverseViewMatrix.Right * amount.X +
                        mInverseViewMatrix.Up * amount.Y + mInverseViewMatrix.Backward * amount.Z;
                    break;

                case TransformSpace.WorldSpace:
                    Position += amount;
                    break;
            }

            mBuildView = true;
        }

        /// <summary>
        /// Translates the camera's position by the specified vector in camera space
        /// </summary>
        /// <param name="amount">The amount by which to translate the camera's position</param>
        public void Translate(Vector3 amount)
        {
            Translate(amount, TransformSpace.CameraSpace);
        }

        /// <summary>
        /// Translates the camera's x position by the specified amount
        /// </summary>
        /// <param name="amount">The amount by which to translate the camera's x position</param>
        /// <param name="space">The vector space in which to perform the transform</param>
        public void TranslateX(float amount, TransformSpace space)
        {
            if (mBuildView)
                BuildViewMatrix();

            switch (space)
            {
                case TransformSpace.CameraSpace:
                    Position += mInverseViewMatrix.Right * amount;
                    break;

                case TransformSpace.WorldSpace:
                    Position.X += amount;
                    break;
            }

            mBuildView = true;
        }

        /// <summary>
        /// Translates the camera's x position by the specified amount in camera space
        /// </summary>
        /// <param name="amount">The amount by which to translate the camera's x position</param>
        public void TranslateX(float amount)
        {
            TranslateX(amount, TransformSpace.CameraSpace);
        }

        /// <summary>
        /// Translates the camera's y position by the specified amount
        /// </summary>
        /// <param name="amount">The amount by which to translate the camera's y position</param>
        /// <param name="space">The vector space in which to perform the transform</param>
        public void TranslateY(float amount, TransformSpace space)
        {
            if (mBuildView)
                BuildViewMatrix();

            switch (space)
            {
                case TransformSpace.CameraSpace:
                    Position += mInverseViewMatrix.Up * amount;
                    break;

                case TransformSpace.WorldSpace:
                    Position.Y += amount;
                    break;
            }

            mBuildView = true;
        }

        /// <summary>
        /// Translates the camera's y position by the specified amount in camera space
        /// </summary>
        /// <param name="amount">The amount by which to translate the camera's y position</param>
        public void TranslateY(float amount)
        {
            TranslateY(amount, TransformSpace.CameraSpace);
        }

        /// <summary>
        /// Translates the camera's z position by the specified amount
        /// </summary>
        /// <param name="amount">The amount by which to translate the camera's z position</param>
        /// <param name="space">The vector space in which to perform the transform</param>
        public void TranslateZ(float amount, TransformSpace space)
        {
            if (mBuildView)
                BuildViewMatrix();

            switch (space)
            {
                case TransformSpace.CameraSpace:
                    Position += mInverseViewMatrix.Backward * amount;
                    break;

                case TransformSpace.WorldSpace:
                    Position.Z += amount;
                    break;
            }

            mBuildView = true;
        }

        /// <summary>
        /// Translates the camera's z position by the specified amount in camera space
        /// </summary>
        /// <param name="amount">The amount by which to translate the camera's z position</param>
        public void TranslateZ(float amount)
        {
            TranslateZ(amount, TransformSpace.CameraSpace);
        }

        /// <summary>
        /// Builds the camera's view matrix
        /// </summary>
        private void BuildViewMatrix()
        {
            Vector3 P;
            Quaternion O;
            Matrix T;
            Matrix R;

            Vector3.Negate(ref Position, out P);
            Quaternion.Negate(ref mOrientation, out O);

            Matrix.CreateTranslation(ref P, out T);
            Matrix.CreateFromQuaternion(ref O, out R);

            Matrix.Multiply(ref T, ref R, out View);
            Matrix.Invert(ref View, out mInverseViewMatrix);

            mBuildView = false;
        }

        /// <summary>
        /// Builds the camera's projection matrix
        /// </summary>
        private void BuildProjectionMatrix()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(FOV,
                AspectRatio, NearPlane, FarPlane);

            mBuildProjection = false;
        }

        /// <summary>
        /// Performs any required updates to the camera's values based on the specified elapsed time
        /// </summary>
        /// <param name="elapsedSeconds">The elapsed time in seconds since the last call to Update()</param>
        public override void Update(ref GameTime gameTime)
        {
            if (mBuildView)
                BuildViewMatrix();

            if (mBuildProjection)
                BuildProjectionMatrix();
        }

        /// <summary>
        /// Returns a world space ray cast from the specified screen projected position
        /// </summary>
        /// <param name="position">The position in screen space to cast a ray from</param>
        /// <param name="viewport">The viewport through which to unproject the specified position</param>
        public Ray GetPickRay(Vector2 position, Viewport viewport)
        {
            Vector3 near = viewport.Unproject(new Vector3(position, 0f), ProjectionMatrix,
                                ViewMatrix, Matrix.Identity);

            Vector3 far = viewport.Unproject(new Vector3(position, 1f), ProjectionMatrix,
                                ViewMatrix, Matrix.Identity);

            return new Ray(near, Vector3.Normalize(far - near));
        }
    }
}