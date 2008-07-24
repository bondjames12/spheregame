// BaseEntity.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Xna.Framework;

using QuickStart.Graphics;

namespace QuickStart.Entities
{
    /// <summary>
    /// Super class for all entity types.
    /// 
    /// Still needs to be designed. This is only a placeholder/skeleton right now.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Holds a list of all <see cref="Camera"/>s attached to this entity.
        /// </summary>
        public List<Camera> Cameras
        {
            get { return this.cameras; }
        }

        private readonly List<Camera> cameras;

        /// <summary>
        /// Gets the <see cref="QSGame"/> for the entity
        /// </summary>
        public QSGame Game
        {
            get { return game; }
        }
        private readonly QSGame game;

        /// <summary>
        /// Position in the scene
        /// </summary>        
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                this.OnPropertyChanged(PropertyName.Position);
            }
        }
        private Vector3 position = Vector3.Zero;

        /// <summary>
        /// Rotation matrix, which stores its right, up, and forward vectors.
        /// </summary>
        /// <remarks>Currently public for performance reasons</remarks>
        public Matrix Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                this.OnPropertyChanged(PropertyName.Rotation);
            }
        }
        public Matrix rotation = Matrix.Identity;

        /// <summary>
        /// Entity's scale
        /// </summary>
        public float Scale
        {
            get { return scale; }
        }
        private float scale = 1.0f;

        /// <summary>
        /// Holds a list of all <see cref="Camera"/>s attached to this entity.
        /// </summary>


        /// <summary>
        /// Sets visibility of the entity. Object will not be considered for rendering when visibility is
        /// set to false.
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
        private bool isVisible;

        /// <summary>
        /// This event is raised when a property is set
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseEntity(QSGame game)
        {
            this.game = game;

            cameras = new List<Camera>();
        }

        /// <summary>
        /// Called from a child class to set position and rotation parameters.
        /// </summary>
        /// <param name="position">Position of entity</param>
        /// <param name="rotation">Rotation of entity</param>
        public BaseEntity(QSGame game, Vector3 position, Matrix rotation)
        {
            this.game = game;

            this.position = position;
            this.rotation = rotation;

            cameras = new List<Camera>();
        }

        /// <summary>
        /// Called from a child class to set position and rotation parameters.
        /// </summary>
        /// <param name="xPos">X coordinate of entity position</param>
        /// <param name="yPos">Y coordinate of entity position</param>
        /// <param name="zPos">Z coordinate of entity position</param>
        /// <param name="rotation">Rotation of entity</param>
        public BaseEntity(float xPos, float yPos, float zPos, Matrix rotation)
        {
            this.position = new Vector3(xPos, yPos, zPos);
            this.rotation = rotation;
        }

        /// <summary>
        /// BaseEntity update methods
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public virtual void Update(GameTime gameTime)
        {
           
        }

        /// <summary>
        /// This method is invoked when a property on the entity is changed
        /// </summary>
        /// <param name="propertyName">Name of the property which has changed</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
