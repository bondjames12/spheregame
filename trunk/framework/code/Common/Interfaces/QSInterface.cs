/*
 * QSInterface.cs
 * 
 * This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
 * for license details.
 */

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace QuickStart.Interfaces
{
    /// <summary>
    /// Generic interface from which all interfaces are derived.
    /// </summary>
    /// <remarks>@todo: A more robust definition should be placed here later. - N.Foster</remarks>
    public abstract class QSInterface
    {
        protected SceneManager sceneMgr;

        /// <summary>
        /// Called from a derived class to construct an interface.
        /// </summary>
        /// <param name="sceneMgr">SceneManager which for which this interface resides in.</param>
        public QSInterface(SceneManager sceneMgr)
        {
            this.sceneMgr = sceneMgr;
        }

        /// <summary>
        /// Updates interface.
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public virtual void Update(GameTime gameTime)
        {
            // This is only reached for interfaces without update methods.
        }
    }
}
