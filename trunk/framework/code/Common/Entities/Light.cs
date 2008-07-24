/*
 * Light.cs
 * 
 * This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
 * for license details.
 */

using System;

using Microsoft.Xna.Framework;

namespace QuickStart.Entities
{
    public class Light : BaseEntity
    {
        /// <summary>
        /// Direction this light is facing (forward vector)
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public Vector3 direction = Vector3.Zero;

        public float AmbientPower
        {
            get { return ambientPower; }
        }
        private float ambientPower = 1.0f;

        public Vector3 setAmbientColor
        {
            set
            {
                ambientColor = value;
                ambientPower = ambientColor.Length();
            }
        }
        public Vector3 ambientColor = Vector3.Zero;

        public float SpecularPower
        {
            get { return specularPower; }
            set { specularPower = value; }
        }
        private float specularPower = 1.0f;

        /// <summary>
        /// Specular color of this light.
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public Vector3 specularColor = Vector3.Zero;

        /// <summary>
        /// Diffuse color of this light.
        /// </summary>
        /// <remarks>Public for performance reasons</remarks>
        public Vector3 diffuseColor = Vector3.Zero;

        /// <summary>
        /// Minimum ambient light produced by this light.
        /// </summary>
        public float MinimumAmbient
        {
            get { return minimumAmbient; }
            set { minimumAmbient = value; }
        }
        private float minimumAmbient = 0.1f;

        //public Vector3 UpVector = Vector3.Up;

        public Light(QSGame game)
            : base(game)
        {

        }
    }
}
