//
// GraphicsSettings.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.
//

using System;

namespace QuickStart.Graphics
{
    /// <summary>
    /// Defines configurable settings applied to a graphics device.
    /// </summary>
    public struct GraphicsSettings
    {
        /// <summary>
        /// The width of the back buffer, in pixels.
        /// </summary>
        public int BackBufferWidth;

        /// <summary>
        /// The height of the back buffer, in pixels.
        /// </summary>
        public int BackBufferHeight;

        /// <summary>
        /// Flag enabling/disabling synchronization with vertical retrace (vsync).
        /// </summary>
        public bool EnableVSync;

        /// <summary>
        /// Flag enabling/disabling multi-sample anti-aliasing.  If true, the highest quality MSAA setting will be used.
        /// </summary>
        public bool EnableMSAA;

        /// <summary>
        /// Flag enabling/disabling full-screen rendering.  Ignored on Xbox 360 platform.
        /// </summary>
        public bool EnableFullScreen;
    }
}
