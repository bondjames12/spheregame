// MessageType.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;

namespace QuickStart
{
    /// <summary>
    /// Defines the core message types
    /// </summary>
    /// <remarks>
    /// Core engine messages are in the range of 0 to 100000, please use another range for custom messages
    /// <see cref="int.MinValue"/> denotes a unknown message
    /// </remarks>
    public static class MessageType
    {
        /// <summary>
        /// Unknown message
        /// </summary>
        public const int Unknown = int.MinValue;

        /// <summary>
        /// Shutdown message
        /// </summary>
        public const int Shutdown = 0;

        /// <summary>
        /// KeyDown message
        /// </summary>
        public const int KeyDown = 1000;
        /// <summary>
        /// KeyPress message
        /// </summary>
        public const int KeyPress = 1001;
        /// <summary>
        /// KeyUp message
        /// </summary>
        public const int KeyUp = 1002;

        /// <summary>
        /// MouseButtonDown message
        /// </summary>
        public const int MouseDown = 2000;
        /// <summary>
        /// MouseButtonPress message
        /// </summary>
        public const int MousePress = 2001;
        /// <summary>
        /// MouseButtonUp message
        /// </summary>
        public const int MouseUp = 2002;
        /// <summary>
        /// MouseScroll message
        /// </summary>
        public const int MouseScroll = 2003;
        /// <summary>
        /// MouseMove message
        /// </summary>
        public const int MouseMove = 2004;

        #region GamePad Messages (3000-3999)
        /// <summary>
        /// GamePad ButtonUp message
        /// </summary>
        public const int ButtonUp = 3000;

        /// <summary>
        /// GamePad ButtonDown message
        /// </summary>
        public const int ButtonDown = 3001;

        /// <summary>
        /// GamePad ButtonHeld message
        /// </summary>
        public const int ButtonHeld = 3002;

        /// <summary>
        /// GamePad Trigger message
        /// </summary>
        public const int Trigger = 3003;

        /// <summary>
        /// GamePad Thumbstick message
        /// </summary>
        public const int Thumbstick = 3004;
        #endregion
    }
}
