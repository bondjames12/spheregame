//
// Scene.cs
// 
// This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
// for license details.
//

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using QuickStart.Entities;
using QuickStart.Graphics;
using QuickStart.Physics;

namespace QuickStart
{
    /// <summary>
    /// A scene is a container that holds all the information for a given area, such as entities and
    /// terrain information. Entities includes things like weather generator, and sky. Scene can also
    /// hold information like gravity and wind directions and strengths.
    /// </summary>
    public class Scene
    {
        protected QSGame game;

        /// <summary>
        /// A list of all the entities in this scene.
        /// </summary>
        public List<BaseEntity> Entities
        {
            get { return this.entities; }
        }
        protected List<BaseEntity> entities;

        // @TODO: TEMPORARY, this is just until we get a unified draw procedure that all
        // entity's draw functions can be called from.
        public Terrain sceneTerrain;
        public Light sceneLight;

        /// <summary>
        /// Scene's gravity, direction and speed.
        /// </summary>
        protected Vector3 Gravity = QSConstants.DefaultGravity;

        /// <summary>
        /// Scene's wind, direction and speed.
        /// </summary>
        protected Vector3 Wind = Vector3.Zero;

        /// <summary>
        /// True when scene is active, otherwise false. A scene will not be processed when it is not active.
        /// </summary>
        public bool Active
        {
            get { return this.active; }
        }
        private bool active = false;

        /// <summary>
        /// Gets the <see cref="IPhysicsScene"/> reference for this scene.
        /// </summary>
        public IPhysicsScene PhysicsScene
        {
            get { return this.physicsScene; }
        }

        private IPhysicsScene physicsScene;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Scene(QSGame game)
        {
            this.game = game;

            this.entities = new List<BaseEntity>();

            this.physicsScene = game.Physics.CreateScene();
            this.physicsScene.Gravity = new Vector3(0.0f, -19.81f, 0.0f);   // Not the real scale, but the "world" with the terrain is too large to be believable, scale-wise.


            // Start the first physics frame.
            GameTime startTime = new GameTime();
            this.physicsScene.BeginFrame(startTime);
        }

        /// <summary>
        /// Activates a scene, letting the scene manager know it is ready
        /// to be run. This method should only be called by the scene manager.
        /// </summary>
        public virtual void Activate()
        {
            this.active = true;
        }

        /// <summary>
        /// Deactivates a scene, which stops it from running/updating.
        /// </summary>
        public virtual void Deactiviate()
        {
            this.active = false;
        }

        /// <summary>
        /// Loads all content needed for the scene.
        /// </summary>
        public virtual void LoadContent()
        {
            sceneTerrain.LoadContent();


        }

        /// <summary>
        /// Unloads all previously loaded content for the scene.
        /// </summary>
        public virtual void UnloadContent()
        {
            this.sceneTerrain.UnloadContent();
        }

        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        /// <param name="sceneCam">Camera could be used for culling, but currently is not used.</param>
        public virtual void Update(GameTime gameTime, Camera sceneCam)
        {
            // Wait for previous physics frame to finish.
            this.physicsScene.EndFrame();

            for (int i = this.entities.Count - 1; i >= 0; i--)
            {
                this.entities[i].Update(gameTime);
            }

            // Begin next physics frame.
            this.physicsScene.BeginFrame(gameTime);
        }

        /// <summary>
        /// DEPRECATED! Draws anything in the scene that isn't drawn by a render queue, currently that is
        /// only Terrain.
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        /// <param name="sceneCam">Camera to render from (view, proj, and frustum)</param>
        public virtual void Draw(GameTime gameTime, Camera sceneCam)
        {
            // TEMPORARY
            //if (sceneTerrain != null && sceneLight != null)
            //{
            //    sceneTerrain.Draw(gameTime, sceneCam, sceneLight);
            //}



        }

        /// <summary>
        /// Queries the scene for all potentially visible RenderChunks, given a RenderPassDesc descriptor.
        /// </summary>
        /// <param name="desc">A descriptor for the current rendering pass.</param>
        public virtual void QueryForRenderChunks(ref RenderPassDesc desc)
        {
            this.sceneTerrain.QueryForRenderChunks(ref desc);

            
        }

        /// <summary>
        /// Adds an entity to the scene.
        /// </summary>
        /// <param name="inEntity">Entity to add to scene</param>        
        public virtual void AddEntity( BaseEntity inEntity )
        {
            this.entities.Add(inEntity);
        }

        /// <summary>
        /// Removes an entity from the scene.
        /// </summary>
        /// <param name="targetEntity">Entity to remove from scene</param>
        /// <remarks>TEMPORARY until we have a manager to remove entities marked for deletion</remarks>
        public virtual void RemoveEntity(BaseEntity targetEntity)
        {
            this.entities.Remove(targetEntity);
        }
    }
}
