using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XFreeLookCamera : XCamera
    {
        Vector3 movement;
        Vector3 rotation;

        public XFreeLookCamera(XMain X, float NearPlane, float FarPlane) : base(X, NearPlane, FarPlane) { }

        public void Translate(Vector3 Movement)
        {
            movement += Movement;
        }

        public void Rotate(Vector3 Rotation)
        {
            rotation += Rotation;
        }

        public override void Update(GameTime gameTime)
        {
            movement *= new Vector3((float)gameTime.ElapsedGameTime.TotalSeconds);

            RotationMatrix = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);

            Vector3 Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);
            Up = Vector3.Transform(Vector3.Up, RotationMatrix);

            movement = Vector3.Transform(movement, RotationMatrix);
            Position += movement;
            movement = Vector3.Zero;

            Target = Vector3.Add(Position, Forward);

            base.Update(gameTime);
        }
    }
}
