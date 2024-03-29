﻿using Microsoft.Xna.Framework;
using System;

namespace XEngine
{
    public class XChaseCamera : XCamera
    {
        public Vector3 ChaseTargetPosition;
        public Vector3 ChaseTargetForward;
        public Vector3 DesiredPositionOffest = new Vector3(10.0f, 4f, 0.0f);
        public Vector3 DesiredPosition;
        public Vector3 LookAtOffset = new Vector3(0, 4f, 0);
        public float Stiffness = 1800.0f;
        public float Damping = 600.0f;
        public float Mass = 50.0f;
        public Vector3 Velocity;

        public XChaseCamera(ref XMain X, float NearPlane, float FarPlane) : base(ref X, NearPlane, FarPlane) { }

        public override void Update(ref GameTime gameTime)
        {
            Matrix transform = Matrix.Identity;
            transform.Forward = ChaseTargetForward;
            transform.Up = Up;
            transform.Right = Vector3.Cross(Up, ChaseTargetForward);

            DesiredPosition = ChaseTargetPosition + Vector3.TransformNormal(DesiredPositionOffest, transform);
            Target = ChaseTargetPosition + Vector3.TransformNormal(LookAtOffset, transform);

            Vector3 Stretch = Position - DesiredPosition;
            if (Stretch.Length() > 0.05)
            {
                Vector3 Force = -Stiffness * Stretch - Damping * Velocity;

                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector3 Acceleration = Force / Mass;
                Velocity += Acceleration * elapsed;
                Position += Velocity * elapsed;
            }

            base.Update(ref gameTime);
        }
    }
}
