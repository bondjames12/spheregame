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
    public class CarObject : PhysicsObject
    {

        private Car car;

        public CarObject(bool FWDrive,
                       bool RWDrive,
                       float maxSteerAngle,
                       float steerRate,
                       float wheelSideFriction,
                       float wheelFwdFriction,
                       float wheelTravel,
                       float wheelRadius,
                       float wheelZOffset,
                       float wheelRestingFrac,
                       float wheelDampingFrac,
                       int wheelNumRays,
                       float driveTorque,
                       float gravity)
        {
            car = new Car(FWDrive, RWDrive, maxSteerAngle, steerRate,
                wheelSideFriction, wheelFwdFriction, wheelTravel, wheelRadius,
                wheelZOffset, wheelRestingFrac, wheelDampingFrac,
                wheelNumRays, driveTorque, gravity);

            this.body = car.Chassis.Body;
            this.collision = car.Chassis.Skin;
            
            SetCarMass(500.0f);

            //car.Chassis.Skin.SetMaterialProperties((int)MaterialTable.MaterialID.UserDefined, new MaterialProperties(0.4f,0.8f,0.7f));

            Data.Add("BasicEffect");
        }

        ArrayList Data = new ArrayList();
        public Matrix[] GetWorldMatrix(Model model, Vector3 Offset)
        {
            Matrix[] World = new Matrix[5];

            World[0] = base.GetWorldMatrix(model, Offset);

            // Draw wheels!
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

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
            inertia.M11 = Ixx; inertia.M22 = Iyy; inertia.M33 = Izz;
            car.Chassis.Body.BodyInertia = inertia;
            car.SetupDefaultWheels();
        }
    }
}
