using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Vehicles;
using JigLibX.Collision;
using System.Collections;

namespace XEngine
{
    public class CarObject : XPhysicsObject
    {
        private Car car;

        public CarObject(bool FWDrive,               // Does the car have front wheel drive?
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
            float gravity              // Gravity affect on the car)
            )
        {
            car = new Car(FWDrive, RWDrive, maxSteerAngle, steerRate,
            wheelFSideFriction, wheelRSideFriction, wheelFwdFriction,
            wheelRwdFriction, handbrakeRSideFriction, handbrakeRwdFriction,
            startSlideFactor, thresh1SlideFactor, thresh2SlideFactor,
            slideThreshold1, slideThreshold2, slideSpeed, slipFactor, wheelTravel,
            wheelRadius, wheelZOffset, wheelRestingFrac, wheelDampingFrac,
            wheelNumRays, rollResistance, topSpeed, driveTorque, gravity);

            this.body = car.Chassis.Body;
            this.collision = car.Chassis.Skin;
         
            SetCarMass(800.0f);

            //car.Chassis.Skin.SetMaterialProperties((int)MaterialTable.MaterialID.UserDefined, new MaterialProperties(0.4f,0.8f,0.7f));

        }


        public Matrix[] GetWorldMatrix(Model model, Vector3 Offset)
        {
            Matrix[] World = new Matrix[5];

            World[0] = base.GetWorldMatrix(model, Offset);

            // Draw wheels!
            //Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            //model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            Wheel wh;

            #region wh1
            wh = car.Wheels[0];

            Matrix rotMat = Matrix.Identity;
            rotMat = Matrix.CreateRotationY(MathHelper.ToRadians(180.0f));

            float steer = wh.SteerAngle;

            World[1] = rotMat * // create scale
                       Matrix.CreateRotationZ(MathHelper.ToRadians(-wh.AxisAngle)) * // rotate the wheels
                       Matrix.CreateRotationY(MathHelper.ToRadians(steer)) *
                       Matrix.CreateTranslation(wh.Pos + wh.Displacement * wh.LocalAxisUp) * car.Chassis.Body.Orientation * // oritentation of wheels
                       Matrix.CreateTranslation(car.Chassis.Body.Position); // translation

            
            #endregion
            
            #region wh2
            wh = car.Wheels[1];
            steer = wh.SteerAngle;

            World[2] = rotMat * // create scale
                        Matrix.CreateRotationZ(MathHelper.ToRadians(-wh.AxisAngle)) * // rotate the wheels
                        Matrix.CreateRotationY(MathHelper.ToRadians(steer)) *
                        Matrix.CreateTranslation(wh.Pos + wh.Displacement * wh.LocalAxisUp) * car.Chassis.Body.Orientation * // oritentation of wheels
                        Matrix.CreateTranslation(car.Chassis.Body.Position); // translation

            #endregion
            
            #region wh3
            wh = car.Wheels[2];
            steer = wh.SteerAngle;
            rotMat = Matrix.Identity;

            World[3] = rotMat * // create scale
                        Matrix.CreateRotationZ(MathHelper.ToRadians(-wh.AxisAngle)) * // rotate the wheels
                        Matrix.CreateRotationY(MathHelper.ToRadians(steer)) *
                        Matrix.CreateTranslation(wh.Pos + wh.Displacement * wh.LocalAxisUp) * car.Chassis.Body.Orientation * // oritentation of wheels
                        Matrix.CreateTranslation(car.Chassis.Body.Position); // translation
            #endregion 

            #region wh4
            wh = car.Wheels[3];
            steer = wh.SteerAngle;

            World[4] = rotMat * // create scale
                        Matrix.CreateRotationZ(MathHelper.ToRadians(-wh.AxisAngle)) * // rotate the wheels
                        Matrix.CreateRotationY(MathHelper.ToRadians(steer)) *
                        Matrix.CreateTranslation(wh.Pos + wh.Displacement * wh.LocalAxisUp) * car.Chassis.Body.Orientation * // oritentation of wheels
                        Matrix.CreateTranslation(car.Chassis.Body.Position); // translation
            #endregion

            return World;
        }

        public Car Car
        {
            get { return this.car; }
        }

        private void SetCarMass(float mass)
        {
            body.Mass = mass;
            Vector3 min, max;
            car.Chassis.GetDims(out min, out max);
            Vector3 sides = max - min;

            float Ixx = (1.0f / 12.0f) * mass * (sides.Y * sides.Y + sides.Z * sides.Z);
            float Iyy = (1.0f / 12.0f) * mass * (sides.X * sides.X + sides.Z * sides.Z);
            float Izz = (1.0f / 12.0f) * mass * (sides.X * sides.X + sides.Y * sides.Y);

            Matrix inertia = Matrix.Identity;
            inertia.M11 = Ixx; 
            inertia.M22 = Iyy; 
            inertia.M33 = Izz;
            car.Chassis.Body.BodyInertia = inertia;
            car.SetupDefaultWheels();
        }
    }
}
