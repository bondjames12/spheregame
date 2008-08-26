using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public class XCar : XComponent, XUpdateable, XDrawable
    {
        public CarObject Car;

        public XModel Chassis;
        public XModel Wheel;

        public Vector3 Position { get { return Car.PhysicsBody.Position; } }
        public Matrix Orientation { get { return Car.PhysicsBody.Orientation; } }
        Vector2 Acceleration = Vector2.Zero;

        public XCar(ref XMain X, XModel Chassis, XModel Wheel, bool FWDrive, bool RWDrive, float maxSteerAngle,
            float steerRate, float wheelSideFriction, float wheelFwdFriction, float wheelTravel,
            float wheelRadius, float wheelZOffset, float wheelRestingFrac, float wheelDampingFrac,
            int wheelNumRays, float driveTorque, float gravity, Vector3 Position)
            : base(ref X)
        {
            //since the car has alphablended windows we set this to true so in the render loop its last
            //this way you can see everything through the window!
            //remember alphablending is draw order dependent!
            AlphaBlendable = true;
            DrawOrder = 100;
            this.Chassis = Chassis;
            this.Wheel = Wheel;

            Car = new CarObject(FWDrive, RWDrive, maxSteerAngle, steerRate, wheelSideFriction, wheelFwdFriction,
                wheelTravel, wheelRadius, wheelZOffset, wheelRestingFrac, wheelDampingFrac, wheelNumRays,
                driveTorque, gravity);
            Car.Car.EnableCar();
            //Car.PhysicsBody.SetDeactivationTime(99999999f);
            Car.PhysicsBody.Position = Position;
        }

        public void Accelerate(float Amount)
        {
            Acceleration.Y += Amount;
        }

        public void Steer(float Amount)
        {
            Acceleration.X += Amount;
        }

        public override void Update(ref GameTime gameTime)
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

        public override void Draw(ref GameTime gameTime, ref  XCamera Camera)
        {
            //World Matrix Compute function
            Matrix[] World = Car.GetWorldMatrix(Chassis.Model, Vector3.Zero);

            //Set camera params, compute matrices on chassis
            Chassis.SASData.Camera.NearFarClipping.X = Camera.NearPlane;
            Chassis.SASData.Camera.NearFarClipping.Y = Camera.FarPlane;
            Chassis.SASData.Camera.Position.X = Camera.Position.X;
            Chassis.SASData.Camera.Position.Y = Camera.Position.Y;
            Chassis.SASData.Camera.Position.Z = Camera.Position.Z;
            Chassis.SASData.Projection = Camera.Projection;
            Chassis.SASData.View = Camera.View;
            Chassis.SASData.Model = World[0];
            Chassis.SASData.ComputeViewAndProjection();

            X.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            X.Renderer.DrawModel(ref Chassis,ref  Camera);

            //the car has alphablended windows turn this off now or else other things go transparent!
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            if (DebugMode)
            {
                //Draw Bounding Box/Frustum
                //X.DebugDrawer.DrawCube(Car.PhysicsBody.CollisionSkin.WorldBoundingBox.Min, Car.PhysicsBody.CollisionSkin.WorldBoundingBox.Max, Color.White, Matrix.Identity, Camera);
                //X.DebugDrawer.DrawLine(Vector3.Zero, Car.PhysicsBody.Position, Color.Blue);
                //X.DebugDrawer.DrawLine(Vector3.Zero, Car.PhysicsSkin.NewPosition, Color.Red);
            }
            
            //Set camera params, compute matrices on WHEELS!
            Wheel.SASData.Camera.NearFarClipping.X = Camera.NearPlane;
            Wheel.SASData.Camera.NearFarClipping.Y = Camera.FarPlane;
            Wheel.SASData.Camera.Position.X = Camera.Position.X;
            Wheel.SASData.Camera.Position.Y = Camera.Position.Y;
            Wheel.SASData.Camera.Position.Z = Camera.Position.Z;
            Wheel.SASData.Projection = Camera.Projection;
            Wheel.SASData.View = Camera.View;
            Wheel.SASData.ComputeViewAndProjection();

            Wheel.SASData.Model = World[1];
            X.Renderer.DrawModel(ref Wheel,ref Camera);

            Wheel.SASData.Model = World[2];
            X.Renderer.DrawModel(ref Wheel,ref Camera);

            Wheel.SASData.Model = World[3];
            X.Renderer.DrawModel(ref Wheel,ref Camera);

            Wheel.SASData.Model = World[4];
            X.Renderer.DrawModel(ref Wheel,ref Camera);

        }
    }
}
