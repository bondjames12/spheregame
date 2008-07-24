// PhysicsScene.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using Microsoft.Xna.Framework;

namespace QuickStart.Physics
{
    /// <summary>
    /// Interface for all physics scenes.
    /// </summary>
    public interface IPhysicsScene
    {
        /// <summary>
        /// Gets/sets the gravity vector.
        /// </summary>
        Vector3 Gravity { get; set; }

        /// <summary>
        /// Creates a new physics actor.
        /// </summary>
        /// <param name="desc">The <see cref="ActorDesc"/> describing the actor.</param>
        /// <returns>Instance of new physics actor.</returns>
        IPhysicsActor CreateActor(ActorDesc desc);

        /// <summary>
        /// Begins processing a physics simulation frame.
        /// </summary>
        /// <param name="gameTime">The elapsed time since the last update.</param>
        void BeginFrame(GameTime gameTime);

        /// <summary>
        /// Blocks until the currently processing physics frame is complete.
        /// </summary>
        void EndFrame();
    }
}
