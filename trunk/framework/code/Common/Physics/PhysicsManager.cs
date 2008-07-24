// PhysicsSystem.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

namespace QuickStart.Physics
{
    /// <summary>
    /// Interface to the physics simulation system.
    /// </summary>
    public abstract class PhysicsManager : BaseManager
    {
        /// <summary>
        /// Constructs a new <see cref="PhysicsSystem"/> instance.
        /// </summary>
        /// <param name="game"></param>
        public PhysicsManager(QSGame game)
            : base(game)
        {
        }

        /// <summary>
        /// Creates a new physics scene.
        /// </summary>
        /// <returns>Instance of the new physics scene.</returns>
        public abstract IPhysicsScene CreateScene();
    }
}
