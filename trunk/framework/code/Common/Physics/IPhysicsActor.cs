// PhysicsActor.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace QuickStart.Physics
{
    /// <summary>
    /// Interface for all physics actors.
    /// </summary>
    public interface IPhysicsActor
    {
        /// <summary>
        /// Gets/sets the density of the actor.
        /// </summary>
        float Density { get; set; }

        /// <summary>
        /// Gets/sets the mass of the actor.
        /// </summary>
        float Mass { get; set; }

        /// <summary>
        /// Gets/sets the position of the actor.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Gets/set the orientation of the actor.
        /// </summary>
        Matrix Orientation { get; set; }

        /// <summary>
        /// Gets the list of shapes composing the actor.
        /// </summary>
        List<ShapeDesc> Shapes { get; }
    }
}
