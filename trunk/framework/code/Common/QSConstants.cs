//
// QSConstants.cs
// 
// This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
// for license details.
//

using System;

using Microsoft.Xna.Framework;

namespace QuickStart
{
    /// <summary>
    /// Used to set levels of graphics detail
    /// </summary>
    public enum GraphicsLevel
    {
        Low = 0,
        Med = 1,
        High = 2,
        Highest = 3
    }    

    /// <summary>
    /// A class to hold any global constants. Good for holding anything
    /// that is considered a "magic number". Magic numbers are what you notice
    /// if you look through someone's code and notice numbers defined for values,
    /// but you have no idea why that number was chosen. Also, having a single
    /// class for global constants creates an easy place to look when you know
    /// you're looking for a constant.
    /// </summary>
    public static class QSConstants
    {
        // ============================================================================================
        // Graphics constants
        // ============================================================================================

        /// <summary>
        /// Change this value to set the screen width.
        /// </summary>
        /// <remarks>
        /// You should maintain a 4:3, 16:9, or 16:10 ratio in whatever
        /// you choose. 4:3 for standard monitor or SDTV, 16:9 for HDTV,
        /// or 16:10 for widescreen LCD monitors. 4:3 will be compatible
        /// with the most devices.
        /// 1024 / 768 = 1.3333 or 4/3 (4:3)
        /// </remarks>
        public const int ScreenWidth = 800;

        /// <summary>
        /// Change this value to set the screen height.
        /// </summary>
        public const int ScreenHeight = 600;

        /// <summary>
        /// Set the engine's graphics detail level here.
        /// Set to lower value for slower computers or graphics cards.
        /// </summary>
        /// <remarks>Maybe this should be inside the graphics system? - N.Foster</remarks>
        public const GraphicsLevel DetailLevel = GraphicsLevel.Highest;

        /// <summary>
        /// Enable or disable a default fullscreen or windowed mode. 
        /// When running in debug mode, or using breakpoints, you should leave this false.
        /// </summary>
        public const bool IsFullScreen = false;

        /// <summary>
        /// Enable or disable v-sync (vertical syncronization) mode. When disabled the
        /// framerate will not be locked to the monitor's refresh rate.
        /// </summary>
        public const bool IsVSyncd = false;

        /// <summary>
        /// Enabling this fixes the framerate to values under 60 frames per second (60hz). This
        /// is because most games do not need to run above this speed as it just puts more strain
        /// on the computer.
        /// </summary>
        public const bool IsFixedTimeStep = false;

        /// <summary>
        /// Enable or disable graphic multisampling. When enabled jagged edges will be less noticable.
        /// Enabling this will lower performance (FPS).
        /// </summary>
        /// <remarks>Multisampling should only be set to true for high-end graphics cards</remarks>
        public const bool IsMultiSampling = (DetailLevel == GraphicsLevel.Highest) ? true : false;

        /// <summary>
        /// Does the video card support 32-bit index buffers?
        /// </summary>
        /// <remarks>For now this has to be set by the user in this file, until we find a way to read
        /// this information from the video card.</remarks>
        public const bool Supports32BitIndexBuffers = true;

        // ============================================================================================
        // Camera constants
        // ============================================================================================

        /// <summary>
        /// Default camera far plane distance.
        /// </summary>
        public const float DefaultFarPlane = 1000.0f;

        /// <summary>
        /// Default camera near plane distance.
        /// </summary>
        public const float DefaultNearPlane = 0.1f;

        /// <summary>
        /// Default camera field-of-view (in degrees).
        /// </summary>
        public const float DefaultFOV = 60.0f;

        /// <summary>
        /// Maximum camera far plane distance.
        /// </summary>
        public const float MaxFarPlane = 10000.0f;

        /// <summary>
        /// Minimum camera near plane distance.
        /// </summary>
        public const float MinNearPlane = 0.01f;

        /// <summary>
        /// Minimum degrees allowable for a field-of-view.
        /// </summary>
        public const float MinFOV = 1.0f;

        /// <summary>
        /// Maximum degrees allowable for a field-of-view.
        /// </summary>
        public const float MaxFOV = 175.0f;

        /// <summary>
        /// Maximum zoom/magnification level (32x)
        /// </summary>
        public const int MaxZoomLevel = 32;        

        // ============================================================================================
        // World constants
        // ============================================================================================

        // ============================================================================================
        // Terrain constants
        // ============================================================================================

        // Will comment these with XML once they're back in use... N.Foster
        public const int MaxTerrainScale = 4;
        public const int DefaultTerrainSmoothing = 10;

        /// <summary>
        /// Default elevation strength of Terrain.
        /// </summary>
        public const float DefaultTerrainElevStr = 6.0f;

        /// <summary>
        /// Must be a power of two value (i.e. 2, 4, 32, 128, etc..).
        /// Should probably never be lower than 32, especially for large terrains.
        /// </summary>
        public const int DefaultQuadTreeWidth = 128;
      
        public const int MaxQuadTreeWidth = (Supports32BitIndexBuffers == true) ? 512 : 128;

        // ============================================================================================
        // Weather system constants
        // ============================================================================================

        // Will comment these with XML once they're back in use... N.Foster
        public const float SnowIntensity = 150f;
        public const float RainIntensity = 275f;

        // ============================================================================================
        // Input constants
        // ============================================================================================

        // Will comment these with XML once they're back in use... N.Foster
        public const float MinControlSensitivity = 0.1f;
        public const float MaxControlSensitivity = 100f;
        public const float MouseSensitivity = 200.0f;
        public const bool MouseDefaultVisible = false;

        // ============================================================================================
        // HUD constants
        // ============================================================================================

        // ============================================================================================
        // Physics constants
        // ============================================================================================

        /// <summary>
        /// Default gravity velocity
        /// </summary>
        public readonly static Vector3 DefaultGravity = new Vector3(0.0f, 0.0f, -9.8f);

        // ============================================================================================
        // Object pool constants
        // ============================================================================================

        /// <summary>
        /// The allocation size of the free items
        /// </summary>
        /// <remarks>This is the number of new items which will be created if no more are available</remarks>
        public const int PoolAllocationSize = 5;

        /// <summary>
        /// Incremental size of the lists used by the pool.
        /// </summary>
        public const int PoolIncrements = 20;
    }
}
