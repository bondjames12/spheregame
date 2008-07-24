// KeyData.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.

using System;
using Microsoft.Xna.Framework.Input;

namespace QuickStart
{
    /// <summary>
    /// Holds information about the current key event
    /// </summary>
    public class KeyData : IPoolItem
    {
        private Keys key;
        private bool leftShift;
        private bool leftAlt;
        private bool leftControl;
        private bool rightAlt;
        private bool rightControl;
        private bool rightShift;

        /// <summary>
        /// Right Shift key is begin pressed
        /// </summary>
        public bool RightShift
        {
            get { return this.rightShift; }
            set { this.rightShift = value; }
        }

        /// <summary>
        /// Right Control key is being pressed
        /// </summary>
        public bool RightControl
        {
            get { return this.rightControl; }
            set { this.rightControl = value; }
        }
	
        /// <summary>
        /// Right Alt key is being pressed
        /// </summary>
        public bool RightAlt
        {
            get { return this.rightAlt; }
            set { this.rightAlt = value; }
        }
	
        /// <summary>
        /// Left Control key is being pressed
        /// </summary>
        public bool LeftControl
        {
            get { return this.leftControl; }
            set { this.leftControl = value; }
        }

        /// <summary>
        /// Left Alt key is being pressed
        /// </summary>
        public bool LeftAlt
        {
            get { return this.leftAlt; }
            set { this.leftAlt = value; }
        }
	
        /// <summary>
        /// Left Shift key is being pressed
        /// </summary>
        public bool LeftShift
        {
            get { return this.leftShift; }
            set { this.leftShift = value; }
        }

        /// <summary>
        /// The <see cref="Keys"/> which raised the event
        /// </summary>
        public Keys Key
        {
            get { return this.key; }
            set { this.key = value; }
        }
        
        /// <summary>
        /// Explicit implementation for releasing
        /// </summary>
        void IPoolItem.Release()
        {
            this.Key = Keys.None;
            this.LeftAlt = false;
            this.LeftControl = false;
            this.LeftShift = false;
        }

        /// <summary>
        /// Explicit implementation for aquiring
        /// </summary>
        void IPoolItem.Aquire()
        {
        }
    }
}
