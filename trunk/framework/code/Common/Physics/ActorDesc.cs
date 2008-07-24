// ActorDesc.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace QuickStart.Physics
{
    /// <summary>
    /// Descriptor for physics actors.
    /// </summary>
    public class ActorDesc
    {
        private Vector3 position = Vector3.Zero;
        private Matrix orientation = Matrix.Identity;
        private float density;
        private bool dynamic;
        private readonly List<ShapeDesc> shapes = new List<ShapeDesc>();

        /// <summary>
        /// Initial position of the actor.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Initial orientation of the actor, as a transformation matrix.
        /// </summary>
        public Matrix Orientation
        {
            get { return this.orientation; }
            set { this.orientation = value; }
        }

        /// <summary>
        /// Initial density of the actor.
        /// </summary>
        public float Density
        {
            get { return this.density; }
            set { this.density = value; }
        }

        /// <summary>
        /// Dynamic flag.  True is mobile, false is static.
        /// </summary>
        public bool Dynamic
        {
            get { return this.dynamic; }
            set { this.dynamic = value; }
        }

        /// <summary>
        /// List of shapes that compose the actor.
        /// </summary>
        public List<ShapeDesc> Shapes
        {
            get { return this.shapes; }
        }
    }
}
