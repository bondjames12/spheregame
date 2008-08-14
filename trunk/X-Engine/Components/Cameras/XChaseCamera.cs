using Microsoft.Xna.Framework;
using System;

namespace XEngine
{
    public class XChaseCamera : XCamera
    {
        public Vector3 ChaseTargetPosition;
        public Vector3 ChaseTargetForward;
        public Vector3 DesiredPositionOffest = new Vector3(8.0f, 3.5f, 0.0f);
        public Vector3 DesiredPosition;
        public Vector3 LookAtOffset = new Vector3(0, 3.2f, 0);
        public Vector3 LookAt;
        public float Stiffness = 1800.0f;
        public float Damping = 600.0f;
        public float Mass = 50.0f;
        public Vector3 Velocity;

        public XChaseCamera(XMain X, float NearPlane, float FarPlane) : base(X, NearPlane, FarPlane) { }

        public override void Update(ref GameTime gameTime)
        {
            Matrix transform = Matrix.Identity;
            transform.Forward = ChaseTargetForward;
            transform.Up = Up;
            transform.Right = Vector3.Cross(Up, ChaseTargetForward);

            DesiredPosition = ChaseTargetPosition + Vector3.TransformNormal(DesiredPositionOffest, transform);
            LookAt = ChaseTargetPosition + Vector3.TransformNormal(LookAtOffset, transform);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 Stretch = Position - DesiredPosition;
            Vector3 Force = -Stiffness * Stretch - Damping * Velocity;

            Vector3 Acceleration = Force / Mass;
            Velocity += Acceleration * elapsed;

            Position += Velocity * elapsed;

            Target = LookAt;

            RotationMatrix = Matrix.CreateFromYawPitchRoll(Direction.X, Direction.Y, 0);

            Matrix rotation = Matrix.Identity;
            rotation.Forward = Target - Position;
            rotation.Up = Vector3.Cross(Vector3.Up, rotation.Forward);
            rotation.Right = Vector3.Cross(Vector3.Left, rotation.Forward);
            RotationMatrix = rotation;

            Up = Vector3.Transform(Vector3.Up, rotation);

            base.Update(ref gameTime);
        }
    }
}
