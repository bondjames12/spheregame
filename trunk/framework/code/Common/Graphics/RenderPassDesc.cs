// RenderPassDesc.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using QuickStart.Entities;

namespace QuickStart.Graphics
{
    /// <summary>
    /// Descriptor for one rendering pass of a graphics frame.
    /// </summary>
    public struct RenderPassDesc
    {
        /// <summary>
        /// The <see cref="Camera"/> defining the view to render.
        /// </summary>
        public Camera RenderCamera;
    }
}
