using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XCar : XComponent, XUpdateable, XDrawable
    {
        CarObject Car;

        XModel Chassis;
        XModel Wheel;

        XMaterial chassismat;
        XMaterial wheelmat;

        public Vector3 Position { get { return Car.PhysicsBody.Position; } }
        public Matrix Orientation { get { return Car.PhysicsBody.Orientation; } }

        public XCar(XMain X, XModel Chassis, XModel Wheel, bool FWDrive, bool RWDrive, float maxSteerAngle, float steerRate, float wheelSideFriction, float wheelFwdFriction, float wheelTravel, float wheelRadius, float wheelZOffset, float wheelRestingFrac, float wheelDampingFrac, int wheelNumRays, float driveTorque, float gravity, Vector3 Position)
            : base(X)
        {
            this.Chassis = Chassis;
            this.Wheel = Wheel;

            Car = new CarObject(FWDrive, RWDrive, maxSteerAngle, steerRate, wheelSideFriction, wheelFwdFriction, wheelTravel, wheelRadius, wheelZOffset, wheelRestingFrac, wheelDampingFrac, wheelNumRays, driveTorque, gravity);
            Car.Car.EnableCar();

            Car.PhysicsBody.Position = Position;

            chassismat = new XMaterial(X, Chassis.Model.Meshes[1].Effects[0].Parameters["Texture"].GetValueTexture2D(), true, null, false, null, false, 100);
            wheelmat = new XMaterial(X, Wheel.Model.Meshes[0].Effects[0].Parameters["Texture"].GetValueTexture2D(), true, null, false, null, false, 1000000000);
        }

        Vector2 Acceleration = Vector2.Zero;

        public void Accelerate(float Amount)
        {
            Acceleration.Y += Amount;
        }

        public void Steer(float Amount)
        {
            Acceleration.X += Amount;
        }

        public override void Update(GameTime gameTime)
        {
            if (Acceleration.Y != 0)
                Car.Car.Accelerate = Acceleration.Y;
            else
                Car.Car.Accelerate = 0;

            if (Acceleration.X != 0)
                Car.Car.Steer = -Acceleration.X;
            else
                Car.Car.Steer = 0;

            Acceleration = Vector2.Zero;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, XCamera Camera)
        {
            Matrix[] World = Car.GetWorldMatrix(Chassis.Model, Vector3.Zero);

            X.Renderer.DrawModel(Chassis, Camera, new Matrix[] { World[0] }, chassismat);

            X.Renderer.DrawModel(Wheel, Camera, new Matrix[] { World[1] } , wheelmat);
            X.Renderer.DrawModel(Wheel, Camera, new Matrix[] { World[2] } , wheelmat);
            X.Renderer.DrawModel(Wheel, Camera, new Matrix[] { World[3] } , wheelmat);
            X.Renderer.DrawModel(Wheel, Camera, new Matrix[] { World[4] } , wheelmat);
        }
    }
}
