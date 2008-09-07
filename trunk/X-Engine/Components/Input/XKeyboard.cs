using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class XKeyboard : XComponent, XIUpdateable
    {
        KeyboardState CurrentState;
        KeyboardState LastState;

        XTimer idleTime;

        public XKeyboard(ref XMain X) : base(ref X)
        {
            DrawOrder = 0;
            UpdateStates();
            idleTime = new XTimer(ref X);
        }

        public override void Update(ref GameTime gameTime)
        {
            UpdateStates();

            if (CurrentState == LastState)
                idleTime.Start();
            else
                idleTime.Reset();
        }

        void UpdateStates()
        {
            LastState = CurrentState;
            CurrentState = Keyboard.GetState();
        }

        public enum States { Last, Current }

        public KeyboardState State(States State)
        {
            if (State == States.Last)
                return LastState;
            else
                return CurrentState;
        }

        public bool KeyDown(Keys Key)
        {
            return (CurrentState.IsKeyDown(Key));
        }

        public bool KeyUp(Keys Key)
        {
            return (CurrentState.IsKeyUp(Key));
        }

        public bool KeyHeld(Keys Key)
        {
            if (CurrentState.IsKeyDown(Key) && LastState.IsKeyDown(Key))
                return true;
            else
                return false;
        }

        public bool KeyPressed(Keys Key)
        {
            if (CurrentState.IsKeyDown(Key) && !LastState.IsKeyDown(Key))
                return true;
            else
                return false;
        }

        public bool KeyReleased(Keys Key)
        {
            if (!CurrentState.IsKeyDown(Key) && LastState.IsKeyDown(Key))
                return true;
            else
                return false;
        }
    }
}