using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XArcBallCamera : XCamera
    {
        // Simply feed this camera the position of whatever you want its target to be
        public Vector3 TargetPosition = Vector3.Zero;

        public float horizontalAngle = MathHelper.PiOver2;
        public float verticalAngle = MathHelper.PiOver2;
        public float verticalAngleMin = 0.01f;
        public float verticalAngleMax = MathHelper.Pi - 0.01f;
        public float zoomMin = 0.1f;
        public float zoomMax = 50.0f;
        public float zoom = 30.0f;

        public XArcBallCamera(ref XMain X, float NearPlane, float FarPlane) : base(ref X, NearPlane, FarPlane)
        {
            
        }

        public void Rotate(float Horizontal, float Vertical)
        {
            horizontalAngle += Horizontal;
            verticalAngle += Vertical;
        }

        public void Zoom(float Zoom)
        {
            zoom += Zoom;
        }

        public void ChangeSettings(float VerticleMin, float VerticleMax, float ZoomMin, float ZoomMax, float VerticleAngle, float HorizontalAngle, float Zoom)
        {
            horizontalAngle = HorizontalAngle;
            verticalAngle = VerticleAngle;
            zoom = Zoom;
            zoomMin = ZoomMin;
            zoomMax = ZoomMax;
            verticalAngleMin = VerticleMin;
            verticalAngleMax = VerticleMax;
        }

        public override void Update(ref GameTime gameTime)
        {
            // Keep vertical angle within tolerances
            verticalAngle = MathHelper.Clamp(verticalAngle, verticalAngleMin, verticalAngleMax);

            // Keep vertical angle within PI
            if (horizontalAngle > MathHelper.TwoPi)
                horizontalAngle -= MathHelper.TwoPi;
            else if (horizontalAngle < 0.0f)
                horizontalAngle += MathHelper.TwoPi;

            // Keep zoom within range
            zoom = MathHelper.Clamp(zoom, zoomMin, zoomMax);

            // Start with an initial offset
            Vector3 cameraPosition = new Vector3(0.0f, zoom, 0.0f);

            // Rotate vertically
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationX(verticalAngle));

            // Rotate horizontally
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationY(horizontalAngle));

            Position = cameraPosition + TargetPosition;
            this.LookAt(TargetPosition);

            Target = Position + this.RotationMatrix.Forward;
            Up = RotationMatrix.Up;

            base.Update(ref gameTime);
        }

        /// <summary>
        /// Points camera in direction of any position.
        /// </summary>
        /// <param name="TargetPosition">Target position for camera to face.</param>
        public void LookAt(Vector3 TargetPosition)
        {
            Vector3 newForward = TargetPosition - this.Position;
            newForward.Normalize();
            this.RotationMatrix.Forward = newForward;

            Vector3 referenceVector = Vector3.UnitY;

            // On the slim chance that the camera is pointer perfectly parallel with the Y Axis, we cannot
            // use cross product with a parallel axis, so we change the reference vector to the forward axis (Z).
            if (this.RotationMatrix.Forward.Y == referenceVector.Y || this.RotationMatrix.Forward.Y == -referenceVector.Y)
            {
                referenceVector = Vector3.UnitZ;
            }

            this.RotationMatrix.Right = Vector3.Cross(this.RotationMatrix.Forward, referenceVector);
            this.RotationMatrix.Up = Vector3.Cross(this.RotationMatrix.Right, this.RotationMatrix.Forward);
        }

    }
}