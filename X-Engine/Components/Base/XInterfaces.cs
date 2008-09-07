using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XEngine
{
    public interface XIUpdateable
    {
        void Update(ref GameTime gameTime);
    }

    public interface XIDrawable
    {
        void Draw(ref GameTime gameTime,ref XCamera Camera);
        void SetProjection(Matrix Projection);
    }

    /// <summary>
    /// Defines an interface for retrieving camera view and projection matrices
    /// </summary>
    public interface XICamera
    {
        /// <summary>
        /// Gets the camera's view matrix
        /// </summary>
        Matrix ViewMatrix { get; }

        /// <summary>
        /// Gets the camera's projection matrix
        /// </summary>
        Matrix ProjectionMatrix { get; }
    }

    /// <summary>
    /// IInputProvider describes a mouse input interface used by the manipulator to determine
    /// mouse movement and button presses
    /// </summary>
    public interface XIInputProvider
    {
        /// <summary>
        /// Gets the position of the mouse after the previous input cycle
        /// </summary>
        Vector2 Start { get; }

        /// <summary>
        /// Gets the current position of the mouse
        /// </summary>
        Vector2 End { get; }

        /// <summary>
        /// Gets the difference between the previous and current mouse positions
        /// </summary>
        Vector2 Delta { get; }

        /// <summary>
        /// Gets the length of the mouse delta vector
        /// </summary>
        float Length { get; }

        /// <summary>
        /// Gets the current X position of the mouse
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets the current Y position of the mouse
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Returns true if the left mouse button is currently in the pressed state, false otherwise
        /// </summary>
        bool LeftButton { get; }

        /// <summary>
        /// Returns true if the left mouse button has entered the pressed state since the
        /// last input cycle (i.e. left mouse button was released after the last input cycle
        /// and is now pressed), false otherwise
        /// </summary>
        bool LeftClick { get; }

        /// <summary>
        /// Returns true if the left mouse button has entered the released state since
        /// the last input cycle (i.e. left button was pressed after the last input cycle
        /// and is now released), false otherwise
        /// </summary>
        bool LeftRelease { get; }

        /// <summary>
        /// Gets a value indicating whether or not the IInputManager is currently focused,
        /// meaning that its input values are valid for the window in which the manipulator
        /// is operating
        /// </summary>
        bool Focused { get; }

        /// <summary>
        /// Gets the viewport within which relative to which input values are interpreted
        /// </summary>
        Viewport Viewport { get; }

        /// <summary>
        /// Updates mouse input values to reflect the current mouse state
        /// </summary>
        void Cycle();

        /// <summary>
        /// Resets the mouse input values to their defaults
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// Represents a 3D geometric transformation through separate
    /// translation, rotation and scale components
    /// </summary>
    public interface XITransform
    {
        /// <summary>
        /// Gets or sets the translation component of the transformation
        /// </summary>
        Vector3 Translation { get; set; }

        /// <summary>
        /// Gets or sets the rotation component of the transformation
        /// </summary>
        Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the scale component of the transformation
        /// </summary>
        Vector3 Scale { get; set; }
    }

    /// <summary>
    /// Represents a transformation for an object that posesses a local
    /// space bounding box
    /// </summary>
    public interface XIBoundedTransform : XITransform
    {
        /// <summary>
        /// Gets the bounds of the transform
        /// </summary>
        BoundingBox Bounds { get; }
    }
}