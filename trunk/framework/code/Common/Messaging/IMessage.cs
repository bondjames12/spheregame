//
// IMessage.cs
// 
// This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
// for license details.
//

using System;
using System.Collections.Generic;
using System.Text;

namespace QuickStart
{
    /// <summary>
    /// Interface for game messages
    /// </summary>
    public interface IMessage : IPoolItem
    {
        /// <summary>
        /// The type of the message
        /// </summary>
        int Type
        {
            get;
        }
    }
}
