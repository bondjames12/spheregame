/*
 * QSMathHelper.cs
 * 
 * This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
 * for license details.
 */

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace QuickStart
{
    /// <summary>
    /// Class for helping with common math functions
    /// </summary>
    public static class QSMath
    {
        /// <summary>
        /// Used for creating random numbers
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// Returns a direction vector from one point to another.
        /// </summary>
        /// <param name="posFirst">First position</param>
        /// <param name="posSecond">Second position</param>
        /// <returns>Direction vector from first position to second position</returns>
        public static Vector3 DirectionFirstToSecond(Vector3 posFirst, Vector3 posSecond)
        {
            return Vector3.Normalize(posSecond - posFirst);
        }

        /// <summary>
        /// Finds the distance between two points in 3D space.
        /// </summary>
        /// <param name="posFirst">First position</param>
        /// <param name="posSecond">Second position</param>
        /// <returns>Distance between the two positions.</returns>
        public static float DistanceVectToVect(Vector3 posFirst, Vector3 posSecond)
        {
            Vector3 diffVect = posFirst - posSecond;
            return diffVect.Length();
        }

        /// <summary>
        /// Finds the distance between two points in 3D space.
        /// </summary>
        /// <param name="posFirst">First position</param>
        /// <param name="posSecond">Second position</param>
        /// <returns>Distance between the two positions.</returns>
        public static float DistanceVectToVect(Vector3 posFirst, ref Vector3 posSecond)
        {
            Vector3 diffVect = posFirst - posSecond;
            return diffVect.Length();
        }

        /// <summary>
        /// Finds the distance between two points in 3D space.
        /// </summary>
        /// <param name="posFirst">First position</param>
        /// <param name="posSecond">Second position</param>
        /// <returns>Distance between the two positions.</returns>
        public static float DistanceVectToVect(ref Vector3 posFirst, ref Vector3 posSecond)
        {
            Vector3 diffVect = posFirst - posSecond;
            return diffVect.Length();
        }

        /// <summary>
        /// Checks if a integer is a power-of-two value (i.e. 2, 4, 8, 16, etc...)
        /// </summary>
        /// <param name="Value">Value to check for power-of-two</param>
        /// <returns>True if number is a power-of-two, otherwise returns false</returns>
        public static bool IsPowerOf2(int Value)
        {
            if (Value < 2)
            {
                return false;
            }

            if ((Value & (Value - 1)) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a number between two values.
        /// </summary>
        /// <param name="min">Lower bound value</param>
        /// <param name="max">Upper bound value</param>
        /// <returns>Random number between bounds.</returns>
        public static float RandomBetween(double min, double max)
        {
            return (float)(min + (float)random.NextDouble() * (max - min));
        }

        /// <summary>
        /// 50/50 chance of returning either -1 or 1
        /// </summary>
        public static int Random5050
        {
            get
            {
                if (RandomBetween(0, 2) >= 1)
                    return 1;
                else
                    return -1;
            }
        }

        /// <summary>
        /// Helper function chooses a random location on a triangle.
        /// </summary>
        public static void PickRandomPoint(Vector3 position1, Vector3 position2, Vector3 position3,
                                           out Vector3 randomPosition)
        {
            float a = (float)random.NextDouble();
            float b = (float)random.NextDouble();

            if (a + b > 1)
            {
                a = 1 - a;
                b = 1 - b;
            }

            randomPosition = Vector3.Barycentric(position1, position2, position3, a, b);
        }

        /// <summary>
        /// Check if a point lies inside a <see cref="BoundingBox"/>
        /// </summary>
        /// <param name="point">3D Point</param>
        /// <param name="box">Bounding box</param>
        /// <returns>True if point lies inside the bounding box</returns>
        public static bool PointInsideBoundingBox(Vector3 point, BoundingBox box)
        {
            if (point.X < box.Min.X)
            {
                return false;
            }

            if (point.Y < box.Min.Y)
            {
                return false;
            }

            if (point.Z < box.Min.Z)
            {
                return false;
            }

            if (point.X > box.Max.X)
            {
                return false;
            }

            if (point.Y > box.Max.Y)
            {
                return false;
            }

            if (point.Z > box.Max.Z)
            {
                return false;
            }

            // Point must be inside box
            return true;
        }

        /// <summary>
        /// Check if a point lies inside a conical region. Good for checking if a point lies in something's
        /// field-of-view cone.
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <param name="coneOrigin">Cone's origin</param>
        /// <param name="coneDirection">Cone's forward direction</param>
        /// <param name="coneAngle">Cone's theta angle (radians)</param>
        /// <returns>True if point is inside the conical region</returns>
        public static bool PointInsideCone(Vector3 point, Vector3 coneOrigin, Vector3 coneDirection, double coneAngle)
        {
            Vector3 tempVect = Vector3.Normalize(point - coneOrigin);

            return Vector3.Dot(coneDirection, tempVect) >= Math.Cos(coneAngle);
        }

        /// <summary>
        /// Check if a point lies inside of a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="point">3D Point</param>
        /// <param name="sphere">Sphere to check against</param>
        /// <returns>True if point is inside of the sphere</returns>
        public static bool PointInsideBoundingSphere(Vector3 point, BoundingSphere sphere)
        {
            Vector3 diffVect = point - sphere.Center;

            return diffVect.Length() < sphere.Radius;
        }

        /// <summary>
        /// Check if a point lies in a sphere. Good for checking is a point lies within a specific
        /// distance of another point, like proximity checking.
        /// </summary>
        /// <param name="point">3D Point</param>
        /// <param name="sphereCenter">Sphere's center</param>
        /// <param name="sphereRadius">Sphere's radius</param>
        /// <returns>True if point is inside of the sphere</returns>
        public static bool PointInsideSphere(Vector3 point, Vector3 sphereCenter, float sphereRadius)
        {
            Vector3 diffVect = point - sphereCenter;

            return diffVect.Length() < sphereRadius;
        }
    }
}

