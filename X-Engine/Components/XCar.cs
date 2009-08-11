using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public class XCar : XComponent, XIUpdateable, XIDrawable
    {
        public CarObject Car;
        public XModel Chassis;
        public XModel Wheel;
        XParticleSettings smoke;
        XParticleSystem tailpipe;
        XParticleEmitter tailpipeEmitter;

        public Vector3 Position { get { return Car.PhysicsBody.Position; } }
        public Matrix Orientation { get { return Car.PhysicsBody.Orientation; } }

        public XCar(ref XMain X, XModel Chassis, XModel Wheel, bool FWDrive,               // Does the car have front wheel drive?
            bool RWDrive,               // Does the car have rear wheel drive?
            float maxSteerAngle,        // Max angle that the wheels can turn (wheel lock angle)    
            float steerRate,            // Supposed to be the max rate wheels can turn but doesn't do anything 
            float wheelFSideFriction,   // Lateral tire friction - For an oversteering vehicle... 
            float wheelRSideFriction,   // ...make the front value larger than the rear 
            float wheelFwdFriction,     // Longitutinal tire friction - try to keep these values...
            float wheelRwdFriction,     // ...lower than 2.0f for reliability (larger values may be fine too)
            float handbrakeRSideFriction,//Amount of lateral friction when handbrake is active
            float handbrakeRwdFriction, // Ditto but longitudinal friction (make for real handbrake)
            float startSlideFactor,     // The amount of drift/slide the car has - When the lateral...
            float thresh1SlideFactor,   // ...wheel velocity is greater than slip thresholds the wheel gets...
            float thresh2SlideFactor,   // ...the next slide factor. Factors are how much drift you'll get... 
            float slideThreshold1,      // ...and Thresholds are at what point they will get it. These...
            float slideThreshold2,      // ...change the smallVel variable in Wheel.cs
            float slideSpeed,           // Adjusts speed lost while drifting - make the same as...
            // ...thresh2SlideFactor to lose least speed during drift
            float slipFactor,           // Standard slip factor for lateral and longitudinal friction
            float wheelTravel,          // Total travel range of the suspension
            float wheelRadius,          // Length of the Rays that test wheel collision
            float wheelZOffset,         // Ride height adjustment - Mounting point of wheels up+/down-  
            float wheelRestingFrac,     // Spring rate - Stiffer at 0.1f, softer at 0.9f
            float wheelDampingFrac,     // Shock dampening - More at 0.9f, less at 0.1f
            int wheelNumRays,           // Number of rays testing wheel collision
            float rollResistance,       // Rolling resistance - higher is more resistance
            float topSpeed,             // Uhh, top speed of vehicle (units arbitrary)                      
            float driveTorque,          // Torque of vehicle (units arbitrary)
            float gravity,              // Gravity affect on the car
            
            Vector3 Position)
            : base(ref X)
        {
            //since the car has alphablended windows we set this to true so in the render loop its last
            //this way you can see everything through the window!
            //remember alphablending is draw order dependent!
            AlphaBlendable = true;
            DrawOrder = 100;
            this.Chassis = Chassis;
            this.Wheel = Wheel;

            Car = new CarObject(FWDrive, RWDrive, maxSteerAngle, steerRate,
            wheelFSideFriction, wheelRSideFriction, wheelFwdFriction,
            wheelRwdFriction, handbrakeRSideFriction, handbrakeRwdFriction,
            startSlideFactor, thresh1SlideFactor, thresh2SlideFactor,
            slideThreshold1, slideThreshold2, slideSpeed, slipFactor, wheelTravel,
            wheelRadius, wheelZOffset, wheelRestingFrac, wheelDampingFrac,
            wheelNumRays, rollResistance, topSpeed, driveTorque, gravity);
            
            Car.Car.EnableCar();
            //Car.PhysicsBody.SetDeactivationTime(99999999f);
            Car.Car.Chassis.Body.AllowFreezing = false;
            Car.PhysicsBody.Position = Position;
/*
            //Create tailpipe particle system
            smoke = new XParticleSettings(ref X);
            smoke.ParticleEffectFile = @"Content\XEngine\Effects\ParticleEffect";
            smoke.ParticleTextureFile = @"Content\XEngine\Textures\smoke";
            smoke.TechniqueName = "RotatingParticles";
            smoke.MaxParticles = 600;
            smoke.Duration = TimeSpan.FromSeconds(10);
            smoke.MinHorizontalVelocity = 0;
            smoke.MaxHorizontalVelocity = 15;
            smoke.MinVerticalVelocity = 10;
            smoke.MaxVerticalVelocity = 20;
            // Create a wind effect by tilting the gravity vector sideways.
            smoke.Gravity = new Vector3(-20, -5, 0);
            smoke.EndVelocity = 0.75f;
            smoke.MinRotateSpeed = -1;
            smoke.MaxRotateSpeed = 1;
            smoke.MinStartSize = 5;
            smoke.MaxStartSize = 10;
            smoke.MinEndSize = 50;
            smoke.MaxEndSize = 200;

            tailpipe = new XParticleSystem(ref X, smoke);
            tailpipe.Load(X.Content);
            // Use the particle emitter helper to output our trail particles.
            tailpipeEmitter = new XParticleEmitter(ref X, tailpipe,30, Position);
            tailpipe.DrawOrder = 190;
            tailpipeEmitter.DrawOrder = 180;
 * */
        }

        public override void Update(ref GameTime gameTime)
        {
            //Controlled by physics can change anything here but position, etc that is related to the physics
            
            //TODO:Move to tailpipe position
            //Update tailpipe particle emitters position
            //tailpipeEmitter.newPosition = Car.PhysicsBody.Position;
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
            Chassis.SASData.World = World[0];
            Chassis.SASData.ComputeViewAndProjection();

            X.Renderer.DrawModel(ref Chassis,ref  Camera);

            //the car has alphablended windows turn this off now or else other things go transparent!
            X.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            
            //Set camera params, compute matrices on WHEELS!
            Wheel.SASData.Camera.NearFarClipping.X = Camera.NearPlane;
            Wheel.SASData.Camera.NearFarClipping.Y = Camera.FarPlane;
            Wheel.SASData.Camera.Position.X = Camera.Position.X;
            Wheel.SASData.Camera.Position.Y = Camera.Position.Y;
            Wheel.SASData.Camera.Position.Z = Camera.Position.Z;
            Wheel.SASData.Projection = Camera.Projection;
            Wheel.SASData.View = Camera.View;
            Wheel.SASData.ComputeViewAndProjection();

            Wheel.SASData.World = World[1];
            X.Renderer.DrawModel(ref Wheel,ref Camera);

            Wheel.SASData.World = World[2];
            X.Renderer.DrawModel(ref Wheel,ref Camera);

            Wheel.SASData.World = World[3];
            X.Renderer.DrawModel(ref Wheel,ref Camera);

            Wheel.SASData.World = World[4];
            X.Renderer.DrawModel(ref Wheel,ref Camera);

#if DEBUG
            X.DebugDrawer.DrawShape(Car.GetCollisionWireframe(),Color.White);
#endif

        }
    }
}
