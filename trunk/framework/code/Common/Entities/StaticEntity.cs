//
// StaticEntity.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.
//

using System;

using Microsoft.Xna.Framework;

using QuickStart.Graphics;

namespace QuickStart.Entities
{
    /// <summary>
    /// Static entities could be used for trigger/spawn points, but shouldn't be required to be drawn. - N.Foster
    /// </summary>
    public class StaticEntity : BaseEntity
    {
        /// <summary>
        /// A Static entity's StaticModel.
        /// </summary>
        public StaticModel Model    // Temporary until we can find a way to integrate render queue into scene manager
        {
            get { return model; }
        }
        private StaticModel model;

        public StaticEntity(QSGame game)
            : base(game)
        {

        }

        /// <summary>
        /// Creates a static entity
        /// </summary>
        /// <param name="modelPath">File path to model file</param>
        public StaticEntity(QSGame game, string modelPath)
            : base(game)
        {
            // Loading a model may be temporary here, I believe StaticEntity was planned to have the option
            // of not having a model, or being rendered.
            LoadModel(modelPath);
        }

        /// <summary>
        /// Creates a static entity
        /// </summary>
        /// <param name="modelPath">File path to model file</param>
        /// <param name="position">Position of entity</param>
        /// <param name="rotation">Rotation of entity</param>
        public StaticEntity(QSGame game, string modelPath, Vector3 position, Matrix rotation)
            :
            base(game, position, rotation)
        {
            LoadModel(modelPath);

        }

        /// <summary>
        /// Creates a static entity
        /// </summary>
        /// <param name="modelPath">File path to model file</param>
        /// <param name="xPos">X coordinate of entity position</param>
        /// <param name="yPos">Y coordinate of entity position</param>
        /// <param name="zPos">Z coordinate of entity position</param>
        public StaticEntity(string modelPath, float xPos, float yPos, float zPos, Matrix rotation)
            :
            base(xPos, yPos, zPos, rotation)
        {
            LoadModel(modelPath);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // TEMPORARY, for testing model rotation.
            this.Rotation = this.rotation * Matrix.CreateRotationY(gameTime.ElapsedGameTime.Milliseconds * 0.0005f);
        }

        /// <summary>
        /// Loads an entity's model
        /// </summary>
        /// <param name="modelPath">File path to the model file</param>
        public void LoadModel(string modelPath)
        {
            // This method could be expanded to do other things we want to happen when
            // models are loaded. - N.Foster

            model = this.Game.ModelLoader.LoadStaticModel(modelPath);
        }
    }
}
