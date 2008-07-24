//
// Message.cs
//
// This file is part of the QuickStart Engine. See http://www.codeplex.com/QuickStartEngine
// for license details.
//

using System;
using System.ComponentModel;

using Microsoft.Xna.Framework;


namespace QuickStart
{
    /// <summary>
    /// Message class
    /// </summary>
    /// <typeparam name="T">Type of the contained data</typeparam>
    public partial class Message<T> : IMessage
    {
        private int type;
        private T data;
        private GameTime gameTime;

        public Message()
        {
            this.data = default(T);
            this.gameTime = null;
            this.type = MessageType.Unknown;
        }

        /// <summary>
        /// Gets/Sets the <see cref="GameTime"/> for this message
        /// </summary>
        public GameTime Time
        {
            get { return this.gameTime; }
            set { this.gameTime = value; }
        }

        /// <summary>
        /// Gets/Sets the data for this message
        /// </summary>
        public T Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        /// <summary>
        /// Gets/Sets the type of the message
        /// </summary>
        public int Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
    
        /// <summary>
        /// Explicit implementation of <see cref="IPoolItem.Release"/>
        /// </summary>
        void IPoolItem.Release()
        {
            this.ReleaseCore();
        }

        /// <summary>
        /// Releases the current message
        /// </summary>
        protected virtual void ReleaseCore()
        {
            this.data = default(T);
            this.gameTime = null;
            this.type = MessageType.Unknown;
        }

        /// <summary>
        /// Explicit implementation of <see cref="IPoolItem.Assign"/>
        /// </summary>
        void IPoolItem.Aquire()
        {
            this.AssignCore();
        }

        /// <summary>
        /// Assign the current message
        /// </summary>
        protected virtual void AssignCore()
        {
        }
    }
}
