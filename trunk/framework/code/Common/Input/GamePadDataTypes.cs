//
// GamePadDataTypes.cs
// 
// This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
// for license details.
//

using Microsoft.Xna.Framework;          // Needed for Vector2 and PlayerIndex
using Microsoft.Xna.Framework.Input;

namespace QuickStart
{
    /// <summary>
    /// Stores info about a gamepad button for gamepad messages.
    /// </summary>
    public struct GamePadButton : IPoolItem
    {
        /// <summary>
        /// Button for which information is being stored.
        /// </summary>
        public Buttons Button;

        /// <summary>
        /// Player who this button info pertains to.
        /// </summary>
        public PlayerIndex Player;

        /// <summary>
        /// Explicit implementation for releasing
        /// </summary>
        void IPoolItem.Release()
        {
        }

        /// <summary>
        /// Explicit implementation for aquiring
        /// </summary>
        void IPoolItem.Aquire()
        {
        }
    }

    /// <summary>
    /// Stores info about a gamepad trigger for gamepad messages.
    /// </summary>
    public struct GamePadTrigger : IPoolItem
    {
        /// <summary>
        /// Value of this trigger. (0.0f to 1.0f, 0 being nothing, 1 being all the way down).
        /// </summary>
        public float TriggerValue;

        /// <summary>
        /// Type of trigger being referred to in this message (left or right).
        /// </summary>
        public GamePadInputSide TriggerType;

        /// <summary>
        /// Player who this trigger info pertains to.
        /// </summary>
        public PlayerIndex Player;

        /// <summary>
        /// Explicit implementation for releasing
        /// </summary>
        void IPoolItem.Release()
        {
            this.TriggerValue = 0.0f;
        }

        /// <summary>
        /// Explicit implementation for aquiring
        /// </summary>
        void IPoolItem.Aquire()
        {
        }
    }

    /// <summary>
    /// Which input side is being used. e.g. "Left" thumbstick, or "Right" trigger
    /// </summary>
    public enum GamePadInputSide
    {
        /// <summary>
        /// Left side of the gamepad
        /// </summary>
        Left = 1,

        /// <summary>
        /// Right side of the gamepad
        /// </summary>
        Right = 2
    }

    /// <summary>
    /// Stores info about a gamepad thumbstick for gamepad messages.
    /// </summary>
    public struct GamePadThumbStick : IPoolItem
    {
        /// <summary>
        /// Holds values for the X and Y axis movements of the thumbstick.
        /// </summary>
        public Vector2 StickValues;
   
        /// <summary>
        /// Type of thumbstick being referred to in this message (left or right).
        /// </summary>
        public GamePadInputSide StickType;

        /// <summary>
        /// Player who this thumbstick info pertains to.
        /// </summary>
        public PlayerIndex Player;

        /// <summary>
        /// Explicit implementation for releasing
        /// </summary>
        void IPoolItem.Release()
        {
            this.StickValues.X = 0.0f;
            this.StickValues.Y = 0.0f;
        }

        /// <summary>
        /// Explicit implementation for aquiring
        /// </summary>
        void IPoolItem.Aquire()
        {
        }
    }
}
